/**
 *  MarketingXpress UI Framework.
 *
 *  Dependencies:
 *      - jQuery
 *      - jQuery UI
 *      - jQuery mjs nestedSortable
 */

(function ($) {

    //=== UI Components

    // UI component: area loader
    $.fn.thAreaLoader = function (options) {

        var element = $(this);
        var _options = {};

        var show = true;
        if (options != undefined && options == false) {
            show = false;
        } else {
            _options = $.extend({
                'min-height': null,
                'text': null
            }, options);
        }

        // remove current
        $(element).find('[data-hook=ui-area-loader]').remove();

        if (show) {

            // position
            var area = element;
            var areaHeight = area.outerHeight();
            var areaWidth = area.outerWidth();
            var areaPosition = area.position();

            // Stop this function if areaposition is undefined > Loader error renders the page unusable not worth considering its only a loader
            if (areaPosition == null || areaPosition == undefined) {
                return;
            }

            var top = 0;
            var left = 0;

            var cssPosition = area.css('position');
            if (cssPosition != 'relative') {
                top = areaPosition.top;
                left = areaPosition.left;
            }

            // note: 'borderRightWidth' vs 'border-right-width'
            function borderWidth(el, prop) {
                var v = 0;
                var cssV = el.css(prop);
                if (cssV.indexOf('px') >= 0) {
                    v = parseInt(cssV.replace('px', ''));
                }
                return v;
            }

            var borderTop = borderWidth(area, 'border-top-width');
            var borderRight = borderWidth(area, 'border-right-width');
            var borderBottom = borderWidth(area, 'border-bottom-width');
            var borderLeft = borderWidth(area, 'border-left-width');

            areaWidth -= (borderLeft + borderRight);
            areaHeight -= (borderTop + borderBottom);

            var style1 = '';
            style1 += 'top:' + top + 'px;';
            style1 += 'left:' + left + 'px;';
            style1 += 'height:' + areaHeight + 'px;';
            style1 += 'width:' + areaWidth + 'px;';

            var style2 = '';
            var style3 = '';

            if (_options['min-height'] != null) {
                style2 += 'min-height:' + _options['min-height'] + ';';
                style3 += 'min-height:' + _options['min-height'] + ';';
            }

            // html
            var loaderImageClass = 'loader-image';
            if (_options['text'] != null) { loaderImageClass = 'loader-image-centered'; }

            var html = '<div data-hook="ui-area-loader" class="ui-area-loader" style="' + style1 + '">';
            html += '<div class="loader-background" style="' + style2 + '"></div><div class="' + loaderImageClass + '" style="' + style3 + '"></div>';

            if (_options['text'] != null) {
                html += '<div class="loader-text-container"><span class="loader-text">' + _options['text'] + '</span></div>';
            }

            html += '</div>';
            element.append(html);
        }
    }

    // UI component: tabs
    $.fn.thTabs = function (options) {
        // DOM
        var container = $(this);
        var selector = container.selector;

        var ul = container.find('ul');
        if (ul.length == 0) return;

        // options
        var _options = $.extend({
            click: null
        }, options);


        // bind: click
        $(document).off('click', selector + ' [data-hook=tab]');
        $(document).on('click', selector + ' [data-hook=tab]', function () {

            // DOM
            var li = $(this);
            var ul = li.parent();

            // reflect
            ul.children().removeClass('active');
            li.addClass('active');

            // callback
            if (_options.click != null) {
                _options.click(li);
            }
        });
    }

    // UI component: filter block
    $.fn.thFilterBlock = function (options) {

        var element = $(this);

        // config
        var config = element.attr('data-config');
        if (config != null && config.length > 0) {
            config = JSON.parse(config);
        } else {
            config = null;
        }

        // options
        var _options = $.extend({
            mode: null,         // checkbox, radio
            click: null,        // func(items)
            edit: null,         // func
            strict: null,       // bool  (in conjunction with radio mode)
            render: null        // rendering restrictions
        }, options);

        _options.render = $.extend({ tooltips: (config != null ? config.tooltips : true), }, options['render']);


        // handle
        var _handle = { element: element, selector: element.selector, options: _options };

        // func: get selection
        _handle.getSelection = function () {

            var selection = [];

            $(_handle.selector + ' [data-hook=filter-option].active').each(function (index, value) {

                var node = $(value);

                selection.push(protoItem(node));
            });

            return selection;
        }

        // func: clear selection
        _handle.clearSelection = function () {

            $(_handle.selector + ' [data-hook=filter-option].active').removeClass('active');
        }

        // func: deselect item
        _handle.deselect = function (options) {
            var _options = $.extend({ key: null, id: null }, options);

            if (_options.key != null) {
                $(_handle.selector + ' [data-key=' + _options.key + '].active').removeClass('active');
            }
            if (_options.id != null) {
                $(_handle.selector + ' [data-id=' + _options.id + '].active').removeClass('active');
            }
        }

        // func: remove item
        _handle.remove = function (options) {
            var _options = $.extend({ key: null, id: null }, options);

            if (_options.key != null) {
                $(_handle.selector + ' [data-key=' + _options.key + ']').remove();
            }
            if (_options.id != null) {
                $(_handle.selector + ' [data-id=' + _options.id + ']').remove();
            }
        }

        // func: add item
        _handle.add = function (item) {
            var _item = $.extend({ id: null, key: null, label: null, count: null, editable: null, iconClass: null, attributes: null, position: null }, item);

            var ul = $(_handle.selector + ' ul.list').last();
            var template = $(_handle.selector + ' [data-hook=template] li');

            var li = template.clone(false);

            if (_item.id != null) { li.attr('data-id', _item.id); }
            if (_item.key != null) { li.attr('data-key', _item.key); }
            if (_item.label != null) {
                li.find('[data-hook=label]').html(_item.label);
                li.find('[data-hook=label]').attr('title', _item.label);
            }
            if (_item.description != null) {
                li.attr('data-description', _item.description);
                li.find('[data-hook=label]').attr('data-tooltip', _item.description);
            }
            if (_item.editable !== true) { li.find('[data-hook=edit]').remove(); }
            if (_item.count != null) {
                li.find('[data-hook=count]').removeClass('hidden');
                li.find('[data-hook=count]').html(_item.count);
            }
            if (_item.iconClass != null) {
                li.find('.icon i').addClass(_item.iconClass);
            } else {
                li.find('.icon').remove();
            }
            if (_item.attributes != null) {
                for (var prop in _item.attributes) {
                    if (_item.attributes.hasOwnProperty(prop)) {
                        li.attr(prop, _item.attributes[prop])
                    }
                }
            }

            if (_item.position == 'top') {
                ul.prepend(li);
            } else {
                ul.append(li);
            }

            // return item
            return _handle.item({ key: _item.key, id: _item.id });
        }

        // func: get item
        _handle.item = function (options) {
            var _options = $.extend({ key: null, id: null }, options);

            var node = null;

            if (_options.key != null) {
                node = $(_handle.selector + ' [data-key=' + _options.key + ']');
            }
            if (_options.id != null) {
                node = $(_handle.selector + ' [data-id=' + _options.id + ']');
            }

            return protoItem(node);
        }

        // func: items
        _handle.items = function (options) {
            var _options = $.extend({}, options);

            var nodes = $(_handle.selector + ' .list [data-hook=filter-option]');

            var items = [];
            for (var i = 0; i < nodes.length; i++) {
                items.push(protoItem($(nodes[i])));
            }
            return items;
        }

        // func: visible (get/set)
        _handle.visible = function (options) {

            var _options = {};

            if (options != undefined) {
                if (options === true) {
                    _handle.element.removeClass('hidden');
                } else if (options === false) {
                    _handle.element.addClass('hidden');
                }
            }

            return !(_handle.element.hasClass('hidden'));
        }

        // bind: edit
        $(document).off('click', _handle.selector + ' [data-hook=filter-option] [data-hook=edit]');
        $(document).on('click', _handle.selector + ' [data-hook=filter-option] [data-hook=edit]', function (e) {

            e.stopPropagation();

            var btn = $(this);
            var li = btn.closest('li');

            var item = null;                        // the clicked item
            var active = li.hasClass('active');     // indicator if the item is active or not
            var selection = [];                     // the current selection of active items

            // item
            item = protoItem(li);

            // selection
            selection = _handle.getSelection();

            // callback
            if (_handle.options.edit != null) {
                _handle.options.edit(item, active, selection);
            }
        });

        // bind: click
        $(document).off('click', _handle.selector + ' [data-hook=filter-option]');
        $(document).on('click', _handle.selector + ' [data-hook=filter-option]', function (e) {

            var li = $(this);

            var item = null; // the clicked item
            var active = null; // indicator if the item is active or not
            var selection = []; // the current selection of active items

            // reflect
            if (_handle.options.mode == 'checkbox') {

                var isActive = li.hasClass('active');
                active = !isActive;

                if (isActive) {

                    // de-select
                    li.removeClass('active');
                } else {

                    // select
                    li.addClass('active');
                }

            } else if (_handle.options.mode == 'radio') {

                var wasActive = li.hasClass('active');
                active = !wasActive;

                // apply class
                li.siblings().removeClass('active');
                if (wasActive) {

                    if (_handle.options.strict === true) {
                        li.addClass('active'); // keep active
                    } else {
                        li.removeClass('active');
                    }
                } else {
                    li.addClass('active');
                }
            }

            // item
            item = protoItem(li);

            // selection
            selection = _handle.getSelection();

            // callback
            if (_handle.options.click != null) {
                _handle.options.click(item, active, selection);
            }
        });

        // prototype: item
        function protoItem(node) {
            if (node == null) return null;

            var _id = parseInt(node.attr('data-id'));
            var _key = node.attr('data-key');
            var _label = node.find('[data-hook=label]').text();
            var _description = node.attr('data-description');
            if (_description == undefined || _description.length == 0) { _description = null; }
            var _iconClass = node.find('.icon > i').prop('class');
            if (_iconClass == undefined || _iconClass.length == 0) { _iconClass = null; }
            var _groupKey = node.attr('data-group-key');
            if (_groupKey == undefined || _groupKey.length == 0) { _groupKey = null; }
            var _groupId = node.attr('data-group-id');
            if (_groupId == undefined || _groupId.length == 0) { _groupId = null; }


            // handle
            var o = {
                '_proto': 'item',
                node: node,
                id: _id,
                key: _key,
                iconClass: _iconClass,
                '_id': _id,
                '_key': _key,
                '_label': _label,
                '_description': _description,
                '_iconClass': _iconClass,
                '_groupKey': _groupKey,
                '_groupId': _groupId,
            };

            // func: property
            o.prop = function (name) {
                return o['_' + name];
            }

            // func: update label
            o.label = function (value) {
                if (value != undefined) {
                    o['_label'] = value;
                    o.node.find('[data-hook=label]').html(o['_label']);
                    o.node.find('[data-hook=label]').attr('title', o['_label']);
                } else {
                    return o['_label'];
                }
            }

            // func: update description (needs re-activation of the 'data-tooltip' script)
            o.description = function (value) {
                if (value != undefined) {
                    o['_description'] = value;
                    o.node.attr('data-description', o['_description']);

                    // tooltip
                    if (_options.render.tooltips === true) {
                        o.node.find('[data-hook=label]').attr('data-tooltip', o['_description']);
                    }
                } else {
                    return o['_description'];
                }
            }

            // func: hidden state
            o.hidden = function (state) {
                if (state != undefined) {
                    if (state === true) {
                        o.node.addClass('hidden');
                    } else if (state === false) {
                        o.node.removeClass('hidden');
                    }
                } else {
                    return o.node.hasClass('hidden');
                }
            }

            // func: count
            o.count = function (value) {

                // node
                var cn = o.node.find('[data-hook=count]');
                if (cn.length == 0) return;

                // current
                var cur = cn.text();
                cur = isNaN(cur) ? null : parseInt(cur);

                // process
                if (value != undefined) {

                    // set
                    if (isNaN(value)) {
                        var _options = value;

                        // object
                        if ((typeof _options === 'object') && (_options !== null)) {

                            // value
                            if (_options.hasOwnProperty('value')) {
                                cn.text(parseInt(_options.value));
                            }

                            // mutation
                            if (_options.hasOwnProperty('mutation')) {
                                cn.text((cur != null ? cur : 0) + parseInt(_options.mutation));
                            }
                        }

                    } else {
                        // number
                        cn.html(parseInt(value));
                    }

                } else {

                    // get
                    return cur;
                }
            }

            // func: active
            o.active = function (state) {
                if (state != undefined) {

                    // set
                    if (state === true) {
                        o.node.addClass('active');
                    } else if (state === false) {
                        o.node.removeClass('active');
                    }

                } else {

                    // get
                    return o.node.hasClass('active');
                }
            }

            return o;
        }

        return _handle;
    }

    // UI component: ive container
    $.fn.thIveContainer = function (options) {

        var element = $(this);
        var _options = $.extend({
            item: null,          // item selector
        }, options);

        // handle
        var _handle = { element: element, selector: element.selector, options: _options };

        // func: search
        _handle.search = function (input, options) {

            var _input = { terms: [] };
            var _options = $.extend({
                scope: null,            // array: attributes to search on
            }, options);

            // input: string or object
            if (input == null) {
                // clear
            } else if (typeof input === 'string' || input instanceof String) {
                // string
                if (input.length > 0) {
                    if (input.indexOf(' ') >= 0) {
                        // mutiple terms
                        var terms = input.split(' ');
                        for (var i = 0; i < terms.length; i++) {
                            if (terms[i].length > 0) { _input.terms.push(terms[i]); }
                        }
                    } else {
                        // single term
                        _input.terms.push(input);
                    }
                }
            } else if (jQuery.isPlainObject(input)) {
                // read special commands...
            }

            // check: item selector
            if (_handle.options.item == null || _handle.options.item.length == 0) return;

            // execute
            var clear = _input.terms.length == 0;
            var items = _handle.element.find(_handle.options.item);

            items.each(function (i, v) {

                var item = $(this);
                if (clear === true) {
                    item.removeClass('hidden');

                } else {

                    var strict = true; // comparison: 'and'
                    var match = false;
                    var searchConfig = item.find('[data-hook=search]');
                    if (searchConfig.length > 0) {
                        var attrs = getDataAttributes(searchConfig[0]);
                        for (var attr in attrs) {
                            var incl = attrs.hasOwnProperty(attr) && match === false && attr != 'search';

                            // check scope
                            if (_options.scope != null && _options.scope.length > 0) {
                                if (_options.scope.indexOf(attr) === -1) { incl = false; }
                            }

                            // include attribute in scope
                            if (incl === true) {

                                var text = attrs[attr].toLowerCase();

                                var _termMatch = strict === true ? true : false;

                                for (var t = 0; t < _input.terms.length; t++) {
                                    var term = _input.terms[t].toLowerCase();

                                    // approach: indexOf
                                    if (strict === true) {

                                        // comparison: and
                                        if (text.indexOf(term) === -1) {
                                            _termMatch = false;

                                            // break
                                            t = _input.terms.length;
                                        }

                                    } else {

                                        // comparison: or
                                        if (text.indexOf(term) !== -1) {
                                            _termMatch = true;

                                            // break
                                            t = _input.terms.length;
                                        }
                                    }
                                }

                                // found
                                if (_termMatch === true) {
                                    match = true;
                                }
                            }
                        }
                    }

                    // hide/show
                    if (match === true) {
                        item.removeClass('hidden');
                    } else {
                        item.addClass('hidden');
                    }
                }
            });

        }

        // helper: extracts all data attributes
        function getDataAttributes(el) {
            return el.dataset || [].slice.call(el.attributes).reduce(function (o, a) {
                return /^data-/.test(a.name) && (o[a.name.substr(5)] = a.value), o;
            }, {});
        }

        return _handle;
    }

    // UI component: upload
    $.fn.thUpload = function (options) {

        var element = $(this);
        var _options = $.extend({}, options);
        var _observers = null;

        // handle
        var _handle = { element: element, selector: element.selector, options: _options };

        // func: bind
        _handle.bind = function () {

            var uTempUpload = '/io/upload/temp';
            var sUploadInput = '[data-upload=input]';
            var sFiles = '[data-upload=files]';
            var sProgress = '[data-upload=progress]';
            var sProgressBar = '[data-upload=progress-bar]';

            $(document).ready(function () {
                'use strict';

                if (typeof $.fn.fileupload !== 'undefined') {

                    // FileUpload => Settings
                    var fus = {
                        dataType: 'json',
                        url: uTempUpload,
                        filesContainer: element.find(sFiles),
                        uploadTemplateId: null,
                        downloadTemplateId: null,
                        progressInterval: 25,
                        autoUpload: true,
                        sequentialUploads: true
                    };

                    // plugin => progressall
                    fus['progressall'] = function (e, data) {
                        var progress = parseInt(data.loaded / data.total * 100, 10);
                        //console.log('progressall', progress + '%');
                        //console.log('progressall', {e: e, data: data });

                        element.find(sProgress).removeClass('hidden');
                        element.find(sProgressBar).css('width', progress + '%');

                        // reset after last upload
                        if (progress >= 100) {
                            window.setTimeout(function () {
                                element.find(sProgressBar).css('width', 0 + '%');
                                element.find(sProgress).addClass('hidden');
                            }, 500);
                        }
                    }

                    // plugin => progress
                    fus['progress'] = function (e, data) {

                        var progress = parseInt(data.loaded / data.total * 100, 10);
                        //console.log('progress', progress + '%');
                        //console.log('progress', { e: e, data: data });
                        //var index = data.context.index();

                        //data.context.find('.progress-bar').css('width', progress + '%');
                        //data.context.find('.progress-status').text(progress + '%');
                    }

                    // plugin => done (success/error)
                    fus['done'] = function (e, data) {
                        //console.log('done', { e: e, data: data });

                        if (data.result != null && data.result.files != null && data.result.files.length > 0) {
                            // success
                            for (var i = 0; i < data.result.files.length; i++) {
                                var file = data.result.files[i];

                                /* file:
                                    deleteType: "DELETE"
                                    deleteUrl: null
                                    name: "example.png"
                                    size: 64512
                                    thumbnailUrl: "/resources/images/thumb-file.png"
                                    url: "/io/download/temp/xxx.png"
                                    xExtension: "png"
                                    xName: "xxx"
                                    xOriginalName: "example"
                                    xSuccess: [true/false]
                                 */

                                // notify
                                if (file.xSuccess === true) {

                                    file['format'] = ('fr|' + file.xName + '|' + file.xExtension + '|' + file.xOriginalName);
                                    notify('upload', file, { e: e, data: data });
                                }
                            }
                        }
                    }


                    // plugin => init
                    element.find(sUploadInput).fileupload(fus);
                }
            });
        }

        // func: observe manipulation
        _handle.observe = function (func) {

            var observer = { func: func }; // add additional conditions

            if (_observers == null) { _observers = []; }
            _observers.push(observer);
        }

        // helper: notify observer
        function notify(event, file, raw) {

            if (_observers == null) { return; }

            for (var i = 0; i < _observers.length; i++) {
                var observer = _observers[i];

                observer.func({ event: event, file: file, raw: raw });
            }
        }

        return _handle;
    }

    // UI component: selector
    $.fn.thSelector = function (options) {

        var element = $(this);
        var _options = $.extend({
            autocomplete: null,         // autocomplete for text input
        }, options);
        var _observers = null;

        // selectors
        var sGuide = '[data-selector=guide]';
        var sUserInputLink = '[data-selector=user-input-link]';
        var sUserInputText = '[data-selector=user-input-text]';
        var sUserSelectionUL = '[data-selector=user-selection] ul';
        var sItem = '[data-selector=item]';
        var sRemove = '[data-selector=remove]';

        // DOM
        var nUserInputText = $(element.selector + ' ' + sUserInputText);
        var userInputText = nUserInputText.length > 0;

        // handle
        var _handle = { element: element, selector: element.selector, options: _options };

        // func: toggle (input, link, guide, ..)
        _handle.toggle = function (unit, value) {

            var node = null;

            if (unit == 'guide') {
                node = $(_handle.selector + ' ' + sGuide);
            } else if (unit == 'link') {
                node = $(_handle.selector + ' ' + sUserInputLink);
            } else if (unit == 'text') {
                node = $(_handle.selector + ' ' + sUserInputText);
            }

            if (node != null) {
                if (value === true) { node.removeClass('hidden'); }
                else if (value === false) { node.addClass('hidden'); }
            }
        }

        // func: add (item)
        _handle.add = function (item) {

            var _item = $.extend({ id: null, icon: null, label: null, value: null }, item);

            var ul = element.find(sUserSelectionUL);
            if (ul.length > 0) {
                var h = itemHtml(_item);
                var el = $(h);

                // add to list
                var x = ul.append(el);
            }
        }

        // func: returns the selection
        _handle.selection = function () {
            var list = [];
            var ul = element.find(sUserSelectionUL);
            if (ul.length > 0) {

                ul.find(sItem).each(function (i, v) {
                    var node = $(this);
                    var _item = htmlToItem(node);
                    list.push(_item);
                });
            }
            return list;
        }

        // func: clear (text, selection)
        _handle.clear = function (options) {

            // options
            var _options = { clearText: false, clearItems: false };

            if (options == null) {
                _options.clearText = true;
                _options.clearItems = true;
            } else if (options == 'text') {
                _options.clearText = true;
            } else if (options == 'items') {
                _options.clearItems = true;
            }

            // clear: text
            if (_options.clearText === true) {
                if (userInputText === true) {
                    nUserInputText.find('input').val('');
                }
            }

            // clear: items
            if (_options.clearItems === true) {

                var items = $(element.selector + ' ' + sUserSelectionUL + ' ' + sItem);

                items.each(function (index, value) {
                    var el = $(this);
                    var _item = htmlToItem(el);
                    notify('remove', 'item', el, _item);
                    el.remove();
                });

                notify('remove', 'items', null, null);
            }
        }

        // func: observe manipulation
        _handle.observe = function (func) {

            var observer = { func: func }; // add additional conditions

            if (_observers == null) { _observers = []; }
            _observers.push(observer);
        }


        // bind: user-input-link
        $(document).off('click', _handle.selector + ' ' + sUserInputLink);
        $(document).on('click', _handle.selector + ' ' + sUserInputLink, function (e) {

            e.stopPropagation();

            var btn = $(this);

            notify('click', 'link', btn, null);
        });

        // bind: user-input-text
        if (userInputText === true) {
            var nInput = nUserInputText.find('input');
            var tempAdd = null;

            nInput.bind("enterKey", function (e) {
                //do stuff here
                if (tempAdd != null) {
                    _handle.add(tempAdd);
                    tempAdd = null;
                } else {
                    var text = this.value;
                    _handle.add({ id: text, label: text, value: text });
                }
                this.value = '';

            });
            nInput.keyup(function (e) {
                if (e.keyCode == 13) {
                    $(this).trigger("enterKey");
                }
            });

            // bind: autocomplete
            if (_options.autocomplete != null) {
                if (userInputText === true) {

                    // options: autocomplete
                    var _ac = $.extend({
                        url: null,
                        parser: null,
                        minLength: 2,
                    }, _options.autocomplete);

                    // ajax
                    if (_ac.url != null) {
                        var ac = $(nInput).autocomplete({
                            classes: { 'ui-autocomplete': 'mxui-selector-autocomplete' },
                            minLength: _ac.minLength,

                            source: function (request, callback) {
                                MX.ajax.postJSON({
                                    url: _ac.url, data: { term: request.term },
                                    success: function (response) {
                                        if (_ac.parser != null) {

                                            var parsedArray = _ac.parser(response); // array of selector item objects

                                            var acArray = [];
                                            for (var i = 0; i < parsedArray.length; i++) {
                                                var pi = parsedArray[i];
                                                acArray.push({ id: pi.id, icon: pi.icon, label: pi.label, value: pi.value });
                                            }
                                            // [ { label: "Choice1", value: "value1" }, ... ]
                                            callback(acArray);
                                        }
                                    },
                                    error: function () {
                                        callback();
                                    }
                                });

                            },
                            select: function (event, ui) {
                                nInput.val(ui.item.value);
                                tempAdd = ui.item;
                                return true;
                            }
                        });
                    }
                }
            }
        }

        // bind: remove-item
        $(document).off('click', _handle.selector + ' ' + sItem + ' ' + sRemove);
        $(document).on('click', _handle.selector + ' ' + sItem + ' ' + sRemove, function (e) {

            e.stopPropagation();

            var el = $(this).closest(sItem);
            var _item = htmlToItem(el);
            notify('remove', 'item', el, _item);
            el.remove();
        });


        // helper: notify observer
        function notify(event, unit, element, item) {

            if (_observers == null) { return; }

            for (var i = 0; i < _observers.length; i++) {
                var observer = _observers[i];

                observer.func({ event: event, unit: unit, element: element, item: item });
            }
        }

        // helper: creates item html string
        function itemHtml(item) {
            var h = '<li data-selector="item" data-id="' + (item.id != null ? item.id : '') + '" data-value="' + (item.value != null ? item.value : '') + '"><div class="item">';
            if (item.icon != null && item.icon.length) {
                h += '<div class="icon" data-selector="icon"><i class="' + fixIcon(item.icon) + '"></i></div>'
            }
            h += '<div class="label" data-selector="label" title="' + (item.label != null ? item.label : '') + '">' + (item.label != null ? item.label : '') + '</div>';
            h += '<div class="remove" data-selector="remove"><i class="fas fa-times"></i></div>';
            h += '</div></li>';
            return h;
        }

        // helper: converts html item to object
        function htmlToItem(node) {
            var id = node.attr('data-id');
            if (id == null || id.length == 0) { id = null; }
            var value = node.attr('data-value');
            if (value == null || value.length == 0) { value = null; }
            var label = node.find('[data-selector=label]').text();
            if (label == null || label.length == 0) { label = null; }
            var icon = node.find('[data-selector=icon] i');
            if (icon == null || icon.length == 0) { icon = null; }
            if (icon != null) {
                icon = icon[0].className;
                if (icon.indexOf('fa ') == 0) { icon = icon.replace('fa ', ''); }
            }
            var _item = { id: id, icon: icon, label: label, value: value };

            // func: update
            _item.update = function (props) {
                updateItem(node, props);
            }

            return _item;
        }

        // helper: updates specified properties
        function updateItem(node, props) {

            if (node == null || props == null) return;

            // id
            if (props.hasOwnProperty('id')) {
                var _id = props['id'] != null ? props['id'] : '';
                node.attr('data-id', _id);
            }

            // label
            if (props.hasOwnProperty('label')) {
                var _label = props['label'] != null ? props['label'] : '';
                node.find('[data-selector=label]').text(_label);
                node.find('[data-selector=label]').attr('title', _label);
            }

            // value
            if (props.hasOwnProperty('value')) {
                var _value = props['value'] != null ? props['value'] : '';
                node.attr('data-value', _value);
            }

            // icon
            if (props.hasOwnProperty('icon')) {
                var _icon = props['icon'] != null ? props['icon'] : '';
                var ni = node.find('[data-selector=icon] i');
                if (ni.length > 0) {
                    ni[0].className = fixIcon(_icon);
                }
            }
        }

        // helper: fixes icon pre/suffixes
        function fixIcon(icon) {
            var out = icon;
            if (icon != null && icon.length > 0) {
                if (icon.indexOf('fa-') == 0) { out = 'fa ' + out; }
            }
            return out;
        }

        return _handle;
    }

    // UI component: dropdown
    $.fn.thDropdown = function (options) {

        var element = $(this);
        var _options = $.extend({
            change: null        // event: change (item)
        }, options);


        // handle
        var _handle = { element: element, selector: element.selector, options: _options };

        // func: current
        _handle.current = function () {
            return parseCurrent(_handle.element);
        }

        // bind: click
        $(element).off('click');
        $(element).click(function (e) {
            //console.log('target', e);

            var nDropdown = null;
            var nOption = null;

            // origin
            if (e.target.dataset['mxui'] == 'dropdown') {
                nDropdown = $(e.target);
            } else {
                nDropdown = $(e.target).closest('[data-mxui=dropdown]');

                // click: option
                if (e.target.dataset['hook'] == 'option') {
                    nOption = $(e.target);
                } else if (e.target.parentNode.dataset['hook'] == 'option') {
                    nOption = $(e.target).closest('[data-hook=option]');
                }
            }

            // state
            var hasFocus = nDropdown.hasClass('focus');
            var open = !hasFocus;

            // toggle
            if (hasFocus) {
                nDropdown.removeClass('focus');
            } else {
                nDropdown.addClass('focus');
            }

            // open
            if (open === true) {

                // check current value/label
                var objCurrent = parseCurrent(nDropdown);

                // display current
                var oCurrent = nDropdown.find('[data-hook=options] [data-hook=current]');
                oCurrent.attr('data-value', objCurrent.value);
                oCurrent.find('[data-prop=label]').html(objCurrent.label);

                // reset options
                nDropdown.find('[data-hook=options] [data-hook=option]').removeClass('hidden');

                // hide current option
                if (objCurrent.value != null) {
                    var cOption = nDropdown.find('[data-hook=options] [data-hook=option][data-value="' + objCurrent.value + '"]');
                    if (cOption.length > 0) {
                        cOption.addClass('hidden');
                    }
                }

                // calculated approach, but differs from css styles...
                //var bordT = el.outerWidth() - el.innerWidth();
                //var paddT = el.innerWidth() - el.width();
                //var margT = el.outerWidth(true) - el.outerWidth()

                // positioning (using jqueryui)
                var extraOffsetTop = parseInt(oCurrent.css('padding-top'));
                nDropdown.find('[data-hook=options]').position({
                    my: 'right top-' + extraOffsetTop,
                    at: 'right top',
                    of: nDropdown
                });

                bindBlur(nDropdown);

                // helper: bind blur
                function bindBlur(node) {

                    // bind: blur (one time)
                    $(document).one('click', function (ed) {

                        var close = false;

                        var _ndd = ed.target == nDropdown[0] ? $(ed.target) : $(ed.target).closest('[data-mxui=dropdown]');
                        if (_ndd.length > 0) {
                            // note: clicked inside a dropdown
                            if (_ndd[0] == nDropdown[0]) {
                                // same ...
                            } else {
                                close = true;
                            }
                        } else {
                            close = true;
                        }


                        // close
                        if (close === true) {
                            nDropdown.removeClass('focus');
                        } else {
                            // if dropdown still has focus > rebind
                            if (nDropdown.hasClass('focus')) {
                                bindBlur(nDropdown);
                            }
                        }
                    });
                }
            }

            // click
            if (hasFocus === true && nOption != null) {

                // data
                var objCurrent = parseCurrent(nDropdown);
                var objOption = parseOption(nOption);

                // reflect
                reflectCurrent(nDropdown, objOption);

                // change
                if (_handle.options.change != null) {

                    // trigger
                    // param1: option
                    // param2: current
                    _handle.options.change(objOption, objCurrent);
                }
            }



            // helper: parse option (from option)
            function parseOption(node) {
                var v = node.attr('data-value');
                if (v == undefined) { v = null; }
                var l = node.children('[data-prop=label]').html();

                return { value: v, label: l };
            }

            // helper: reflect new data on current node
            function reflectCurrent(node, obj) {
                var curNode = node.children('[data-hook=current]');
                if (curNode.length > 0) {
                    curNode.attr('data-value', obj.value);
                    curNode.find('[data-prop=label]').html(obj.label);
                }
            }

        });

        // helper: parse current (from dropdown)
        function parseCurrent(node) {
            var curNode = node.children('[data-hook=current]');
            var cValue = curNode.attr('data-value');
            if (cValue == undefined) { cValue = null; }
            var cLabel = curNode.children('[data-prop=label]').html();

            return { value: cValue, label: cLabel };
        }

        return _handle;
    }

    // UI component: steps
    $.fn.thSteps = function (options) {

        var element = $(this);
        var _options = $.extend({
        }, options);


        // handle
        var _handle = { element: element, selector: element.selector, options: _options };

        // func: progress (get/set)
        _handle.progress = function (step) {

            // set
            if (step != undefined && step.length > 0) {

                // reflect target node
                var node = $(_handle.selector + ' [data-key=' + step + ']');
                node.addClass('active');
                node.removeClass('complete');

                // reflect next nodes
                var nextAll = node.nextAll();
                nextAll.removeClass('active');
                nextAll.removeClass('complete');

                // reflect prev nodes
                var prevAll = node.prevAll();
                prevAll.removeClass('active');
                prevAll.addClass('complete');
            }
            // get
            else {
                var node = $(_handle.selector + ' .active[data-key]');
                if (node.length == 0) return null;
                return itemObj(node);
            }
        }

        // helper: item object
        function itemObj(_node) {

            // props
            var item = decorate(_node);
            if (item == null) return null;

            // method: prev
            item.prev = function () {
                return decorate(item.node.prev());
            }

            // method: next
            item.next = function () {
                return decorate(item.node.next());
            }

            function decorate(node) {
                if (node == null || node.length == 0) return null;

                return {
                    node: node,
                    key: node.attr('data-key'),
                    index: node.find('.index').text(),
                    label: node.find('.label').text(),
                    active: node.hasClass('active'),
                    complete: node.hasClass('complete')
                };
            }

            return item;
        }

        return _handle;
    }

    // UI component: toolbar
    $.fn.thToolbar = function (options) {

        var element = $(this);

        //var ul = container.find('ul');
        //if (ul.length == 0) return null;

        // options
        var _options = $.extend({
            //click: null
        }, options);

        // handle
        var _handle = { element: element, selector: element.selector, options: _options, bindings: null };


        // func: bind   (bind different toolbar units)
        _handle.bind = function (unit, opt) {
            //console.log('func:bind', { unit: unit, opt: opt });

            // options
            var _opt = $.extend({
                click: null,
                reflect: null
            }, opt);

            // binding
            var binding = {
                unit: unit,
                options: _opt,
            };

            // binding: display-switch
            if (binding.unit == 'display-switch') {

                // node
                binding['node'] = $(_handle.selector + ' [data-unit=' + unit + ']');

                // bind: click
                if (binding.options.click != null) {

                    // unit: option
                    binding.node.find('[data-unit=option]').click(function (e) {
                        var el = $(this);
                        var _key = el.attr('data-key');
                        if (_key == null || _key.length == 0) { _key = null; }

                        var _obj = { key: _key };

                        //console.log('binding', binding);
                        //console.log('click', { opt: opt, e: e });

                        // reflect: change button to reflect clicked option
                        if (binding.options.reflect == true) {
                            var btn = el.closest('[data-unit=display-switch]').find('[data-unit=button]');
                            btn.html(el.html());
                        }

                        // callback
                        binding.options.click(_obj);
                    });
                }
            }

            // add to collection
            if (_handle.bindings == null) {
                _handle.bindings = [];
            }
            _handle.bindings.push(binding);

            return binding;
        }

        _handle.reflect = function (unit, key, callback) {
            if (unit == 'display-switch') {
                var node = $(_handle.selector + ' [data-unit=' + unit + ']');
                var btn = node.find('[data-unit=button]');
                var option = node.find('[data-unit=options]').find('[data-key=' + key + ']');
                btn.html(option.html());
            }

            if (callback != null && callback != undefined) {
                callback();
            }
        }

        return _handle;
    }

    // UI component: pipeline
    $.fn.thPipeline = function (options) {

        var element = $(this);

        // options
        var _options = protoSettings(options);

        // handle
        var _handle = {
            element: element,
            selector: element.selector,
            options: _options,
            //bindings: null
        };

        // init: bindings
        fnBindings();


        // method: reload pipeline
        _handle.reload = function (args) {

            var _args = $.extend({
                full: null,             // [bool]
                ajax: null              // [object]
            }, args);

            // if ajax specified, execute it first
            if (_args.ajax != null) {

                var _callback = function (response) {
                    // response.data
                    // response.html
                    if (response != null) {
                        if (response.html != null) {
                            refreshHtml(response.html, _args.full);
                        }
                    }

                }
                // execute
                _args.ajax(_callback);
            }

            function refreshHtml(_html, _full) {

                if (_full === true) {
                    _handle.element.replaceWith(_html);
                    _handle.element = $(_handle.selector);

                    fnBindings();
                }

            }
        }

        // method: update counter
        _handle.updateCounter = function (args) {
            var _args = $.extend({
                stage: null,
                counter: null,
                action: null
            }, args)

            var currentCounter = $(_handle.selector + ' [data-hook=pipeline-stage][data-id=' + _args.stage + '] [data-hook=pipeline-counter]');

            if (_args.action == 'add') {
                currentCounter.html(parseInt(currentCounter.text()) + parseInt(_args.counter));
            }
            else if (_args.action == 'update') {
                currentCounter.html(_args.counter);
            }
        }

        // method: get current amounts in overview
        _handle.getCurrentEntries = function () {
            var stageEntries = [];

            var stages = $(_handle.selector + ' [data-hook=pipeline-stage]');

            for (var i = 0; i < stages.length; i++) {

                var stageId = $(stages[i]).attr('data-id');
                var entries = $(_handle.selector + ' [data-hook=pipeline-stage][data-id=' + stageId + '] [data-hook=pipeline-entry]').length;
                stageEntries.push({ stage: stageId, entries: entries });
            }

            return stageEntries;
        }

        // helper: force bindings
        function fnBindings() {

            // bind: entry-click
            if (_handle.options.entry.selector != null) {

                $(document).off('click', _handle.selector + ' ' + _handle.options.entry.selector);
                $(document).on('click', _handle.selector + ' ' + _handle.options.entry.selector, function (e) {

                    var node = $(this);
                    var container = _handle.element;

                    var entry = null; // the clicked item
                    var active = null; // indicator if the item is active or not
                    var selection = []; // the current selection of active items


                    // behaviour: visited
                    if (_handle.options.entry.visited === true) {

                        container.find(_handle.options.entry.selector).removeClass('visited');

                        node.addClass('visited');
                    }

                    // reference object
                    entry = protoEntry(node);

                    // callback
                    if (_handle.options.entry.click != null) {
                        _handle.options.entry.click(entry);
                    }
                });
            }

            // Handle scroll      
            if (_handle.options.scroll != null) {
                $(_handle.selector + ' [data-hook=pipeline-stage-entries]').scroll(function () {

                    var scrollHeight = $(this).scrollTop() + $(this).innerHeight();
                    var scrollTriggerHeight = $(this).prop('scrollHeight') - 200;

                    //console.log({ element: $(this), scroll: scrollHeight, height: scrollTriggerHeight });

                    if (scrollHeight >= scrollTriggerHeight) {
                        var stageId = $(this).parent('[data-hook=pipeline-stage]').data('id');
                        var current = $(this).children('[data-hook=pipeline-entry]').length;

                        // Callback
                        _handle.options.scroll.action($(this), stageId, current);
                    }

                });
            }

            // bind: entry-move
            if (_handle.options.entry.move != null) {

                //========================

                var plugin_sortable = typeof $.fn.sortable !== 'undefined';
                var plugin_nestedSortable = typeof (jQuery.mjs.nestedSortable) != 'undefined';

                // only enable if plugin is available
                if (plugin_sortable === true) {

                    // selectors
                    var sPipeline = _handle.selector;
                    var sStages = '[data-hook=pipeline-stages]';
                    var sStage = '[data-hook=pipeline-stage]';
                    var sEntries = '[data-hook=pipeline-stage-entries]';
                    var sEntry = _handle.options.entry.selector;
                    var sEntrySort = sEntry; // '[data-hook=sort-item]'
                    var sPush = '[data-hook=pipeline-push]';

                    // settings
                    var _maxDepth = 1;
                    var useNestedSortable = _maxDepth != 1;
                    var multiColumns = true;//_columns.length > 1;


                    //========================================

                    // set stretchers
                    //var useStretchers = _columns.length > 1;
                    //if (useStretchers) { enableListStretchers(); }

                    // sort: bind (options)
                    var sort_options = {
                        //handle: sEntrySort, // must be inner node
                        items: '> *',// '> div', //'li',
                        placeholder: 'pipeline-move-placeholder',
                        //opacity: 0.5, // opacity helper during moving
                        cursor: 'move',
                        tolerance: 'pointer',
                        //forcePlaceholderSize: true,
                        classes: {
                            'ui-sortable-helper': 'pipeline-move-helper'
                        },
                        update: function (event, ui) {

                            // ignore the second callback (happens with list exchange)
                            if (ui.sender != null) return;

                            // stabilize
                            stabilize();

                            // entry
                            var node_entry = ui.item;
                            var id = node_entry.attr('data-id');
                            var entry = protoEntry(node_entry);

                            // previous sibling/entry
                            var prevEntryId = null;
                            var prevEntry = node_entry.prevAll(sEntry).first();
                            if (prevEntry.length > 0) {
                                prevEntryId = prevEntry.attr('data-id');
                            }

                            // next sibling/entry
                            var nextEntryId = null;
                            var nextEntry = node_entry.nextAll(sEntry).first();
                            if (nextEntry.length > 0) {
                                nextEntryId = nextEntry.attr('data-id');
                            }

                            // stage
                            var stageId = node_entry.closest(sStage).attr('data-id');

                            // index
                            var index = node_entry.index();

                            // callback
                            if (_handle.options.entry.move != null) {

                                var func = _handle.options.entry.move;
                                var change = {
                                    entryId: id,
                                    stageId: stageId,
                                    prevEntryId: prevEntryId,
                                    nextEntryId: nextEntryId,
                                    //parentId: parentId,
                                    index: index
                                };

                                func(entry, change)
                            }

                            // re-set stretchers
                            //if (useStretchers) {
                            //    enableListStretchers(false);
                            //    enableListStretchers();
                            //}
                        },
                        //over: function(event, ui) { },
                        start: function (event, ui) {
                            $(sPipeline).addClass('pipeline-state-moving');
                        },
                        stop: function (event, ui) {

                            $(sPipeline).removeClass('pipeline-state-moving');
                        }
                    };

                    // set multi column options
                    if (multiColumns === true) {
                        sort_options.connectWith = sPipeline + ' ' + sEntries;
                        sort_options.containment = sPipeline + ' ' + sStages;
                    }

                    // sort: bind
                    if (plugin_nestedSortable === true && useNestedSortable === true) {

                        stabilize();

                        // not used

                        sort_options.toleranceElement = '> div';
                        sort_options.listType = 'ul';
                        sort_options.maxLevels = _maxDepth;
                        sort_options.isAllowed = function (item, parent) {
                            // event function (gets triggered a lot, so dont use for server calls)
                            return true;
                        };

                        // nestedSortable
                        $(sPipeline + ' ' + sEntries).nestedSortable(sort_options);

                    } else if (plugin_sortable === true) {

                        stabilize();

                        // default jquery ui
                        var y = $(sPipeline + ' ' + sEntries).sortable(sort_options);
                    }

                    function stabilize() {

                        if ($(sPipeline).hasClass('pipeline-state-flex') !== true) {

                            $(sPipeline + ' ' + sEntries).each(function (i, v) {
                                var node_entries = $(v);
                                node_entries.css('height', '');
                            });

                            $(sPipeline).addClass('pipeline-state-flex')
                        }


                        // extract from first stage (because of flexbox)
                        var node_entries_first = $($(sPipeline + ' ' + sEntries)[0]);
                        var node_stage_first = $($(sPipeline + ' ' + sStage)[0]);

                        var h = node_entries_first.outerHeight();

                        /*
                        var node_push = node_entries_first.siblings(sPush);
                        node_push.removeClass('hidden');
                        h += node_push.outerHeight();
                        node_push.addClass('hidden');
                        */
                        //node_entries_first.css('height', h + 'px');

                        var node_head = $(sPipeline + ' ' + sStage + ' .head')[0];
                        if (node_head != null && node_head != undefined) {
                            h = (h - node_head.clientHeight * 1.5);
                        }


                        // change state
                        $(sPipeline).removeClass('pipeline-state-flex');

                        $(sPipeline + ' ' + sEntries).each(function (i, v) {

                            var node_entries = $(v);
                            //var h = node_entries.outerHeight();
                            //var node_push = node_entries.siblings(sPush);
                            //node_push.removeClass('hidden');
                            //h += node_push.outerHeight();
                            //node_push.addClass('hidden');
                            node_entries.css('height', h + 'px');
                        });
                    }
                }






                //==================
            }
        }


        // prototype: settings
        function protoSettings(opt) {

            var defaults = {
                entry: {                                        // [object]     Entry behaviour
                    click: null,                                // [func]       - click event   [params: entry]
                    move: null,                                 // [func]       - move event    [params: entry, change]
                    selector: '[data-hook=pipeline-entry]',     // [string]     - selector (-override)
                    visited: null                               // [bool]       - allow visited state after click   
                },
                stage: {                    // [object]     Stage behaviour
                    click: null,            // [string]     - empty/ no-items
                },
                source: {                   // [object]     Source
                    //
                },
                scroll: {
                    action: null
                }
            }

            // proto
            var o = {
                '_proto': 'settings'
            };

            // merge defaults and options, without modifying defaults
            //$.extend({}, defaults);

            o['entry'] = $.extend({}, defaults['entry'], opt['entry']);

            o['stage'] = $.extend({}, defaults['stage'], opt['stage']);

            o['source'] = $.extend({}, defaults['source'], opt['source']);

            o['scroll'] = $.extend({}, defaults['scroll'], opt['scroll']);


            return o;
        }

        // prototype: entry
        function protoEntry(arg) {

            var createDefault = false;
            var parseNode = false;
            var node = null;

            if (arg == null || arg == 'default') {
                createDefault = true;
            } else {
                parseNode = true;
                node = arg;
            }


            var _id = null;
            var _label = null;
            var _caption = null;

            if (parseNode === true) {

                _id = node.attr('data-id');
                if (_id == undefined || _id.length == 0) { _id = null; }

                _label = node.find('[data-bind=label]').text();
                if (_label == undefined || _label.length == 0) { _label = null; }

                _caption = node.find('[data-bind=caption]').text();
                if (_caption == undefined || _caption.length == 0) { _caption = null; }

                //var _image = node.find('[data-bind=image] > img').prop('src');
                //if (_image == undefined || _image.length == 0) { _image = null; }
            }



            // proto
            var o = {
                '_proto': 'entry',
                '_id': _id,
                '_label': _label,
                '_caption': _caption,
                node: node
            };

            // func: property
            o.prop = function (name) {
                return o['_' + name];
            }

            // func: hidden state
            o.hidden = function (state) {
                if (state != undefined) {
                    if (state === true) {
                        o.node.addClass('hidden');
                    } else if (state === false) {
                        o.node.removeClass('hidden');
                    }
                } else {
                    return o.node.hasClass('hidden');
                }
            }

            return o;
        }


        return _handle;
    }



    //=== Helpers

    // function: calculate visible height
    $.fn.visibleHeight = function () {
        var elBottom, elTop, scrollBot, scrollTop, visibleBottom, visibleTop;
        scrollTop = $(window).scrollTop();
        scrollBot = scrollTop + $(window).height();
        elTop = this.offset().top;
        elBottom = elTop + this.outerHeight();
        visibleTop = elTop < scrollTop ? scrollTop : elTop;
        visibleBottom = elBottom > scrollBot ? scrollBot : elBottom;
        return visibleBottom - visibleTop;
    }

    // function: find scrollable parent
    $.fn.scrollParent = function () {
        var overflowRegex = /(auto|scroll)/,
            position = this.css("position"),
            excludeStaticParent = position === "absolute",
            scrollParent = this.parents().filter(function () {
                var parent = $(this);
                if (excludeStaticParent && parent.css("position") === "static") {
                    return false;
                }
                return (overflowRegex).test(parent.css("overflow") + parent.css("overflow-y") + parent.css("overflow-x"));
            }).eq(0);

        return position === "fixed" || !scrollParent.length ? $(this[0].ownerDocument || document) : scrollParent;
    };

    // function: center element in window
    $.fn.center = function () {
        this.css("position", "absolute");
        this.css("top", Math.max(0, (($(window).height() - $(this).outerHeight()) / 2) +
            $(window).scrollTop()) + "px");
        this.css("left", Math.max(0, (($(window).width() - $(this).outerWidth()) / 2) +
            $(window).scrollLeft()) + "px");
        return this;
    }

    // function: adds an event delay for input/textareas (change, keyup)
    $.fn.searchDelay = function (options) {

        var _options = $.extend({
            delay: 400,
            callback: null
        }, options);

        if (_options.callback == null) { return; }

        var elements = $(this);

        var searchTimeout;
        var lastSearchTerm;
        var filterTextEvent = function (e) {

            var element = $(this);

            // 1. When a key is pressed:
            //      1. Check if there's an existing timer - stop it if there is one
            //      2. start a timer.
            // 2. When the timer expires, call the server method.
            if (searchTimeout != undefined) { clearTimeout(searchTimeout); }
            searchTimeout = setTimeout(function () {

                // search term
                var text = element.val();
                if (text == lastSearchTerm) return;

                // pin last search
                lastSearchTerm = text;

                // callback
                _options.callback(text, { text: text, event: e, element: element });

            }, _options.delay);
        }

        // bind
        elements.change(filterTextEvent);
        elements.keyup(filterTextEvent);
    }


    //=== Wrappers

    /*
     * MX Context Menu (webui-popover wrapper) (deprecated).
     * @param items The items to display.
     * @param options The context menu's options.
     * @param pluginOptions The webui-popover's settings.
     */
    $.fn.thContextMenu = function (items, options, pluginOptions) {

        //console.log('contextMenu', { items: items, options: options, pluginOptions: pluginOptions });

        var element = $(this);

        // command: destroy 
        if (items == 'destroy') {
            element.webuiPopover('destroy')
            return element;
        }

        /*
         * items:
         * [{ 
         *      label: '', 
         *      icon: '' or false
         *      attributes: {},
         * }]
         */

        // options
        var _options = $.extend({
            skin: null,
            attributes: {}
        }, options);

        // create list
        var ul = $('<ul></ul>');

        // create list items
        for (var i = 0; i < items.length; i++) {

            var first = i == 0;
            var last = i == (items.length - 1);

            var item = items[i];
            var attributes = (item.attributes != undefined) ? item.attributes : null;
            var label = (item.label != undefined) ? item.label : null;
            var icon = (item.icon != undefined) ? item.icon : null;

            // node
            var li = $('<li></li>');

            // icon
            if (icon != null) {
                li.append('<span class="icon ' + icon + '"></span>');
            }

            // label
            li.append('<span class="label">' + label + '</span>');

            // attributes
            if (attributes != null) {
                for (var attr in attributes) {
                    if (attributes.hasOwnProperty(attr)) {
                        li.attr(attr, attributes[attr]);
                    }
                }
            }

            // skin
            if (first) { li.addClass('first'); }
            if (last) { li.addClass('last'); }
            if (!first && !last) { li.addClass('between'); }

            // add
            ul.append(li);
        }

        // create content
        var html = $('<div class="mx-context-menu skin"></div>');
        html.append(ul);

        // content skin
        if (_options.skin != null) { html.addClass(_options.skin); }

        // content attributes
        var oAttr = _options.attributes != undefined ? _options.attributes : null;
        if (oAttr != null) {
            for (var attr in oAttr) {
                if (oAttr.hasOwnProperty(attr)) {
                    html.attr(attr, oAttr[attr]);
                }
            }
        }

        // initialize plugin
        element
            .webuiPopover('destroy')
            .webuiPopover({
                trigger: 'click',
                content: html[0].outerHTML,
                multi: false,
                closeable: false,
                delay: 300,
                padding: false,
                placement: 'bottom'
            }).webuiPopover('show');

        // set destroy on inner click (todo)
        //html.one('click', function () {
        //    $(this).thcontextmenu('destroy');
        //});

        return element;
    };

    $.fn.thColorPicker = function (options) {
        var element = $(this);
        var parent = element.parent();

        var _options = $.extend({
            'colors': ['#61bd4f', '#f2d600', '#ff9f1a', '#eb5a46', '#c377e0', '#0079bf', '#00c2e0', '#51e898', '#ff78cb', '#355263'],
            'enableInput': false
        }, options);

        if (!_options.enableInput) element.css('display', 'none');
        var colorPicker = $('<div class="ui-color-picker"></div>');

        for (var i = 0; i < _options.colors.length; i++) {
            var color = _options.colors[i];
            var colorOption = $('<div class="ui-color-option" data-color="' + color + '" style="background-color: ' + color + '"></div>');
            if (color === element.val()) colorOption.addClass('selected');

            colorPicker.append(colorOption);
        }

        colorPicker.children('.ui-color-option').on('click', function () {
            var colorOption = $(this);
            element.attr('value', colorOption.data('color'));

            // Set selected class
            colorPicker.children('.ui-color-option').removeClass('selected');
            $(this).addClass('selected');
        });

        parent.append(colorPicker);
    };

    /*
    $.fn.thPushNotification = function (path) {
        if (Notification.permission == 'granted') {
            navigator.serviceWorker.getRegistration().then(function (reg) {
                var options = {
                    body: 'Here is a notification body!',
                    icon: 'images/example.png',
                    vibrate: [100, 50, 100],
                    data: {
                        dateOfArrival: Date.now(),
                        primaryKey: 1
                    }
                };
                reg.showNotification('Hello world!', options);
            });
        }
    };
*/

}(jQuery));


//=== Selector-less

/*
 * Popover (selector-less).
 * @param options
 */
function mxPopover(options) {

    var _options = fnSettings(options);


    // handle
    var _handle = $.isWindow(this) ? {} : this;

    // set: id
    var _id = _options.dom != null ? _options.dom.id : null;
    if (_id == null) { _id = 'popover_' + $('[data-tool=popover]').length; } // not safe
    _handle.id = _id;

    // set: selector
    var selector = '#' + _id + '[data-tool=popover]';
    _handle.selector = selector;

    // set: triggers
    _handle.clickTriggers = null;
    if (_options.click != null && _options.click.trigger != null) {
        if ($.isArray(_options.click.trigger) && _options.click.trigger.length > 0) {
            if (_options.click.trigger.length > 0) {
                _handle.clickTriggers = _options.click.trigger;
            }
        }
        else {
            _handle.clickTriggers = [_options.click.trigger];
        }
    }
    if (_options.hover != null && _options.hover.trigger != null) {
        if ($.isArray(_options.hover.trigger) && _options.hover.trigger.length > 0) {
            if (_options.hover.trigger.length > 0) {
                _handle.hoverTriggers = _options.hover.trigger;
            }
        }
        else {
            _handle.hoverTriggers = [_options.hover.trigger];
        }
    }

    // bind: triggers
    if (_handle.clickTriggers != null) {
        for (var i = 0; i < _handle.clickTriggers.length; i++) {
            var t = _handle.clickTriggers[i];

            $(t).on('click', null, function () {
                _handle.show();
            });
        }
    }
    if (_handle.hoverTriggers != null) {
        for (var i = 0; i < _handle.hoverTriggers.length; i++) {
            var t = _handle.hoverTriggers[i];

            $(t).mouseenter(function () {
                _handle.show();


                $(t).mouseleave(function (e) {
                    //console.log('leave', e);

                    var enteredPopover = false;

                    $(_handle.selector).mouseenter(function () {
                        enteredPopover = true;
                    });
                    $(_handle.selector).mouseleave(function () {
                        enteredPopover = false

                        _handle.hide();
                        $(t).off('mouseleave');
                        $(_handle.selector).off('mouseenter');
                        $(_handle.selector).off('mouseleave');
                    });

                    window.setTimeout(function () {
                        if (enteredPopover == false) {

                            _handle.hide();
                            $(t).off('mouseleave');
                            $(_handle.selector).off('mouseenter');
                            $(_handle.selector).off('mouseleave');
                        }
                    }, 200);

                });
            });

        }
    }


    // function: show/render popover
    _handle.show = function () {

        // check if popover already exists in the DOM
        if ($(selector).length > 0) { $(selector).remove(); }

        // class
        var _class = 'tool-popover no-user-select';
        if (_options.dom != null && _options.dom.class != null) { _class += ' ' + _options.dom.class; }

        // style
        var _style = '';
        if (_options.dom != null && _options.dom.style != null) {
            for (var p in _options.dom.style) {
                if (_options.dom.style.hasOwnProperty(p)) {
                    style += p + ':' + _options.dom.style[p];
                }
            }
        }

        //=== html

        // root
        var h = '<div data-tool="popover" id="' + _id + '" class="' + _class + '" style="' + _style + '"';

        // root: attributes
        if (_options.dom != null && _options.dom.attributes != null) {
            for (var p in _options.dom.attributes) {
                if (_options.dom.attributes.hasOwnProperty(p) && (p != 'id' && p != 'class' && p != 'style')) {
                    h += ' ' + p + '="' + _options.dom.attributes[p] + '"';
                }
            }
        }

        // root
        h += '>';

        // content
        var _classContent = 'content';
        if (_options.effects != null && _options.effects.indexOf('shadow') >= 0) {
            _classContent += ' shadow';
        }
        h += '<div class="' + _classContent + '">';

        // header: title
        if (_options.text != null && _options.text.title != null) {
            h += '<div class="head"><div class="title">' + _options.text.title + '</div></div>';
        }

        // body
        if (_options.content != null && (_options.content.items != null || _options.content.html != null)) {

            h += '<div class="body" data-hook="body">';

            // search
            if (_options.search != null && _options.search.enabled === true) {
                h += '<div class="search border" data-hook="search">';
                h += '<input type="text" placeholder="' + (_options.search.placeholder != null ? _options.search.placeholder : '') + '" />';

                if (_options.search.actions != null) {

                    h += '<div class="actions">';

                    for (var a = 0; a < _options.search.actions.length; a++) {
                        var action = _options.search.actions[a];
                        var actionData = '';

                        if (action.data != null) {
                            for (var d = 0; d < action.data.length; d++) {
                                actionData += 'data-' + action.data[d].key + '="' + action.data[d].value + '"';
                            }
                        }
                        h += '<div class="action">';
                        h += '<i class="' + action.icon + '" ' + actionData + '></i>';
                        h += '</div>';
                    }

                    h += '</div>';
                }

                h += '</div>';
            }

            // body: items
            if (_options.content.items != null) {

                // empty
                if (_options.content.items.length == 0) {
                    if (_options.text != null && _options.text.empty != null) {
                        h += '<div class="empty">' + _options.text.empty + '</div>';
                    }
                }

                // items
                if (_options.content.items.length > 0) {

                    h += '<ul class="items">';

                    for (var i = 0; i < _options.content.items.length; i++) {
                        var item = defItem(_options.content.items[i]);

                        if (item.type == 'divider') {
                            h += '<li class="divider"></li>';
                        } else {
                            // item
                            var _itemClass = 'item';
                            if (item.type == 'link') { _itemClass += ' link'; }

                            h += '<li data-hook="popover-item" class="' + _itemClass + '" data-id="' + (item.id != null ? item.id : '') + '" data-value="' + (item.value != null ? item.value : '') + '" data-search="' + (item.searchable != null ? '1' : '0') + '">';
                            if (item.type == 'link') { h += '<a href="' + item.url + '" target="' + item.target + '">'; }
                            if (item.icon != null) {
                                h += '<i class="icon ' + (item.icon.indexOf('fa') == 0 ? 'fa ' : '') + (item.icon) + '"></i>';
                            }
                            if (item.text != null) {
                                if (item.icon != null) {
                                    h += '<span>' + item.text + '</span>';
                                } else {
                                    h += '<div>' + item.text + '</div>';
                                }
                            }
                            if (item.type == 'link') { h += '</a>'; }

                            if (item.actions != null) {

                                h += '<div class="actions">';

                                for (var ia = 0; ia < item.actions.length; ia++) {
                                    var itemAction = item.actions[ia];
                                    var ItemActionData = '';

                                    if (action.data != null) {
                                        for (var id = 0; id < itemAction.data.length; id++) {
                                            ItemActionData += 'data-' + itemAction.data[id].key + '="' + itemAction.data[id].value + '"';
                                        }
                                    }
                                    h += '<div class="action">';
                                    h += '<i class="' + itemAction.icon + '" ' + ItemActionData + '></i>';
                                    h += '</div>';
                                }

                                h += '</div>';
                            }
                            h += '</li>';
                        }
                    }
                    h += '</ul>';
                }
            }

            // body: html
            if (_options.content.html != null) {

                if (jQuery.isFunction(_options.content.html)) {
                    h += _options.content.html();
                } else {
                    h += _options.content.html;
                }
            }

            h += '</div>';
        }

        // loader
        h += '<div class="loader ' + (_options.loader === true ? '' : 'hidden') + '" data-hook="loader"><div class="image"></div></div>';

        h += '</div>';

        h += '</div>';

        // append
        $('body').append(h);

        var node = $(selector);

        // position
        var _pos = _options.position;

        // properties for 'popping over'
        node.css('z-index', '1000');
        node.css("position", "absolute");

        // position: center
        if (_options.position == null || _options.position.type == null || _options.position.type == 'center') {

            var _window = $(window);
            node.css('top', Math.max(0, ((_window.height() - node.outerHeight()) / 2) + _window.scrollTop()) + "px");
            node.css('left', Math.max(0, ((_window.width() - node.outerWidth()) / 2) + _window.scrollLeft()) + "px");

        } else if (_options.position.type == 'relative') {

            var rnode = $(_options.position.to);

            // placement
            //var placement = 'bottom';// getPlacement(null, _options.relativeTo);
            var rpos = rnode.position();
            var rHeight = rnode.outerHeight(true);
            var rWidth = rnode.outerWidth(false);

            // offset
            var offsetTop = 0;
            var offsetLeft = 0;
            if (_options.position.offset != null) {
                if (_options.position.offset.top != null) { offsetTop = parseInt(_options.position.offset.top); }
                if (_options.position.offset.left != null) { offsetLeft = parseInt(_options.position.offset.left); }
            }

            // place            
            var jqueryUiLoaded = (typeof jQuery.ui !== 'undefined');
            if (jqueryUiLoaded) {

                // todo: apply offset

                // jquery ui implementation
                node.position({
                    my: 'right top' + (offsetTop != 0 ? '+' + offsetTop : ''),
                    at: "right bottom",
                    of: rnode,// this, // or $("#otherdiv)
                    collision: "fit"
                });
            } else {

                // fallback implementation
                if (_options.position.placement == 'top') { }
                else if (_options.position.placement == 'bottom') {

                    node.css('top', ((rpos.top + rHeight) + offsetTop) + 'px');
                    node.css('left', ((rpos.left) + offsetLeft) + 'px');

                } else if (_options.position.placement == 'left') {

                    node.css('top', ((rpos.top + rHeight) + offsetTop) + 'px');
                    node.css('left', ((rpos.left) + offsetLeft - node.width() - rnode.width()) + 'px');

                }
                else if (_options.position.placement == 'right') { }
            }


            // helper: get placement relative to node
            function getPlacement(context, source) {

                var position = $(source).position();

                if (position.left > 515) { return "left"; }

                if (position.left < 515) { return "right"; }

                if (position.top < 110) { return "bottom"; }

                if (position.top > 480) { return "top"; }

                return "top";
            }
        }


        // ...

        // bind: close (outside)
        var mup = $(document).on('mouseup.' + _handle.id, function (e) {
            var container = $(selector);
            if (container.hasClass('hidden')) return;

            if (!container.is(e.target) // if the target of the click isn't the container...
                && container.has(e.target).length === 0) // ... nor a descendant of the container
            {
                container.hide();
                $(document).off('mouseup.' + _handle.id);

                if (_options.events != null && _options.events.close != null) {
                    _options.events.close();
                }
            }
        });

        // bind: search
        if (options.search != null && options.search.enabled === true) {
            var nSearch = $(selector).find('[data-hook=search] > input');

            // set focus
            nSearch.focus();

            var filterTextEvent = function (e) {

                var element = $(this);
                var text = element.val().toLowerCase();
                var clear = text == null || text.length == 0;

                var searchableItems = $(selector).find('[data-hook=popover-item][data-search=1]');

                searchableItems.each(function (i, v) {
                    var nItem = $(v);
                    if (clear === true) {
                        nItem.removeClass('hidden');
                    } else {
                        if (nItem.text().toLowerCase().indexOf(text) >= 0) {
                            nItem.removeClass('hidden');
                        } else {
                            nItem.addClass('hidden');
                        }
                    }
                });
            }

            nSearch.change(filterTextEvent);
            nSearch.keyup(filterTextEvent);
        }

        // events
        if (_options.events != null) {

            // bind: item click
            if (_options.events.click != null) {
                $(selector).find('[data-hook=popover-item]').click(function () {

                    var dValue = $(this).attr('data-value');
                    var dId = $(this).attr('data-id');

                    var item = null;

                    if (dId.length > 0 && _options.content != null && _options.content.items != null && _options.content.items.length > 0) {
                        for (var i = 0; i < _options.content.items.length; i++) {
                            var oItem = _options.content.items[i];
                            if (oItem.id == dId) { item = oItem; break; }
                        }
                    }

                    _options.events.click(dValue, item);

                    if (_options.click != null && _options.click.hide == true) {
                        $(selector).remove();
                    }
                });
            }

            // execute: on open
            if (_options.events.open != null) {
                _options.events.open(_handle);
            }
        }
    }

    // function: hide popover
    _handle.hide = function () {

        // check exists
        if (selector != null) { $(selector).addClass('hidden'); }
    }

    // function: check if popover is visible
    _handle.visible = function () {

        // check exists
        if (selector != null) {
            var node = $(selector);
            if (node.length > 0) {

                if (node.hasClass('hidden')) { return false; }
                if (node.css('display') == 'none') { return false; }
                return node.is(':visible');
            }
        }

        return false;
    }

    // function: destroy popover
    _handle.destroy = function () {

        // check exists
        if (selector != null) { $(selector).remove(); }

        // unbind triggers
        if (_handle.clickTriggers != null) {
            for (var i = 0; i < _handle.clickTriggers.length; i++) {
                var t = _handle.clickTriggers[i];
                $(t).off('click');
            }
        }
        if (_handle.hoverTriggers != null) {
            for (var i = 0; i < _handle.hoverTriggers.length; i++) {
                var t = _handle.hoverTriggers[i];
                $(t).off('mouseenter');
                $(t).off('mouseleave');
            }
        }
    }

    // function: set html
    _handle.html = function (content) {

        if (_handle.selector != null) {
            var _body = $(selector).find('[data-hook=body]');
            _body.html(content);
        }
    }

    // function: loader (hide/show)
    _handle.loader = function (options) {
        if (_handle.selector != null) {

            var _loader = $(selector).find('[data-hook=loader]');

            if (options === false) {
                _loader.addClass('hidden');
            } else if (options === true) {
                _loader.removeClass('hidden');
            }
        }
    }

    // helper: create settings
    function fnSettings(opt) {

        var defaults = {
            dom: {                      // [object]     DOM properties
                id: null,               // [string]     - id
                'class': null,          // [string]     - class
                style: null,            // [object]     - style (key/value)
                attributes: null        // [object]     - attributes (key/value)
            },
            text: {                     // [object]     Text presets
                title: null,            // [string]     - title
                empty: null,            // [string]     - empty/ no-items
            },
            content: {                  // [object]     Content methods
                html: null,             // [string/func]    - custom html
                items: null             // [array]          - item engine   [{ id: '', value: '', text: ''}]
            },
            loader: null,               // [bool]       Show loader
            click: {                    // [object]     Click behaviour
                hide: true,             // [bool]           - hide on click
                trigger: null,          // [string/array]   - selectors for click event
            },
            hover: {                    // [object]     Hover behaviour
                trigger: null           // [string/array]   - selectors for hover event
            },
            events: {                   // [object]     Events (callbacks)
                click: null,            // [func]       - on click
                open: null,             // [func]       - on open
                close: null             // [func]       - on close         
            },
            position: {                 // [object]     Positioning
                type: null,             // [string]     - type: 'center', 'relative'
                to: null,               // [dom]        - position towards an element (relative)
                placement: 'bottom',        // [string]     - placement around element: 'bottom', 'left'
                offset: {               // [object]     - offset top/left
                    top: null,          // [int]
                    left: null          // [int]
                },
                // containment: null        // todo: render inside specified selector instead of document body
            },
            effects: null,              // [array]      Effects
            search: {                   // [object]     Search
                enabled: null,          // [bool]       Enable search bar
                placeholder: null,      // [string]     Placeholder text
                actions: null           // [array]      - Action { Icon: .. Data: ... }
            }
        }

        // merge defaults and options, without modifying defaults
        var settings = {}; //$.extend({}, defaults);

        settings['dom'] = $.extend({}, defaults['dom'], opt['dom']);

        settings['text'] = $.extend({}, defaults['text'], opt['text']);

        settings['content'] = $.extend({}, defaults['content'], opt['content']);

        settings['click'] = $.extend({}, defaults['click'], opt['click']);

        settings['hover'] = $.extend({}, defaults['hover'], opt['hover']);

        settings['events'] = $.extend({}, defaults['events'], opt['events']);

        settings['position'] = $.extend({}, defaults['position'], opt['position']);

        settings['search'] = $.extend({}, defaults['search'], opt['search']);


        // settings.position.offset
        if (opt.position != null) {
            if (opt.position.offset != null) {
                settings['position']['offset'] = $.extend({}, defaults['position']['offset'], opt['position']['offset']);
            } else {
                settings['position']['offset'] = null;
            }
        }

        // settings.effects
        settings['effects'] = [];// ['shadow'];

        //if(options.hasOwnProperty())
        //console.log('settings', settings);
        return settings;
    }

    // helper: default item definition
    function defItem(obj) {

        var _obj = null;

        if (obj == 'divider') {
            _obj = { type: 'divider' };
        } else {
            _obj = $.extend({ id: null, text: null, value: null, icon: null, type: null, searchable: null }, obj);

            if (_obj.type == 'link') {
                _obj = $.extend(_obj, { url: null, target: '_self' }, obj);
            }
        }

        return _obj;
    }

    return _handle;
}

/*
 * Grid (selector-less).
 * @param options
 */
function mxGrid(options) {

    // interesting for grid update: https://www.datatables.net/examples/server_side/ids.html

    var _handle = this;
    var _options = fnSettings(options);

    // set options reference
    _handle.options = _options;

    // set: selector
    var selector = _options.selector; //'#' + _id + '[data-tool=grid]';
    this.selector = selector;

    var dt = null;
    var colRenders = [];
    var editOriginalValues = {};

    // SelectionTool
    var itemSelection = new SelectionTool();
    var bulkActionsPopOver = null;

    // function: init grid
    this.init = function () {

        // grid: data
        var _data = null;
        if (_options.data != null) {
            _data = _options.data;
        }

        // grid: row-handler-key
        var rowHandlerKey = null;

        // grid: columns
        var _columns = null;
        if (_options.columns != null && $.isArray(_options.columns)) {
            _columns = [];
            for (var i = 0; i < _options.columns.length; i++) {

                var gridCol = defGridColumn(_options.columns[i]);
                _columns.push(gridCol);

                // check handler
                if (gridCol.handler === true) { rowHandlerKey = gridCol.name; }
            }
        }

        // datatable configuration
        var dtc = {
            sPaginationType: "full_numbers",
            searching: true, // necessary for api functionality
            paging: _options.paging,
            order: _options.order // todo: make smart by converting the 'name' to index
        };

        // datatable: ui elements
        var uiLengthMenu = false;
        var uiFiltering = false;
        var uiInfo = true;
        var uiPagination = false;
        var uiProcessing = false;

        // datatable: language
        var languageObj = createLanguageObj(_options.languageId, _options.label);
        if (languageObj != null) { dtc.language = languageObj; }

        // datatable: ajax
        if (_options.ajax != null) {

            dtc.serverSide = true;

            // format: response from server
            /*{
                "draw": 1,
                "recordsTotal": 57,
                "recordsFiltered": 57,
                "data": [...]
            }*/

            dtc.ajax = function (data, callback, settings) {

                // datatables ajax:
                // data         - Data to send to the server.
                // callback     - Callback function that must be executed when the required data has been obtained. That data should be passed into the callback as the only parameter.
                // settings     - DataTables settings object.

                _options.ajax(data, function (response) {

                    var _response = $.extend({
                        draw: null,
                        recordsTotal: null,
                        recordsFiltered: null,
                        data: null
                    }, response);

                    // defaults
                    if (_response.draw == null) { _response.draw = data.draw; }
                    if (_response.data == null) { _response.data = []; }
                    if (_response.recordsTotal == null) { _response.recordsTotal = _response.data.length; }
                    if (_response.recordsFiltered == null) { _response.recordsFiltered = _response.recordsTotal; }

                    // additional mappings etc...

                    callback(_response);

                }, settings);
            }
        }

        // datatable: searching
        if (_options.searching === true) {
            uiFiltering = true;
        }

        // datatable: processing
        if (_options.processing != null) {
            dtc.processing = _options.processing;
            var uiProcessing = true;
        }

        // datatable: paging
        if (_options.paging === true) {
            uiPagination = true;
        }
        if (_options.pageLength != null) {
            dtc.pageLength = _options.pageLength;
            uiPagination = true;
        }
        if (_options.lengthMenu != null) {
            if (_options.lengthMenu == 'default' || _options.lengthMenu === true) {
                // allow default
            } else {
                var lengthMenuLabels = [];
                for (var i = 0; i < _options.lengthMenu.length; i++) {
                    var lmi = _options.lengthMenu[i];
                    if (lmi == -1) {
                        // apply label
                        lmi = languageObj != null ? languageObj['xAll'] : '...';
                    } else if (lmi == 'all') {
                        // reverse approach
                        _options.lengthMenu[i] = -1;
                        lmi = languageObj != null ? languageObj['xAll'] : '...';
                    }

                    lengthMenuLabels[i] = lmi;
                }

                // [['10', '-1'], ['10', 'all']];
                dtc.lengthMenu = [_options.lengthMenu, lengthMenuLabels];
            }
            uiLengthMenu = true;
        } else {
            dtc.lengthChange = false;
        }

        // datatable: info
        if (_options.info === false) {
            uiInfo = false;
        }

        // datatable: columns
        dtc.columns = null;
        if (_columns != null) {
            dtc.columns = [];
            for (var i = 0; i < _columns.length; i++) {
                var gcol = _columns[i];

                // basic options
                var dtCol = {

                    // DataTables: Set the column title
                    // https://datatables.net/reference/option/columns.title
                    'sTitle': gcol.label + (gcol.sortable ? '&nbsp;&nbsp;' : ''), // css sorting fix

                    // DataTables: Set the data source for the column from the rows data object / array
                    // https://datatables.net/reference/option/columns.data
                    'mDataProp': gcol.name,

                    // DataTables: Set a descriptive name for a column
                    // https://datatables.net/reference/option/columns.name
                    'name': gcol.name,

                    // DataTables: Enable or disable filtering on the data in this column
                    // https://datatables.net/reference/option/columns.searchable
                    'bSearchable': gcol.searchable,

                    // DataTables: Enable or disable ordering on this column
                    // https://datatables.net/reference/option/columns.orderable
                    'bSortable': gcol.sortable,

                    // DataTables: Enable or disable the display of this column
                    // https://datatables.net/reference/option/columns.visible
                    'visible': gcol.visible,

                    // DataTables: Class to assign to each cell in the column
                    // https://datatables.net/reference/option/columns.className
                    'sClass': gcol.cssClass,

                    // DataTables: Define multiple column ordering as the default order for a column
                    // https://datatables.net/reference/option/columns.orderData
                    'orderData': gcol.orderData,

                    // DataTables: Column width assignment
                    // https://datatables.net/reference/option/columns.width
                    'width': gcol.width,

                    // DataTables: Set the column type - used for filtering and sorting string processing
                    // https://datatables.net/reference/option/columns.type
                    // options:
                    // - date
                    // - num
                    // - num-fmt
                    // - html-num
                    // - html-num-fmt
                    // - html
                    // - string
                    'sType': gcol.type,

                    // Custom: Editable (inline-edit)
                    'cEditable': gcol.editable
                };

                // rendering
                if (gcol.render != null) {
                    colRenders[i] = gcol.render;
                    var _renderFunc = function (data, type, full, meta) {
                        // data - the value specific to this column (or same as full if mapping was set 'null').
                        // type - the type of instigation requesting a (re-)draw ('filter', 'display', 'type' or 'sort'.)
                        // full - the full row data (object).
                        // meta - additional cell information (row & col indexes).

                        var rendered = colRenders[meta.col](data, type, full, meta);

                        // render inline-edit trigger
                        if (_options.onInlineEdit != null) {
                            var dtColumn = meta.settings.aoColumns[meta.col];
                            if (dtColumn.cEditable == true) {
                                var tInlineEdit = languageObj != null ? languageObj['xInlineEdit'] : null;
                                rendered += '<div class="trigger fas fa-pencil-alt" title="' + tInlineEdit + '"></div>';
                            }
                        }

                        return rendered;
                    }

                    // DataTables: Render (process) the data for use in the table
                    // https://datatables.net/reference/option/columns.render
                    dtCol.render = _renderFunc;
                }

                dtc.columns.push(dtCol);
            }
        }

        // datatable: data
        dtc.data = _data;

        // datatable: created row callback
        dtc.createdRow = function (row, data, dataIndex) {

            // set: handler
            if (rowHandlerKey != null) {

                // key
                $(row).attr('data-handler-key', rowHandlerKey);

                // value
                $(row).attr('data-handler-value', data[rowHandlerKey]);
            }

            // set: clickable
            if (_options.row.click != null) {
                $(row).addClass('clickable');
            }

            // set: editable class  
            if (_options.onInlineEdit != null) {

                // row index
                var apiRow = _handle.api.row(row);
                var rowIndex = apiRow.index();

                // column configuration
                var dtColumns = _handle.api.settings()[0].aoColumns;

                for (var i = 0; i < dtColumns.length; i++) {
                    var dtCol = dtColumns[i];

                    // inline-edit enabled
                    if (dtCol['cEditable'] == true) {

                        // cell
                        var cell = apiRow.cell(rowIndex, i);

                        // node
                        var node = $(cell.node());

                        // mark
                        node.addClass('inline-edit');
                        //node.append('<div class="trigger fas fa-pencil-alt"></div>');
                    }
                }
            }

            // custom
            if (_options.createdRow != null) { _options.createdRow(row, data, dataIndex); }
        }

        // datatable: row callback
        if (_options.rowCallback != null) {
            dtc.rowCallback = function (row, data, index) {
                _options.rowCallback(row, data, index);
            }
        }

        // datatable: draw callback
        if (_options.drawCallback != null) {
            dtc.drawCallback = _options.drawCallback;
        }

        // datatable: pre-draw callback
        if (_options.preDrawCallback != null) {
            dtc.preDrawCallback = _options.preDrawCallback;
        }

        // datatabe: state
        if (_options.state != null) {
            var _stateSettings = _options.state;

            // datatable: remember state -> For this function to work _options.rememberStateKey is necessary. UNIQUE(Used as the identifier in the localstorage)
            // Currently only supports sorting.
            // Can be extended with any setting.

            if (_stateSettings.key != null) {
                var storageKey = 'mx_grid_' + _stateSettings.key;

                var _localStorage = null;
                if (typeof (Storage) !== 'undefined' && window['localStorage'] != null) {
                    var _localStorage = window['localStorage'];
                }

                // Remembers the settings of the grid for next page load - Can be extended by adding more to _options
                var rememberStateFunc = function (settings) {
                    var prevSettings = _localStorage != null ? JSON.parse(_localStorage.getItem(storageKey)) : null;
                    if (prevSettings != null) {

                        // settings: sort
                        if (_stateSettings.sort === true) {

                            // read sort settings
                            if (prevSettings.sorting != null) {
                                settings.aaSorting = prevSettings.sorting;
                            }
                        }
                    }
                }

                // Store settings -> this gets called every time the grid updates
                var saveStateFunc = function (settings) {
                    var _storageData = {};

                    // settings: sort
                    if (_stateSettings.sort === true) {

                        // store sort settings
                        var sorting = settings.aaSorting;
                        _storageData.sorting = sorting;
                    }

                    if (_localStorage != null) {
                        _localStorage.setItem(storageKey, JSON.stringify(_storageData));
                    }
                }

                dtc.stateSave = true;
                dtc.stateLoadCallback = rememberStateFunc;
                dtc.stateSaveCallback = saveStateFunc;
            }
        }



        // datatable: dom configuration
        // l - length changing input control
        // f - filtering input
        // t - The table!
        // i - Table information summary
        // p - pagination control
        // r - processing display element
        var sDom = '';
        if (uiFiltering) { sDom += 'f'; }
        sDom += 't';
        if (uiLengthMenu) { sDom += 'l'; }
        if (uiInfo) { sDom += 'i'; }
        if (uiPagination) { sDom += 'p'; }
        if (uiProcessing) { sDom += 'r'; }
        dtc.dom = sDom;
        if (_options.dom != null && _options.dom != undefined) { dtc.dom = _options.dom; }

        // datatable: ui
        if (_options.autoWidth != null) { dtc.autoWidth = _options.autoWidth; }
        if (_options.scrollX != null) { dtc.scrollX = _options.scrollX; }
        if (_options.scrollY != null) { dtc.scrollY = _options.scrollY; }


        // datatable: DOM element
        var dtSelector = selector;
        var node = $(selector);
        if (node[0].tagName != 'TABLE') {

            // create inner element
            var dtSelector = selector + ' table[data-hook=tool-grid-table]';

            // datatable: check existing (destroy first)
            if ($.fn.DataTable.isDataTable(dtSelector)) {
                $(dtSelector).DataTable().destroy();
                node.empty();
                node = $(selector);
            }

            // dom: table 
            node.append('<table data-hook="tool-grid-table"></table>');

            //node.replaceWith(makeNewElementFromElement('table', node[0]));

            // temp style
            // $(selector).addClass('dashboard');
        }

        // datatable: check existing (destroy first)
        if ($.fn.DataTable.isDataTable(dtSelector)) {
            $(dtSelector).DataTable().destroy();
        }

        // datatable: init
        dt = $(dtSelector).dataTable(dtc);

        // set reference
        _handle.dataTable = dt;
        _handle.api = dt.api();




        // bind: inline-edit
        if (_options.onInlineEdit != null) {

            // click on column/data (inline edit)
            $(selector).on('click', 'tbody td.inline-edit .trigger', function (e) {

                // onInlineEdit must be a method
                if (typeof _options.onInlineEdit !== "function") return;
                var td = $(this).closest('td');

                // Get current data
                var cell = dt.DataTable().cell(td[0]);
                var currentData = cell.data();
                var rowId = cell.indexes()[0].row;
                var colId = cell.indexes()[0].column;
                var identifier = rowId + '|' + colId;

                // Check if column is editable
                var isEditable = dt.fnSettings().aoColumns[colId].cEditable;
                var colName = dt.fnSettings().aoColumns[colId].name;
                if (!isEditable) return;

                // Get handler info
                var parentRow = td.parents('tr');
                var handlerValue = parentRow.attr('data-handler-value');
                var handlerKey = parentRow.attr('data-handler-key');
                if (typeof handlerValue == "undefined" || handlerValue == false) handlerValue = null;
                if (typeof handlerKey == "undefined" || handlerKey == false) handlerKey = null;

                // Already inline-editing? Do nothing
                if (currentData != null && (currentData.toString().indexOf("<input ") != -1
                    || currentData.toString().indexOf("<select") != -1)
                    && currentData.indexOf("data-hook=") != -1) return;

                if (currentData.toString().indexOf('<span') >= 0 && $(currentData).is("span")) {
                    currentData = JSON.parse($(currentData).attr('data-options'));
                }

                if ($.isPlainObject(currentData) && (currentData.hasOwnProperty('dropdownitems') || currentData.hasOwnProperty('lookupParams'))) {
                    var currentSelection = JSON.parse(currentData.selected);

                    // Set original data
                    editOriginalValues[identifier] = currentSelection.value;

                    var availableOptions = [];

                    if (currentData.hasOwnProperty('dropdownitems')) {
                        availableOptions = JSON.parse(currentData.dropdownitems);
                    }

                    if (currentData.hasOwnProperty('lookupParams')) {
                        var lookupUrl = currentData.sourceUrl;
                        var lookupParams = currentData.lookupParams;

                        if (lookupUrl != null) {
                            // call server
                            MX.ajax.postJSON({
                                url: lookupUrl,
                                data: { parameters: lookupParams },
                                success: function (response) {
                                    var items = response;
                                    if (items == null) items = [];

                                    for (var i = 0; i < items.length; i++) {
                                        var option = { value: items[i].id, label: items[i].text };
                                        availableOptions.push(option);
                                    }
                                }
                            });
                        }
                    }

                    // 0.3s delay before displaying the select option for the dropdown/lookup inline-edit
                    // Options need to be loaded from a source
                    setTimeout(function () {
                        var selectOptions = "";
                        var selectField = $('<select/>');
                        for (var i = 0; i < availableOptions.length; i++) {
                            var option = availableOptions[i];
                            var selected = false;
                            if (currentSelection != null) {
                                selected = option.value == currentSelection.value;
                            }
                            selectField.append($('<option/>').attr('value', option.value).attr('selected', selected).html(option.label));
                        }

                        selectField.attr('data-identifier', identifier)
                            .attr('data-hook', 'inline-edit')
                            .attr('data-colname', colName)
                            .attr('data-handler-value', handlerValue)
                            .attr('data-handler-key', handlerKey)
                            .attr('data-options', JSON.stringify(currentData))
                            .attr('style', 'width: 100%; height: 100%;');

                        td.html(selectField);

                        // Set class
                        td.addClass('in-edit');

                        $('select[data-identifier="' + identifier + '"]').focus();
                    }, 300);
                }
                else {
                    var isDate = false;
                    var value = null;
                    var displayFormat = null;

                    if ($.isPlainObject(currentData) && currentData.hasOwnProperty('datepicker')) {
                        isDate = true;
                        displayFormat = currentData.datepicker;
                        displayFormat = displayFormat.replace('yyyy', 'yy').toLowerCase();

                        // Default flow of selected being the already active value
                        value = JSON.parse(currentData.selected).label;
                    } else {
                        value = currentData;
                    }

                    // Set original data
                    editOriginalValues[identifier] = value;

                    // Replace with input
                    var inputField = $('<input />')
                        .attr('value', value)
                        .attr('data-identifier', identifier)
                        .attr('data-hook', 'inline-edit')
                        .attr('data-colname', colName)
                        .attr('data-handler-value', handlerValue)
                        .attr('data-handler-key', handlerKey)
                        .attr('style', 'width: 100%; height: 100%;');

                    if (isDate) {
                        inputField.attr('data-options', JSON.stringify(currentData));
                    }

                    td.html(inputField.prop('outerHTML'));

                    // Set class
                    td.addClass('in-edit');

                    // Enable datepicker
                    if (isDate) {
                        var cElement = $('input[data-identifier="' + identifier + '"]');
                        cElement.datepicker({
                            dateFormat: displayFormat,
                            beforeShow: function () {
                                cElement.addClass('active-datepicker');
                            },
                            onClose: function () {
                                cElement.removeClass('active-datepicker');
                                cElement.trigger('blur');
                            }
                        });
                    }

                    // Set focus
                    $('input[data-identifier="' + identifier + '"]').focus();
                }
                e.stopPropagation();
            });
        }

        // bind: click
        if (_options.row.click != null) {
            $(selector).on('click', 'tbody tr.clickable', function (e) {

                var tr = $(this);

                // check in-edit mode
                var inEdit = tr.find('td.in-edit').length > 0;
                if (inEdit == true) return;

                if (_options.row.click == 'click') {

                    // callback                    
                    if (_options.events.click != null) {

                        // handle
                        var rowHandle = _handle.api.row(tr);
                        var row = fnRowHandle(rowHandle);

                        // fire
                        _options.events.click(row);
                    }

                } else if (_options.row.click == 'select') {

                    // apply selected class
                    if (tr.hasClass('selected')) {
                        tr.removeClass('selected');
                    } else {
                        tr.addClass('selected');
                    }
                }
            });
        }

        // bind: action
        if (_options.events.action != null) {
            $(selector).on('click', 'tbody td [data-grid-action]', function (e) {
                e.stopPropagation();

                var btn = $(this);
                var tr = btn.closest('tr');
                var key = btn.attr('data-grid-action');

                // callback                    

                // handle
                var rowHandle = _handle.api.row(tr);
                var row = fnRowHandle(rowHandle);

                if (key == "checkbox") {
                    var checked = btn.find('input')[0].checked;
                    if (checked === true) {
                        itemSelection.add(row.handler.value);
                    }
                    else {
                        itemSelection.remove(row.handler.value);
                    }
                    return;
                }

                // fire
                _options.events.action(row, key);
            });
        }

        // bind: select all visible
        $(selector).on('click', 'thead th [data-grid-action=check-all]', function () {
            var select = $(this)[0].checked;

            if (select === true) {
                var rowLength = _handle.api.rows()[0].length;
                itemSelection.clear();
                for (var i = 0; i < rowLength; i++) {
                    var row = fnRowHandle(_handle.api.row(i));
                    itemSelection.add(row.handler.value);
                    $('[data-row-checkbox=' + row.index + ']').prop('checked', true);
                }
            }
            else {
                itemSelection.clear();
                $('[data-row-checkbox]').prop('checked', false);
            }
        });

        // bind: bulk edits        
        $(selector).on('click', 'thead th [data-hook=bulk-actions]', function () {
            bulkActionsPopOver.show();
        });
    }

    // function: destroy grid
    this.destroy = function () {

        // todo: datatable cleanup

        // check exists
        if (selector != null) { $(selector).remove(); }
    }

    // function: get rows
    this.rows = function (options) {

        var _options = $.extend({
            handler: null // { key: '', value: '' }
        }, options);

        var result = [];

        // get rows by handler
        if (_options.handler != null) {

            var api = dt.api();

            var indexes = api.rows('[data-handler-key="' + _options.handler.key + '"][data-handler-value=' + _options.handler.value + ']');

            for (var i = 0; i < indexes.length; i++) {
                var rowHandle = api.row(indexes[0]);

                var row = fnRowHandle(rowHandle);

                result.push(row);
            }
        }

        return result;
    }

    // function: global search
    this.search = function (text, options) {

        if (text == null || text.length == 0) {
            //grid.api.columns().search('').draw();
            _handle.api.search('').draw();
        } else {
            //grid.api.columns().search(_options.text).draw();
            _handle.api.search(text).draw();
        }

        //if(text == null || text.length == 0){
        //    //grid.api.columns().search('').draw();
        //    grid.api.search('').draw();
        //} else {
        //    //grid.api.columns().search(_options.text).draw();
        //    grid.api.search(text).draw();
        //}
    }

    // function: column filtering
    this.filter = function (cols) {

        var col = cols; // single

        _handle.api.column(col.name + ':name').search(col.value == null ? '' : col.value).draw();
    }

    // function: reloads grid data
    this.reload = function (options) {

        // future: options for resetting filtering and such

        if (itemSelection != null && itemSelection != undefined) {
            $(selector + ' thead th [data-grid-action=check-all]').prop('checked', false);
            itemSelection.clear();
        }

        _handle.api.ajax.reload(null, false); // user paging is not reset on reload
    }

    // function: gets the current selection
    this.currentSelection = function (options) {

        if (options.hidePopOver != undefined && options.hidePopOver == true) { bulkActionsPopOver.hide(); }
        return itemSelection.current();
    }

    // helper: create settings
    function fnSettings(options) {

        var defaults = {
            selector: null,
            data: null,
            columns: null,
            createdRow: null,
            rowCallback: null,
            ajax: null,
            searching: false,
            pageLength: null,
            lengthMenu: null,
            processing: null,
            paging: true,
            info: null,
            languageId: 1,
            label: {
                entry: null,
                entries: null
            },
            autoWidth: null,
            scrollX: null,
            scrollY: null,
            skin: null,
            onInlineEdit: null,
            drawCallback: null,
            preDrawCallback: function (settings) {
                var api = new $.fn.dataTable.Api(settings);
                $(this).closest('.dataTables_wrapper').find('.dataTables_length, .dataTables_info').toggle(api.page.info().pages > 1);
            },

            // default row settings
            row: {
                // indicates if the 'selected' state can be applied [true/false]
                //selectable: null, not implemented yet

                // indicates the click behaviour and adds hover [select, click]
                click: null
            },

            // built-in events
            events: {
                // event: row click
                click: null,

                // event: action click
                action: null,

                // cell edit
                edit: null
            },

            // state settings
            state: {
                key: null,      // [string] localStorage suffix identifier
                sort: null,     // [bool] enable state for sorting
                // ...
            }
        };

        // merge defaults and options, without modifying defaults
        var s = $.extend({}, defaults, options);

        s['label'] = $.extend({}, defaults['label'], options['label']);
        s['row'] = $.extend({}, defaults['row'], options['row']);
        s['events'] = $.extend({}, defaults['events'], options['events']);
        s['state'] = options['state'] != null ? ($.extend({}, defaults['state'], options['state'])) : null;

        return s;
    }

    // helper: create row handle
    function fnRowHandle(rowHandle) {

        // construct
        var _row = {};
        _row.handle = rowHandle;
        _row.index = rowHandle.index();
        _row.data = rowHandle.data();
        _row.dom = rowHandle.node();
        _row.node = $(_row.dom);
        _row.handler = { key: _row.node.attr('data-handler-key'), value: _row.node.attr('data-handler-value') };

        // function: update data object
        _row.updateData = function (obj) {
            for (var prop in _row.data) {
                if (_row.data.hasOwnProperty(prop)) {
                    _row.data[prop] = obj[prop];
                }
            }
        }

        // function: remove row
        _row.remove = function () {
            _row.handle.remove();
            _row.handle.draw();
        }

        // function: persist changes to object
        _row.persist = function () {
            _row.handle.invalidate();
        }

        return _row;
    }

    // helper: default grid column obj
    function defGridColumn(column) {

        var _col = $.extend({

            // MX: Column label
            label: '',

            // MX: Property name
            name: null,

            // MX: Indicates if this column is searchable
            searchable: false,

            // MX: Indicates if this column is sortable/orderable
            sortable: false,

            // MX: Indicates if this column is visible
            visible: true,

            // MX: Indicates if this column is the handler
            handler: false,

            // MX: Overrides the cell rendering
            render: null,

            // MX: Additional css class (-es) to be added to the cells
            cssClass: null,

            // MX: Indicates if the cells are editable (inline-edit)
            editable: false,

            // MX: Column type (mx type or datatables type)
            // Options:
            // - actions
            // - ...datables native options...
            type: null,

            // MX: Column actions
            actions: null,

            // MX: Bulk actions
            bulkActions: null
        }, column);

        // default: actions
        if (_col.actions != null) {
            for (var i = 0; i < _col.actions.length; i++) {

                var _action = $.extend({
                    icon: null,
                    key: null,      // [edit, ...]
                    label: null
                }, _col.actions[i]);
                _col.actions[i] = _action;
            }
        }

        // type: actions
        if (_col.type == 'actions') {

            // override: css class
            _col.cssClass = 'actions' + (_col.cssClass != null ? ' ' + _col.cssClass : '');

            // override: rendering
            if (_col.render == null) {
                _col.render = function (data, type, full, meta) {
                    var _actions = _col.actions; // pin reference                    
                    var h = '';

                    for (var i = 0; i < _actions.length; i++) {
                        var _action = _actions[i];
                        h += '<div data-grid-action="' + (_action.key != null ? _action.key : '') + '" class="action" title="' + (_action.label != null ? _action.label : '') + '">';
                        if (_action.icon != null) {
                            h += '<i class="' + (_action.icon.indexOf('fa-') == 0 ? 'fa ' : '') + (_action.icon) + ' cursor"></i>';
                        }
                        h += '</div>';
                    }
                    return h;
                }
            }
        }

        if (_col.type == "multi-select") {
            _col.label = '<div data-hook="multi-select-toggle" class="multi-select hidden"><input id="checkbox" type="checkbox" data-grid-action="check-all"/></div>';
            _col.cssClass = 'multi-select';
            _col.render = function (data, type, full, meta) {
                var h = '';
                h += '<div class="hidden" data-hook="multi-select-toggle" data-grid-action="checkbox">';
                h += '<input id="checkbox" type="checkbox" data-row-checkbox="' + meta.row + '" />';
                h += '</div>';

                return h;
            }

            var html = '';
            html += '<div class="grid-pop-over"><ul>';
            var rowClass = null;
            for (var i = 0; i < _col.bulkActions.length; i++) {

                if (rowClass == null) { rowClass = 'odd'; }
                else if (rowClass == 'odd') { rowClass = 'even'; }
                else if (rowClass == 'even') { rowClass = 'odd'; }

                var a = _col.bulkActions[i];
                html += '<li class="option ' + rowClass + ' " data-bulk-action=' + a.key + '><div>' + a.label + '</div></li>';
            }
            html += '</ul></div>';

            bulkActionsPopOver = new mxPopover({
                dom: {
                    id: 'bulk_edit_grid'
                },
                content: {
                    html: html
                },
                position: {
                    type: 'relative',
                    to: 'thead th [data-hook=bulk-actions]'
                }
            });

        }

        // default: render
        if (_col.render == null) {
            _col.render = function (data, type, full, meta) {

                // special rendering
                if ($.isPlainObject(data) && data.hasOwnProperty('type')) {
                    if (data.type == 'link') {
                        return '<a onclick="event.stopPropagation();" href="' + data.value + '" target="_blank">preview</a>';
                    }
                }

                if ($.isPlainObject(data) && data.hasOwnProperty('selected')) {
                    return JSON.parse(data.selected).label;
                }

                return data;
            }
        }

        return _col;
    }

    // helper: convert DOM element
    function makeNewElementFromElement(tag, elem) {

        var newElem = document.createElement(tag),
            i, prop,
            attr = elem.attributes,
            attrLen = attr.length;

        // Copy children 
        elem = elem.cloneNode(true);
        while (elem.firstChild) {
            newElem.appendChild(elem.firstChild);
        }

        // Copy DOM properties
        for (i in elem) {
            try {
                prop = elem[i];
                if (prop && i !== 'outerHTML' && (typeof prop === 'string' || typeof prop === 'number')) {
                    newElem[i] = elem[i];
                }
            } catch (e) { /* some props throw getter errors */ }
        }

        // Copy attributes
        for (i = 0; i < attrLen; i++) {
            newElem.setAttribute(attr[i].nodeName, attr[i].nodeValue);
        }

        // Copy inline CSS
        newElem.style.cssText = elem.style.cssText;

        return newElem;
    }

    // helper: datatable translations
    function createLanguageObj(languageId, label) {

        // label settings
        var entry = null;
        var entries = null;
        var empty = null;
        var showInfoFiltered = true;

        if (label != null) {
            if (label.entry != null) { entry = label.entry; }
            if (label.entries != null) { entries = label.entries; }
            if (label.empty != null) { empty = label.empty; }
            if (label.showInfoFiltered != null) { showInfoFiltered = label.showInfoFiltered; }
        }
        //console.log('label', label);
        // note: grabbed from 'http://datatables.net/plug-ins/i18n/' (Internationalisation)
        if (languageId == 1) {
            return {
                sEmptyTable: (empty != null ? empty : 'No data available in table'),
                sInfo: "- _START_ to _END_ of _TOTAL_ " + (entries != null ? entries : 'entries'),
                sInfoEmpty: "- 0 to 0 of 0 " + (entries != null ? entries : 'entries'),
                sInfoFiltered: (showInfoFiltered == true ? "(filtered from _MAX_ total " + (entries != null ? entries : 'entries') + ")" : ""),
                sInfoPostFix: "",
                sInfoThousands: ",",
                sLengthMenu: "Show _MENU_ " + (entries != null ? entries : 'entries'),
                sLoadingRecords: "Loading...",
                sProcessing: "Processing...",
                sSearch: "Search:",
                sZeroRecords: "No matching records found",
                oPaginate: {
                    sFirst: "First",
                    sLast: "Last",
                    sNext: "Next",
                    sPrevious: "Previous"
                },
                oAria: {
                    sSortAscending: ": activate to sort column ascending",
                    sSortDescending: ": activate to sort column descending"
                },

                // custom
                xAll: 'All',
                xInlineEdit: 'Edit value'
            }
        }
        else if (languageId == 2) {
            return {
                processing: "Bezig...",
                search: "Zoeken:",
                lengthMenu: "_MENU_ " + (entries != null ? entries : 'resultaten') + " weergeven",
                info: "- _START_ tot _END_ van _TOTAL_ " + (entries != null ? entries : 'resultaten') + "",
                infoEmpty: "Geen " + (entries != null ? entries : 'resultaten') + " om weer te geven",
                infoFiltered: (showInfoFiltered == true ? " (gefilterd uit _MAX_ " + (entries != null ? entries : 'resultaten') + ")" : ""),
                infoPostFix: "",
                //infoThousands": ".",
                loadingRecords: "Een moment geduld aub - bezig met laden...",
                zeroRecords: "Geen resultaten gevonden",
                emptyTable: (empty != null ? empty : "Geen resultaten aanwezig in de tabel"),
                paginate: {
                    first: "Eerste",
                    last: "Laatste",
                    next: "Volgende",
                    previous: "Vorige"
                },

                // custom
                xAll: 'Alles',
                xInlineEdit: 'Bewerk waarde'
            };
        } else if (languageId == 3) {
            return {
                processing: "Traitement en cours...",
                search: "Rechercher&nbsp;:",
                lengthMenu: "Afficher _MENU_ &eacute;l&eacute;ments",
                info: "- de l'&eacute;l&eacute;ment _START_ &agrave; _END_ sur _TOTAL_ &eacute;l&eacute;ments",
                infoEmpty: "- de l'&eacute;l&eacute;ment 0 &agrave; 0 sur 0 &eacute;l&eacute;ment",
                infoFiltered: (showInfoFiltered == true ? "(filtr&eacute; de _MAX_ &eacute;l&eacute;ments au total)" : ""),
                infoPostFix: "",
                //infoThousands": ".",
                loadingRecords: "Chargement en cours...",
                zeroRecords: "Aucun &eacute;l&eacute;ment &agrave; afficher",
                emptyTable: (empty != null ? empty : "Aucune donn&eacute;e disponible dans le tableau"),
                paginate: {
                    first: "Premier",
                    last: "Pr&eacute;c&eacute;dent",
                    next: "Suivant",
                    previous: "Dernier"
                },
                aria: {
                    sortAscending: ": activer pour trier la colonne par ordre croissant",
                    sortDescending: ": activer pour trier la colonne par ordre d&eacute;croissant"
                },

                // custom
                xAll: 'Tout',
                xInlineEdit: 'Modifier la valeur'
            };
        }

        return null;
    }

    //========================================

    // helper: save new item
    // identifier:      {rowId}|{colId}
    // colName:         the name of the column
    // handlerValue:    the handler value
    // handlerKey:      the handler key
    // value:           the new value
    // url:             the url to call
    // customData:      a custom data object
    function saveEditableColumnValue(identifier, colName, handlerValue, handlerKey, value, url, customData) {

        // Get ids
        var rowId = identifier.split('|')[0];
        var colId = identifier.split('|')[1];

        var newValue = value;

        if ($.isPlainObject(value)) {
            newValue = value.value;
        }

        // Create post object
        var data = {
            colName: colName,
            handlerValue: handlerValue,
            handlerKey: handlerKey,
            value: newValue,
            data: customData
        }

        // Make request
        MX.ajax.postJSON({
            url: url,
            data: data,
            success: function (response) {

                // Check response
                if (typeof response.success != "undefined" && response.success == true) {

                    // Set data
                    var cell = dt.DataTable().cell(rowId, colId);
                    cell.data(value);

                    if ($.isPlainObject(value)) {
                        var options = JSON.parse(value.options);
                        var selected = JSON.parse(options.selected);
                        if (selected.hasOwnProperty('value')) {
                            selected.value = value.value;
                        }
                        selected.label = value.label;
                        options.selected = JSON.stringify(selected);

                        cell.data("<span data-options='" + JSON.stringify(options) + "'>" + value.label + "</span>");
                    }
                    else {
                        cell.data(value);
                    }

                    // Remove parent class
                    cell.nodes().toJQuery().removeClass('in-edit');

                    // Show message
                    if (typeof response.message != "undefined" && response.message != null) {
                        Feedback.success({ message: response.message });
                    }

                }
                else {
                    Feedback.error({ message: response.message });
                }

            },
            error: function () {
                restoreEditableColumnValue(identifier, "There was a problem writing the data, please try again.")
            }
        });

    }

    // helper: restore new column value
    // identifier:  {rowId}|{colId}
    // message:     the message to show (error)
    function restoreEditableColumnValue(identifier, message) {

        // Get original value
        var originalVal = editOriginalValues[identifier];

        // Get ids
        var rowId = parseInt(identifier.split('|')[0]);
        var colId = parseInt(identifier.split('|')[1]);

        // Get cell, set data
        var cell = _handle.api.cell(rowId, colId);
        //console.log('cell', cell.node());
        try {
            cell.data(originalVal);
        } catch (ex) { console.log('datatables exception...'); }

        //cell.invalidate();

        // Detele original value
        delete editOriginalValues.identifier;

        // Remove parent class
        cell.nodes().toJQuery().removeClass('in-edit');

        // Handle message
        if (typeof message != 'undefined' && message != null) {
            Feedback.error({ message: message });
        }
    }



    // action: click enter in editable field
    $(selector).on('keyup', '[data-hook=inline-edit]', function (e) {
        //console.log('keyup', e);
        // Do nothing if not enter
        if (e.keyCode != 13 && e.keyCode != 27) return;

        // Get id
        var identifier = $(this).attr('data-identifier');
        var colName = $(this).attr('data-colname');
        var handlerValue = $(this).attr('data-handler-value');
        var handlerKey = $(this).attr('data-handler-key');

        // Set back old value if escape pressed
        if (e.keyCode == 27) {
            $(this).attr('escape-clicked', true);
            restoreEditableColumnValue(identifier);
            return;
        }

        // Enter is pressed, call save function
        var newValue = $(this).val();

        if ($(this).is('select')) {
            var options = $(this).attr('data-options');
            var label = $(this).find('option:selected').text();
            newValue = { value: $(this).val(), label: label, options: options };
        }

        $(this).attr('enter-clicked', true);

        // Call user-defined save function
        _options.onInlineEdit(identifier, colName, handlerValue, handlerKey, newValue, saveEditableColumnValue, restoreEditableColumnValue);

    });

    // action: loose focus of input field
    $(selector).on('blur', '[data-hook=inline-edit]', function (e) {
        //console.log('blur', e);
        // Blur because of enter or escape?
        var enterClicked = $(this).attr('enter-clicked');
        var escapeClicked = $(this).attr('escape-clicked');

        if (typeof enterClicked != "undefined" && (enterClicked === true || enterClicked == "true")) return;
        if (!$(this).is('select') && typeof escapeClicked != "undefined" && (escapeClicked === true || escapeClicked == "true")) return;

        // Get id
        var identifier = $(this).attr('data-identifier');

        // Save on focus out (when clicking out of the field)
        if (e.type == 'focusout') {
            var newValue = $(this).val();

            //console.log('before', newValue);
            if ($(this).is('select')) {
                var options = $(this).attr('data-options');
                var label = $(this).find('option:selected').text();
                newValue = { value: $(this).val(), label: label, options: options };
            }

            //console.log('after', newValue);

            var colName = $(this).attr('data-colname');
            var handlerValue = $(this).attr('data-handler-value');
            var handlerKey = $(this).attr('data-handler-key');

            var element = $(this);
            if (element.hasClass('hasDatepicker')) {
                if (!element.hasClass('active-datepicker')) {
                    setTimeout(function () {
                        // The calendar needs to update the value of the "newvalue" because it is being set by the datepicker
                        var options = element.attr('data-options');
                        var label = element.val();
                        newValue = { value: element.val(), label: label, options: options };
                        _options.onInlineEdit(identifier, colName, handlerValue, handlerKey, newValue, saveEditableColumnValue, restoreEditableColumnValue);
                    }, 300);
                }
            }
            else {
                _options.onInlineEdit(identifier, colName, handlerValue, handlerKey, newValue, saveEditableColumnValue, restoreEditableColumnValue);
            }
            return;
        }

        restoreEditableColumnValue(identifier);
    });

}


//=== Bindings (deprecated, todo: refactor)

// UI: Toggle
$(document).on('click', '[data-mx-ui=toggle]', function (e) {

    var classExpanded = 'expanded';
    var classCollapsed = 'collapsed';

    var trigger = $(this);
    if (trigger.length > 0) {

        var toggleKey = trigger.attr('data-toggle-target');
        var targetNode = $('[data-toggle-key=' + toggleKey + ']');

        if (targetNode.length > 0) {

            trigger.removeClass(classExpanded);
            trigger.removeClass(classCollapsed);
            targetNode.removeClass(classExpanded);
            targetNode.removeClass(classCollapsed);

            if (targetNode.is(':visible')) {
                trigger.addClass(classCollapsed);
                targetNode.addClass(classCollapsed);
                targetNode.slideUp({ duration: 'fast' });
            } else {
                trigger.addClass(classExpanded);
                targetNode.addClass(classExpanded);
                targetNode.slideDown({ duration: 'fast' });
            }
        }
    }
});

// UI: Dropdown
$(document).on('click', '[data-mx-ui=dropdown]', function (e) {

    //'[data-mx-ui=dropdown] > [data-dropdown=drop]'
    //var dropNode = $(this);
    //var dropdownNode = dropNode.closest('[data-mx-ui=dropdown]');
    //var optionsNode = dropNode.siblings('[data-dropdown=options]');

    var dropNode = $(this).find('[data-dropdown=drop]');
    var dropdownNode = dropNode.closest('[data-mx-ui=dropdown]');
    var optionsNode = dropNode.siblings('[data-dropdown=options]');

    // check already open
    if (optionsNode.is(':visible')) {
        optionsNode.hide();
        return;
    }

    // close other dropdowns first
    $('[data-dropdown=options]').hide();

    // check options
    if (optionsNode.length == 0) return;

    // get positions
    var buttonPositions = dropdownNode.position();

    // get height
    var outerHeight = dropdownNode.outerHeight();

    // get width
    var borderWidth = dropdownNode.outerWidth() - dropdownNode.innerWidth();
    var paddingWidth = dropdownNode.innerWidth() - dropdownNode.width();
    var marginWidth = dropdownNode.outerWidth(true) - dropdownNode.outerWidth();
    var elementWidth = dropdownNode.width() + paddingWidth - marginWidth - borderWidth;

    // position on screen
    optionsNode.css('position', 'absolute');
    optionsNode.css('top', (buttonPositions.top + outerHeight) + "px");
    optionsNode.css('left', buttonPositions.left + "px")
    optionsNode.css('display', 'block');
    optionsNode.children().css('margin-left', dropdownNode.css('margin-left'));

    // Set width?
    if (optionsNode.children().outerWidth() <= (elementWidth - 2)) {
        optionsNode.children().css('width', (elementWidth - 2) + "px");
    }

    return;
});
$(document).on('click', function (e) {

    // Must not be self, or children
    if (e.target !== this) {
        if (e.target.attributes['data-dropdown'] == undefined) {

            $('[data-dropdown=options]').hide();

        } else if (e.target.attributes['data-dropdown'].Value == 'drop') {
            $('[data-dropdown=options]').hide();
        }
    }
});

// UI: Splitdrop
$(document).off('click', 'span[data-dropdown=drop]');
$(document).on('click', 'span[data-dropdown=drop]', function (e) {

    // Stop normal execution
    e.preventDefault;

    var btn = $(this);
    var parent = btn.parents('.splitdrop');

    // Get placement
    var placement = 'bottom';
    if (btn.attr('data-dropdown-placement') == 'top') {
        placement = 'top';
    }

    // Get positions
    var buttonPositions = parent.position();

    // Get height
    var outerHeight = parent.outerHeight();

    // Get width
    var borderWidth = parent.outerWidth() - parent.innerWidth();
    var paddingWidth = parent.innerWidth() - parent.width();
    var marginWidth = parent.outerWidth(true) - parent.outerWidth();
    var elementWidth = parent.width() + paddingWidth - marginWidth - borderWidth;

    // Set list parameters
    var list = $('#drop-' + parent.attr('id'));

    // Check if it exists
    if (list.length == 0) return;

    // Set top or bottom position
    if (placement == 'bottom') {
        list.css('top', (buttonPositions.top + outerHeight) + "px");
    }
    else if (placement == 'top') {
        // Get bottom margin
        var marginBottom = parseInt(list.css('margin-bottom').replace('px', ''));
        list.css('top', (buttonPositions.top - list.outerHeight()) - marginBottom + "px");
    }

    list.css('position', 'absolute');
    list.css('left', buttonPositions.left + "px")
    list.css('display', 'block');
    list.find('a').css('margin-left', parent.css('margin-left'));

    // Set width (only if list appears to be smaller than the parent)
    var biggestChild = 0;
    $.each(list.children('li').children('a'), function (i, c) { if ($(c).width() > biggestChild) { biggestChild = $(c).width(); } })
    if (biggestChild < parent.width()) {
        list.children().children('a').css('width', parent.width() + "px");
    }
    else {

        // Set list children widths, to have all items with the same width
        list.children('li').children('a').css('width', biggestChild + 'px');

    }


    return false;
});

// enable dropdown on button itsself
$(document).off('click', '[data-dropdown-enable=true]');
$(document).on('click', '[data-dropdown-enable=true]', function () {

    // Click on inner span
    $(this).children('span[data-dropdown=drop]').click();

});
$(document).on('click', function (e) {
    var hasClassItem = (e.target.className != null && e.target.className != null && e.target.className.indexOf != null && e.target.className.indexOf('splitdropitem') >= 0);
    var hasClassBtn = (e.target.className != null && e.target.className != null && e.target.className.indexOf != null && e.target.className.indexOf('dropdown-button') >= 0);


    // Must not be self, or children
    if (e.target !== this
        && !hasClassItem
        && $(e.target).attr('data-dropdown-enable') == null
        && !hasClassBtn) {
        $('.splitdrop-dropdown').hide();
    }

});

// Disable datatables warning via the global alert function
window.alert = (function () {
    var nativeAlert = window.alert;
    return function (message) {
        window.alert = nativeAlert;
        message.indexOf("DataTables warning") === 0 ?
            console.warn(message) :
            nativeAlert(message);
    }
})();