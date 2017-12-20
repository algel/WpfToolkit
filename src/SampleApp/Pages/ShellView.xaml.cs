using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SampleApp.Pages
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView
    {
        public ShellView()
        {
            InitializeComponent();

        }

        private void BtnApplyColumnDefinitions_OnClick(object sender, RoutedEventArgs e)
        {
            BindingOperations.GetBindingExpression(tbColumnDefinitions, TextBox.TextProperty)?.UpdateSource();
        }
    }
}
