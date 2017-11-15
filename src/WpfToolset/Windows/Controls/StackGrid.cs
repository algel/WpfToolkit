﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using WpfToolset.Linq;

namespace WpfToolset.Windows.Controls
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
    /// <StackGrid RowDefinitionsScript="Auto;*;Auto" ColumnDefinitionsScript="Auto;*">
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
    /// </StackGrid>
    /// ]]>
    /// </code>
    /// </example>
    [PublicAPI]
    public class StackGrid : GridEx
    {
        #region Fields

        private bool _isInitialized;

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
        /// Identifies the WpfToolset.Windows.Controls.StackGrid.AutogenerateRows dependency property.
        /// </summary>
        public static readonly DependencyProperty AutogenerateRowsProperty = DependencyProperty.Register(nameof(AutogenerateRows), typeof(bool), typeof(StackGrid), new PropertyMetadata(default(bool)));

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StackGrid"/> class.
        /// </summary>
        public StackGrid()
        {
            if (DesignerHelper.IsInDesignModeStatic)
            {
                Loaded += OnInitialized;
            }
            else
            {
                Initialized += OnInitialized;
            }
        }

        #endregion

        #region Method

        /// <summary>
        /// Called when the control is initialized.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// In the non-WPF implementation, this event is actually hooked to the <c>LayoutUpdated</c> event.
        /// </remarks>
        private void OnInitialized(object sender, EventArgs e)
        {
            if (DesignerHelper.IsInDesignModeStatic)
            {
                Loaded -= OnInitialized;
            }
            else
            {
                Initialized -= OnInitialized;
            }

            FinalInitialize();
        }

        /// <summary>
        /// Final initialize so the <see cref="StackGrid"/> is actually created.
        /// </summary>
        private void FinalInitialize()
        {
            if (_isInitialized)
            {
                return;
            }

            if (AutogenerateRows && RowDefinitions.Count != 0)
                throw new InvalidOperationException("It is not allowed to specify AutogenerateRows=True and RowDefinitions");

            SetColumnsAndRows();

            _isInitialized = true;
        }

        /// <summary>
        /// Sets the columns and rows.
        /// </summary>
        private void SetColumnsAndRows()
        {
            var columnCount = Math.Max(ColumnDefinitions.Count, 1);
            var currentColumn = 0;
            var currentRow = 0;

            foreach (FrameworkElement child in Children)
            {
                if (GetDisableAutoAllocation(child) || child is ControlMaxWidthLimiter)
                    continue;

                if (GetStretchToLastColumn(child))
                    SetColumnSpan(child, columnCount - currentColumn);

                var columnSpan = GetColumnSpan(child);

                if (child is EmptyRow)
                {
                    // If not yet reached the end of columns, force a new increment anyway
                    if (currentColumn != 0 && currentColumn <= columnCount)
                    {
                        currentRow++;
                    }

                    // The current column for an empty row is alway zero
                    currentColumn = 0;

                    SetColumn(child, currentColumn);
                    SetColumnSpan(child, columnCount);
                    SetRow(child, currentRow);

                    currentRow++;
                    continue;
                }

                SetColumn(child, currentColumn);
                SetRow(child, currentRow);

                if (currentColumn + columnSpan >= columnCount || GetIsRowBreak(child))
                {
                    currentColumn = 0;
                    currentRow++;
                }
                else
                {
                    // Increment the current column by the column span
                    currentColumn = currentColumn + columnSpan;
                }
            }

            if (AutogenerateRows)
            {
                Enumerable.Range(0, currentRow + 1).ForEach(i => RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }));
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
