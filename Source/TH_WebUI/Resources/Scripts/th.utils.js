(function ($) {
    // gets the form values and __RequestVerificationToken
    $.fn.antiForgerySerializeArray = function () {

        if (!$(this).is("form"))
            return;

        var data = this.serializeArray();
        data.__RequestVerificationToken = this.find('input[name=__RequestVerificationToken]').val();
        return data;
    };

    // replace element and returns newly created element
    $.fn.replaceWithPush = function (elem) {
        var $elem = $(elem);

        this.replaceWith($elem);
        return $elem;
    };

    // serialize a from to an oject
    $.fn.serializeObject = function () {
        var o = {};

        // Find disabled inputs, and remove the "disabled" attribute
        var disabled = this.find(':input:disabled').removeAttr('disabled');

        // serialize the form
        var a = this.serializeArray();

        // re-disabled the set of inputs that you previously enabled
        disabled.attr('disabled', 'disabled');


        $.each(a, function () {
            if (o[this.name] !== undefined) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };

    // returns the window reference of an iframe
    $.fn.iframeWindow = function () {

        return _getIframeWindow(this[0]);

        // private => iframe helper
        function _getIframeWindow(iframe_object) {
            var doc;

            if (iframe_object.contentWindow) {
                return iframe_object.contentWindow;
            }

            if (iframe_object.window) {
                return iframe_object.window;
            }

            if (!doc && iframe_object.contentDocument) {
                doc = iframe_object.contentDocument;
            }

            if (!doc && iframe_object.document) {
                doc = iframe_object.document;
            }

            if (doc && doc.defaultView) {
                return doc.defaultView;
            }

            if (doc && doc.parentWindow) {
                return doc.parentWindow;
            }

            return undefined;
        }
    }

    // attaches observer
    var addedListeners = {};
    $.fn.observe = function (event, func) {
        // note: target should be window        


        // check already added
        if (addedListeners[event]) return;

        // pin
        addedListeners[event] = func;

        if ($.isWindow(this[0])) {

            var w = this[0];

            //  postmessage protocol
            if (event == 'message') {
                if (w.addEventListener) {
                    w.addEventListener("message", func, false);
                }
                else if (window.attachEvent) {
                    window.attachEvent("onmessage", func, false);
                }
            }
        } else {

            // attach
            this[0].addEventListener(event, func);
        }
    }
}(jQuery));