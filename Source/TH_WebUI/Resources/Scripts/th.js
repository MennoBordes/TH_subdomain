/**
 * Dependencies:
 * - jQuery
 * - th.feedback
 */

var TH = (function ($) {

    _feedback = window.Feedback || null

    var th = {};

    //=== GLOBAL ===//
    var global = th.global = {}

    global.messages = {
        'verify-network': 'Could not connect, please verify your network.',
        '400': 'Bad request. [400',
        '401': 'Login session expired. [401]',
        '403': 'Unauthorized request. [403]',
        '404': 'Requested page not found. [404]',
        '500': 'Internal Server Error [500].',
        'json-parse-fail': 'Requested JSON parse failed.',
        'timeout': 'Oops, lost connection... please try again',
        'ajax-request-abort': 'Ajax request aborted.',
        'error': 'An error occured while processing your request.'
    };

    //=== AJAX ===//
    var ajax = th.ajax = {};

    /**
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
                    _options.success(data, status, xhr);
                }
            },
            error: function (xhr, status, error) {
                var message = null;

                // error
                if (xhr.status === 0) {
                    message = global.messages['verify-network'];

                } else if (xhr.status == 400) {
                    message = global.messages['400'];

                } else if (xhr.status == 401) {
                    message = global.messages['401'];

                } else if (xhr.status == 403) {
                    message = global.messages['403'];

                } else if (xhr.status == 404) {
                    message = global.messages['404'];

                } else if (xhr.status == 500) {
                    message = global.messages['500'];

                } else if (status === 'parsererror') {
                    message = global.messages['json-parse-fail'];

                } else if (status === 'timeout') {
                    message = global.messages['timeout'];

                } else if (status === 'abort') {
                    message = global.messages['ajax-request-abort'];

                } else {
                    message = global.messages['error'];
                }

                // message
                if (error != undefined && error != null && error.length > 0) {
                    message = error;
                }

                // feedback
                if (_options.feedback.error) {
                    if (_feedback != null) {
                        _feedback.error({ message: message });
                    }
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

    /**
     * Ajax post (basic execute method).
     */
    ajax.post = function (options) {

        /*
         * default contentTypes
         * - 'application/x-www-form-urlencoded; charset=UTF-8'
         * - 'multipart/form-data; charset=UTF-8'
         * - 'text/plain; charset=UTF-8'
         * - 'application/json; charset=utf-8'
         */

        var _options = $.extend({
            type: 'POST',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        }, options);

        ajax.execute(_options);
    };

    /**
     * Ajax post JSON (basic execute method).
     */
    ajax.postJSON = function (options) {

        var _options = $.extend({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            processData: false,
            data: null
        }, options);

        // process data
        _options.data = JSON.stringify(_options.data);

        ajax.execute(_options);
    }

    /*
     *  Ajax events.
     */
    ajax.events = {
        unauthorized: null
    };

    return th;

}(jQuery));