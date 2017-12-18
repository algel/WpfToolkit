using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Algel.WpfTools.Windows.Converters
{
    [ValueConversion(typeof(object), typeof(TextAlignment))]
    public class HorizontalContentAlignmentToTextAlignmentConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return TextAlignment.Left;
            switch ((HorizontalAlignment) Enum.Parse(typeof (HorizontalAlignment), value.ToString(), true))
            {
                case HorizontalAlignment.Left:
                    return TextAlignment.Left;
                case HorizontalAlignment.Center:
                    return TextAlignment.Center;
                case HorizontalAlignment.Right:
                    return TextAlignment.Right;
                case HorizontalAlignment.Stretch:
                    return TextAlignment.Left;
                default:
                    return TextAlignment.Left;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        /// <summary>When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension. </summary>
        /// <returns>The object value to set on the property where the extension is applied. </returns>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
