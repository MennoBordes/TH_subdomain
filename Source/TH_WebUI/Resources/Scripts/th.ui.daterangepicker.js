/*!
 * jQuery UI date range picker widget
 * Copyright (c) 2015 Tamble, Inc.
 * Licensed under MIT (https://github.com/tamble/jquery-ui-daterangepicker/raw/master/LICENSE.txt)
 *
 * Depends:
 *   - jQuery 1.8.3+
 *   - jQuery UI 1.9.0+ (widget factory, position utility, button, menu, datepicker)
 *   - moment.js 2.3.0+
 */

// original: http://tamble.github.io/jquery-ui-daterangepicker/

(function ($, window, undefined) {

	var uniqueId = 0; // used for unique ID generation within multiple plugin instances

	$.widget('mx.daterangepicker', {
		version: '0.4.3',

		options: {

			// configuration
			presets: null,                      // preset: enables the preset panel and options
			time: null,                         // time: enables the time panel
			calendar: null,                     // calendar: advanced calendar configuration

			// settings
			range: 'default',                   // the range behaviour
			rangeSplitter: ' - ',               // string to use between dates
			dateFormat: 'M d, yy',              // displayed date format. Available formats: http://api.jqueryui.com/datepicker/#utility-formatDate
			altFormat: 'yy-mm-dd',              // submitted date format - inside JSON {"start":"...","end":"..."}
			mirrorOnCollision: true,            // reverse layout when there is not enough space on the right
			applyOnMenuSelect: true,            // auto apply menu selections
			autoFitCalendars: true,             // override numberOfMonths option in order to fit widget width

			// dimension
			verticalOffset: 0,
			positionFrom: null,                 // the location from which to position (dom node)

			// visual
			icon: 'ui-icon-triangle-1-s',
			skin: null,

			// callbacks
			onOpen: null,                       // callback that executes when the dropdown opens
			onClose: null,                      // callback that executes when the dropdown closes
			onChange: null,                     // callback that executes when the date range changes

			// component
			datepickerOptions: {                // object containing datepicker options. See http://api.jqueryui.com/datepicker/#options
				numberOfMonths: 1,              // number of months to display in the datepicker section
				firstDay: 1,                    // default is monday
				//				showCurrentAtPos: 1             // bug; use maxDate instead
				maxDate: '',                    // the maximum selectable date is today (also current month is displayed on the last position), example: '0' = today
				classes: { 'ui-datepicker': 'mxui-datepicker' }
			},

			// labels
			initialText: 'Select date range...', // placeholder text - shown when nothing is selected
			applyButtonText: 'apply',
			clearButtonText: 'clear',
			cancelButtonText: 'cancel',
			language: null,
			labels: null,                  // object: key/value collection of labels

			// Options to hide buttons
			enableCancel: true,
			enableEmpty: true,
			enableSelect: true
		},


		// private: create
		_create: function () {
			//console.log('_create');

			// modify: presets
			if (this.options.presets === true) {

				// object
				this.options.presets = {};

				// ranges
				// - presetRanges: array of objects; each object describes an item in the presets menu and must have the properties: text, dateStart, dateEnd.
				// - dateStart, dateEnd are functions returning a moment object
				this.options.presets['ranges'] = [
					{ text: 'today', dateStart: function () { return moment() }, dateEnd: function () { return moment() } },
					{ text: 'yesterday', dateStart: function () { return moment().subtract(1, 'days') }, dateEnd: function () { return moment().subtract(1, 'days') } },
					{ text: 'last-7-days', dateStart: function () { return moment().subtract(6, 'days') }, dateEnd: function () { return moment() } },
					{ text: 'last-week', dateStart: function () { return moment().subtract(7, 'days').isoWeekday(1) }, dateEnd: function () { return moment().subtract(7, 'days').isoWeekday(7) } },
					{ text: 'month-to-date', dateStart: function () { return moment().startOf('month') }, dateEnd: function () { return moment() } },
					{ text: 'prev-month', dateStart: function () { return moment().subtract(1, 'month').startOf('month') }, dateEnd: function () { return moment().subtract(1, 'month').endOf('month') } },
					{ text: 'year-to-date', dateStart: function () { return moment().startOf('year') }, dateEnd: function () { return moment() } }
				];

			} else {

				// clear
				this.options['presets'] = null;
			}

			// modify: time
			if (this.options.time != null) {

				// object
				if (this.options.time === true) {
					this.options.time = {};
				}

				// options
				this.options.time = $.extend({
					interval: null,             // interval hours/minutes (future)
					options: null,              // array with options (future)
				}, this.options.time);

				// ...
			}

			// modify: calendar
			if (this.options.calendar != null) {

				// object
				if (this.options.calendar === true) {
					this.options.calendar = {};
				}

				// options
				this.options.calendar = $.extend({
					days: null,                     // specific day configurations
				}, this.options.calendar);

				// ...
			}

			// modify: range
			if (this.options.range != null && this.options.range == 'default') {
				this.options.range = {
					// min, max, ...
				};
			}

			// modify: labels
			// ....

			this._eventController = createEventController();

			// build
			this._dateRangePicker = buildComponent(this.element, this.options, this._eventController);
		},

		// private: destroy
		_destroy: function () {
			//console.log('_destroy');
			this._dateRangePicker.destroy();
		},

		// private: set options
		_setOptions: function (options) {
			//console.log('_setOptions');
			this._super(options);
			this._dateRangePicker.enforceOptions();
		},


		// command: component
		component: function () {
			return this._dateRangePicker;
		},

		// command: widget (returns the container)
		widget: function () {
			return this._dateRangePicker.getContainer();
		},


		// command: open
		open: function () {
			this._eventController.execute('open:before', this._dateRangePicker, this.options);
			this._dateRangePicker.open();
			this._eventController.execute('open:after', this._dateRangePicker, this.options);
		},

		// command: close
		close: function () {
			this._dateRangePicker.close();
		},

		// command: set range
		setRange: function (range) {
			this._dateRangePicker.setRange(range);
		},

		// command: get range
		getRange: function () {
			return this._dateRangePicker.getRange();
		},

		// command: clear range
		clearRange: function () {
			this._dateRangePicker.clearRange();
		},

		// command: text (displayed label)
		text: function () {
			return this._dateRangePicker.getDisplayLabel();
		},


		// command: event
		event: function (type, func) {
			this._eventController.register(type, func);
		}

	});

    /**
     * factory for the widget
     *
     * @param {jQuery} originalElement jQuery object containing the input form element used to instantiate this widget instance
     * @param {Object} options
     */
	function buildComponent(originalElement, options, eventController) {

		// vars
		var classname = 'mx-daterangepicker';
		var skin = options.skin;
		var currentDisplayLabel = null;
		var labels = null;

		// vars: dom elements
		var container; // the dropdown
		var $mask; // ui helper (z-index fix)
		var triggerButton;
		var consolePanel;
		var presetsMenu;
		var timePanel;
		var calendar;
		var buttonPanel;

		// vars: switches
		var isOpen = false;
		var autoFitNeeded = false;
		var showOriginalElement = false;
		var showTriggerBtn = false; // dropdown
		var reflectOriginalElement = true;
		var positionFromElement = options.positionFrom;

		// vars: positioning ??
		var LEFT = 0;
		var RIGHT = 1;
		var TOP = 2;
		var BOTTOM = 3;
		var sides = ['left', 'right', 'top', 'bottom'];
		var hSide = RIGHT; // initialized to pick layout styles from CSS
		var vSide = null;
		var layout = ['date', 'options'];

		// component
		var _component = {

			// references
			originalElement: originalElement,

			// members
			toggle: toggle,
			destroy: destroy,
			open: open,
			close: close,
			setRange: setRange,
			getRange: getRange,
			clearRange: clearRange,
			reset: reset,
			enforceOptions: enforceOptions,
			getContainer: getContainer,
			getDisplayLabel: getDisplayLabel,
			getOptions: function () { return options; },

			// components
			_console: null,
			_presets: null,
			_time: null,
			_calendar: null,
			_buttons: null
		};

		// initialize
		init(_component);


		// helper: initialize
		function init(component) {

			// init: labels
			labels = initLabels(options);

			// switch: original element
			if (originalElement[0].tagName == 'INPUT') {
				if (originalElement[0].type == 'text') {
					showOriginalElement = true;
				}
			}

			// button: trigger
			triggerButton = buildTriggerButton(originalElement, classname, options);

			// panel: buttons
			if (options.applyButtonText) { if (labels.hasOwnProperty(options.applyButtonText)) { options.applyButtonText = labels[options.applyButtonText]; } }
			if (options.clearButtonText) { if (labels.hasOwnProperty(options.clearButtonText)) { options.clearButtonText = labels[options.clearButtonText]; } }
			if (options.cancelButtonText) { if (labels.hasOwnProperty(options.cancelButtonText)) { options.cancelButtonText = labels[options.cancelButtonText]; } }
			buttonPanel = buildButtonPanel(classname, options, {
				onApply: function () {
					close();
					setRange();
				},
				onClear: function () {
					close();
					clearRange();
				},
				onCancel: function () {
					close();
					reset();
				}
			});

			component['_buttons'] = buttonPanel;

			// panel: presets
			if (options.presets != null) {
				if (options.presets.ranges != null) {
					$.each(options.presets.ranges, function (i, v) { if (labels.hasOwnProperty(v.text)) { v.text = labels[v.text]; } });
				}
				presetsMenu = buildPresetsMenu(classname, options, usePreset);

				component['_presets'] = presetsMenu;
			}

			// panel: time
			if (options.time != null) {
				timePanel = buildTimePanel(classname, options, function () {
					close();
					setRange();

				});

				component['_time'] = timePanel;
			}

			// panel: console
			consolePanel = buildConsolePanel(classname, options, { presets: presetsMenu, buttons: buttonPanel, time: timePanel });
			component['_console'] = consolePanel;

			// panel: calendar
			calendar = buildCalendar(component, classname, options, labels['_locale'], timePanel, eventController);
			component['_calendar'] = calendar;

			eventController.execute('calendar.init', _component, options);

			autoFit.numberOfMonths = options.datepickerOptions.numberOfMonths; // save initial option!
			if (autoFit.numberOfMonths instanceof Array) { // not implemented
				options.autoFitCalendars = false;
			}

			render();
			autoFit();
			reset();
			bindEvents();
		}

		// helper: render
		function render() {

			// create container/dropdown
			container = $('<div></div>', { 'class': classname + ' ' + classname + '-' + sides[hSide] + (skin != null ? ' ' + skin : '') + ' ui-front' }) // ui-widget ui-corner-all ui-widget-content
				.append($('<div></div>', { 'class': classname + '-main ' }) //ui-widget-content
					.append(calendar.getElement())
					.append(consolePanel.getElement())
				)
				.append($('<div class="ui-helper-clearfix"></div>')
					//.append(buttonPanel.getElement())
				);
			container.hide();

			// show/hide original element
			if (!showOriginalElement) {
				originalElement.hide();
			}
			// place trigger button
			originalElement.after(triggerButton.getElement());

			// show/hide trigger button
			if (!showTriggerBtn) {
				triggerButton.getElement().hide();
			}

			$mask = $('<div></div>', { 'class': 'ui-front ' + classname + '-mask' }).hide();

			$('body').append($mask).append(container);
		}

		// helper: auto adjusts the number of months in the date picker
		function autoFit() {
			if (options.autoFitCalendars) {

				// vars
				var maxWidth = $(window).width();
				var initialWidth = container.outerWidth(true);
				var $calendar = calendar.getElement();
				var numberOfMonths = $calendar.datepicker('option', 'numberOfMonths');
				var initialNumberOfMonths = numberOfMonths;

				if (initialWidth > maxWidth) {
					while (numberOfMonths > 1 && container.outerWidth(true) > maxWidth) {
						$calendar.datepicker('option', 'numberOfMonths', --numberOfMonths);
					}
					if (numberOfMonths !== initialNumberOfMonths) {
						autoFit.monthWidth = (initialWidth - container.outerWidth(true)) / (initialNumberOfMonths - numberOfMonths);
					}
				} else {
					while (numberOfMonths < autoFit.numberOfMonths && (maxWidth - container.outerWidth(true)) >= autoFit.monthWidth) {
						$calendar.datepicker('option', 'numberOfMonths', ++numberOfMonths);
					}
				}
				reposition();
				autoFitNeeded = false;
			}
		}

		// helper: destroy the 'acting' element
		function destroy() {
			container.remove();
			triggerButton.getElement().remove();
			originalElement.show();
		}

		// helper: bind events
		function bindEvents() {
			triggerButton.getElement().click(toggle);
			triggerButton.getElement().keydown(keyPressTriggerOpenOrClose);
			$mask.click(close);
			$(window).resize(function () { isOpen ? autoFit() : autoFitNeeded = true; });
		}

		// helper: format range for display
		function formatRangeForDisplay(range) {
			//console.log('formatRangeForDisplay', range);

			var dateFormat = options.dateFormat;
			var dtLabels = datetimeLabels(options.labels['_locale']);

			var formatted = $.datepicker.formatDate(dateFormat, range.start.o, dtLabels);
			if (+range.end.o !== +range.start.o) { formatted += options.rangeSplitter + $.datepicker.formatDate(dateFormat, range.end.o, dtLabels); }
			return formatted;
		}

		// helper: formats a date range as JSON
		function formatRange(range) {
			//console.log('formatRange', range);
			var dateFormat = options.altFormat;
			var formattedRange = {};

			formattedRange.start = $.datepicker.formatDate(dateFormat, (range.start ? range.start.o : null));
			formattedRange.end = $.datepicker.formatDate(dateFormat, (range.end ? range.end.o : null));

			return JSON.stringify(formattedRange);
		}

		// helper: parses a date range in JSON format
		function parseRange(text) {
			//console.log('parseRange', text);
			var dateFormat = options.altFormat,
				range = null;
			if (text) {
				try {
					range = JSON.parse(text, function (key, value) {
						return key ? $.datepicker.parseDate(dateFormat, value) : value;
					});
				} catch (e) {
				}
			}
			return range;
		}

		// helper: reset range
		function reset() {
			var range = getRange();
			if (range) {
				currentDisplayLabel = formatRangeForDisplay(range);
				triggerButton.setLabel(currentDisplayLabel);
				calendar.setRange(range);
			} else {
				calendar.reset();
			}
		}

		// helper: 

		// helper: set range
		function setRange(value) {
			//console.log('component:setRange', value);


			// values
			var range = value || calendar.getRange();
			var time = timePanel != null ? timePanel.getValue() : null;

			// range is specified
			if (value != null) {
				// analyze and restore

				// .start
				if (value.start != null) {
					if (value.start.key != null) {
						value.start.o = $.datepicker.parseDate('yymmdd', value.start.key);
						value.start.day = value.start.o.getDate();
						value.start.month = value.start.o.getMonth() + 1;
						value.start.year = value.start.o.getFullYear();
					}
				}
				// .end
				if (value.end != null) {
					if (value.end.key != null) {
						value.end.o = $.datepicker.parseDate('yymmdd', value.end.key);
						value.end.day = value.end.o.getDate();
						value.end.month = value.end.o.getMonth() + 1;
						value.end.year = value.end.o.getFullYear();
					}
				}
				// .time
				if (value.time != null) {
					if (timePanel != null) {
						value.time.label = timePanel.formatTimeLabel(value.time);
					}
				}
				// label
				value.label = formatRangeForDisplay(value);


				// set
				range = value;
				time = value.time;
			}

			// if no start range, ignore
			if (!range.start) {
				return;
			}

			// if no end range, use start range
			if (!range.end) {
				range.end = range.start;
			}

			value && calendar.setRange(range);

			currentDisplayLabel = formatRangeForDisplay(range);

			triggerButton.setLabel(currentDisplayLabel);

			setOriginalElementValue(formatRange(range));

			originalElement.change();


			// selection
			var selection = { start: range.start, end: range.end, time: time, label: currentDisplayLabel };

			// callback: onChange
			if (options.onChange) {
				options.onChange(selection);
			}
		}

		// helper: get range
		function getRange() {
			return parseRange(getOriginalElementValue());
		}

		// helper: clear range
		function clearRange() {
			triggerButton.reset();
			calendar.reset();
			currentDisplayLabel = null;
			setOriginalElementValue('');
			originalElement.change();
			if (options.onChange) {
				options.onChange(null);
			}
		}

		// helper: callback - used when the user clicks a preset range
		function usePreset() {

			var $this = $(this);
			var start = $this.data('dateStart')().startOf('day').toDate();
			var end = $this.data('dateEnd')().startOf('day').toDate();

			calendar.setRange({ start: dpvToObj(start), end: dpvToObj(end) });

			if (options.applyOnMenuSelect) {
				close();
				setRange();
			}
			return false;
		}

		// helper: adjusts dropdown's position taking into account the available space
		function reposition() {

			var _of = positionFromElement != null && !showTriggerBtn ? positionFromElement : triggerButton.getElement();

			container.position({
				my: 'left top',
				at: 'left bottom' + (options.verticalOffset < 0 ? options.verticalOffset : '+' + options.verticalOffset),
				of: _of,
				collision: 'flipfit flipfit',
				using: function (coords, feedback) {

					// vars
					var containerCenterX = feedback.element.left + feedback.element.width / 2;
					var triggerButtonCenterX = feedback.target.left + feedback.target.width / 2;
					var prevHSide = hSide;
					var last;
					var containerCenterY = feedback.element.top + feedback.element.height / 2;
					var triggerButtonCenterY = feedback.target.top + feedback.target.height / 2;
					var prevVSide = vSide;
					var vFit; // is the container fit vertically

					hSide = (containerCenterX > triggerButtonCenterX) ? RIGHT : LEFT;
					if (hSide !== prevHSide) {
						if (options.mirrorOnCollision) {
							last = (hSide === LEFT) ? consolePanel : calendar;
							container.children().first().append(last.getElement());
						}
						container.removeClass(classname + '-' + sides[prevHSide]);
						container.addClass(classname + '-' + sides[hSide]);
					}

					// reposition dom element
					container.css({
						left: coords.left,
						top: coords.top
					});

					vSide = (containerCenterY > triggerButtonCenterY) ? BOTTOM : TOP;
					if (vSide !== prevVSide) {
						if (prevVSide !== null) {
							triggerButton.getElement().removeClass(classname + '-' + sides[prevVSide]);
						}
						triggerButton.getElement().addClass(classname + '-' + sides[vSide]);
					}
					vFit = vSide === BOTTOM && feedback.element.top - feedback.target.top !== feedback.target.height + options.verticalOffset
						|| vSide === TOP && feedback.target.top - feedback.element.top !== feedback.element.height + options.verticalOffset;

					triggerButton.getElement().toggleClass(classname + '-vfit', vFit);
				}
			});
		}

		// helper: kill event (stop propagation)
		function killEvent(event) {
			event.preventDefault();
			event.stopPropagation();
		}

		// helper: key press
		function keyPressTriggerOpenOrClose(event) {
			switch (event.which) {
				case $.ui.keyCode.UP:
				case $.ui.keyCode.DOWN:
					killEvent(event);
					open();
					return;
				case $.ui.keyCode.ESCAPE:
					killEvent(event);
					close();
					return;
				case $.ui.keyCode.TAB:
					close();
					return;
			}
		}

		// helper: open picker
		function open() {
			if (!isOpen) {
				triggerButton.getElement().addClass(classname + '-active');
				$mask.show();
				isOpen = true;
				autoFitNeeded && autoFit();
				calendar.scrollToRangeStart();
				container.show();
				reposition();
			}
			if (options.onOpen) {
				options.onOpen();
			}
		}

		// helper: close picker
		function close() {
			if (isOpen) {
				container.hide();
				$mask.hide();
				triggerButton.getElement().removeClass(classname + '-active');
				isOpen = false;
			}
			if (options.onClose) {
				options.onClose();
			}
		}

		// helper: toggle picker
		function toggle() {
			isOpen ? close() : open();
		}

		// helper: get container
		function getContainer() {
			return container;
		}

		// helper: enforce options
		function enforceOptions() {
			if (options.presets != null) {
				var oldPresetsMenu = presetsMenu;
				presetsMenu = buildPresetsMenu(classname, options, usePreset);
				oldPresetsMenu.getElement().replaceWith(presetsMenu.getElement());
			}
			else {
				if (oldPresetsMenu != null) {
					oldPresetsMenu.getElement().remove();
				}
			}

			calendar.enforceOptions();
			buttonPanel.enforceOptions();
			triggerButton.enforceOptions();
			var range = getRange();
			if (range) {
				triggerButton.setLabel(formatRangeForDisplay(range));
			}
		}

		// helper: get value of original element
		function getOriginalElementValue() {

			if (originalElement[0].tagName == 'INPUT' || originalElement[0].tagName == 'TEXTAREA') {
				return originalElement.val();
			} else {
				return originalElement.text();
			}
		}

		// helper: set value of original element
		function setOriginalElementValue(value) {

			if (originalElement[0].tagName == 'INPUT' || originalElement[0].tagName == 'TEXTAREA') {
				originalElement.val(value);
			} else {
				originalElement.text(value);
			}
		}

		// helper: get current displayed label
		function getDisplayLabel() {
			return currentDisplayLabel;
		}

		// helper: initialize labels
		function initLabels(options) {

			var t = {
				// settings
				'_language': '1',
				'_locale': 'en',

				// buttons
				'apply': 'Apply',
				'clear': 'Clear',
				'cancel': 'Cancel',

				// presets
				'today': 'Today',
				'yesterday': 'Yesterday',
				'last-7-days': 'Last 7 Days',
				'last-week': 'Last Week (Mo-Su)',
				'month-to-date': 'Month to Date',
				'prev-month': 'Previous Month',
				'year-to-date': 'Year to Date',

				// time
				'choose-time': 'Choose time'
			}

			if (options.language != null) {
				if (!isNaN(options.language)) {

					// 1. EN (default)
					// 2. NL
					if (options.language == '2') {

						t['_language'] = '2';
						t['_locale'] = 'nl';

						t['apply'] = 'Toepassen';
						t['clear'] = 'Leegmaken';
						t['cancel'] = 'Annuleren';

						t['today'] = 'Vandaag';
						t['yesterday'] = 'Gisteren';
						t['last-7-days'] = 'Laatste 7 dagen';
						t['last-week'] = 'Vorige week';
						t['month-to-date'] = 'Maand tot nu';
						t['prev-month'] = 'Vorige maand';
						t['year-to-date'] = 'Jaar tot nu';

						t['choose-time'] = 'Kies je tijd';
					}
				}
			}

			options['labels'] = t;

			return t
		}

		return _component;
	}

	/**
	 * factory for the trigger button (which visually replaces the original input form element)
	 *
	 * @param {jQuery} $originalElement jQuery object containing the input form element used to instantiate this widget instance
	 * @param {String} classnameContext classname of the parent container
	 * @param {Object} options
	 */
	function buildTriggerButton(originalElement, classnameContext, options) {

		// vars
		var element;
		var id;

		// initialize
		init();

		// component
		var _component = {
			getElement: function () { return element; },
			getLabel: getLabel,
			setLabel: setLabel,
			reset: reset,
			enforceOptions: enforceOptions
		};


		// helper: init
		function init() {
			fixReferences();
			element = $('<button type="button"></button>')
				.addClass(classnameContext + '-triggerbutton')
				.attr({ 'title': originalElement.attr('title'), 'tabindex': originalElement.attr('tabindex'), id: id })
				.button({
					icons: {
						secondary: options.icon
					},
					label: options.initialText
				});
		}

		// helper: fix references
		function fixReferences() {
			id = 'drp_autogen' + uniqueId++;
			$('label[for="' + originalElement.attr('id') + '"]')
				.attr('for', id);
		}

		// helper: get label
		function getLabel() {
			return element.button('option', 'label');
		}

		// helper: set label
		function setLabel(value) {
			element.button('option', 'label', value);
		}

		// helper: reset
		function reset() {
			originalElement.val('').change();
			setLabel(options.initialText);
		}

		// helper: enforce options
		function enforceOptions() {
			element.button('option', {
				icons: {
					secondary: options.icon
				},
				label: options.initialText
			});
		}

		return _component;
	}

    /**
     * factory for the console panel
     *
     * @param {String} classnameContext classname of the parent container
     * @param {Object} options
     */
	function buildConsolePanel(classnameContext, options, components) {

		// vars
		var self;
		var selfInner;
		var _menu;

		// initialize
		init();

		// component
		var _component = {
			getElement: function () { return self; }
		};


		// helper: init
		function init() {
			self = $('<div class="' + classnameContext + '-console' + '"><div class="inner"><div></div>');
			selfInner = self.children('.inner');

			if (components['presets'] != null) {
				selfInner.append(components['presets'].getElement());
			}

			if (components['time'] != null) {
				selfInner.append(components['time'].getElement());
			}

			if (components['buttons'] != null) {
				selfInner.append(components['buttons'].getElement());
			}
		}

		return _component;
	}

	/**
	 * factory for the presets menu (containing built-in date ranges)
	 *
	 * @param {String} classnameContext classname of the parent container
	 * @param {Object} options
	 * @param {Function} onClick callback that executes when a preset is clicked
	 */
	function buildPresetsMenu(classnameContext, options, onClick) {

		// vars
		var self;
		var _menu;

		// initialize
		init();

		// component
		var _component = {
			getElement: function () { return self; }
		};


		// helper: init
		function init() {
			self = $('<div></div>')
				.addClass(classnameContext + '-presets');

			_menu = $('<ul></ul>');

			if (options.presets != null && options.presets.ranges != null) {
				$.each(options.presets.ranges, function () {
					$('<li class="option"><a href="javascript:void(0);">' + this.text + '</a></li>')
						.data('dateStart', this.dateStart)
						.data('dateEnd', this.dateEnd)
						.click(onClick)
						.appendTo(_menu);
				});
			}

			self.append(_menu);

			// jquery-ui menu (disabled)
			//_menu.menu().data('ui-menu').delay = 0; // disable submenu delays
		}

		return _component;
	}

    /**
	 * factory for the time menu
	 *
	 * @param {String} classnameContext classname of the parent container
	 * @param {Object} options
	 */
	function buildTimePanel(classnameContext, options, onClick) {

		// vars
		var self;
		var nLabel;
		var nOptions;

		// initialize
		init();

		// component
		var _component = {
			getElement: function () { return self; },
			setLabel: setLabel,
			setOptions: setOptions,
			getValue: getValue,
			formatTimeLabel: function (timeObj) {

				var _label = displayTime(timeObj.start);
				if (timeObj.end != null) {
					_label += ' - ' + displayTime(timeObj.end);
				}
				return _label;
			}
		};


		// helper: init
		function init() {
			self = $('<div></div>').addClass(classnameContext + '-time');

			nLabel = $('<div class="label">' + options.labels['choose-time'] + '</div>');
			self.append(nLabel);

			nOptions = $('<div class="time-options"></div>');
			self.append(nOptions);

			// jquery-ui menu (disabled)
			//_menu.menu().data('ui-menu').delay = 0; // disable submenu delays


		}

		// func: set label
		function setLabel(text) {
			nLabel.text(text);
		}

		// func: set (time-) options
		function setOptions(timeOptions) {

			if (timeOptions != null && timeOptions.length != 0) {

				nOptions.empty();

				var _ul = $('<ul></ul>');

				$.each(timeOptions, function (key, value) {
					var opt = this;

					var _label = opt.label;

					if (_label == null || _label.length == 0) {
						_label = displayTime(opt.start);
						if (opt.end != null) {
							_label += ' - ' + displayTime(opt.end);
						}
					}

					var nOption = $('<li class="option" data-hook="time-option" data-time-start="' + opt.start + '" data-time-end="' + opt.end + '"><div class="label">' + _label + '    <i class="fa fa-caret-right" aria-hidden="true"></div></li>')
						//.click(onClick)
						.appendTo(_ul);

					// bind: click
					nOption.click(function () {
						var no = $(this);
						var wasSelected = no.hasClass('selected');

						if (wasSelected) {
							no.parent().children().removeClass('selected');
						} else {
							no.siblings().removeClass('selected');
							no.addClass('selected');
						}
						if (options.enableSelect === false && onClick != null) {
							onClick();
						}
					});

				});
				nOptions.append(_ul);
			} else {
				nOptions.empty();
			}
		}

		// func: get value (or selection)
		function getValue() {

			var nSelected = nOptions.find('.selected');
			if (nSelected.length > 0) {

				var _start = nSelected.attr('data-time-start');
				var _end = nSelected.attr('data-time-end');
				var _label = nSelected.find('.label').text();

				var time = {
					start: _start != null && _start.length > 0 ? _start : null,
					end: _end != null && _end.length > 0 ? _end : null,
					label: _label != null && _label.length > 0 ? _label : null
				};

				return time;
			}

			return null;
		}

		// helper: formats time for display
		function displayTime(v) {

			var s = v;
			if (v.length == 4) {
				s = v.substring(0, 2) + ':' + v.substring(2, 4);
			}
			return s;
		}

		return _component;
	}

	/**
	 * factory for the multiple month date picker
	 *
	 * @param {String} classnameContext classname of the parent container
	 * @param {Object} options
	 */
	function buildCalendar(mainComponent, classnameContext, options, locale, timePanel, eventController) {

		// vars
		var $self;
		var range = { start: null, end: null }; // selected range
		var state = {
			view: {     // current view
				year: null,
				month: null,
				date: []
			}
		};

		// locale: datepicker defaults
		var dayNamesMin = ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'];
		var dayNamesShort = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
		var dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
		var monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
		var monthNamesShort = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

		// extract locale labels from momentjs
		if (locale != null) {
			moment.locale(locale); // example: 'en', 'nl', 'fr'
			dayNamesMin = moment.weekdaysMin();
			dayNamesShort = moment.weekdaysShort();
			dayNames = moment.weekdays();
			monthNames = moment.months();
			monthNamesShort = moment.monthsShort();
		}

		// component
		var _component = {
			getElement: function () { return $self; },
			scrollToRangeStart: function () { return scrollToRangeStart(); },
			getRange: function () { return range; },
			setRange: function (value) { range = value; refresh(); },
			refresh: refresh,
			reset: reset,
			enforceOptions: enforceOptions,
			getState: function () { return state; },
			redraw: redraw
		};

		// initialize
		init();


		// func: init
		function init() {
			$self = $('<div></div>', { 'class': classnameContext + '-calendar ui-widget-content' });

			var dpOptions = $.extend({
				dayNamesMin: dayNamesMin,
				dayNamesShort: dayNamesShort,
				dayNames: dayNames,
				monthNames: monthNames,
				monthNamesShort: monthNamesShort
			}, options.datepickerOptions, { beforeShowDay: beforeShowDay, onSelect: onSelectDay, onChangeMonthYear: onChangeMonthYear });

			$self.datepicker(dpOptions);
			updateAtMidnight();

			clearState();
		}

		// func: enforce options
		function enforceOptions() {
			$self.datepicker('option', $.extend({}, options.datepickerOptions, { beforeShowDay: beforeShowDay, onSelect: onSelectDay }));
		}

		// func-event: called when a day is selected.
		function onSelectDay(dateText, instance) {
			// parse
			var dateFormat = options.datepickerOptions.dateFormat || $.datepicker._defaults.dateFormat;
			var selectedDate = $.datepicker.parseDate(dateFormat, dateText);

			// simple obj
			var dateObj = dpvToObj(selectedDate);

			// if: range configuration is enabled
			if (options.range != null) {

				if (!range.start || range.end) { // start not set, or both already set
					range.start = dateObj;
					range.end = null;
				} else if (dateObj.o < range.start.o) { // start set, but selected date is earlier
					range.end = range.start;
					range.start = dateObj;
				} else {
					range.end = dateObj;
				}
			} else {
				// range configuration is not enabled
				range.start = dateObj;
				range.end = dateObj;
			}


			if (options.datepickerOptions.hasOwnProperty('onSelect')) {
				options.datepickerOptions.onSelect(dateText, instance);
			}

			// extension: time
			if (options.time != null) {

				// note:
				// instance.selectedDay = 1 based
				// instance.selectedMonth = 0 based
				// instance.selectedYear = 1 based

				var dayCode = formatDateKey(instance.selectedYear, instance.selectedMonth + 1, instance.selectedDay);

				var days = [
					//{
					//    date: '20170217',
					//    state: 'available',
					//    info: null,
					//    time: [{ start: '090000', end: '100000', label: '09:00 - 10:00' }, { start: '130000', end: '140000', label: '13:00 - 14:00' }]
					//}
				];
				if (options.calendar != null && options.calendar.days != null) {
					days = options.calendar.days;
				}
				reflectTimeOptions(days, dayCode);

				// helper: reflect time options
				function reflectTimeOptions(days, dayCode) {

					// check day configuration
					var defDay = null;
					var matchDay = null;
					for (var i = 0; i < days.length; i++) {
						var day = days[i];
						if (day.date == null) { defDay = day; continue; }
						if (day.date == dayCode) { matchDay = day; continue; }
					}

					if (matchDay == null && defDay != null) {
						matchDay = defDay;
					}

					var compTime = timePanel;
					compTime.setOptions(matchDay != null && matchDay.time != null ? matchDay.time : null);
				}
			}
		}

		// func-event: called for each day in the datepicker before it is displayed.
		function beforeShowDay(date) {

			// note: The plus converts a string to a float.
			// with plus:       if (obj.length === +obj.length)
			// without plus:    if (obj.length === Number(obj.length))

			//console.log('beforeShowDay', date);

			var o = dpvToObj(date);

			// state
			state.view.date.push(o);

			// day configuration
			var dayConfig = null;
			if (options.calendar != null && options.calendar.days != null) {
				for (var i = 0; i < options.calendar.days.length; i++) {
					var d = options.calendar.days[i];
					if (d.date == o.key) {
						dayConfig = d;
						i = options.calendar.days.length;
					}
				}
			}

			//  ui-datepicker-unselectable state-available ui-state-disabled

			var _selectable = true;
			var _isWithinRange = range.start && ((+date === +range.start.o) || (range.end && range.start.o <= date && date <= range.end.o));
			var _classToAdd = _isWithinRange ? 'ui-state-highlight' : '';

			if (dayConfig != null && dayConfig.state != null && dayConfig.state.length > 0) {
				_classToAdd += ' state-' + dayConfig.state;

				if (dayConfig.state != 'available') { // available, disabled, blocked
					_selectable = false;
				}
			}

			var result = [_selectable, _classToAdd]; // 0. selectable	 // 1. class to be added
			var userResult = [true, ''];

			if (options.datepickerOptions.hasOwnProperty('beforeShowDay')) {
				userResult = options.datepickerOptions.beforeShowDay(date);
			}

			var selectable = result[0] && userResult[0];
			var classToAdd = result[1] + ' ' + userResult[1];
			return [selectable, classToAdd];
		}

		// func-event: called when the datepicker moves to a new month and/or year. 
		function onChangeMonthYear(year, month, instance) {

			clearState();
			var daysInMonth = getDaysInMonth(year, month);

			for (var i = 0; i < daysInMonth.length; i++) {
				state.view.date.push(dpvToObj(daysInMonth[i]));
			}

			//console.log('onChangeMonthYear', { year: year, month: month, instance: instance });

			eventController.execute('calendar.navigate', mainComponent, options);
		}

		// func: update at midnight
		function updateAtMidnight() {
			setTimeout(function () {
				refresh();
				updateAtMidnight();
			}, moment().endOf('day') - moment());
		}

		// func: scroll to range start
		function scrollToRangeStart() {
			if (range.start) {
				$self.datepicker('setDate', range.start.o);
			}
		}

		// func: refresh
		function refresh() {
			clearState();
			$self.datepicker('refresh');
			$self.datepicker('setDate', null); // clear the selected date
		}

		// func: reset
		function reset() {
			range = { start: null, end: null };
			refresh();
		}

		// func: redraw
		function redraw() {
			clearState();
			$self.datepicker('refresh');
		}

		// helper: clear state
		function clearState() {

			state.view.date.splice(0, state.view.date.length);
		}

		// helper: get all days in a month
		function getDaysInMonth(year, month) {
			var _month = month - 1;// conver to zero based
			var date = new Date(year, _month, 1);
			var days = [];
			while (date.getMonth() === _month) {
				days.push(new Date(date));
				date.setDate(date.getDate() + 1);
			}
			return days;
		}

		return _component;
	}

	/**
	 * factory for the button panel
	 *
	 * @param {String} classnameContext classname of the parent container
	 * @param {Object} options
	 * @param {Object} handlers contains callbacks for each button
	 */
	function buildButtonPanel(classnameContext, options, handlers) {

		// vars
		var self; // node
		var applyButton;
		var clearButton;
		var cancelButton;

		// initialize
		init();

		// component
		var _component = {
			getElement: function () { return self; },
			enforceOptions: enforceOptions
		};


		// helper: init
		function init() {
			self = $('<div></div>')
				.addClass(classnameContext + '-buttonpanel');

			if (options.enableSelect === true) {
				if (options.applyButtonText) {
					applyButton = $('<button type="button" class="button primary"></button>')
						.text(options.applyButtonText);
					//.button(); // jquery-ui button

					self.append(applyButton);
				}
			}

			if (options.enableEmpty === true) {
				if (options.clearButtonText) {
					clearButton = $('<button type="button" class="button alternate"></button>')
						.text(options.clearButtonText);
					//.button(); // jquery-ui button

					self.append(clearButton);
				}
			}

			if (options.enableCancel === true) {
				if (options.cancelButtonText) {
					cancelButton = $('<button type="button" class="button cancel"></button>')
						.text(options.cancelButtonText);
					//.button(); // jquery-ui button

					self.append(cancelButton);
				}
			}



			bindEvents();
		}

		// helper: enforce options
		function enforceOptions() {
			if (applyButton) {
				applyButton.button('option', 'label', options.applyButtonText);
			}

			if (clearButton) {
				clearButton.button('option', 'label', options.clearButtonText);
			}

			if (cancelButton) {
				cancelButton.button('option', 'label', options.cancelButtonText);
			}
		}

		// helper: bind events
		function bindEvents() {
			if (handlers) {
				if (applyButton) { applyButton.click(handlers.onApply); }
				if (clearButton) { clearButton.click(handlers.onClear); }
				if (cancelButton) { cancelButton.click(handlers.onCancel); }

			}
		}

		return _component;
	}

	// create: event controller
	function createEventController() {

		var events = [];

		var _controller = {
			register: register,
			get: get,
			execute: execute
		};

		function register(type, func) {

			if (type.constructor === Array) {
				for (var i = 0; i < type.length; i++) {
					events.push({ type: type[i], func: func });
				}
			} else {
				events.push({ type: type, func: func });
			}
		}

		function get(type) {
			for (var i = events.length - 1; i >= 0; i--) {
				if (events[i].type == type) {
					return events[i];
				}
			}
			return null;
		}

		function execute(type, component, options) {

			var _registered = get(type);

			if (_registered != null && _registered.func != null) {

				var e = {
					wait: function () { /* not implemented */ },
					'continue': function () { /* not implemented */ },
					type: _registered.type
				}

				_registered.func(e, component, options);
			}
		}

		return _controller;
	}

	// helper: datepicker object to simple object
	function dpvToObj(v) {
		if (v) {
			var obj = {
				o: v,
				key: null,
				day: v.getDate(),
				month: v.getMonth() + 1,
				year: v.getFullYear()
			}
			obj['key'] = formatDateKey(obj.year, obj.month, obj.day);
			return obj;
		}
		return null;
	}

	// helper: format date key
	function formatDateKey(year, month, day) {

		var _code = f(year, 4) + '' + f(month, 2) + '' + f(day, 2);

		function f(n, d) {
			if (d === 4) { return n > 999 ? '' + n : '0' + n; }
			else { return n > 9 ? '' + n : '0' + n; }
		}

		return _code;
	}

	// helper: get datetime labels
	function datetimeLabels(locale) {

		// locale: datepicker defaults
		var dayNamesMin = ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'];
		var dayNamesShort = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
		var dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
		var monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
		var monthNamesShort = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

		// extract locale labels from momentjs
		if (locale != null) {
			moment.locale(locale); // example: 'en', 'nl', 'fr'
			dayNamesMin = moment.weekdaysMin();
			dayNamesShort = moment.weekdaysShort();
			dayNames = moment.weekdays();
			monthNames = moment.months();
			monthNamesShort = moment.monthsShort();
		}

		// note: jquery-ui property names
		return {
			dayNamesMin: dayNamesMin,
			dayNamesShort: dayNamesShort,
			dayNames: dayNames,
			monthNames: monthNames,
			monthNamesShort: monthNamesShort
		}
	}


})(jQuery, window);