using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace WpfToolset.Windows.Controls
{
        /// <summary>
    /// Represents an element that has a value within a specific range.
    /// </summary>
    public abstract class RangeBase : Control
    {
        #region Fields

        /// <summary>
        /// Identifies the LargeChange dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(decimal), typeof(RangeBase), new FrameworkPropertyMetadata(decimal.MaxValue, OnMaximumChanged, CoerceMaximum)/*, IsValidDoubleValue*/);

        /// <summary>
        /// Identifies the LargeChange dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(decimal), typeof(RangeBase), new FrameworkPropertyMetadata(decimal.MinValue, OnMinimumChanged)/*, IsValidDoubleValue*/);

        /// <summary>
        /// Identifies the LargeChange dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(decimal?), typeof(RangeBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, OnCoerceValue)/*, IsValidDoubleValue*/);

        /// <summary>
        /// Identifies the AllowNullValue dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowNullValueProperty = DependencyProperty.Register("AllowNullValue", typeof(bool), typeof(RangeBase), new PropertyMetadata(true));
        #endregion

        /// <summary>
        /// Occurs when the range value changes.
        /// </summary>
        [Category("Behavior")]
        [PublicAPI]
        public event EventHandler<RangeBaseValueChangedEventArgs> ValueChanged;

        #region Properties

        /// <summary>
        ///  Gets or sets the highest possible RadRangeBase.Value of the range element.
        /// </summary>
        [PublicAPI]
        public decimal Maximum
        {
            get => (decimal)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        /// <summary>
        ///  Gets or sets the lowest possible RadRangeBase.Value of the range element.
        /// </summary>
        [PublicAPI]
        public decimal Minimum
        {
            get => (decimal)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        /// <summary>
        /// Gets or sets the current setting of the range control, which may be coerced.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        [PublicAPI]
        public decimal? Value
        {
            get => (decimal?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        [PublicAPI]
        public bool AllowNullValue
        {
            get => (bool)GetValue(AllowNullValueProperty);
            set => SetValue(AllowNullValueProperty, value);
        }

        #endregion


        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} Minimum:{1} Maximum:{2} Value:{3}", base.ToString(), Minimum, Maximum, Value);
        }

        /// <summary>
        /// Called when the RadRangeBase.Maximum property changes.
        /// </summary>
        /// <param name="oldMaximum">Old value of the RadRangeBase.Maximum property.</param>
        /// <param name="newMaximum">New value of the RadRangeBase.Maximum property.</param>
        protected virtual void OnMaximumChanged(decimal oldMaximum, decimal newMaximum)
        {
        }

        /// <summary>
        /// Called when the RadRangeBase.Minimum property changes.
        /// </summary>
        /// <param name="oldMinimum">Old value of the RadRangeBase.Minimum property.</param>
        /// <param name="newMinimum">New value of the RadRangeBase.Minimum property.</param>
        protected virtual void OnMinimumChanged(decimal oldMinimum, decimal newMinimum)
        {
        }

        /// <summary>
        /// Raises the RadRangeBase.ValueChanged routed event.
        /// </summary>
        protected virtual void OnValueChanged(RangeBaseValueChangedEventArgs e)
        {
            var valueChanged = ValueChanged;
            valueChanged?.Invoke(this, e);
        }

        protected static object OnCoerceValue(DependencyObject d, object value)
        {
            var rangeBase = (RangeBase)d;

            object correctValue = null;
            if (value != null)
                correctValue = GetValueInRange(rangeBase, (decimal)value);
            else if (!rangeBase.AllowNullValue)
                correctValue = GetValueInRange(rangeBase, 0m);

            return correctValue;
        }

        private static object CoerceMaximum(DependencyObject d, object maximum)
        {
            var rangeBase = (RangeBase)d;

            var minimum = rangeBase.Minimum;
            if ((decimal)maximum < minimum)
            {
                return minimum;
            }
            return maximum;
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rangeBase = (RangeBase)d;
            rangeBase.CoerceValue(ValueProperty);
            rangeBase.OnMaximumChanged((decimal)e.OldValue, (decimal)d.GetValue(e.Property));
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rangeBase = (RangeBase)d;
            rangeBase.CoerceValue(MaximumProperty);
            rangeBase.CoerceValue(ValueProperty);
            rangeBase.OnMinimumChanged((decimal)e.OldValue, (decimal)d.GetValue(e.Property));
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RangeBase)d).OnValueChanged(new RangeBaseValueChangedEventArgs((decimal?)e.OldValue, (decimal?)d.GetValue(e.Property)));
        }

        private static decimal GetValueInRange(RangeBase numeric, decimal value)
        {
            var minimum = numeric.Minimum;
            var maximum = numeric.Maximum;
            var doubleValue = value;
            if (doubleValue < minimum || minimum > maximum)
            {
                return minimum;
            }
            if (doubleValue > maximum)
            {
                return maximum;
            }

            return value;
        }


        #endregion

    }

    public class RangeBaseValueChangedEventArgs : EventArgs
    {
        public decimal? OldValue { get; set; }

        public decimal? NewValue { get; set; }

        public RangeBaseValueChangedEventArgs(decimal? oldValue, decimal? newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
