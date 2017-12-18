using System.Windows;
using JetBrains.Annotations;

namespace Algel.WpfTools.Windows.Controls
{
    [PublicAPI]
    public static class StackGridHelper
    {
        public static readonly DependencyProperty ColumnDefinitionsScriptBindableProperty = DependencyProperty.RegisterAttached("ColumnDefinitionsScriptBindable", typeof(string), typeof(StackGridHelper), new PropertyMetadata(default(string), ColumnDefinitionStringBindableProperty_PropertyChangedCallback));

        public static readonly DependencyProperty RowDefinitionsScriptBindableProperty = DependencyProperty.RegisterAttached("RowDefinitionsScriptBindable", typeof(string), typeof(StackGridHelper), new PropertyMetadata(default(string), RowDefinitionsScriptBindableProperty_PropertyChangedCallback));

        public static void SetColumnDefinitionsScriptBindable(DependencyObject element, string value)
        {
            element.SetValue(ColumnDefinitionsScriptBindableProperty, value);
        }

        public static string GetColumnDefinitionsScriptBindable(DependencyObject element)
        {
            return (string) element.GetValue(ColumnDefinitionsScriptBindableProperty);
        }

        public static void SetRowDefinitionsScriptBindable(DependencyObject element, string value)
        {
            element.SetValue(RowDefinitionsScriptBindableProperty, value);
        }

        public static string GetRowDefinitionsScriptBindable(DependencyObject element)
        {
            return (string) element.GetValue(RowDefinitionsScriptBindableProperty);
        }

        private static void ColumnDefinitionStringBindableProperty_PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is GridEx grid)
                grid.ColumnDefinitionsScript = e.NewValue as string;
        }

        private static void RowDefinitionsScriptBindableProperty_PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is GridEx grid)
                grid.RowDefinitionsScript = e.NewValue as string;
        }
    }
}