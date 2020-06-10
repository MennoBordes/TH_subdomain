/**
 * Dependencies:
 *      - jQuery
 */

var TH = (function ($) {

    var th = {};

    var ajax = th.ajax = {};

    /* 
     * Execute ajax request.
     */
    ajax.execute = function (options) {

        // http://api.jquery.com/jquery.ajax/

        var _options = $.extend({
            url: null,
            async: true,
            cache: false,
            type: 'POST',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            data: null,
            processData: true,
            success: null,
            error: null,
            complete: null,
            timeout: 0,
            dataType: null,
            feedback: {
                success: false,
                error: true,
                complete: false
            }
        }, options);

        $.ajax({
            url: _options.url,
            async: _options.async,
            cache: _options.cache,
            type: _options.type,
            contentType: _options.contentType,
            data: _options.data,
            timeout: _options.timeout,
            dataType: _options.dataType,
            success: function (data, status, xhr) {

                // callback
                if (_options.success != null) {
                    _options.success(data, status, xhr)
                }
            },
            error: function (xhr, status, error) {
                var message = null;
                message = "ERROR";

                // message
                if (error != undefined && error != null && error.length > 0) {
                    message = error;
                }

                // event: unauthorized
                if (xhr.status == 401 && ajax.events.unauthorized != null) ajax.event.unauthorized(xhr);

                // callback
                if (_options.error != null) {
                    _options.error(xhr, status, error);
                }
            },
            complete: function (xhr, status) {

                // callback
                if (_options.complete != null) {
                    _options.complete(xhr, status);
                }
            }
        });
    }

    /*
     * Ajax post (uses the basic execute method).
     */
    ajax.post = function (options) {

        var _options = $.extend({
            type: 'POST',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        }, options);

        ajax.execute(_options);
    };

    /* 
     * Ajax post JSON (uses the basic execute method).
     */
    ajax.postJSON = function (options) {

        var _options = $.extend({
            type: 'POST',
            contentType: 'application/json; charset=UTF-8',
            processData: false,
            data: null
        }, options);

        // process data
        _options.data = JSON.stringify(_options.data);

        ajax.execute(_options);
    }

    /*
     * Ajax events.
     */
    ajax.events = {
        unauthorized: null
    };

    return th;
}(jQuery));