using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

// ReSharper disable UnusedMember.Global

namespace Algel.WpfTools.Windows.Controls
{
    /// <summary>
    /// Represents a RadNumericUpDown control.
    /// </summary>
    [DefaultProperty("Value")]
    [DefaultEvent("ValueChanged")]
    public class NumericEditor : RangeBase
    {
        #region Fields

        /// <summary>
        /// Identifies the ShowTextBox dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowTextBoxProperty = DependencyProperty.Register("ShowTextBox", typeof(bool), typeof(NumericEditor), new PropertyMetadata(true, OnShowTextBoxChanged));

        /// <summary>
        /// Identifies the ValueFormat dependency property. 
        /// </summary>
        public static readonly DependencyProperty ValueFormatProperty = DependencyProperty.Register("ValueFormat", typeof(ValueFormat), typeof(NumericEditor), new PropertyMetadata(ValueFormat.Numeric, OnValueFormatChanged));

        /// <summary>
        ///  Identifies the NumberFormatInfo dependency property. 
        /// </summary>
        public static readonly DependencyProperty NumberFormatInfoProperty = DependencyProperty.Register("NumberFormatInfo", typeof(NumberFormatInfo), typeof(NumericEditor), new PropertyMetadata(null, (s, e) => ((NumericEditor)s).UpdateText()));

        /// <summary>
        ///    Identifies the UpdateValueEvent dependency property. 
        /// </summary>
        public static readonly DependencyProperty UpdateValueEventProperty = DependencyProperty.Register("UpdateValueEvent", typeof(UpdateValueEvent), typeof(NumericEditor), new PropertyMetadata(UpdateValueEvent.LostFocus));

        /// <summary>
        /// Identifies the NullValue dependency property.
        /// </summary>
        public static readonly DependencyProperty NullValueProperty = DependencyProperty.Register("NullValue", typeof(string), typeof(NumericEditor), new PropertyMetadata(string.Empty, (s, e) => ((NumericEditor)s).UpdateText()));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is highlighted.
        /// </summary>
        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(NumericEditor), new PropertyMetadata(ForceVisualState));

        /// <summary>
        ///  Identifies the IsEditable dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(NumericEditor), new PropertyMetadata(true));

        /// <summary>
        ///  Identifies the IsReadOnly dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(NumericEditor), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the CustomUnit dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomUnitProperty = DependencyProperty.Register("CustomUnit", typeof(string), typeof(NumericEditor), new PropertyMetadata((s, e) => ((NumericEditor)s).UpdateText()));

        /// <summary>
        /// Identifies the IsInteger dependency property.
        /// </summary>
        public static readonly DependencyProperty IsIntegerProperty = DependencyProperty.Register("IsInteger", typeof(bool), typeof(NumericEditor), new PropertyMetadata(false, (s, e) => ((NumericEditor)s).UpdateText()));

        /// <summary>
        /// Identifies the NumberDecimalDigits dependency property.
        /// </summary>
        public static readonly DependencyProperty NumberDecimalDigitsProperty = DependencyProperty.Register("NumberDecimalDigits", typeof(int), typeof(NumericEditor), new PropertyMetadata(-1, (s, e) => ((NumericEditor)s).UpdateText()));

        /// <summary>
        /// Identifies the HideTrailingZeros dependency property.
        /// </summary>
        public static readonly DependencyProperty HideTrailingZerosProperty = DependencyProperty.Register("HideTrailingZeros", typeof(bool), typeof(NumericEditor), new PropertyMetadata(false, (s, e) => ((NumericEditor)s).UpdateText()));

        /// <summary>
        /// Identifies the NumberDecimalSeparator dependency property.
        /// </summary>
        public static readonly DependencyProperty NumberDecimalSeparatorProperty = DependencyProperty.Register("NumberDecimalSeparator", typeof(string), typeof(NumericEditor), new PropertyMetadata(string.Empty, (s, e) => ((NumericEditor)s).UpdateText()));

        /// <summary>
        /// Identifies the UpdateValueToMatchTextOnLostFocusProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty UpdateValueToMatchTextOnLostFocusProperty = DependencyProperty.Register("UpdateValueToMatchTextOnLostFocus", typeof(bool), typeof(NumericEditor), new PropertyMetadata(false));

        public static readonly DependencyPropertyKey IsNegativePropertyKey = DependencyProperty.RegisterReadOnly("IsNegative", typeof(bool), typeof(NumericEditor), new PropertyMetadata(false));

        public static readonly DependencyProperty IsNegativeProperty = IsNegativePropertyKey.DependencyProperty;

        private TextBox _textBox;
        private decimal? _inputValue;
        private string _lastInput = string.Empty;
        private bool _updatingContent;
        private bool _isFromCurrentCulture;

        private bool _isTextBoxContextMenuOpen;


        #endregion

        #region .ctor

        /// <summary>
        /// Initializes static members of the <see cref="NumericEditor"/> class.
        /// </summary>
        static NumericEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericEditor), new FrameworkPropertyMetadata(typeof(NumericEditor)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericEditor"/> class.
        /// </summary>
        public NumericEditor()
        {
            IsEnabledChanged += OnNumericIsEnabledChanged;
        }


        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the number of decimal digits that will be displayed in the control.
        /// </summary>
        public int NumberDecimalDigits
        {
            get { return (int)GetValue(NumberDecimalDigitsProperty); }
            set { SetValue(NumberDecimalDigitsProperty, value); }
        }

        /// <summary>
        /// Gets or sets additional string to appear in the end of numeric values. This is a dependency property.
        /// </summary>
        public string CustomUnit
        {
            get { return (string)GetValue(CustomUnitProperty); }
            set { SetValue(CustomUnitProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the control is editable or not. This is a dependency property.
        /// </summary>
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the control is readonly or not. This is a dependency property.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the format is integer.
        /// </summary>
        public bool IsInteger
        {
            get { return (bool)GetValue(IsIntegerProperty); }
            set { SetValue(IsIntegerProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the textbox of RadNumericUpDown is visible.
        /// </summary>
        public bool ShowTextBox
        {
            get { return (bool)GetValue(ShowTextBoxProperty); }
            set { SetValue(ShowTextBoxProperty, value); }
        }

        /// <summary>
        /// Gets or sets the current value format. This is a dependency property.
        /// </summary>
        public ValueFormat ValueFormat
        {
            get { return (ValueFormat)GetValue(ValueFormatProperty); }
            set { SetValue(ValueFormatProperty, value); }
        }

        /// <summary>
        /// Gets or sets the NumberFormatInfo value, for more info see <see>ValueFormat</see>. This is a dependency property.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public NumberFormatInfo NumberFormatInfo
        {
            get { return (NumberFormatInfo)GetValue(NumberFormatInfoProperty); }
            set { SetValue(NumberFormatInfoProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this control is highlighted.
        /// </summary>
        public bool IsHighlighted
        {
            get
            {
                return (bool)GetValue(IsHighlightedProperty);
            }
            set
            {
                SetValue(IsHighlightedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets string that is displayed when the Value is null.
        /// </summary>
        /// <value>The null value.</value>
        public string NullValue
        {
            get { return (string)GetValue(NullValueProperty); }
            set { SetValue(NullValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the way the Value property is updated. This is a dependency property.
        /// </summary>
        public UpdateValueEvent UpdateValueEvent
        {
            get { return (UpdateValueEvent)GetValue(UpdateValueEventProperty); }
            set { SetValue(UpdateValueEventProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the trailing zeros should be displayed or not. This is a dependency property.
        /// </summary>
        public bool HideTrailingZeros
        {
            get { return (bool)GetValue(HideTrailingZerosProperty); }
            set { SetValue(HideTrailingZerosProperty, value); }
        }

        /// <summary>
        /// Gets or sets NumberDecimalSeparator string to be used. This is a dependency property.
        /// </summary>
        /// This property is added due to a bug in SL, which XAML parser cannot find mscorlib namespace when reading enums defined in System.Globalization
        public string NumberDecimalSeparator
        {
            get { return (string)GetValue(NumberDecimalSeparatorProperty); }
            set { SetValue(NumberDecimalSeparatorProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to update the Value property to match the current formatted text on LostFocus. This is a dependency property.
        /// </summary>
        public bool UpdateValueToMatchTextOnLostFocus
        {
            get { return (bool)GetValue(UpdateValueToMatchTextOnLostFocusProperty); }
            set { SetValue(UpdateValueToMatchTextOnLostFocusProperty, value); }
        }

        /// <summary>
        /// Gets the current text content held by the textbox. This is a dependency property.
        /// </summary>
        public string ContentText
        {
            get
            {
                if (_textBox != null)
                {
                    return _textBox.Text;
                }
                return FormatDisplay();
            }
        }

        private bool AllowDecimalSeparator => !IsInteger && NumberDecimalDigits != 0;

        public bool IsNegative => (bool)GetValue(IsNegativeProperty);

        #endregion




        #region Methods

        /// <summary>
        /// Selects the entire content of RadNumericUpDown textbox.
        /// </summary>
        public void SelectAll()
        {
            if (_textBox != null)
            {
                FocusTextBox();
                _textBox.SelectAll();
            }
        }

        /// <summary>
        /// Selects a range of text in the RadNumericUpDown textbox.
        /// </summary>
        /// <param name="start">The zero based start.</param>
        /// <param name="length">The length of the selection.</param>
        public void Select(int start, int length)
        {
            _textBox?.Select(start, length);
        }

        /// <summary>
        /// Formats the display value when the control is not focused.
        /// </summary>
        /// <returns>Returns value that is displayed when the control doesn't have focus.</returns>
        public virtual string FormatDisplay()
        {
            var value = Value;

            if (value.HasValue)
            {
                var formatSpecifier = string.Empty;

                var formatInfo = (NumberFormatInfo)GetNumberFormatInfo().Clone();

                var decimals = GetNumberDecimalDigits();
                switch (ValueFormat)
                {
                    case ValueFormat.Numeric:
                        formatSpecifier = "N";
                        formatInfo.NumberDecimalDigits = decimals;
                        break;
                    case ValueFormat.Currency:
                        formatSpecifier = "C";
                        formatInfo.CurrencyDecimalDigits = decimals;
                        break;
                    case ValueFormat.Percentage:
                        formatSpecifier = "P";
                        formatInfo.PercentDecimalDigits = decimals;
                        break;
                }

                var result = value.Value.ToString(formatSpecifier, formatInfo);

                if (!string.IsNullOrEmpty(CustomUnit))
                {
                    result += string.Format(CultureInfo.CurrentCulture, " {0}", CustomUnit);
                }

                return result;
            }
            return NullValue ?? string.Empty;
        }

        /// <summary>
        /// Formats the value when the control is in focus and the user is editing the content.
        /// </summary>
        /// <returns>Return the value when the control is in focus and the user is editing the content.</returns>
        public virtual string FormatEdit()
        {
            var value = Value;

            var formatInfo = (NumberFormatInfo)GetNumberFormatInfo().Clone();

            formatInfo.NumberGroupSeparator = string.Empty;

            return value?.ToString("N" + GetNumberDecimalDigits(), formatInfo) ?? string.Empty;
        }

        /// <summary>
        /// Overrides OnApplyTemplate and attach all necessary events to the controls.
        /// TODO: change the binding to TemplateBinding.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_textBox != null)
            {
                _textBox.TextChanged -= OnTextBoxTextChanged;
                RemoveHandler(KeyDownEvent, new KeyEventHandler(OnKeyDown));
                _textBox.PreviewKeyDown -= OnTextBoxPreviewKeyDown;
                _textBox.RemoveHandler(PreviewLostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnPreviewLostKeyboardFocus));

            }
            _textBox = (TextBox)GetTemplateChild("textbox");
            if (_textBox != null)
            {
                _textBox.TextChanged += OnTextBoxTextChanged;
                AddHandler(KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
                _textBox.PreviewKeyDown += OnTextBoxPreviewKeyDown;
                _textBox.AddHandler(PreviewLostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnPreviewLostKeyboardFocus), true);
                _textBox.SetBinding(IsHitTestVisibleProperty, new Binding("IsKeyboardFocusWithin") { Source = this });

                if (ReadLocalValue(TabNavigationExtensions.IsTabStopProperty) != DependencyProperty.UnsetValue)
                {
                    _textBox.IsTabStop = TabNavigationExtensions.GetIsTabStop(this);
                }
            }

            ChangeVisualState(true);

            UpdateText();
        }

        internal int GetNumberDecimalDigits()
        {
            var numberDecimalDigits = 0;

            if (IsInteger)
            {
                numberDecimalDigits = 0;
            }
            else if (NumberDecimalDigits > -1)
            {
                numberDecimalDigits = NumberDecimalDigits;
            }
            else
            {
                var numberFormatInfo = GetNumberFormatInfo();

                switch (ValueFormat)
                {
                    case ValueFormat.Currency:
                        numberDecimalDigits = numberFormatInfo.CurrencyDecimalDigits;
                        break;
                    case ValueFormat.Numeric:
                        numberDecimalDigits = numberFormatInfo.NumberDecimalDigits;
                        break;
                    case ValueFormat.Percentage:
                        numberDecimalDigits = numberFormatInfo.PercentDecimalDigits;
                        break;
                }
            }

            var decimalDigitsCount = int.MaxValue;

            if (HideTrailingZeros)
            {
                if (Value > decimal.MinValue && Value < decimal.MaxValue)
                {
                    decimalDigitsCount = BitConverter.GetBytes(decimal.GetBits(Convert.ToDecimal(Value))[3])[2];
                }
                else
                {
                    // if value doesn't fit in decimal all the zeros should be hidden
                    decimalDigitsCount = 0;
                }
            }

            return Math.Min(numberDecimalDigits, decimalDigitsCount);
        }

        internal void UpdateText()
        {
            string value;
            if (IsKeyboardFocusWithin)
            {
                value = FormatEdit();
            }
            else
            {
                value = FormatDisplay();
            }

            if (_textBox != null)
            {
                _lastInput = value;
                _textBox.Text = value;
            }
        }

        internal void UpdateValue()
        {
            var newValue = _inputValue.HasValue
                ? (decimal?)Math.Min(Maximum, Math.Max(Minimum, _inputValue.Value))
                : null;

            if (Value != newValue && !IsReadOnly)
            {
                SetCurrentValue(ValueProperty, OnCoerceValue(this, newValue));
            }
        }

        internal bool OnTextBoxKeyDownInternal(Key key)
        {
            switch (key)
            {
                case Key.Enter:
                    UpdateValue();
                    UpdateText();

                    return false;
                default:
                    return false;
            }
        }

        internal void SimulateUserInput(string input)
        {
            if (_textBox != null)
            {
                _textBox.Text = input;
            }
        }

        internal NumberFormatInfo GetNumberFormatInfo()
        {
            NumberFormatInfo numberFormatInfo;
            if (NumberFormatInfo == null)
            {
                // The separator should be updated only if no NumberFormatInfo is set explicitly to the NumericUpDown.
                numberFormatInfo = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                SetNumberDecimalSeparator(numberFormatInfo);
                _isFromCurrentCulture = true;
            }
            else
            {
                numberFormatInfo = (NumberFormatInfo)NumberFormatInfo.Clone();
                _isFromCurrentCulture = false;
            }

            return numberFormatInfo;
        }

        internal bool IsMatch(string input, NumberFormatInfo formatInfo)
        {
            var groupSeparator = Regex.Escape(formatInfo.NumberGroupSeparator);
            var decimalSeparator = Regex.Escape(formatInfo.NumberDecimalSeparator);

            string rule;

            if (AllowDecimalSeparator)
            {
                if (input.StartsWith(formatInfo.NumberDecimalSeparator))
                {
                    rule = $@"^{decimalSeparator}\d*$";
                }
                else
                {
                    rule = $@"^[\d\+-](\d*{groupSeparator})*\d*{decimalSeparator}?\d*$";
                }
            }
            else
            {
                rule = $@"^[\d\+-](\d*{groupSeparator})*\d*$";
            }

            var regex = new Regex(rule);
            return regex.IsMatch(input);
        }

        /// <summary>
        /// Raises the <see cref="E:ValueChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="RangeBaseValueChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnValueChanged(RangeBaseValueChangedEventArgs e)
        {
            _inputValue = e.NewValue;

            SetValue(IsNegativePropertyKey, e.NewValue.GetValueOrDefault() < 0m);

            if (!IsKeyboardFocusWithin)
            {
                UpdateText();
            }

            base.OnValueChanged(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.KeyDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        /// <param name="sender">The sender of the event.</param>
        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!IsReadOnly && OnTextBoxKeyDownInternal(e.Key))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            ChangeVisualState(true);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseEnter"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            ChangeVisualState(true);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.LostFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            OnLostFocus();
            ChangeVisualState(true);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.GotFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            OnGotFocus();

            ChangeVisualState(true);
        }

        /// <summary>
        /// Called before the MouseLeftButtonDown event occurs.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            FocusTextBox();
            SelectAll();
            e.Handled = true;
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked just before the <see cref="E:System.Windows.UIElement.IsKeyboardFocusWithinChanged"/> event is raised by this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (!_isTextBoxContextMenuOpen)
                {
                    OnGotFocus();
                }
                else
                {
                    // Focus came back from closing the ContextMenu - just toggle the flag.
                    _isTextBoxContextMenuOpen = false;
                }
            }
            else
            {
                if (!_isTextBoxContextMenuOpen)
                {
                    OnLostFocus();
                }
            }

            ChangeVisualState(true);
        }

        /// <summary>
        /// Forces the update of all visual states.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ForceVisualState(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((NumericEditor)sender).ChangeVisualState(true);
        }

        /// <summary> 
        ///     ValueFormatProperty property changed handler. 
        /// </summary>
        /// <param name="sender">RadNumericUpDown that changed its ValueFormatProperty.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnValueFormatChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!Enum.IsDefined(typeof(ValueFormat), e.NewValue))
            {
                throw new ArgumentException(@"Invalid value for the ValueFormat enum.", nameof(e));
            }
            ((NumericEditor)sender).UpdateText();
        }

        /// <summary>
        /// Called when [property changed that requires update text].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowTextBoxChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var numeric = (NumericEditor)sender;
            numeric.ChangeVisualState(true);
        }

        // This is added because of a bug in the .NET Framework, which is that the double.TryParse() method does not uses correctly the NumberFormatInfo properties according to 
        // the NumberStyles set. That is why all separators are unified in order to avoid typing in excess characters.
        private static void UnifyGroupSeparators(NumberFormatInfo info, bool isFromCurrentCulture)
        {
            if (!info.IsReadOnly && !isFromCurrentCulture)
            {
                if (info.NumberGroupSeparator == info.PercentGroupSeparator && info.NumberGroupSeparator != info.CurrencyGroupSeparator)
                {
                    info.NumberGroupSeparator = info.PercentGroupSeparator = info.CurrencyGroupSeparator;
                }
                if (info.NumberGroupSeparator == info.CurrencyGroupSeparator && info.NumberGroupSeparator != info.PercentGroupSeparator)
                {
                    info.NumberGroupSeparator = info.CurrencyGroupSeparator = info.PercentGroupSeparator;
                }
                if (info.CurrencyGroupSeparator == info.PercentGroupSeparator && info.CurrencyGroupSeparator != info.NumberGroupSeparator)
                {
                    info.CurrencyGroupSeparator = info.PercentGroupSeparator = info.NumberGroupSeparator;
                }
            }
        }

        private static void UnifyDecimalSeparators(NumberFormatInfo info, bool isFromCurrentCulture)
        {
            if (!info.IsReadOnly && !isFromCurrentCulture)
            {
                if (info.NumberDecimalSeparator == info.PercentDecimalSeparator && info.NumberDecimalSeparator != info.CurrencyDecimalSeparator)
                {
                    info.NumberDecimalSeparator = info.PercentDecimalSeparator = info.CurrencyDecimalSeparator;
                }
                if (info.NumberDecimalSeparator == info.CurrencyDecimalSeparator && info.NumberDecimalSeparator != info.PercentDecimalSeparator)
                {
                    info.NumberDecimalSeparator = info.CurrencyDecimalSeparator = info.PercentDecimalSeparator;
                }
                if (info.CurrencyDecimalSeparator == info.PercentDecimalSeparator && info.CurrencyDecimalSeparator != info.NumberDecimalSeparator)
                {
                    info.CurrencyDecimalSeparator = info.PercentDecimalSeparator = info.NumberDecimalSeparator;
                }
            }
        }

        private void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        private void OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var contextMenu = e.NewFocus as ContextMenu;
            if (contextMenu != null && Equals(contextMenu.PlacementTarget, _textBox))
            {
                _isTextBoxContextMenuOpen = true;
            }
        }

        private void OnNumericIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {

            }

            ChangeVisualState(true);
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_updatingContent)
            {
                _updatingContent = false;
                return;
            }

            if ((IsEditable == false || IsReadOnly) && _textBox.Text != _lastInput)
            {
                _updatingContent = true;
                ReturnPreviousInput();
                return;
            }

            if (IsFocusWithin())
            {
                if (string.IsNullOrEmpty(_textBox.Text))
                {
                    _inputValue = null;
                    _lastInput = string.Empty;
                }
                else
                {
                    if (!TryParseTextBoxTextToValue())
                    {
                        if (InputMethod.Current.ImeState == InputMethodState.On)
                        {
                            e.Handled = true;
                            return;
                        }

                        _updatingContent = true;
                        ReturnPreviousInput();
                    }
                }

                if (UpdateValueEvent == UpdateValueEvent.PropertyChanged)
                {
                    UpdateValue();
                }
            }

        }

        private bool TryParseTextBoxTextToValue(bool shouldUpdateValueToMatchText = false)
        {
            var formatInfo = GetNumberFormatInfo();
            UnifyDecimalSeparators(formatInfo, _isFromCurrentCulture);
            UnifyGroupSeparators(formatInfo, _isFromCurrentCulture);

            var text = _textBox.Text;

            if (shouldUpdateValueToMatchText)
            {
                // we need to remove the additional symbols in order to be able to parse the text to double below
                text = TrimText(text, formatInfo);
            }

            var isMatch = IsMatch(text, formatInfo);
            if (isMatch)
            {
                decimal parsedValue;
                if (decimal.TryParse(text, NumberStyles.Any, formatInfo, out parsedValue))
                {
                    if (ValueFormat == ValueFormat.Percentage && shouldUpdateValueToMatchText)
                    {
                        // if using ValueFormat.Percentage parsed value should be devided by 100 to be correct
                        _inputValue = parsedValue / 100;
                    }
                    else
                    {
                        _inputValue = parsedValue;
                    }
                }

                _lastInput = text;
            }

            return isMatch;
        }

        private string TrimText(string text, NumberFormatInfo formatInfo)
        {
            switch (ValueFormat)
            {
                case ValueFormat.Currency:
                    text = text.Replace(formatInfo.CurrencySymbol, string.Empty).Trim();
                    break;
                case ValueFormat.Percentage:
                    text = text.Replace(formatInfo.PercentSymbol, string.Empty).Trim();
                    break;
            }

            if (!string.IsNullOrEmpty(CustomUnit))
            {
                text = text.Replace(CustomUnit, string.Empty).Trim();
            }

            return text;
        }

        private void ReturnPreviousInput()
        {
            var selectionLenght = _textBox.SelectionLength;
            var selectionStart = _textBox.SelectionStart;

            _textBox.Text = _lastInput;

            _textBox.SelectionStart = selectionStart == 0 ? 0 : selectionStart - 1;
            _textBox.SelectionLength = selectionLenght;
        }

        private void SetNumberDecimalSeparator(NumberFormatInfo numberFormatInfo)
        {
            if (!string.IsNullOrEmpty(NumberDecimalSeparator))
            {
                numberFormatInfo.NumberDecimalSeparator = NumberDecimalSeparator;
            }
        }

        private bool IsFocusWithin()
        {

            return IsKeyboardFocusWithin;
        }

        private void OnLostFocus()
        {
            UpdateValue();
            UpdateText();

            if (UpdateValueToMatchTextOnLostFocus && TryParseTextBoxTextToValue(true)
                && _inputValue.HasValue && _inputValue.Value != Value)
            {
                UpdateValue();
            }
        }

        private void OnGotFocus()
        {
            if (IsFocusWithin())
            {
                UpdateText();
                SelectAll();
            }
        }

        private void ChangeVisualState(bool useTransitions)
        {
            if (IsMouseOver)
            {
                if (ShowTextBox)
                {
                    GoToState(useTransitions, "MouseOver");
                }
                else
                {
                    GoToState(useTransitions, "MouseOverTextHidden");
                }
            }
            else if (IsEnabled)
            {
                GoToState(useTransitions, "Normal");
            }
            else
            {
                GoToState(useTransitions, "Disabled");
            }

            if (ShowTextBox)
            {
                GoToState(useTransitions, "ShowTextBox");
            }
            else
            {
                GoToState(useTransitions, "HideTextBox");
            }
            if (IsKeyboardFocusWithin)
            {
                if (ShowTextBox)
                {
                    GoToState(useTransitions, "Focused");
                }
                else
                {
                    GoToState(useTransitions, "FocusedTextHidden", "Focused");
                }
            }
            else
            {
                GoToState(useTransitions, "Unfocused");
            }
        }

        private void GoToState(bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (var str in stateNames)
                {
                    if (VisualStateManager.GoToState(this, str, useTransitions))
                    {
                        return;
                    }
                }
            }
        }

        private void FocusTextBox()
        {
            _textBox?.Focus();
        }


        #endregion
    }

    public enum ValueFormat
    {
        Numeric,
        Currency,
        Percentage,
    }
    
    public enum UpdateValueEvent
    {
        PropertyChanged,
        LostFocus,
    }

    public static class TabNavigationExtensions
    {
        public static readonly DependencyProperty IsTabStopProperty = DependencyProperty.RegisterAttached("IsTabStop", typeof(bool), typeof(TabNavigationExtensions), new PropertyMetadata(true, OnIsTabStopChanged));

        public static bool GetIsTabStop(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsTabStopProperty);
        }

        public static void SetIsTabStop(DependencyObject obj, bool value)
        {
            obj.SetValue(IsTabStopProperty, value);
        }

        private static void OnIsTabStopChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            var textBox = element?.ChildrenOfType<TextBox>().FirstOrDefault();
            if (textBox == null)
                return;
            textBox.IsTabStop = (bool)e.NewValue;
        }
    }

    public static class ChildrenOfTypeExtensions
    {
        public static IEnumerable<T> ChildrenOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            return element.GetChildrenRecursive().OfType<T>();
        }

        public static T FindChildByType<T>(this DependencyObject element) where T : DependencyObject
        {
            return element.ChildrenOfType<T>().FirstOrDefault();
        }

        internal static IEnumerable<T> FindChildrenByType<T>(this DependencyObject element) where T : DependencyObject
        {
            return element.ChildrenOfType<T>();
        }

        internal static FrameworkElement GetChildByName(this FrameworkElement element, string name)
        {
            return (FrameworkElement)element.FindName(name) ?? element.ChildrenOfType<FrameworkElement>().FirstOrDefault(c => c.Name == name);
        }

        internal static T GetFirstDescendantOfType<T>(this DependencyObject target) where T : DependencyObject
        {
            var obj = target as T;
            if (obj != null)
                return obj;
            return target.ChildrenOfType<T>().FirstOrDefault();
        }

        internal static IEnumerable<T> GetChildren<T>(this DependencyObject parent) where T : FrameworkElement
        {
            return parent.GetChildrenRecursive().OfType<T>();
        }

        private static IEnumerable<DependencyObject> GetChildrenRecursive(this DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child != null)
                {
                    yield return child;
                    foreach (var dependencyObject in child.GetChildrenRecursive())
                        yield return dependencyObject;
                }
            }
        }

        internal static IEnumerable<T> ChildrenOfType<T>(this DependencyObject element, Type typeWhichChildrenShouldBeSkipped)
        {
            return element.GetChildrenOfType(typeWhichChildrenShouldBeSkipped).OfType<T>();
        }

        private static IEnumerable<DependencyObject> GetChildrenOfType(this DependencyObject element, Type typeWhichChildrenShouldBeSkipped)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); ++i)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                yield return child;
                if (!typeWhichChildrenShouldBeSkipped.IsInstanceOfType(child))
                {
                    foreach (var dependencyObject in child.GetChildrenOfType(typeWhichChildrenShouldBeSkipped))
                        yield return dependencyObject;
                }
            }
        }
    }
}
