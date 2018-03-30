using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Algel.WpfTools.Windows.Controls
{
    public static class TextBlockService
    {
        #region Fields
        /// <summary>
        /// Enables automatic text display in the tooltip when the mouse hovers over, if part of the text is trimmed.
        /// </summary>
        public static readonly DependencyProperty AutomaticToolTipEnabledProperty = DependencyProperty.RegisterAttached("AutomaticToolTipEnabled", typeof(bool), typeof(TextBlockService), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

        private static readonly DependencyPropertyKey IsTextTrimmedKey = DependencyProperty.RegisterAttachedReadOnly("IsTextTrimmed", typeof(bool), typeof(TextBlockService), new PropertyMetadata(false));
        
        /// <summary>
        /// Returns True, if part of the text is trimmed
        /// </summary>
        public static readonly DependencyProperty IsTextTrimmedProperty = IsTextTrimmedKey.DependencyProperty;

        /// <summary>
        /// The text you want to highlight
        /// </summary>
        public static readonly DependencyProperty HighlightedTextProperty = DependencyProperty.RegisterAttached("HighlightedText", typeof(string), typeof(TextBlockService), new PropertyMetadata(SelectText));

        /// <summary>
        /// Style for highlighted text
        /// </summary>
        public static readonly DependencyProperty HighlightedItemStyleProperty = DependencyProperty.RegisterAttached("HighlightedItemStyle", typeof(Style), typeof(TextBlockService), new PropertyMetadata(default(Style)));

        #endregion

        #region .ctor

        static TextBlockService()
        {
            EventManager.RegisterClassHandler(typeof(TextBlock), UIElement.MouseEnterEvent, new MouseEventHandler(OnTextBlockMouseEnter), true);
        }

        #endregion

        #region AttachedProperty accessors

        #region AutomaticToolTipEnabled

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static bool GetAutomaticToolTipEnabled(DependencyObject element)
        {
            if (null == element)
            {
                throw new ArgumentNullException(nameof(element));
            }
            return (bool)element.GetValue(AutomaticToolTipEnabledProperty);
        }

        public static void SetAutomaticToolTipEnabled(DependencyObject element, bool value)
        {
            if (null == element)
                throw new ArgumentNullException(nameof(element));

            element.SetValue(AutomaticToolTipEnabledProperty, value);
        }

        #endregion

        [AttachedPropertyBrowsableForType(typeof(TextBlock))]
        public static bool GetIsTextTrimmed(TextBlock target)
        {
            return (bool)target.GetValue(IsTextTrimmedProperty);
        }

        #region HighlightedText

        [AttachedPropertyBrowsableForType(typeof(TextBlock))]
        public static void SetHighlightedText(DependencyObject element, string value)
        {
            element.SetValue(HighlightedTextProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBlock))]
        public static string GetHighlightedText(DependencyObject element)
        {
            return (string)element.GetValue(HighlightedTextProperty);
        }

        #endregion

        #region HighlightedItemStyle

        [AttachedPropertyBrowsableForType(typeof(TextBlock))]
        public static void SetHighlightedItemStyle(DependencyObject element, Style value)
        {
            element.SetValue(HighlightedItemStyleProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBlock))]
        public static Style GetHighlightedItemStyle(DependencyObject element)
        {
            return (Style)element.GetValue(HighlightedItemStyleProperty);
        }

        #endregion

        #endregion

        #region Methods

        private static void OnTextBlockMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock textBlock)
                SetIsTextTrimmed(textBlock, TextTrimming.None != textBlock.TextTrimming && CalculateIsTextTrimmed(textBlock));
        }

        private static bool CalculateIsTextTrimmed(TextBlock textBlock)
        {
            if (!textBlock.Inlines.Any())
                return false;

            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return textBlock.DesiredSize.Width - textBlock.ActualWidth > -0.3;

        }

        private static void SelectText(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;
            if (!(d is TextBlock))
                throw new InvalidOperationException("Only valid for TextBlock");

            var txtBlock = (TextBlock)d;
            var text = txtBlock.Text;
            if (string.IsNullOrEmpty(text))
                return;

            var highlightText = GetHighlightedText(d);
            if (string.IsNullOrEmpty(highlightText))
                return;

            var index = text.IndexOf(highlightText, StringComparison.CurrentCultureIgnoreCase);
            if (index < 0)
                return;

            var highlightItemStyle = GetHighlightedItemStyle(d);

            txtBlock.Inlines.Clear();
            while (true)
            {
                txtBlock.Inlines.Add(new Run(text.Substring(0, index)));

                var highlightedItem = new Run(text.Substring(index, highlightText.Length));
                if (highlightItemStyle != null)
                    highlightedItem.Style = highlightItemStyle;

                txtBlock.Inlines.Add(highlightedItem);

                text = text.Substring(index + highlightText.Length);
                index = text.IndexOf(highlightText, StringComparison.CurrentCultureIgnoreCase);

                if (index < 0)
                {
                    txtBlock.Inlines.Add(new Run(text));
                    break;
                }
            }
        }

        private static void SetIsTextTrimmed(TextBlock target, bool value)
        {
            target.SetValue(IsTextTrimmedKey, value);
        }

        #endregion

    }
}
