/**
 *  WIDGET : FORM
 */

/** 
 * Plugins
 */
(function ($) {
    // Character counter
    $.fn.formCharCount = function (options) {

        var element = this;
        var _options = $.extend({
            tinymce: null
        }, options);

        // check
        if (element.length == 0) return this;

        // options: tinymce
        if (_options.tinymce != null) {
            _options.tinymce = $.extend({ selector: null, id: null }, _options.tinymce);
        }

        // handle
        var handle = {
            element: element,
            mode: 'default',
            options: _options
        };

        // mode
        if (handle.options.tinymce != null && handle.options.tinymce.id != null) {
            handle.mode = 'tinymce';
        }

        // init
        refresh();

        // bind
        if (handle.mode == 'default') {

            // bind: update counter on keyup
            element.keyup(function () {
                refresh();
            });

            // bind: update counter on change
            element.change(function () {
                refresh();
            });

        } else if (handle.mode == 'tinymce') {

            var instance = tinymce.get(handle.options.tinymce.id)
            if (instance != null) {

                // bind: change
                instance.on('change', function (e) {
                    refresh();
                });

                // bind: keyup
                instance.on('keyup', function (e) {
                    refresh();
                });
            }
        }

        // helper: sets the value of the character counter
        function refresh() {

            var text = null;

            // get: text
            if (handle.mode == 'default') {

                var el = handle.element;
                text = el.val();
            }
            else if (handle.mode == 'tinymce') {

                var instance = tinymce.get(handle.options.tinymce.id)
                if (instance != null) {
                    var body = instance.getBody();
                    text = tinymce.trim(body.innerText || body.textContent);
                    //text = $(body).text(); // jquery method

                    console.log('text', text);
                    console.log('textLength', text.length);
                }
            }

            // get: stats
            var stats = null;
            if (text != null) {
                stats = {
                    chars: text.length,
                    words: text.split(/[\w\u2019\'-]+/).length
                };
            }

            // reflect: stats
            if (stats != null) {
                var elId = handle.element.attr("id");
                var curCount = parseInt(stats.chars);

                var labelCur = $("[data-form-label=" + elId + "_charcount_cur]");
                labelCur.text(curCount);
            }
        }


        return handle;
    };

    //gets the form values and __RequestVerificationToken
    $.fn.antiForgerySerializeArray = function () {

        if (!$(this).is("form"))
            return;

        var data = this.serializeArray();
        data.__RequestVerificationToken = this.find('input[name=__RequestVerificationToken]').val();
        return data;
    };

    // Bind: show/hide password
    $(document).off('click', '[data-show-password]');
    $(document).on('click', '[data-show-password]', function () {
        var elementName = $(this).data('show-password');
        var input = document.getElementById(elementName);

        if (input.type == 'password') {
            input.setAttribute('type', 'text');
        } else {
            input.setAttribute('type', 'password');
        }
    });

    //// adjusts the textarea's height based on it's content
    //$.fn.autoFitTextarea = function () {

    //    var $textArea = this;

    //    resizeTextArea($textArea);

    //    // disabled
    //    //$textArea.off("keyup.textarea").on("keyup.textarea", function() {
    //    //    resizeTextArea($(this));
    //    //});

    //    function resizeTextArea($element) {
    //        $element.height($element[0].scrollHeight);
    //    }​
    //};
}(jQuery));


/** 
 * The Global Form Manager object
 */
var FormManager = FormManager || (function () {
    var manager = {};

    manager.collection = [];
    manager.fields = [];

    //=== Public

    /**
     * Register Form
     */
    manager.register = function (form) {

        // Remove duplicate form
        _removeFormObj(form.id);

        // Register new form
        var _form = _initFormObj(form);

        // Add unspecified properties (todo...)
        //var _form = $.extend(_form, form);

        // Set initial values
        _storeInitialValues(_form);

        var buttonCancel = _getCancelButton(_form);
        var buttonSave = _getSaveButton(_form);

        // Bind Cancel action
        if (buttonCancel != null) {
            // Attach event
            $(document).off('click', '#' + buttonCancel.id);
            $(document).on('click', '#' + buttonCancel.id, function (e) {
                e.preventDefault();
                //alert("cancel");

                var elementId = $(this).attr('id');
                var form = _getFormByChildId(elementId);

                _reset(form);
            });
        }

        // Bind Save action
        if (buttonSave != null) {
            // Attach event
            $(document).off('click', '#' + buttonSave.id);
            $(document).on('click', '#' + buttonSave.id, function (e) {
                e.preventDefault();
                //alert("save");

                var elementId = $(this).attr('id');
                var form = _getFormByChildId(elementId);

                //$domForm = $("#" + form.id);

                _save(form);
            });
        }

        // Add form to collection
        manager.collection.push(_form);
    };

    /**
     * Get form from collection.
     */
    manager.form = function (id) {

        var form = _getFormObj(id);

        return form;
    }

    /**
     *  Collects and returns all the form values.
     *  @param id The form id.
     *  #returns The form value collection.
     */
    manager.data = function (id) {

        var form = _getFormObj(id);
        var collection = _getData(form);
        return collection;
    }

    /**
     *  Removes disabled attribute from disabled form elements.
     *  @param id The form id.
     *  @returns Array of undisabled elements.
     */
    manager.undisableElements = function (id) {

        var form = _getFormObj(id);
        return _undisableElements(form);
    }

    /**
     *  Assigns disabled attribute to specified form elements.
     *  @param elements The elements to disable.
     *  @returns Array of disabled elements.
     */
    manager.disableElements = function (elements) {

        return _disableElements(elements);
    }

    /**
     *  Resets the form restoring the initial values.
     *  @param id The form id.
     */
    manager.reset = function (id) {

        var form = _getFormObj(id);
        _reset(form);
    }

    /**
     *  Saves the form.
     *  @param id The form id.
     */
    manager.save = function (id, callback) {

        var form = _getFormObj(id);
        _save(form, callback);
    }

    /**
     * Disables the target form.
     * Usefull when the user needs to wait for a certain process to finish.
     * @param request The request containing the form target and the instigator.
     */
    manager.disable = function (request) {
        // request = { formId, instigator }

        var _request = $.extend({
            formId: null,
            instigator: null
        }, request);

        var form = _getFormByChildId(_request.instigator);

        if (form != null) {
            var saveButton = _getSaveButton(form);

            if (saveButton != null) {
                $('#' + saveButton.id).prop('disabled', true);
                $('#' + saveButton.id).addClass('disabled');
            }
        }
    }

    /**
     * Enables the target form.
     * Usefull when a sub process is finished, the user can continue to save to form.
     * @param request The request containing the form target and the instigator.
     */
    manager.enable = function (request) {
        // request = { formId, instigator }

        var _request = $.extend({
            formId: null,
            instigator: null
        }, request);

        var form = _getFormByChildId(_request.instigator);

        if (form != null) {
            var saveButton = _getSaveButton(form);

            if (saveButton != null) {
                $('#' + saveButton.id).prop('disabled', false);
                $('#' + saveButton.id).removeClass('disabled');
            }
        }
    }

    /**
     * Get or sets a global field instance.
     * @param key The key which will be used for indexing.
     * @param instance The field instance.
     */
    manager.field = function (key, instance) {
        if (key == null) return null;

        if (instance != null) {
            manager.fields[key] = instance;
        }
        return manager.fields[key];
    }

    /**
     * Constructs a default instance of specified type
     * @param type Field type.
     * @param attrs Attributes (eg: id).
     */
    manager.construct = function (type, attrs) {

        var fieldTypes = ['calendar', 'dropdown', 'lookup', 'list'];

        if (type == 'field' || fieldTypes.indexOf(type) >= 0) {

            var field = {

                // public: properties 
                type: null,
                attributes: null,

                // private: properties
                _bindings: null,
                _getters: null,
                _setters: null,

                // public: methods
                bind: null,
                get: null,
                set: null,
                prop: null,

                // private: methods
                _execute: null
            };

            // mark type
            if (fieldTypes.indexOf(type) >= 0) {
                field['type'] = type;
            }

            // add attributes
            field.attributes = {};
            if (attrs != null) {
                field.attributes = $.extend({}, attrs);
            }

            // property: params
            if (type == 'list') {
                field['params'] = null;
            }

            // function: bind (mechanic) (eg: change, ...)
            field['bind'] = function (event, func) {
                if (event == null || func == null) return;

                if (field._bindings == null) { field._bindings = []; }

                field._bindings.push({ event: event, func: func });
            };

            // function: get (propertyName, options)
            field['get'] = function (prop, opt) {
                if (prop == null || field._getters == null || field._getters[prop] == null) return null;

                return (field._getters[prop])(opt);
            }

            // function: set (propertyName, data, options)
            field['set'] = function (prop, data, opt) {
                if (prop == null || field._setters == null || field._setters[prop] == null) return null;

                return (field._setters[prop])(data, opt);
            }

            // function: property (key) (within field interface)
            field['prop'] = function (key) {

                // read from attributes
                if (field.attributes.hasOwnProperty(key)) {
                    return field.attributes[key];
                }
                return null;
            }

            // execute
            field['_execute'] = function (event, data, domEvent) {
                if (event == null) return;

                if (field._bindings != null) {
                    for (var i = 0; i < field._bindings.length; i++) {
                        var b = field._bindings[i];
                        if (b.event == event) {
                            b.func(data, domEvent);
                        }
                    }
                }
            }




            return field;
        }

        return null;
    }


    //=== Helpers

    /**
     * Get cancel button reference from form reference.
     * @returns button reference or null.
     */
    function _getCancelButton(form) {
        for (var i = 0; i < form.buttons.length; i++) {
            var button = form.buttons[i];
            if (button.type == 'cancel') {
                return button;
            }
        }

        return null;
    }

    /**
     * Get save button reference from form reference.
     * @returns button reference or null.
     */
    function _getSaveButton(form) {
        for (var i = 0; i < form.buttons.length; i++) {
            var button = form.buttons[i];
            if (button.type == 'save') {
                return button;
            }
        }

        return null;
    }

    /**
     *  Returns the registered form object.
     */
    function _getFormObj(id) {

        // Loop collection
        for (var i = 0; i < manager.collection.length; i++) {
            var form = manager.collection[i];

            if (form.id == id) {
                return form;
            }
        }

        return null;
    }

    /**
     *  Removes a registered form object.
     */
    function _removeFormObj(id) {

        // Loop collection
        for (var i = 0; i < manager.collection.length; i++) {
            var form = manager.collection[i];

            if (form.id == id) {
                manager.collection.splice(i, 1);
            }
        }
    }

    /**
     * Finds the form reference by element id.
     * @returns form reference or null.
     */
    function _getFormByChildId(id) {
        // Loop forms
        for (var i = 0; i < manager.collection.length; i++) {
            var form = manager.collection[i];

            // Loop fields
            for (var j = 0; j < form.fields.length; j++) {
                var field = form.fields[j];

                if (field.id == id) {
                    return form;
                }
            }

            // Loop buttons
            for (var k = 0; k < form.buttons.length; k++) {
                var button = form.buttons[k];

                if (button.id == id) {
                    return form;
                }
            }
        }

        return null;
    }

    /**
     * Store initial (field-) values in form reference
     * @param form The form reference
     */
    function _storeInitialValues(form) {
        for (var i = 0; i < form.fields.length; i++) {
            var field = form.fields[i];

            var initialValue = $("#" + field.id).val();
            field.initialValue = initialValue;
        }
    }

    /**
     *  Extracts the form data for specified form.
     *  @param form The form object.
     *  @returns The form collection.
     */
    function _getData(form) {
        // Get dom element
        var domForm = $("#" + form.id);

        // Process certain elements
        for (var i = 0; i < form.fields.length; i++) {
            var field = form.fields[i];
            if (field.type == 'html') {
                // extract tinymce content
                try {
                    var editorContent = tinymce.get(field.id).getContent();
                    domForm.find('#' + field.id).val(editorContent);
                } catch (ex) { }
            }
        }

        var collection = {};

        // Add metadata
        for (var key in form.metadata) {
            collection['__metadata.' + key] = form.metadata[key];
        }

        // Add form values
        var domCollection = domForm.serializeObject();
        for (var key in domCollection) {
            if (key == '__RequestVerificationToken') {
                collection['__RequestVerificationToken'] = domCollection[key];
            }
            else {
                var item = domCollection[key];
                if (item instanceof Array) {
                    collection['__data.' + key] = JSON.stringify(item);
                }
                else {
                    collection['__data.' + key] = item;
                }
            }
        }

        return collection;
    }

    /**
     *  Resets the form restoring the initial values.
     *  @param form The form object.
     */
    function _reset(form) {

        // Reset initial values on the form
        if (form != null) {
            for (var i = 0; i < form.fields.length; i++) {
                var field = form.fields[i];

                $("#" + field.id).val(field.initialValue);
            }
        }
    }

    /**
     *  Saves the form.
     *  @param form The form object.
     */
    function _save(form, callback) {

        var collection = _getData(form);

        //collection = $.extend(collection, $domForm.serializeObject());

        //alert(JSON.stringify(collection));
        //return;

        var request = {
            cache: false,
            async: false,
            url: form.action,
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(collection)
        };

        $.ajax(request)
            .done(function (response) {
                var messages = [];
                if (response.Messages !== undefined || response.messages !== undefined) {
                    messages = response.Messages || response.messages;
                }
                if (response.Message !== undefined || response.message !== undefined) {
                    messages.push(response.Message || response.message);
                }

                if (response.Success == true || response.success == true) {
                    // Update initial values
                    _storeInitialValues(form);

                    Feedback.success({ message: messages });

                    // Callback
                    if (callback != undefined && callback != null) callback(true, response);
                }
                else {
                    Feedback.error({ message: messages });

                    // Callback
                    if (callback != undefined && callback != null) callback(false, response);
                }
            })
            .fail(function (xhr, textStatus, errorThrown) {
                Feedback.error({ message: "Server error ... (" + errorThrown + ")." });

                // Callback
                if (callback != undefined && callback != null) callback(false);
            })
            .always(function () { });
    }

    /**
     *  Removes disabled attribute from disabled form elements.
     *  @param form The form object.
     *  @returns Array of undisabled elements.
     */
    function _undisableElements(form) {

        var undisabledElements = [];

        if (form != null) {
            for (var i = 0; i < form.fields.length; i++) {

                var jElement = $('#' + form.fields[i].id);

                if (jElement.length > 0) {
                    var element = jElement[0];
                    if (element.disabled) {

                        element.disabled = false;
                        undisabledElements.push(element);
                    }
                }
            }
        }
        return undisabledElements;
    }

    /**
     *  Addign disabled attribute to specified form elements.
     *  @param elements The elements to disable.
     *  @returns Array of disabled elements.
     */
    function _disableElements(elements) {

        if (elements != null) {
            for (var i = 0; i < elements.length; i++) {

                elements[0].disabled = true;
            }
        }
        return elements;
    }

    /**
     *  Decorates the form with additional functions.
     *  @returns the form object.
     */
    function _initFormObj(form) {

        /*
        var form = {
            id: 'f1',
            action: 'url/...',
            metadata: {
                prop1: 'a',
                prop2: 'b',
                prop3: 'c'
            },
            fields: [
                { id: 'fe1', type: 'input', value: 'abc', initialValue: 'abc' },
                { id: 'fe2', type: 'textarea', value: 'abc', initialValue: 'abc' }
            ],
            buttons: [
                { id: 3, type: 'save' }
            ]
        };
        */
        /* 
         *  Form Object
        
            form
                - id
                - action
                - metadata
                    - x
                - fields[]
                    - field
                        - id
                        - type
                        - value
                        - initialValue
                - buttons[]
                    - button
                        - id
                        - type (cancel, save, link)
        */

        var _form = $.extend({}, form);

        // func: get field
        _form.getField = function (fieldId) {
            for (var i = 0; i < _form.fields.length; i++) {
                var _field = _form.fields[i];
                if (_field.id == fieldId) { return _field; }
            }
            return null;
        }

        // func: get element
        _form.getElement = function (outer) {

            // the 'form' dom element
            var j = $('#' + _form.id);
            if (j.length == 0) return null;

            // return the most outer element
            if (outer === true) {
                return j.parent();
            }

            return j;
        }

        // helper: init field
        function initField(field) {

            // func: value
            field.val = function () {

                // note: first implementation is calendar...

                if (field.type == 'calendar') {

                    var jForm = _form.getElement(); // note: should be optional...
                    var jField = jForm.find('input#' + field.id);
                    if (jField.length > 0) {

                        return { raw: jField.val() };
                    }
                }

                return null;
            }
        }


        // init: fields
        for (var i = 0; i < form.fields.length; i++) {
            var _field = form.fields[i];
            initField(_field);
        }

        return _form;
    }

    return manager;
}());