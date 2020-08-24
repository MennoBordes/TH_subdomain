/**
 * FeedBack.
 */
var Feedback = (function () {

	var _feedback = {};

	/**
	 * Feedback => Success.
	 */
	_feedback.success = function (options) {

		var _options = $.extend({
			message: []
		}, options);

		var _messages = [];

		if ($.isArray(_options.message)) {
			_messages = _options.message;
		} else {
			_messages.push(_options.message);
		}

		Feedback2({ Type: 'success', Messages: _messages, Duration: _options.duration });
	}

	/**
	 * Feedback => Error.
	 */
	_feedback.error = function (options) {

		var _options = $.extend({
			message: []
		}, options);

		var _messages = [];

		if ($.isArray(_options.message)) {
			_messages = _options.message;
		} else {
			_messages.push(_options.message);
		}

		Feedback2({ Type: 'error', Messages: _messages });
	}

	/**
	 * Feedback => Confirm.
	 */
	_feedback.confirm = function (options) {
		var _options = $.extend({
			title: null,
			description: null,
			message: null,
			confirm: null,
			cancel: null,
			cancelButtonText: 'Cancel',
			proceedButtonText: 'Proceed'
		}, options);

		if (_options.message != null && _options.description == null) {
			_options.description = _options.message;
		}

		Feedback2({
			Type: 'confirm',
			Title: _options.description,
			OnConfirm: _options.confirm,
			OnCancel: _options.cancel,
			CancelButtonText: _options.cancelButtonText,
			ProceedButtonText: _options.proceedButtonText
		});
	}

	return _feedback;
}());

function Feedback2(settings) {

	// Set default settings
	var _settings = {
		ContainerClass: 'ui-feedback_container',		// => The container id
		BackgroundClass: 'ui-feedback_backround',		// => The background id
		Type: null,                                     // => The feedback type
		Message: null,                                  // => A single message
		Messages: null,                                 // => A message list (array)
		Content: null,                                  // => Additional content (html)
		Title: null,                                    // => The dialog title
		Description: null,                              // => The dialog description
		ProceedButtonText: 'Proceed',                   // => The proceed button text
		CancelButtonText: 'Cancel',                     // => The cancel button text
		ProceedButtonId: 'btnfeedbackproceed',          // => The proceed button id
		CancelButtonId: 'btnfeedbackcancel',            // => The cancel button id
		OnConfirm: null,                                // => The action to be executed (when confirmed)
		OnCancel: null,                                 // => The action to be executed (when cancelled)
		ForceConfirm: true,                             // => Indicates if a confirm/cancel must be clicked
		Environment: 'dashboard',                       // => The environment class to be added to the container
		Duration: null                                  // => Indicaties the duration: default (error: 8 seconds, success: 1 seconds)
	}

	// Merge settings
	_settings = $.extend(_settings, settings);

	// Set message list
	if (_settings.Messages == null) {
		_settings.Messages = [];
	}
	if (_settings.Message != null) {
		_settings.Messages.push(_settings.Message);
	}

	// Remove previous feedback
	$('.ui-feedback').remove();

	// Variables
	var xtype = '';
	var icon = '<div class="icon"></div>';
	var content = '';
	var background = '';

	// Build
	if (_settings.Type == 'success') {
		xtype = 'success';

		content += '<div class="messagelist">';

		content += '<ul>';
		for (var i = 0; i < _settings.Messages.length; i++) {
			content += '<li>' + _settings.Messages[i] + '</li>';
		}
		content += '</ul>';

		content += '</div>';
	}
	else if (_settings.Type == 'error') {
		xtype = 'error';

		content += '<div class="messagelist">';

		content += '<ul>';
		for (var i = 0; i < _settings.Messages.length; i++) {
			content += '<li>' + _settings.Messages[i] + '</li>';
		}
		content += '</ul>';

		content += '</div>';

		content += '<div class="close"><a href="javascript:void(0);"></a></div>';
	}
	else if (_settings.Type == 'confirm') {
		xtype = 'confirm';

		content += '<div class="contentholder">';

		// Build content
		content += '<div class="text">';

		if (_settings.Title != null) {
			content += '<h3>' + _settings.Title + '</h3>';
		}
		if (_settings.Description != null) {
			content += '<p>' + _settings.Description + '</p>';
		}

		content += '</div>';

		content += '<div class="actions">';

		content += '<div class="formRow buttonRow">';


		content += '<input id="' + _settings.ProceedButtonId + '" type="button" class="button" value="' + _settings.ProceedButtonText + '"/>';

		content += '<input id="' + _settings.CancelButtonId + '" type="button" class="button cancel" value="' + _settings.CancelButtonText + '"/>';

		//content += '</div>'; // split2
		content += '</div>'; // formrow buttonrow

		content += '</div>'; // actions

		content += '</div>'; // contentholder
	}
	else {
		icon = '';

		content += '<ul>';
		for (var i = 0; i < _settings.Messages.length; i++) {
			content += '<li>' + _settings.Messages[i] + '</li>';
		}
		content += '</ul>';
	}

	// Assemble
	var output = '<div class="ui-feedback"><div class="messages ' + xtype + '">' + icon + content + '</div></div>' + background;

	// Add output
	$('.' + _settings.ContainerClass).prepend(output);
	$('.' + _settings.ContainerClass).addClass(_settings.Environment);

	// Add close logic
	if (xtype == 'error') {
		$(".ui-feedback .close").click(function () {
			// Hide feedback
			$(".ui-feedback").hide();
		});
	}

	// Add cancel logic
	if (xtype == 'confirm') { // cancel??
		$("#" + _settings.CancelButtonId).click(function () {
			// Hide feedback
			$(".ui-feedback").hide();
			$(".ui-feedback_background").hide();

			// Remove bind from buttons
			$("#" + _settings.CancelButtonId).off();
			$("#" + _settings.ProceedButtonId).off();

			// Remove bind from background
			$("." + _settings.BackgroundClass).off();

			// Execute custom action
			if (_settings.OnCancel != null) {
				_settings.OnCancel();
			}
		});

		// Only allow background if ForeceConfirm = false
		if (_settings.ForceConfirm == false) {
			$("." + _settings.BackgroundClass).click(function () {
				// Hide feedback
				$(".ui-feedback").hide();
				$(".ui-feedback_background").hide();

				// Remove bind from buttons
				$("#" + _settings.CancelButtonId).off();
				$("#" + _settings.ProceedButtonId).off();

				// Remove bind from background
				$("." + _settings.BackgroundClass).off();

				// Execute custom action
				if (_settings.OnCancel != null) {
					_settings.OnCancel();
				}
			})
		}
	}

	// Add proceed logic
	if (xtype == 'confirm') {
		$("#" + _settings.ProceedButtonId).click(function () {
			// Hide feedback
			$(".ui-feedback").hide();
			$(".ui-feedback_background").hide();

			// Remove bind from buttons
			$("#" + _settings.CancelButtonId).off();
			$("#" + _settings.ProceedButtonId).off();

			// Remove bind from background
			$("." + _settings.BackgroundClass).off();

			// Execute custom action
			if (_settings.OnConfirm != null) {
				_settings.OnConfirm();
			}
		})
	}

	// Show output
	showFeedback(xtype, _settings.Duration);

	function showFeedback(type, duration) {
		var _type = type;

		if (typeof (type) == 'undefined') {
			type = $('.ui-feedback .messages').not(':empty').first();
		} else {
			type = $('.ui-feedback .' + type);
		}

		// types: success / error / confirm / ''
		if (_type == 'confirm') {
			$('.ui-feedback_background').show();
		}

		// Show feedback
		type.show().css('opacity', 0).animate(
			{ 'opacity': 0.95 },
			{
				'duration': 500,
				'complete': function () {
					var obj = $(this), timeout = 1000;

					if (duration != null && (obj.is('.error') || obj.is('.success'))) {
						timeout = duration

						setTimeout(function () {
							obj.animate(
								{ 'opacity': 0 },
								{
									'duration': 500,
									'complete': function () {
										$(this).hide();
									}
								}
							);
						}, timeout);
					}
					else if (obj.is('.error')) {
						// time before the message will start to fade out
						timeout == 8000;

						// Fade out feedback
						setTimeout(function () {
							obj.animate(
								{ 'opacity': 0 },
								{
									'duration': 500,
									'complete': function () {
										$(this).hide();
									}
								}
							);
						}, timeout);
					}
					else if (obj.is('.confirm')) {
						// Confirm doesn't close automatically
					}
					else {
						// Fade out feedback
						setTimeout(function () {
							obj.animate(
								{ 'opacity': 0 },
								{
									'duration': 500,
									'complete': function () {
										$(this).hide();
									}
								}
							);
						}, timeout);
					}
				}
			});
	}
}