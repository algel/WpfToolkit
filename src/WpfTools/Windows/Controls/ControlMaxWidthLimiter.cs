using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using JetBrains.Annotations;

namespace Algel.WpfTools.Windows.Controls
{
    /// <summary>
    /// Allows you to block the automatic increase in the width of the control as the content content (e.g. TextBox tends to stretch in width as you type text)
    /// The problem is relevant when the element is in the grid, and is a column with Width="Auto" (if the element spans multiple columns, the problem occurs if at least one column has Width="Auto")
    /// <example>
    /// <code>
    /// <![CDATA[
    /// <Grid>
    ///    <Grid.ColumnDefinitions>
    ///         <ColumnDefinition Width="Auto"/>
    ///         <ColumnDefinition Width="100"/>
    ///         <ColumnDefinition Width="Auto"/>
    ///    </Grid.ColumnDefinitions>
    /// 
    ///     <Label Grid.Column="0" Content="..." Target="{x:Reference myTextBox}/>
    ///     <TextBox x:Name="myTextBox" Grid.Column="1" Grid.ColumnSpan="2" />
    ///     <awt:ControlMaxWidthLimiter Target={x:Reference myTextBox}/>
    /// </Grid>
    /// 
    /// <!--OR-->
    /// 
    /// <TextBox awt:ControlMaxWidthLimiter.FixAutoGrowMaxWidthBehavior="True"/>
    /// ]]>
    /// </code>
    /// </example>
    /// </summary>
    public class ControlMaxWidthLimiter : FrameworkElement
    {
        #region Fields

        /// <summary>
        /// Identifies the WpfToolset.Windows.Controls.ControlMaxWidthLimiter.TargetProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(nameof(Target), typeof(FrameworkElement), typeof(ControlMaxWidthLimiter), new PropertyMetadata(default(FrameworkElement)));

        /// <summary>
        /// Setting the value to True will automatically add in Grid and set the object ControlMaxWidthLimiter
        /// </summary>
        public static readonly DependencyProperty FixAutoGrowMaxWidthProperty = DependencyProperty.RegisterAttached("FixAutoGrowMaxWidth", typeof(bool), typeof(ControlMaxWidthLimiter), new PropertyMetadata(false, OnFixAutoGrowMaxWidthPropertyChanged));

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the item for which you want to control the width
        /// </summary>
        public FrameworkElement Target
        {
            get => (FrameworkElement)GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        #endregion

        #region Methods

        /// <summary>Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement" /> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)" />.</summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == TargetProperty)
                OnTargetPropertyChanged((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue);
            else if (e.Property == ActualWidthProperty)
                OnActualWidthPropertyChanged((double)e.NewValue);
            base.OnPropertyChanged(e);
        }

        private void OnActualWidthPropertyChanged(double newValue)
        {
            if (Target != null)
                Target.MaxWidth = newValue;
        }

        private void OnTargetPropertyChanged(FrameworkElement oldValue, FrameworkElement newValue)
        {
            if (oldValue != null)
            {
                BindingOperations.ClearBinding(this, MarginProperty);
                BindingOperations.ClearBinding(this, Grid.ColumnProperty);
                BindingOperations.ClearBinding(this, Grid.ColumnSpanProperty);
            }
            if (newValue != null)
            {
                SetBinding(MarginProperty, new Binding(nameof(Margin)) { Source = newValue, Mode = BindingMode.OneWay });
                SetBinding(Grid.ColumnProperty, new Binding("(Grid.Column)") { Source = newValue, Mode = BindingMode.OneWay });
                SetBinding(Grid.ColumnSpanProperty, new Binding("(Grid.ColumnSpan)") { Source = newValue, Mode = BindingMode.OneWay });

                OnActualWidthPropertyChanged(ActualWidth);
            }
        }

        private static void OnFixAutoGrowMaxWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            var grid = element?.Parent as Grid;
            if (grid != null)
            {
                if ((bool)e.NewValue)
                {
                    var item = new ControlMaxWidthLimiter();
                    item.SetValue(MarginProperty, element.GetValue(MarginProperty));
                    item.SetValue(Grid.ColumnProperty, element.GetValue(Grid.ColumnProperty));
                    item.SetValue(Grid.ColumnSpanProperty, element.GetValue(Grid.ColumnSpanProperty));
                    grid.Children.Add(item);

                    item.Target = element;
                }
                else
                {
                    var item = grid.Children.OfType<ControlMaxWidthLimiter>().FirstOrDefault(x => Equals(x.Target, element));
                    if (item != null)
                    {
                        grid.Children.Remove(item);
                        item.Target = null;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the value of the WpfToolset.Windows.Controls.ControlMaxWidthLimiter.FixAutoGrowMaxWidth attached property to a given System.Windows.UIElement.
        /// </summary>
        /// <param name="element">The element on which to set the attached property.</param>
        /// <param name="value">The property value to set.</param>
        [PublicAPI]
        public static void SetFixAutoGrowMaxWidth(DependencyObject element, bool value)
        {
            element.SetValue(FixAutoGrowMaxWidthProperty, value);
        }

        /// <summary>
        /// Gets the value of the WpfToolset.Windows.Controls.ControlMaxWidthLimiter.FixAutoGrowMaxWidth attached property from a given System.Windows.UIElement.
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the WpfToolset.Windows.Controls.StackGrid.IsRowBreak attached property.</returns>
        [PublicAPI]
        public static bool GetFixAutoGrowMaxWidth(DependencyObject element)
        {
            return (bool)element.GetValue(FixAutoGrowMaxWidthProperty);
        }

        #endregion
    }

}
