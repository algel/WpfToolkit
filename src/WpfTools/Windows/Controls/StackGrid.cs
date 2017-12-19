using System;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace Algel.WpfTools.Windows.Controls
{
    /// <summary>
    /// A grid-like control that allows a developer to specify the rows and columns, but gives the freedom
    /// not to define the actual grid and row numbers of the controls inside the <see cref="StackGrid"/>.
    /// <para />
    /// The <see cref="StackGrid"/> automatically builds up the internal grid.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// <awt:StackGrid RowDefinitionsScript="Auto;*;Auto" ColumnDefinitionsScript="Auto;*">
    /// 
    ///   <!-- Name, will be set to row 0, column 1 and 2 -->
    ///   <Label Content="Name" />
    ///   <TextBox Text="{Bindng Name}" />
    /// 
    ///   <!-- Empty row -->
    ///   <EmptyRow />
    /// 
    ///   <!-- Wrappanel, will span 2 columns -->
    ///   <WrapPanel StackGrid.ColumnSpan="2">
    ///     <Button Command="{Binding OK}" />
    ///   </WrapPanel>
    /// </awt:StackGrid>
    /// ]]>
    /// </code>
    /// </example>
    [PublicAPI]
    public class StackGrid : GridEx
    {
        #region Fields

        private int _lastMeasureChildrenCount;

        /// <summary>
        /// Sign line feed. I.e. even if the current row is not filled cells, then the following items need to be placed on a new line
        /// </summary>
        public static readonly DependencyProperty IsRowBreakProperty = DependencyProperty.RegisterAttached("IsRowBreak", typeof(bool), typeof(StackGrid), new PropertyMetadata(false));

        /// <summary>
        /// To disable the automatic arrangement in lines and columns
        /// </summary>
        public static readonly DependencyProperty DisableAutoAllocationProperty = DependencyProperty.RegisterAttached("DisableAutoAllocation", typeof(bool), typeof(StackGrid), new PropertyMetadata(default(bool)));

        /// <summary>
        /// To put ColumnSpan in such a way that the element is stretched up to the rightmost column
        /// </summary>
        public static readonly DependencyProperty StretchToLastColumnProperty = DependencyProperty.RegisterAttached("StretchToLastColumn", typeof(bool), typeof(StackGrid), new PropertyMetadata(default(bool)));

        /// <summary>
        /// Optimisation for skip already allocated childs
        /// </summary>
        private static readonly DependencyProperty IsAutoAllocatedProperty = DependencyProperty.RegisterAttached("IsAutoAllocated", typeof(bool), typeof(StackGrid), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="AutogenerateRows"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutogenerateRowsProperty = DependencyProperty.Register(nameof(AutogenerateRows), typeof(bool), typeof(StackGrid), new PropertyMetadata(default(bool)));

        #endregion

        #region Constructor

        ///// <summary>
        ///// Initializes a new instance of the <see cref="StackGrid"/> class.
        ///// </summary>
        //public StackGrid()
        //{
        //    if (DesignerHelper.IsInDesignModeStatic)
        //    {
        //        Loaded += OnInitialized;
        //    }
        //    else
        //    {
        //        Initialized += OnInitialized;

        //    }
        //}

        #endregion

        #region Method

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            var currentMeasureChildrenCount = Children.Count;
            if (_lastMeasureChildrenCount != currentMeasureChildrenCount)
            {
                SetPositionForAllChildren();
                _lastMeasureChildrenCount = currentMeasureChildrenCount;
            }

            return base.MeasureOverride(constraint);
        }

        private void SetPositionForAllChildren(bool forced = false)
        {
            UIElement previous = null;
            foreach (UIElement child in Children)
            {
                SetPositionForElement(child, previous, forced);
                previous = child;
            }
        }

        private void SetPositionForElement(UIElement element, UIElement prevousElement, bool forced)
        {
            if ((!forced && GetIsAutoAllocated(element))
                || GetDisableAutoAllocation(element)
                || element is ControlMaxWidthLimiter)
                return;

            var columnCount = Math.Max(ColumnDefinitions.Count, 1);
            var currentColumn = 0;
            var currentRow = 0;

            if (prevousElement != null)
            {
                var pColumn = GetColumn(prevousElement);
                var pColumnSpan = GetColumnSpan(prevousElement);
                var pRow = GetRow(prevousElement);

                if (GetIsRowBreak(prevousElement) || GetStretchToLastColumn(prevousElement) || pColumn + pColumnSpan >= columnCount)
                {
                    currentColumn = 0;
                    currentRow = pRow + 1;
                }
                else
                {
                    currentColumn = pColumn + pColumnSpan;
                    currentRow = pRow;
                }
            }

            SetRow(element, currentRow);

            if (element is EmptyRow)
            {
                SetColumn(element, 0);
                SetColumnSpan(element, columnCount);
            }
            else
            {
                SetColumn(element, currentColumn);

                if (GetStretchToLastColumn(element))
                    SetColumnSpan(element, columnCount - currentColumn);
            }

            SetIsAutoAllocated(element, true);

            if (AutogenerateRows && RowDefinitions.Count == currentRow)
            {
                RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
        }

        /// <summary>
        /// Sets the value of the WpfToolset.Windows.Controls.StackGrid.IsRowBreak attached property to a given System.Windows.UIElement.
        /// </summary>
        /// <param name="element">The element on which to set the attached property.</param>
        /// <param name="value">The property value to set.</param>
        [PublicAPI]
        public static void SetIsRowBreak(DependencyObject element, bool value)
        {
            element.SetValue(IsRowBreakProperty, value);
        }

        /// <summary>
        /// Gets the value of the WpfToolset.Windows.Controls.StackGrid.IsRowBreak attached property from a given System.Windows.UIElement.
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the WpfToolset.Windows.Controls.StackGrid.IsRowBreak attached property.</returns>
        [PublicAPI]
        public static bool GetIsRowBreak(DependencyObject element)
        {
            return (bool)element.GetValue(IsRowBreakProperty);
        }

        /// <summary>
        /// Sets the value of the WpfToolset.Windows.Controls.StackGrid.DisableAutoAllocation attached property to a given System.Windows.UIElement.
        /// </summary>
        /// <param name="element">The element on which to set the attached property.</param>
        /// <param name="value">The property value to set.</param>
        [PublicAPI]
        public static void SetDisableAutoAllocation(DependencyObject element, bool value)
        {
            element.SetValue(DisableAutoAllocationProperty, value);
        }

        /// <summary>
        /// Gets the value of the WpfToolset.Windows.Controls.StackGrid.DisableAutoAllocation attached property from a given System.Windows.UIElement.
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the  WpfToolset.Windows.Controls.StackGrid.DisableAutoAllocation attached property.</returns>
        [PublicAPI]
        public static bool GetDisableAutoAllocation(DependencyObject element)
        {
            return (bool)element.GetValue(DisableAutoAllocationProperty);
        }

        /// <summary>
        /// Sets the value of the WpfToolset.Windows.Controls.StackGrid.StretchToLastColumn attached property to a given System.Windows.UIElement.
        /// </summary>
        /// <param name="element">The element on which to set the attached property.</param>
        /// <param name="value">The property value to set.</param>
        [PublicAPI]
        public static void SetStretchToLastColumn(DependencyObject element, bool value)
        {
            element.SetValue(StretchToLastColumnProperty, value);
        }

        /// <summary>
        /// Gets the value of the WpfToolset.Windows.Controls.StackGrid.StretchToLastColumn attached property from a given System.Windows.UIElement.
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the  WpfToolset.Windows.Controls.StackGrid.StretchToLastColumn attached property.</returns>
        [PublicAPI]
        public static bool GetStretchToLastColumn(DependencyObject element)
        {
            return (bool)element.GetValue(StretchToLastColumnProperty);
        }

        private static bool GetIsAutoAllocated(DependencyObject element)
        {
            return (bool)element.GetValue(IsAutoAllocatedProperty);
        }

        private static void SetIsAutoAllocated(DependencyObject element, bool value)
        {
            element.SetValue(IsAutoAllocatedProperty, value);
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == ColumnDefinitionsScriptProperty)
                SetPositionForAllChildren(true);
        }

        #endregion

        /// <summary>
        /// Allows to describe the definition of lines, the algorithm automatically determines the required number of rows and adds them to the definition.
        /// </summary>
        [PublicAPI]
        public bool AutogenerateRows
        {
            get => (bool)GetValue(AutogenerateRowsProperty);
            set => SetValue(AutogenerateRowsProperty, value);
        }
    }
}
