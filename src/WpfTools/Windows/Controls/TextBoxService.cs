using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using JetBrains.Annotations;

namespace Algel.WpfTools.Windows.Controls
{
    public class TextBoxService
    {
        #region BackgroundReadOnlyProperty

        public static readonly DependencyProperty BackgroundReadOnlyProperty = DependencyProperty.RegisterAttached("BackgroundReadOnly", typeof(Brush), typeof(TextBoxService), new PropertyMetadata(default(Brush)));

		public static void SetBackgroundReadOnly(TextBox element, Brush value)
		{
			element.SetValue(BackgroundReadOnlyProperty, value);
		}

        public static Brush GetBackgroundReadOnly(TextBox element)
        {
            return (Brush)element.GetValue(BackgroundReadOnlyProperty);
        }

        #endregion

        #region IsTextTrimmedProperty

        private static readonly DependencyPropertyKey IsTextTrimmedKey = DependencyProperty.RegisterAttachedReadOnly("IsTextTrimmed", typeof(bool), typeof(TextBoxService), new PropertyMetadata(false));
        public static readonly DependencyProperty IsTextTrimmedProperty = IsTextTrimmedKey.DependencyProperty;

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetIsTextTrimmed(DependencyObject target)
        {
            return (bool)target.GetValue(IsTextTrimmedProperty);
        }

        private static void SetIsTextTrimmed([NotNull] DependencyObject target, bool value)
        {
            target.SetValue(IsTextTrimmedKey, value);
        }

        #endregion

        /// <summary>
        /// Разрешить ввод только цифр
        /// </summary>
        public static readonly DependencyProperty AllowDigitsOnlyProperty = DependencyProperty.RegisterAttached("AllowDigitsOnly", typeof(bool), typeof(TextBoxService), new PropertyMetadata(false, OnAllowDigitsOnlyPropertyChanged));

        private static void OnAllowDigitsOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                    textBox.SetValue(MaskProperty, @"^\d*$");
                else
                    textBox.ClearValue(MaskProperty);
            }

        }

        public static void SetAllowDigitsOnly(DependencyObject element, bool value)
        {
            element.SetValue(AllowDigitsOnlyProperty, value);
        }

        public static bool GetAllowDigitsOnly(DependencyObject element)
        {
            return (bool)element.GetValue(AllowDigitsOnlyProperty);
        }


        #region Masking

        private static readonly DependencyPropertyKey _maskExpressionPropertyKey = DependencyProperty.RegisterAttachedReadOnly("MaskExpression", typeof(Regex), typeof(TextBoxService), new FrameworkPropertyMetadata());

        /// <summary>
        /// Регулярное выражение для фильтра ввода.
        /// </summary>
        public static readonly DependencyProperty MaskProperty = DependencyProperty.RegisterAttached("Mask", typeof(string), typeof(TextBoxService), new FrameworkPropertyMetadata(OnMaskChanged));

        private static readonly DependencyProperty MaskExpressionProperty = _maskExpressionPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the mask for a given <see cref="TextBox"/>.
        /// </summary>
        /// <param name="textBox">
        /// The <see cref="TextBox"/> whose mask is to be retrieved.
        /// </param>
        /// <returns>
        /// The mask, or <see langword="null"/> if no mask has been set.
        /// </returns>
        public static string GetMask(TextBox textBox)
        {
            if (textBox == null)
            {
                throw new ArgumentNullException(nameof(textBox));
            }

            return textBox.GetValue(MaskProperty) as string;
        }

        /// <summary>
        /// Sets the mask for a given <see cref="TextBox"/>.
        /// </summary>
        /// <param name="textBox">
        /// The <see cref="TextBox"/> whose mask is to be set.
        /// </param>
        /// <param name="mask">
        /// The mask to set, or <see langword="null"/> to remove any existing mask from <paramref name="textBox"/>.
        /// </param>
        public static void SetMask(TextBox textBox, string mask)
        {
            if (textBox == null)
            {
                throw new ArgumentNullException(nameof(textBox));
            }

            textBox.SetValue(MaskProperty, mask);
        }

        /// <summary>
        /// Gets the mask expression for the <see cref="TextBox"/>.
        /// </summary>
        /// <remarks>
        /// This method can be used to retrieve the actual <see cref="Regex"/> instance created as a result of setting the mask on a <see cref="TextBox"/>.
        /// </remarks>
        /// <param name="textBox">
        /// The <see cref="TextBox"/> whose mask expression is to be retrieved.
        /// </param>
        /// <returns>
        /// The mask expression as an instance of <see cref="Regex"/>, or <see langword="null"/> if no mask has been applied to <paramref name="textBox"/>.
        /// </returns>
        private static Regex GetMaskExpression(TextBox textBox)
        {
            if (textBox == null)
            {
                throw new ArgumentNullException(nameof(textBox));
            }

            return textBox.GetValue(MaskExpressionProperty) as Regex;
        }

        private static void SetMaskExpression(TextBox textBox, Regex regex)
        {
            textBox.SetValue(_maskExpressionPropertyKey, regex);
        }

        private static void OnMaskChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (!(dependencyObject is TextBox textBox))
                return;

            var mask = (string)e.NewValue;
            textBox.PreviewTextInput -= TextBoxPreviewTextInput;
            textBox.PreviewKeyDown -= TextBoxPreviewKeyDown;
            DataObject.RemovePastingHandler(textBox, Pasting);

            if (mask == null)
            {
                textBox.ClearValue(MaskProperty);
                textBox.ClearValue(MaskExpressionProperty);
            }
            else
            {
                textBox.SetValue(MaskProperty, mask);
                SetMaskExpression(textBox, new Regex(mask, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace));
                textBox.PreviewTextInput += TextBoxPreviewTextInput;
                textBox.PreviewKeyDown += TextBoxPreviewKeyDown;
                DataObject.AddPastingHandler(textBox, Pasting);
            }

            #region Nested methods

            void TextBoxPreviewTextInput(object sender, TextCompositionEventArgs args)
            {
                var tb = (TextBox)sender;
                var maskExpression = GetMaskExpression(tb);

                if (maskExpression == null)
                    return;

                var proposedText = GetProposedText(tb, args.Text);

                if (!maskExpression.IsMatch(proposedText))
                    args.Handled = true;
            }

            void TextBoxPreviewKeyDown(object sender, KeyEventArgs args)
            {
                //pressing space doesn't raise PreviewTextInput - no idea why, but we need to handle
                //explicitly here
                if (args.Key != Key.Space)
                    return;

                var tb = (TextBox)sender;
                var maskExpression = GetMaskExpression(tb);

                if (maskExpression == null)
                    return;

                var proposedText = GetProposedText(tb, " ");

                if (!maskExpression.IsMatch(proposedText))
                    args.Handled = true;
            }

            void Pasting(object sender, DataObjectPastingEventArgs args)
            {
                var tb = (TextBox)sender;
                var maskExpression = GetMaskExpression(tb);

                if (maskExpression == null)
                    return;

                if (args.DataObject.GetDataPresent(typeof(string)))
                {
                    var pastedText = args.DataObject.GetData(typeof(string)) as string;
                    var proposedText = GetProposedText(tb, pastedText);

                    if (!maskExpression.IsMatch(proposedText))
                        args.CancelCommand();
                }
                else
                {
                    args.CancelCommand();
                }
            }

            string GetProposedText(TextBox tb, string newText)
            {
                var text = tb.Text;

                if (tb.SelectionStart != -1)
                    text = text.Remove(tb.SelectionStart, tb.SelectionLength);

                text = text.Insert(tb.CaretIndex, newText);

                return text;
            }
            #endregion
        }

        #endregion

        #region .ctor

        static TextBoxService()
        {
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.MouseEnterEvent, new MouseEventHandler(TextBox_MouseEnter_Event), true);
        }

        #endregion

        #region Methods

        private static void TextBox_MouseEnter_Event(object sender, MouseEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                SetIsTextTrimmed(textBox, TextIsTrimmed(textBox));
            }
        }

        private static bool TextIsTrimmed([NotNull] TextBox textBox)
        {
            // All magic here!
            if (textBox.ExtentWidth > textBox.ActualWidth || textBox.ExtentHeight > textBox.ActualHeight)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
