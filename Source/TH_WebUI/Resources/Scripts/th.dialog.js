/**
 * Dialog System
 * 
 * Dependencies:
 * - jQuery
 */

/**
 * Dialog System
 */
var DialogSystem = (function ($) {

    //=== Limits
    var _MAX_TIER = 4;

    //=== defaults
    var _dOverlayIndex = -1;

    //=== current
    var _cOverlayIndex = -1;

    //=== selectors
    var _ID_CONTAINER = "ds_container";
    var _ID_CONTAINER_SIDEBAR = "ds_container_sidebar";
    var _ID_CONTAINER_POPUP = "ds_container_popup";

    var _ID_MAIN = "ds-main";
    var _ID_OVERLAY = "ds-overlay";
    var _ID_LOADER = "ds-loader";
    var _ID_LOADERTEXT = "ds-loader-text";
    var _ID_BODY = "ds-body";
    var _ID_HEADER = "ds-header";
    var _ID_TITLE = "ds-title";
    var _ID_CLOSE = "ds-close";
    var _ID_NAVIGATION = "ds-navigation";
    var _ID_CONTENT = "ds-content";
    var _ID_BUTTONS = "ds-buttons";
    var _ID_COLINACTIVE = "ds-collection-inactive";
    var _ID_RESIZE = "ds-resize";

    var ds = {};

    //=== public

    /**
     * Initializes necessary settings and html.
     */
    ds.init = function () {

        // render
        var h = '';

        // render: container
        h += '<div id="' + _ID_CONTAINER + '" class="ds-container">';

        // render: container-sidebar
        h += '<div id="' + _ID_CONTAINER_SIDEBAR + '" class="ds-container-sidebar">';

        // ...

        h += '</div>';


        // render: container-popup
        h += '<div id="' + _ID_CONTAINER_POPUP + '" class="ds-container-popup">';

        // render: popup
        h += '<div id="' + _ID_MAIN + '">';
        h += '<div id="' + _ID_OVERLAY + '"></div>';
        h += '<div id="' + _ID_LOADER + '" class="dialog-loader">';
        h += '<div data-hook="preview-loader" class="th-preview-loader"></div>';
        h += '<div id="' + _ID_LOADERTEXT + '"></div>';
        h += '</div>';

        // Tier X
        for (var i = 1; i <= _MAX_TIER; i++) {

            h += '<div id="' + _ID_BODY + '" class="ds-dialog" data-ds-tier="' + i + '" data-ds-key="" style="z-index:' + ((i - 1) * 2) + '" >' +
                '<div id="' + _ID_HEADER + '" class="ds-header">' +
                '<div id="' + _ID_TITLE + '" class="ds-title"></div>' +
                '<div class="actions align-middle">' +
                '<div class="action"> <a id="' + _ID_RESIZE + '" href="#" class="ds-resize resize"><i class="fas fa-expand"></i></a> </div>' + // Resize
                '<div class="action"> <a id="' + _ID_CLOSE + '" href="#" class="ds-close close"><i class="fas fa-times"></i></a> </div>' + // X
                '</div>' +
                '</div>' +
                '<div id="' + _ID_NAVIGATION + '" class="ds-navigation">' +
                '<ul class="ds-list"></ul>' +
                '</div>' +
                '<div id="' + _ID_CONTENT + '" class="ds-content"></div>' +
                '<div id="' + _ID_BUTTONS + '" class="ds-buttons buttonRow"></div>' +
                '</div>';
        }

        h += '<div id="' + _ID_COLINACTIVE + '" style="display:none;"></div>';

        // close: popup
        h += '</div>';

        // close: container-popup
        h += '</div>';

        // close: container
        h += '</div>';

        $("body").append(h);

        // Set Event 'Close'
        $(document).on('click', '#' + _ID_CLOSE, function (e) {
            e.preventDefault();
            e.stopPropagation();

            ds.done({ action: 'close' });
        });

        // Set Event 'Resize'
        $(document).on('click', '#' + _ID_RESIZE, function (e) {

            // Get dialog
            var key = $(this).closest("[data-ds-key]").data("ds-key");
            var dialog = chain.get(key);

            // Set dimension
            if (dialog.dimension.full === true) {
                dialog.dimension.full = false;
            }
            else {
                dialog.dimension.full = true;
            }

            // Toggle icon
            $(this).find('i').toggleClass('fa-expand fa-compress');

            // Update
            _setDimension(dialog, dialog.dimension);
        });

        // Set Event 'Navigation'
        $(document).on('click', '#' + _ID_NAVIGATION + ' li > a', function (e) {
            e.preventDefault();
            e.stopPropagation();

            $a = $(this);
        });
    }

    /**
     *  Show/Hide the loader.
     */
    ds.loader = function (options) {

        // container (loader is only supported in popup mechanic)
        var container = $('#' + _ID_CONTAINER);
        var containerType = $('#' + _ID_CONTAINER_POPUP);


        var nMain = containerType.find('#' + _ID_MAIN);
        var overlay = containerType.find('#' + _ID_OVERLAY);
        var nLoader = containerType.find('#' + _ID_LOADER);

        if (options === false) {

            // check chain

            // Check if dialogs are visible
            var anyDialogs = chain.count() != 0;
            //$body1 = _element(containerType, _ID_BODY, 1);
            //$body2 = _element(containerType, _ID_BODY, 2);

            // hide container/main
            //if (!$body1.is(":visible") && !$body2.is(":visible")) {
            if (anyDialogs === false) {
                container.hide();
                nMain.hide();
            }

            // restore overlay
            overlay.css({ 'z-index': _cOverlayIndex });

            // Hide
            nLoader.hide();
        }
        else {

            var _options = { text: null };
            if (options !== true) {
                _options = $.extend(_options, options);
            }

            // show container
            if (container.is(":visible") === false) { container.show(); }

            // show main
            if (nMain.is(":visible") === false) { nMain.show(); }

            // set overlay
            var _pOverlayIndex = parseInt(overlay.css('z-index'));
            overlay.css({ 'z-index': (_pOverlayIndex + 8) });

            // set text
            var loaderTextNode = $("#" + _ID_LOADERTEXT);
            loaderTextNode.text(_options.text);
            if (_options.text != null && _options.text.length > 0) {
                loaderTextNode.show();
                nLoader.addClass('wide');
            } else {
                loaderTextNode.hide();
                nLoader.removeClass('wide');
            }

            // show
            nLoader.show();
        }
    }

    /**
     *  Opens a (default) dialog.
     *  Returns a dialog object.
     */
    ds.open = function (options) {

        // options
        var _options = $.extend({
            title: null,            // The title of the dialog.
            content: null,          // The content to display in the dialog.
            ajax: null,             // Ajax Content
            loader: null,           // Indicator if a loader must be showed.
            dimension: null,        // The dimension to display the dialog with.
            tier: true,             // Indicator if the dialog should be shown in a higher tier. Will be stored in the chaining as an index (1-based).
            closable: true,         // Indicator if the default close button is shown.
            callback: null,         // The callback to fire when the dialog will be closed.
            environment: null,      // The environment to display the content in (portal, dashboard).
            skin: null,             // The skin (alias css class to use)
            displayHeader: true,    // Display header
            displayButtons: true,   // Display buttons      
            display: null           // display settings
        }, options);


        // options > display
        if (_options.display != null) {

            var _val = _options.display;
            var isString = (typeof _val === 'string' || _val instanceof String);

            if (isString) {
                _options.display = $.extend({ type: _val }, _options.display);
            } else {
                _options.display = $.extend({ type: null }, _options.display);
            }

            // check types
            if (_options.display.type != 'popup' && _options.display.type != 'sidebar') {
                _options.display = null;
            }
        }

        // options > display (default)
        if (_options.display == null || _options.display.type == null) {

            // default: popup
            _options.display = { type: 'popup' };
        }


        // options > dimension
        _options.dimension = defaultDimension(_options.dimension);

        // options > dimension > popup
        if (_options.display.type == 'popup') {

            // dimension > width/height > disable full-mode
            if (_options.dimension.width != null || _options.dimension.height != null) {
                _options.dimension.full = false;
            }

            // dimension > full > disable draggable
            if (_options.dimension.full === true) {
                _options.dimension.draggable = false;
            }
        }

        // open
        if (_options.display.type == 'sidebar') {

            // open dialog
            var _dialog = _open(_options);

            return _dialog;

        } else {

            // load via ajax
            if (_options.ajax != null) {

                var _ajax = _options.ajax;

                ds.loader({ text: _ajax.text });

                TH.ajax.postJSON({
                    url: _ajax.url,
                    data: _ajax.data,
                    success: function (response) {

                        _options.content = response;

                        // open dialog
                        var _dialog = _open(_options);

                        if (_ajax.success != null) {
                            _ajax.success(_dialog, response);
                        }
                    },
                    error: function () {
                        ds.loader(false);
                    }
                });

                return null;
            } else {

                // open dialog
                var _dialog = _open(_options);

                return _dialog;
            }
        }
    }

    /**
     *  Resolves the current dialog and passes data back to the instigator.
     */
    ds.done = function (output) {

        //= parse
        var _output = $.extend({
            action: null, // custom (or ds close => 'close')
            data: null
        }, output);

        //= dialog
        var _dialog = chain.last();
        if (_dialog == null) return; // just to be safe...

        // mark as closing
        _dialog.state = 'closing';

        // loader (not necessary)
        //ds.loader({ text: 'Closing...' });

        // container
        var container = $('#' + _ID_CONTAINER);
        var containerType = container;
        if (_dialog.display.type == 'popup') {
            containerType = $('#' + _ID_CONTAINER_POPUP);
        } else if (_dialog.display.type == 'sidebar') {
            containerType = $('#' + _ID_CONTAINER_SIDEBAR);
        }


        //= callback
        if (_dialog.callback != null) {

            var event = { action: _output.action, close: true };
            var data = (_output.data != null ? _output.data : undefined);

            _dialog.callback(event, data);
        }

        //=== close      
        var _wholeTier = chain.tier(_dialog.tier);



        $body = _element(containerType, _ID_BODY, _dialog.tier);

        // animate (disabled)
        //if (_wholeTier.length < 2)
        //{
        //    $body.stop(true).animate({ 'opacity': 0 }, {
        //        'duration': 500,
        //        'complete': function () {
        //            _logic();
        //        }
        //    });
        //}
        //else
        //{
        _logic();
        //}

        // following logic (because of the timeout animation)
        function _logic() {

            // container
            //var container = $('#' + _ID_CONTAINER);

            // dialog

            // remove from the dom
            _deactivate(_dialog.key);
            _removeContainer(_dialog.key);

            // popup: tier
            if (_dialog.display.type == 'popup') {

                // check underlaying dialogs
                if (_wholeTier.length > 1) {

                    // activate
                    var _underlayingDialog = chain.get(_dialog.key - 1);

                    _activate(_underlayingDialog.key);
                }

                // hide tier body
                if (_wholeTier.length < 2) {

                    // hide
                    $body.hide();

                    // update overlay counter
                    _cOverlayIndex = (_dialog.tier > 2 ? (_dialog.tier - 1) : -1);

                    // adjust overlay
                    var overlay = $('#' + _ID_OVERLAY);
                    if (_dialog.tier > 2) {
                        overlay.css({ 'z-index': (_dialog.tier - 1) });
                    }
                    else {
                        overlay.css({ 'z-index': '-1' });
                    }
                }
            }

            // remove from chain
            chain.remove(_dialog.key);

            // hide system
            if (chain.count() === 0) {

                // hide container
                container.css('display', 'none');
            }

            // popup: hide overlay
            if (_dialog.display.type == 'popup') {
                if (chain.count() === 0) {

                    // hide popup
                    container.find('#' + _ID_MAIN).hide();
                }
            }

            // sidebar: clear
            if (_dialog.display.type == 'sidebar') {

                //var containerSidebar = container.find('#' + _ID_CONTAINER_SIDEBAR);
                var node = containerType.find('[data-ds-key=' + _dialog.key + ']');
                node.remove();
            }

            // release page scrolling
            _blockScroll(false);

            // remove loader (not necessary)
            //ds.loader(false);
        }
    }

    /**
     *  Sends an update to the dialog's awaiting instigator.
     *  Uses the same response format as with 'done' function).
     */
    ds.notify = function (output) {

        // parse
        var _output = $.extend({
            close: false,
            action: null,
            data: null
        }, output);

        // dialog
        var _dialog = chain.active();
        if (_dialog == null) return;

        // callback
        if (_dialog.callback != null) {
            var event = { action: _output.action, close: false };
            var data = (_output.data != null ? _output.data : undefined);

            _dialog.callback(event, data)
        }
    }

    /**
     *  Returns the active dialog's key.
     */
    ds.key = function () {

        var _dialog = chain.last();

        return (_dialog != null ? _dialog.key : null);
    }

    /**
     *  Returns the element (wrapper in jquery).
     */
    ds.element = function (element) {

        var elements = {
            'main': '#' + _ID_MAIN
        };

        if (elements[element] != undefined) {
            return $(elements[element]);
        }
        else {
            return undefined;
        }
    }

    // Finds a dialog.
    ds.dialog = function (options) {

        // options = 'key', future: check for obj
        var _key = options;

        // search in chain
        var _dialog = null;
        if (_key != null) {
            _dialog = chain.get(_key);
        } else {
            _dialog = chain.last();
        }

        return _dialog;
    }

    // Sets a new dialog title
    ds.title = function (options) {

        // Options
        var _options = $.extend({
            title: null,            // The title of the dialog.
            key: null              // The key of the dialog.
        }, options);

        // Get key of current dialog if null
        if (options.key === null) {
            _options.key = DialogSystem.key;
        }

        // Get dialog
        var dialog = DialogSystem.dialog(_options.key);

        // Get element
        var element = _element($('#' + _ID_CONTAINER), _ID_TITLE, dialog.tier);

        // Set title
        element.html(_options.title);
    }


    //=== private

    // opens the (actual) dialog.
    // returns a dialog object.
    function _open(_options) {

        //=== Chain
        var _activeTier = chain.activeTier();
        var _currentDialog = chain.last();

        var _dialog = chain.add(_options);

        // check if there is a dialog already active
        if (_currentDialog != null) {
            // check if tier is shifted
            if (_activeTier == _dialog.tier) {
                _deactivate(_currentDialog.key);
            }
        }

        // container
        var container = $('#' + _ID_CONTAINER);
        container.css('display', 'block');

        // open: type
        if (_options.display.type == 'sidebar') {
            _openSidebar(_dialog, _options);
        } else if (_options.display.type == 'popup') {
            _openPopup(_dialog, _options);
        } else {
            _openPopup(_dialog, _options);
        }

        // return dialog handle
        return _dialog;
    }

    // func: opens dialog of type popup
    function _openPopup(_dialog, _options) {

        //=== CONTAINER

        var container = $('#' + _ID_CONTAINER);
        var containerPopup = container.find('#' + _ID_CONTAINER_POPUP);

        //=== Reference

        $main = containerPopup.find('#' + _ID_MAIN);
        var overlay = containerPopup.find('#' + _ID_OVERLAY);
        $loader = containerPopup.find('#' + _ID_LOADER);
        $body = _element(containerPopup, _ID_BODY, _dialog.tier);
        $title = $body.find("#" + _ID_TITLE);
        $content = $body.find("#" + _ID_CONTENT);
        $buttons = $body.find("#" + _ID_BUTTONS);

        //=== Set key
        $body.attr('data-ds-key', _dialog.key);


        //=== Set Environment / Skin
        if (_options.environment != null) { $body.addClass(_options.environment); }
        if (_options.skin != null) { $body.addClass(_options.skin); }


        //=== HEADER

        // hide header
        if (_options.displayHeader == false) {
            $body.children('div#' + _ID_HEADER).css('display', 'none');
        }
        else {
            $body.children('div#' + _ID_HEADER).css('display', '');
        }

        // Cleanup steps
        var stepsArea = $body.children('div#' + _ID_HEADER).find('[data-hook=ds-steps]');
        if (stepsArea.length > 0) { stepsArea.remove(); }

        //=== BUTTONS

        // hide buttons
        if (_options.displayButtons == false) {
            $body.children('div#' + _ID_BUTTONS).css('display', 'none');
        }
        else {
            $body.children('div#' + _ID_BUTTONS).css('display', '');
        }

        // Reset resize button and hide resize button on full dimension
        $resizeButton = _element(containerPopup, _ID_RESIZE, _dialog.tier);
        $resizeButton.removeClass("hidden");
        if (_options.dimension.full === true) {
            $resizeButton.addClass("hidden");
        }




        //=== CONTENT : RESET

        // clear current content
        $content.empty();

        // Show popup
        $main.show();
        $body.show();


        //=== DIMESION / OVERLAY

        // block page scrolling
        _blockScroll();

        // Hide close button
        if (_options.closable) {
            _element(containerPopup, _ID_CLOSE, _dialog.tier).show();
        } else {
            _element(containerPopup, _ID_CLOSE, _dialog.tier).hide();
        }

        // Adjust Main & Loader visibility
        if ($main.is(":visible") == false) { $main.show(); }
        if ($loader.is(":visible") == true) { ds.loader(false); }

        // update overlay counter
        _cOverlayIndex = (_dialog.tier > 1 ? _dialog.tier : -1);

        // adjust overlay
        if (_dialog.tier > 1) {
            overlay.css({ 'z-index': _dialog.tier });
        }

        // Show 'main'
        //$main.css({ 'opacity': 'initial' });
        //$main.css({ 'opacity': '1' });
        //$main.show();

        // Show 'body' (animation disabled)
        // animate only when there no underlaying dialogs in the tier
        //var _wholeTier = chain.tier(_dialog.tier);
        //if (_wholeTier.length < 2) {
        //    $body.stop(true).animate({ 'opacity': '1' }, 700);
        //} else {
        //$body.css({ 'opacity': 'initial' });
        //$body.css({ 'opacity': '1' });
        $body.show();
        //}



        //=== TITLE

        // clear current title
        $title.text('');

        // Set title (override)
        if (_options.title != null) {
            $title.text(_options.title);
        }


        //=== CONTENT : SET

        // set content
        if (_options.content != null) {
            // Fix: use opacity '0' instead of display:none to allow height calculations          
            //$main.css({ 'opacity': '0', 'display': 'block' });
            //$body.css({ 'opacity': '0', 'display': 'block' });

            $content.html(_options.content);
        }



        //=== TITLE : OVERRIDE

        // Try find title inside content
        $dataTitle = $content.find('div[data-ds-title]');
        if ($dataTitle.length > 0) {
            // cleanup default title
            var overrideTitle = $dataTitle.attr('data-ds-title');
            $dataTitle.remove();

            // if there was no override, set title
            if (_options.title == null) {
                // set title
                $title.text(overrideTitle);

                // update dialog obj
                _dialog.title = overrideTitle;
            }
        }


        //=== NAVIGATION

        // update navigation
        _navigation(_dialog.tier);


        // Adjust dimension
        _setDimension(_dialog, _options.dimension);

        //=== BUTTON: SAVE

        // try find button container => Save
        $dataButtonSave = $content.find('div[data-ds-button=save]');
        if ($dataButtonSave.length > 0) {
            $bCon = $dataButtonSave;
            $bCon.children().appendTo($buttons);
            $bCon.remove();
        }

        // try find button attribute => Save
        $dataButtonSave = $content.find('[data-ds-button=save]');
        if ($dataButtonSave.length > 0) {
            $dataButtonSave.removeAttr('data-ds-button');
            $dataButtonSave.appendTo($buttons);
        }


        //=== BUTTON: CANCEL

        // try find button container => Cancel
        $dataButtonCancel = $content.find('div[data-ds-button=cancel]');
        if ($dataButtonCancel.length > 0) {
            $bCon = $dataButtonCancel;
            $bCon.children().appendTo($buttons);
            $bCon.remove();
        }

        // try find button attribute => Cancel
        $dataButtonCancel = $content.find('[data-ds-button=cancel]');
        if ($dataButtonCancel.length > 0) {
            $dataButtonCancel.removeAttr('data-ds-button');
            $dataButtonCancel.appendTo($buttons);
        }


        //=== BUTTON: ADDITIONAL

        // try find button => button (additional/multiple)
        $dataButtons = $content.find('div[data-ds-button=button]');
        for (var i = 0; i < $dataButtons.length; i++) {
            $bCon = $($dataButtons.get(i));
            $button = $bCon.children();
            $button.appendTo($buttons);
        }
        $dataButtons.remove();

        // try find button attribute => button (additional/multiple)
        $dataButtons = $content.find('[data-ds-button=button]');
        for (var i = 0; i < $dataButtons.length; i++) {
            $button = $($dataButtons.get(i));
            $button.removeAttr('data-ds-button');
            $button.appendTo($buttons);
        }
        $dataButtons.remove();


        //=== BUTTON-GROUP

        // try find button group
        $dataButtonGroup = $content.find('div[data-ds-buttons]');
        for (var i = 0; i < $dataButtonGroup.length; i++) {
            $bCon = $($dataButtonGroup.get(i));
            $button = $bCon.children();
            $button.appendTo($buttons);
        }
        $dataButtonGroup.remove();


        //=== TRIGGER

        // Handle resize ( // Todo: .off(resize) when DS is closed )
        $(window).on('resize', function () {
            _setDimension(_dialog, _options.dimension);
        });

        // Trigger event 'open'
        $main.trigger("open.dialog", { key: _dialog.key, height: $content.height() });
    }

    // func: opens dialog of type sidebar
    function _openSidebar(dialog, options) {

        // loader (disable)
        var nLoader = $("#" + _ID_LOADER);
        if (nLoader.is(":visible") == true) { ds.loader(false); }

        var container = $('#' + _ID_CONTAINER);
        var containerSidebar = container.find('#' + _ID_CONTAINER_SIDEBAR);

        // options: skin
        var cContentClass = '';
        if (options.environment != null) { cContentClass += ' ' + options.environment; }
        if (options.skin != null) { cContentClass += ' ' + options.skin; }

        // options: dimension
        var cSidebarStyle = '';
        //if (options.dimension != null) {
        //if (options.dimension.maxWidth != null) { cSidebarStyle += ' max-width:' + options.dimension.maxWidth + ';'; }
        //if (options.dimension.width != null) { cSidebarStyle += ' width:' + options.dimension.width + ';'; }
        //if (options.dimension.minWidth != null) { cSidebarStyle += ' min-width:' + options.dimension.minWidth + ';'; }
        //}

        // block page scrolling
        _blockScroll();

        // render: dialog
        var h = '';
        h += '<div data-ds-dialog="sidebar" data-ds-key="' + dialog.key + '" class="ds-dialog-sidebar">';
        h += '<div data-ds-hook="overlay" class="ds-overlay"></div>';
        h += '<div data-ds-hook="sidebar" class="ds-sidebar" style="' + cSidebarStyle + '">';
        h += '<div data-ds-hook="content" class="ds-content ' + cContentClass + '">';

        // loader
        h += '<div data-ds-hook="loader" class="ds-loader"><div class="ds-loader-image"></div></div>';

        h += '</div>';
        h += '</div>';
        h += '</div>';

        // append
        var node = containerSidebar.append(h).find('[data-ds-key=' + dialog.key + ']');

        // animate
        if (options.dimension != null) {
            var _node = containerSidebar.find('[data-ds-key=' + dialog.key + ']');
            var _sidebar = _node.find('[data-ds-hook=sidebar]');

            var transform = {};
            if (options.dimension.maxWidth != null) { transform['max-width'] = options.dimension.maxWidth; }
            if (options.dimension.width != null) { transform['width'] = options.dimension.width; }
            if (options.dimension.minWidth != null) { transform['min-width'] = options.dimension.minWidth; }

            _sidebar.animate(transform, '300', complete);
        } else {
            complete();
        }

        // after setup is complete
        function complete() {

            // content
            if (options.content != null) {
                var nodeContent = node.find('[data-ds-hook=content]');
                nodeContent.html(options.content);
            }

            // set node
            dialog.node = containerSidebar.find('[data-ds-key=' + dialog.key + ']');

            // bind: close
            var nOverlay = node.find('[data-ds-hook=overlay]');
            $(nOverlay).click(function (e) {
                e.preventDefault();
                e.stopPropagation();

                ds.done({ action: 'close' });
            });

            // load via ajax
            if (options.ajax != null) {

                var _ajax = options.ajax;

                TH.ajax.postJSON({
                    url: _ajax.url,
                    data: _ajax.data,
                    success: function (response) {

                        var _content = response;

                        dialog.node.find('[data-ds-hook=content]').html(_content);

                        if (_ajax.success != null) {
                            _ajax.success(dialog, response);
                        }
                    },
                    error: function () {
                        dialog.node.find('[data-ds-hook=content]').html('');
                    }
                });
            }
        }
    }

    /**
     *  Set the 'body' dimension.
     *  @param options The dimension options.
     *  @param tier The target tier (1-based).
     */
    function _setDimension(_dialog, options) {

        // vars
        var _options = options;
        var _tier = parseInt(_dialog.tier);

        // container
        var container = $('#' + _ID_CONTAINER);
        //if (_dialog.display.type == 'popup') {
        //    container = $('#' + _ID_CONTAINER_POPUP);
        //} else if (_dialog.display.type == 'sidebar') {
        //    container = $('#' + _ID_CONTAINER_SIDEBAR);
        //}

        // nodes
        var nBody = _element(container, _ID_BODY, _tier);

        // Make it visible, temporarily (if needed)
        // This is needed to calculate the widths and heights, if its not visible everything is 0
        var hideWhenReady = false;
        if (nBody.css('display') == 'none') {
            hideWhenReady = true;
            nBody.css({ 'opacity': '0', 'display': 'block' });
        }

        // Percentages
        var widthPercentage, heightPercentage, widthOffsetPercentage, heightOffsetPercentage;

        // Calculate dimensions
        var percentageChop = (_tier * 5); // default: 100 - (1 * 5) = 95
        var fullPercentage = 100 - percentageChop;

        if (_options.full === true) {
            heightPercentage = fullPercentage;
            widthPercentage = fullPercentage;
        } else {

            var _wPercentage, _hPercentage;

            // Check if the width is in pixels or percentage
            if (_options.width == null) {
                _wPercentage = fullPercentage;
            } else if (!isNaN(_options.width) || _options.width.indexOf('px') > -1) {
                _wPercentage = Math.round((parseInt(_options.width) / $(window).outerWidth()) * 100);
            } else {
                _wPercentage = parseInt(_options.width);
            }

            // Check if the height is in pixels or percentage
            if (_options.height == null) {
                _hPercentage = fullPercentage;
            }
            else if (!isNaN(_options.height) || _options.height.indexOf('px') > -1) {
                _hPercentage = Math.round((parseInt(_options.height) / $(window).outerHeight()) * 100);
            } else {
                _hPercentage = parseInt(_options.height);
            }

            heightPercentage = _hPercentage;
            widthPercentage = _wPercentage;
        }

        // Collect height/width data
        var bodyHeight = $(window).outerHeight();
        var bodyWidth = $(window).outerWidth();
        var dsHeaderHeight = nBody.children('#' + _ID_HEADER).outerHeight();
        var dsButtonsHeight = nBody.children('#' + _ID_BUTTONS).outerHeight();
        var dsNavigationHeight = 0;

        // header / buttons hidden?
        if (nBody.children('#' + _ID_HEADER + ':visible').length == 0) dsHeaderHeight = 0;
        if (nBody.children('#' + _ID_BUTTONS + ':visible').length == 0) dsButtonsHeight = 0;

        if (nBody.children('#' + _ID_NAVIGATION).is(':visible')) {
            dsNavigationHeight = nBody.children('#' + _ID_NAVIGATION).outerHeight();
        }

        // Calculate new dimensions for box
        var newDsHeight = (bodyHeight * (heightPercentage / 100));
        var newDsWidth = (bodyWidth * (widthPercentage / 100));
        var newDsTop = ((bodyHeight - newDsHeight) / 2);
        var newDsLeft = ((bodyWidth - newDsWidth) / 2);

        // Set box
        nBody.css({
            'height': newDsHeight + 'px',
            'width': newDsWidth + 'px',
            'top': newDsTop + 'px',
            'left': newDsLeft + 'px'
        });

        // Set box
        //nBody.css({
        //    'height': 'calc(' + heightPercentage + '% - 60px)', // 60px as correction for buttonRow
        //    'width': widthPercentage + '%',
        //    'top': 'calc(' + ((100 - heightPercentage) / 2) + '% + 30px)', // 30px as correction for buttonRow (50px / 2)
        //    'left': ((100 - widthPercentage) / 2) + '%'
        //});

        // Calculate new dimensions for box content
        var fixedContentHeight = nBody.height() - (dsHeaderHeight + dsNavigationHeight + dsButtonsHeight);

        // Set box content
        nBody.children('#' + _ID_CONTENT).css({ 'height': fixedContentHeight + 'px', 'overflow-y': 'auto' });

        // Reset visibility if needed
        if (hideWhenReady === true) {
            nBody.css({ 'opacity': '', 'display': 'none' });
        }

        // feature: draggable
        if (_options.draggable === true) {
            if ($.fn.draggable) {
                nBody.addClass('draggable');

                nBody.draggable({
                    //containment: 'parent',
                    handle: '> #ds-header',
                    cursor: 'move'
                });
            }
        } else {
            nBody.removeClass('draggable');
            if ($.fn.draggable) {
                try { nBody.draggable('destroy'); } catch (ex) { }
            }
        }
    }

    /**
     *  Get html element (wrapped in jquery).
     * @param container The jquery container element.
     */
    function _element(container, id, tier) {

        if (tier != undefined) {
            if (id == _ID_BODY) {
                return $('div[id=' + id + '][data-ds-tier=' + tier + ']');
            }
            else {
                return $('div[data-ds-tier=' + tier + '] [id=' + id + ']');
            }
        } else {
            //return $('#' + id);
            return $('[id=' + id + ']');
        }
    }

    /**
     *  Deactivate dialog.
     *  @param key The dialog's key.
     */
    function _deactivate(key) {

        var _dialog = chain.get(key);

        // check
        if (_dialog == null) return;

        // container
        var container = $('#' + _ID_CONTAINER);
        if (_dialog.display.type == 'popup') {
            container = $('#' + _ID_CONTAINER_POPUP);
        } else if (_dialog.display.type == 'sidebar') {
            container = $('#' + _ID_CONTAINER_SIDEBAR);
        }

        // prepare dialog container
        var container_html = '' +
            '<div data-ds-key="' + _dialog.key + '">' +
            '<div data-ds-content=""></div>' +
            '<div data-ds-buttons=""></div>' +
            '</div>';

        var nCollection = $('#' + _ID_COLINACTIVE);
        nCollection.append(container_html);

        var nBody = _element(container, _ID_BODY, _dialog.tier);

        // remove environment / skin
        if (_dialog.environment != null && nBody.hasClass(_dialog.environment)) { nBody.removeClass(_dialog.environment); }
        if (_dialog.skin != null && nBody.hasClass(_dialog.skin)) { nBody.removeClass(_dialog.skin); }

        // move content
        nBody.find('#' + _ID_CONTENT + ' > *').appendTo(nCollection.find('div[data-ds-key=' + _dialog.key + '] > div[data-ds-content]'));

        // move buttons
        if (nBody.find('#' + _ID_BUTTONS).length > 0) {
            nBody.find('#' + _ID_BUTTONS + ' > *').appendTo(nCollection.find('div[data-ds-key=' + _dialog.key + '] > div[data-ds-buttons]'));
        }
    }

    /**
     *  Activate dialog.
     *  @param key The dialog's key.
     */
    function _activate(key) {

        var _dialog = chain.get(key);

        // check
        if (_dialog == null) return;

        // container
        var container = $('#' + _ID_CONTAINER);
        if (_dialog.display.type == 'popup') {
            container = $('#' + _ID_CONTAINER_POPUP);
        } else if (_dialog.display.type == 'sidebar') {
            container = $('#' + _ID_CONTAINER_SIDEBAR);
        }

        $body = _element(container, _ID_BODY, _dialog.tier);

        // set title
        $title = _element(container, _ID_TITLE, _dialog.tier);
        $title.text(_dialog.title != null ? _dialog.title : '');

        // set environment / skin
        if (_dialog.environment != null && !$body.hasClass(_dialog.environment)) { $body.addClass(_dialog.environment); }
        if (_dialog.skin != null && !$body.hasClass(_dialog.skin)) { $body.addClass(_dialog.skin); }

        // move content
        $collection.find('div[data-ds-key=' + _dialog.key + '] > div[data-ds-content] > *')
            .appendTo($body.find('#' + _ID_CONTENT));

        // move buttons
        if ($body.find('#' + _ID_BUTTONS).length > 0) {
            $collection.find('div[data-ds-key=' + _dialog.key + '] > div[data-ds-buttons] > *')
                .appendTo($body.find('#' + _ID_BUTTONS));
        }

        // update navigation (need to filter the tier a little bit, 
        //          because on this point the tier is not fully updated yet).
        var _wholeTier = chain.tier(_dialog.tier);
        var _aTier = [];
        for (var i = 0; i < _wholeTier.length; i++) {
            var _d = _wholeTier[i];
            if (_d.key <= _dialog.key) {
                _aTier.push(_d);
            }
        }
        _navigation(_dialog.tier, _aTier);

        // set dimension
        _setDimension(_dialog, _dialog.dimension);

        // remove container
        _removeContainer(_dialog.key);
    }

    /**
     *  Remove dialog container from the collection.
     *  @param [key] The dialog's key, set key undefined to remove all containers.
     */
    function _removeContainer(key) {

        $collection = $('#' + _ID_COLINACTIVE);

        if (key == undefined) {
            $collection.empty();
        }
        else {
            $collection.find('div[data-ds-key=' + key + ']').remove();
        }
    }

    /**
     *  Updates the navigation of specified tier.
     *  @param tier The tier id.
     *  @param [dialogs] The tier arraylist (optional).
     */
    function _navigation(tier, dialogs) {

        var containerPopup = $('#' + _ID_CONTAINER_POPUP);

        $navigation = _element(containerPopup, _ID_NAVIGATION, tier);

        // reset
        $navigation.css({ 'display': 'none' });
        $list = $navigation.find('ul');
        $list.empty();

        var _aTier = (dialogs != undefined ? dialogs : chain.tier(tier));

        if (_aTier.length > 1) {
            var html = '';
            for (var i = 0; i < _aTier.length; i++) {
                var _dialog = _aTier[i];

                var _title = (_dialog.title != null ? _dialog.title : '...');

                if (i == 0) {
                    // first
                    html += '<li data-ds-key="' + _dialog.key + '" class="first"><a href="#">' + _title + '</a><span class="divider">/</span></li>';
                }
                else if (i == (_aTier.length - 1)) {
                    // last
                    html += '<li data-ds-key="' + _dialog.key + '" class="last">' + _title + '</li>';
                }
                else {
                    // middle
                    html += '<li data-ds-key="' + _dialog.key + '" ><a href="#">' + _title + '</a><span class="divider">/</span></li>';
                }
            }

            $list.html(html);
            $navigation.css({ 'display': 'block' });
        }
    }

    /**
     *  Block the page's scrolling.
     */
    function _blockScroll(block) {

        $html = $('html');

        if (block === false) {
            if (chain.count() == 0 && $html.hasClass('ds-noscroll')) {
                $html.removeClass('ds-noscroll');
            }
        }
        else {
            if (!$html.hasClass('ds-noscroll')) {
                $html.addClass('ds-noscroll');
            }
        }
    }

    /**
     *  Chain.
     */
    var chain = (function () {

        var _chain = {};

        /**
         *  The dialog collection.
         */
        var _collection = [];

        /**
         *  Add dialog to the chain.
         *  @param options The options argument from the 'open' function.
         *  @return The added dialog.
         */
        _chain.add = function (options) {

            // format => options
            var _options = $.extend({
                title: null,
                content: null,
                dimension: { full: true, width: null },
                tier: false,
                callback: null,
                environment: null,
                skin: null,
                display: null
            }, options);

            // create => dialog
            var _dialog = newDialog();
            _dialog.key = newKey();
            _dialog.tier = 1;
            _dialog.title = _options.title;
            _dialog.dimension = _options.dimension;
            _dialog.callback = _options.callback;
            _dialog.state = null;
            _dialog.environment = _options.environment;
            _dialog.skin = _options.skin;
            _dialog.node = null;
            _dialog.display = _options.display;

            // set tier
            var _activeTier = _chain.activeTier();

            if (_options.tier === true && _chain.last() != null) {
                if (_activeTier < _MAX_TIER) {
                    _activeTier += 1;
                }
            }
            _dialog.tier = _activeTier;

            _collection.push(_dialog);

            return _dialog;
        }

        /**
         *  Remove dialog from the chain.
         *  @param key The key (zero-based).
         */
        _chain.remove = function (key) {

            var _key = parseInt(key);

            if (_collection.length > 0) {
                // note: reversed based on usage
                for (var i = (_collection.length - 1); i >= 0; i--) {
                    var _dialog = _collection[i];

                    if (_dialog.key === _key) {
                        _collection.splice(i, 1);
                        i = -1;
                    }
                }
            }
        }

        /**
         *  Get dialog from the chain.
         *  @param key The key (zero-based).
         *  @returns The dialog.
         */
        _chain.get = function (key) {

            var _key = parseInt(key);

            if (_collection.length > 0) {
                // note: reversed based on usage
                for (var i = (_collection.length - 1); i >= 0; i--) {
                    var _dialog = _collection[i];

                    if (_dialog.key === _key) {
                        return _dialog;
                    }
                }
            }

            return null;
        }

        /**
         *  Get the last dialog from the chain.
         */
        _chain.last = function () {

            if (_collection.length == 0) {
                return null;
            } else {
                return _collection[_collection.length - 1];
            }
        }

        /**
         *  Get the active dialog from the chain.
         *  Checks if the dialog is not closing.
         */
        _chain.active = function () {

            if (_collection.length > 0) {
                for (var i = _collection.length - 1; i >= 0; i--) {

                    var _dialog = _collection[i];
                    if (_dialog.state != 'closing') {
                        return _dialog;
                    }
                }
            }

            return null;
        }

        /**
         *  Returns the 'active' tier.
         */
        _chain.activeTier = function () {

            if (_collection.length > 0) {
                var _activeTier = 1;

                for (var i = 0; i < _collection.length; i++) {
                    var _dialog = _collection[i];

                    var _dTier = parseInt(_dialog.tier);

                    if (_dTier > _activeTier) {
                        _activeTier = _dTier;
                    }
                }

                return _activeTier;
            }
            else {
                return 1;
            }
        }

        /**
         *  Returns all dialogs in the active tier.
         *  @param tier The target tier.
         *  @return Returns the array with dialogs.
         */
        _chain.tier = function (tier) {

            var _tier = parseInt(tier);
            var _array = [];

            if (_collection.length > 0) {
                for (var i = 0; i < _collection.length; i++) {
                    var _dialog = _collection[i];

                    if (_tier === parseInt(_dialog.tier)) {
                        _array.push(_dialog);
                    }
                }

                return _array;
            }
            else {
                return _aTier;
            }
        }

        /**
         *  Returns the count of all dialogs in the chain.
         */
        _chain.count = function () {
            return _collection.length;
        }

        /**
         *  Returns the whole collection.
         */
        _chain.collection = function () {
            return _collection;
        }

        /**
         *  Creates a new key based on the current collection.
         */
        function newKey() {

            var _dialog = _chain.last();

            if (_dialog == null) {
                return 0;
            } else {
                var key = parseInt(_dialog.key);

                return (key + 1);
            }
        }

        return _chain;
    }());


    //=== prototypes

    // prototype: dialog
    function newDialog() {

        var d = {};

        d.key = null;
        d.tier = null;
        d.title = null;
        d.dimension = null;
        d.callback = null;
        d.state = null;
        d.environment = null; // deprecated, replace with 'skin'
        d.skin = null;
        d.steps = null; // array (obj => { key, label })

        // func: dimension
        d.setDimension = function (options) {

            if (options == 'initial') {
                _setDimension(d, d.dimension);
            } else {
                var _options = defaultDimension(options);

                _setDimension(d, _options);
            }
        }

        // func: render steps
        d.renderSteps = function () {

            if (d.steps == null || d.steps.length == 0) return;

            // container (default is popup)
            var containerPopup = $('#' + _ID_CONTAINER_POPUP);

            // header
            var header = _element(containerPopup, _ID_HEADER, d.tier);
            header.find('[data-hook=ds-steps]').remove();

            // render
            var h = '<div data-hook="ds-steps" class="ds-steps no-user-select">';
            h += '<ul class="">';

            for (var i = 0; i < this.steps.length; i++) {
                var step = this.steps[i];

                h += '<li data-key="' + step.key + '" class="step ' + (i == 0 ? 'active' : '') + '">';
                h += '<div class="line"></div>';
                h += '<div class="index">' + (i + 1) + '</div>';
                h += '<div class="label truncate">' + step.label + '</div>';
                h += '</li>';
            }

            h += '</ul>';
            h += '</div>';

            header.append(h);
        }

        return d;
    }

    // default: dimension
    function defaultDimension(options) {
        return $.extend({
            full: true,                     // display the dialog filling the screen
            width: null,                    // the dialog width
            height: null,                   // the dialog height
            minWidth: null,                 // the dialog min width
            minHeight: null,                // the dialog min height
            maxWidth: null,                 // the dialog max width
            maxHeight: null,                // the dialog max height
            draggable: true                 // draggable dialog
        }, options);
    }

    return ds;
}(jQuery));


/**
 * Dialog System Initialization
 */
$(document).ready(function () {
    DialogSystem.init();
});