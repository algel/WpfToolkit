using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Algel.WpfTools.Linq;
using JetBrains.Annotations;

namespace Algel.WpfTools.Windows.Controls
{
    /// <summary>
    /// It allows you to describe the rows and columns of the table using a string
    /// <example>
    /// <code>&lt;awt:GridEx RowDefinitionsScript="[3]Auto;*" ColumnDefinitionScript="Auto;*"&gt;</code>
    /// </example>
    /// </summary>
    [PublicAPI]
    public class GridEx : Grid
    {
        #region Fields

        /// <summary>
        /// Allows you to describe the row and column position for child element of the table using a string
        /// <example>
        /// <code>
        /// GridEx.Position="0 0" Equals to Grid.Row="0" Grid.Column="0"<br/>
        /// GridEx.Position="1 3" Equals to Grid.Row="1" Grid.Column="3"<br/>
        /// GridEx.Position="0 1-3" Equals to Grid.Row="0" Grid.Column="1" Grid.ColumnSpan="3"<br/>
        /// GridEx.Position="0-2 3-4" Equals to Grid.Row="0" Grid.RowSpan="3" Grid.Column="1" Grid.ColumnSpan="2"<br/>
        /// </code>
        /// </example>
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached("Position", typeof(string), typeof(GridEx), new PropertyMetadata(default(string), OnPositionPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ColumnDefinitionsScript"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnDefinitionsScriptProperty = DependencyProperty.Register(nameof(ColumnDefinitionsScript), typeof(string), typeof(GridEx), new PropertyMetadata(default(string), null, CoerceScriptDefinitionsValueCallback));

        /// <summary>
        /// Identifies the <see cref="RowDefinitionsScript"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowDefinitionsScriptProperty = DependencyProperty.Register(nameof(RowDefinitionsScript), typeof(string), typeof(GridEx), new PropertyMetadata(default(string), null, CoerceScriptDefinitionsValueCallback));

        private bool _isColumnDefinitionsScriptPropertyInternalChanging;
        private bool _isRowDefinitionsScriptPropertyInternalChanging;

        private static readonly string RowColumnScriptPartJoinDelimiter = ";";
        private static readonly string[] RowColumnScriptPartSplitDelimiters = { RowColumnScriptPartJoinDelimiter };

        #endregion

        #region Methods

        private static void OnPositionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (string)e.NewValue;
            if (string.IsNullOrEmpty(newValue))
            {
                sender.ClearValue(RowProperty);
                sender.ClearValue(RowSpanProperty);
                sender.ClearValue(ColumnProperty);
                sender.ClearValue(ColumnSpanProperty);
            }
            else
            {
                var parts = newValue.Split(new[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    throw new InvalidOperationException("Входная строка имеет неверный формат");

                var rowTuple = ParseDefinition(parts[0]);
                var columnTuple = ParseDefinition(parts[1]);

                sender.SetValue(RowProperty, rowTuple.Item1);
                sender.SetValue(RowSpanProperty, rowTuple.Item2);
                sender.SetValue(ColumnProperty, columnTuple.Item1);
                sender.SetValue(ColumnSpanProperty, columnTuple.Item2);
            }
        }

        private static Tuple<int, int> ParseDefinition(string definition)
        {
            var parts = definition.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 1 || parts.Length > 2)
                throw new FormatException("Входная строка имеет неверный формат");

            var index = int.Parse(parts[0]);
            var span = 1;
            if (parts.Length == 2)
                span = int.Parse(parts[1]) - index + 1;

            return new Tuple<int, int>(index, span);
        }

        /// <summary>
        /// Sets the value of the WpfToolset.Windows.Controls.GridEx.Position attached property to a given System.Windows.UIElement.
        /// </summary>
        /// <param name="element">The element on which to set the attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetPosition(DependencyObject element, string value)
        {
            element.SetValue(PositionProperty, value);
        }

        /// <summary>
        /// Gets the value of the WpfToolset.Windows.Controls.GridEx.Position attached property from a given System.Windows.UIElement.
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the WpfToolset.Windows.Controls.GridEx.Position attached property.</returns>
        public static string GetPosition(DependencyObject element)
        {
            return (string)element.GetValue(PositionProperty);
        }

        private static IEnumerable<string> ExpandStringDefinition(string source)
        {
            if (!source.StartsWith("["))
                return new[] { source };

            var i2 = source.IndexOf("]", StringComparison.Ordinal);
            var cnt = int.Parse(source.Substring(1, i2 - 1));
            var sourceWithoutCount = source.Substring(i2 + 1).Trim();

            return Enumerable.Repeat(sourceWithoutCount, cnt);
        }

        private static IEnumerable<string> CompactStringDefinition(IEnumerable<string> source)
        {
            string previewsItem = null;
            var counter = 0;
            foreach (var item in source)
            {
                if (previewsItem == null)
                {
                    previewsItem = item;
                    counter++;
                    continue;
                }

                if (string.Compare(previewsItem, item, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    counter++;
                    continue;
                }

                yield return counter > 1 ? $"[{counter}]{previewsItem}" : previewsItem;

                previewsItem = item;
                counter = 1;
            }
            yield return counter > 1 ? $"[{counter}]{previewsItem}" : previewsItem;
        }

        private void GenerateRowDefinitionsFromScript(string script)
        {
            RowDefinitions.Clear();
            script?.Split(RowColumnScriptPartSplitDelimiters, StringSplitOptions.RemoveEmptyEntries)
                .SelectMany(ExpandStringDefinition)
                .ForEach(e =>
                {
                    RowDefinitions.Add(RowColumnDefinition.FromString(e).ToRowDefinition());
                });
        }

        private string GenerateScriptFromRowDefinitions()
        {
            return string.Join(RowColumnScriptPartJoinDelimiter, CompactStringDefinition(RowDefinitions.Select(rd => RowColumnDefinition.FromRowDefinition(rd).ToString())));
        }

        private void GenerateColumnDefinitionsFromScript(string script)
        {
            ColumnDefinitions.Clear();
            script?.Split(RowColumnScriptPartSplitDelimiters, StringSplitOptions.RemoveEmptyEntries)
                .SelectMany(ExpandStringDefinition)
                .ForEach(e =>
                {
                    ColumnDefinitions.Add(RowColumnDefinition.FromString(e).ToColumnDefinition());
                });
        }

        private string GenerateScriptFromColumnDefinitions()
        {
            return string.Join(RowColumnScriptPartJoinDelimiter, CompactStringDefinition(ColumnDefinitions.Select(cd => RowColumnDefinition.FromColumnDefinition(cd).ToString())));
        }

        private static string OptimizeScript(string script)
        {
            if (string.IsNullOrWhiteSpace(script))
                return null;

            var expandedParts = script.Split(RowColumnScriptPartSplitDelimiters, StringSplitOptions.RemoveEmptyEntries)
                .SelectMany(ExpandStringDefinition);
            return string.Join(RowColumnScriptPartJoinDelimiter, CompactStringDefinition(expandedParts));
        }

        private static object CoerceScriptDefinitionsValueCallback(DependencyObject dependencyObject, object baseValue)
        {
            return OptimizeScript((string) baseValue);
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ColumnDefinitionsScriptProperty && !_isColumnDefinitionsScriptPropertyInternalChanging)
                GenerateColumnDefinitionsFromScript((string)e.NewValue);
            if (e.Property == RowDefinitionsScriptProperty && !_isRowDefinitionsScriptPropertyInternalChanging)
                GenerateRowDefinitionsFromScript((string)e.NewValue);
        }

        #endregion

        #region Properties

        ///// <summary>
        ///// Text description of table rows
        ///// The row descriptions are separated by semicolons, the string parameters are separated by a space
        ///// If the same row are several rows, you can reduce the entry specifying the beginning of the description, the number of repetitions in square brackets
        ///// 
        ///// <example>
        ///// Examples:
        ///// <code>
        ///// RowDefinitionsScript="Auto;" equals &lt;RowDefinition Height="Auto"/&gt;
        ///// RowDefinitionsScript="Auto;Auto;Auto;" equals RowDefinitionsScript="[3]Auto;"
        ///// 
        ///// RowDefinitionsScript="30 GroupName;" equals &lt;RowDefinition Height="30" SharedSizeGroup="GroupName" /&gt;
        ///// RowDefinitionsScript="20 Auto 50;" equals &lt;RowDefinition MinHeight="20" Height="Auto" MaxHeight=50" /&gt;
        ///// RowDefinitionsScript="20 Auto 50 GroupName;" equals &lt;RowDefinition MinHeight="20" Height="Auto" MaxHeight=50" SharedSizeGroup="GroupName" /&gt;
        ///// </code>
        ///// </example>
        ///// </summary>
        //public string RowDefinitionsScript
        //{
        //    get
        //    {
        //        return GenerateScriptFromRowDefinitions();
        //    }
        //    set
        //    {
        //        var oldValue = RowDefinitionsScript;
        //        if (string.Compare(oldValue, value, StringComparison.OrdinalIgnoreCase) != 0)
        //        {
        //            GenerateRowDefinitionsFromScript(value);
        //            RaisePropertyChanged();
        //        }
        //    }
        //}

        ///// <summary>
        ///// Text description of table columns
        ///// Descriptions of columns are separated by semicolons, the parameters column are separated by a space
        ///// If the same column are several columns, you can reduce the entry specifying the beginning of the description, the number of repetitions in square brackets
        ///// 
        ///// <example>
        ///// Examples:
        ///// <code>
        ///// ColumnDefinitionsScript="Auto;" equals &lt;ColumnDefinition Width="Auto"/&gt;
        ///// ColumnDefinitionsScript="Auto;Auto;Auto;" equals ColumnDefinition="[3]Auto;"
        ///// 
        ///// ColumnDefinitionsScript="30 GroupName;" equals &lt;ColumnDefinition Width="30" SharedSizeGroup="GroupName" /&gt;
        ///// ColumnDefinitionsScript="20 Auto 50;" equals &lt;ColumnDefinition MinWidth="20" Width="Auto" MaxWidth=50" /&gt;
        ///// ColumnDefinitionsScript="20 Auto 50 GroupName;" equals &lt;ColumnDefinition MinWidth="20" Width="Auto" MaxWidth=50" SharedSizeGroup="GroupName" /&gt;
        ///// </code>
        ///// </example>
        ///// </summary>
        //public string ColumnDefinitionsScript
        //{
        //    get
        //    {
        //        return GenerateScriptFromColumnDefinitions();
        //    }

        //    set
        //    {
        //        var oldValue = ColumnDefinitionsScript;
        //        if (string.Compare(oldValue, value, StringComparison.OrdinalIgnoreCase) != 0)
        //        {
        //            GenerateColumnDefinitionsFromScript(value);
        //            RaisePropertyChanged();
        //        }
        //    }
        //}

        /// <summary>
        /// Text description of table columns
        /// Descriptions of columns are separated by semicolons, the parameters column are separated by a space
        /// If the same column are several columns, you can reduce the entry specifying the beginning of the description, the number of repetitions in square brackets
        /// 
        /// <example>
        /// Examples:
        /// <code>
        /// ColumnDefinitionsScript="Auto;" equals &lt;ColumnDefinition Width="Auto"/&gt;
        /// ColumnDefinitionsScript="Auto;Auto;Auto;" equals ColumnDefinition="[3]Auto;"
        /// 
        /// ColumnDefinitionsScript="30 GroupName;" equals &lt;ColumnDefinition Width="30" SharedSizeGroup="GroupName" /&gt;
        /// ColumnDefinitionsScript="20 Auto 50;" equals &lt;ColumnDefinition MinWidth="20" Width="Auto" MaxWidth=50" /&gt;
        /// ColumnDefinitionsScript="20 Auto 50 GroupName;" equals &lt;ColumnDefinition MinWidth="20" Width="Auto" MaxWidth=50" SharedSizeGroup="GroupName" /&gt;
        /// </code>
        /// </example>
        /// </summary>
        public string ColumnDefinitionsScript
        {
            get { return (string)GetValue(ColumnDefinitionsScriptProperty); }
            set { SetValue(ColumnDefinitionsScriptProperty, value); }
        }

        /// <summary>
        /// Text description of table columns
        /// Descriptions of columns are separated by semicolons, the parameters column are separated by a space
        /// If the same column are several columns, you can reduce the entry specifying the beginning of the description, the number of repetitions in square brackets
        /// 
        /// <example>
        /// Examples:
        /// <code>
        /// ColumnDefinitionsScript="Auto;" equals &lt;ColumnDefinition Width="Auto"/&gt;
        /// ColumnDefinitionsScript="Auto;Auto;Auto;" equals ColumnDefinition="[3]Auto;"
        /// 
        /// ColumnDefinitionsScript="30 GroupName;" equals &lt;ColumnDefinition Width="30" SharedSizeGroup="GroupName" /&gt;
        /// ColumnDefinitionsScript="20 Auto 50;" equals &lt;ColumnDefinition MinWidth="20" Width="Auto" MaxWidth=50" /&gt;
        /// ColumnDefinitionsScript="20 Auto 50 GroupName;" equals &lt;ColumnDefinition MinWidth="20" Width="Auto" MaxWidth=50" SharedSizeGroup="GroupName" /&gt;
        /// </code>
        /// </example>
        /// </summary>
        public string RowDefinitionsScript
        {
            get { return (string)GetValue(RowDefinitionsScriptProperty); }
            set { SetValue(RowDefinitionsScriptProperty, value); }
        }

        #endregion

        #region Netsed types

        private struct RowColumnDefinition
        {
            private double MinHeightWidth { get; set; }
            private double MaxHeightWidth { get; set; }
            private GridLength HeightWidth { get; set; }
            private string SharedSizeGroup { get; set; }

            /// <summary>
            /// Returns the fully qualified type name of this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.String"/> containing a fully qualified type name.
            /// </returns>
            public override string ToString()
            {
                var gridLengthConverter = new GridLengthConverter();
                var lengthConverter = new LengthConverter();
                var lst = new List<string>();
                if (!Double.IsPositiveInfinity(MaxHeightWidth) || MinHeightWidth > 0.0)
                {
                    lst.Add(lengthConverter.ConvertToInvariantString(MinHeightWidth));
                    lst.Add(gridLengthConverter.ConvertToInvariantString(HeightWidth));
                    lst.Add(lengthConverter.ConvertToInvariantString(MaxHeightWidth));
                }
                else
                {
                    lst.Add(gridLengthConverter.ConvertToInvariantString(HeightWidth));
                }

                if (!string.IsNullOrWhiteSpace(SharedSizeGroup))
                {
                    lst.Add(SharedSizeGroup);
                }
                return string.Join(" ", lst);
            }

            /// <exception cref="ArgumentException">Не удалось распознать параметры.</exception>
            public static RowColumnDefinition FromString(string info)
            {
                var gridLengthConverter = new GridLengthConverter();
                var lengthConverter = new LengthConverter();
                var splittedParts = info.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    // ReSharper disable PossibleNullReferenceException
                    switch (splittedParts.Length)
                    {
                        case 1:
                            {
                                return new RowColumnDefinition
                                {
                                    MinHeightWidth = 0,
                                    HeightWidth = (GridLength)gridLengthConverter.ConvertFromInvariantString(splittedParts[0]),
                                    MaxHeightWidth = double.PositiveInfinity,
                                };
                            }
                        case 2:
                            {
                                return new RowColumnDefinition
                                {
                                    MinHeightWidth = 0,
                                    HeightWidth = (GridLength)gridLengthConverter.ConvertFromInvariantString(splittedParts[0]),
                                    MaxHeightWidth = double.PositiveInfinity,
                                    SharedSizeGroup = splittedParts[1]
                                };
                            }
                        case 3:
                            {
                                return new RowColumnDefinition
                                {
                                    MinHeightWidth = (double)lengthConverter.ConvertFromInvariantString(splittedParts[0]),
                                    HeightWidth = (GridLength)gridLengthConverter.ConvertFromInvariantString(splittedParts[1]),
                                    MaxHeightWidth = (double)lengthConverter.ConvertFromInvariantString(splittedParts[2])
                                };
                            }
                        case 4:
                            {
                                return new RowColumnDefinition
                                {
                                    MinHeightWidth = (double)lengthConverter.ConvertFromInvariantString(splittedParts[0]),
                                    HeightWidth = (GridLength)gridLengthConverter.ConvertFromInvariantString(splittedParts[1]),
                                    MaxHeightWidth = (double)lengthConverter.ConvertFromInvariantString(splittedParts[2]),
                                    SharedSizeGroup = splittedParts[3]
                                };
                            }

                        default:
                            throw new ArgumentException("Не удалось распознать параметры.", nameof(info));
                            // ReSharper restore PossibleNullReferenceException
                    }
                }
                catch (NullReferenceException e)
                {
                    throw new ArgumentException($"Не удалось распознать параметры (info={info}).", nameof(info), e);
                }
            }

            public static RowColumnDefinition FromColumnDefinition(ColumnDefinition cd)
            {
                return new RowColumnDefinition
                {
                    MinHeightWidth = cd.MinWidth,
                    HeightWidth = cd.Width,
                    MaxHeightWidth = cd.MaxWidth,
                    SharedSizeGroup = cd.SharedSizeGroup
                };
            }

            public static RowColumnDefinition FromRowDefinition(RowDefinition cd)
            {
                return new RowColumnDefinition
                {
                    MinHeightWidth = cd.MinHeight,
                    HeightWidth = cd.Height,
                    MaxHeightWidth = cd.MaxHeight,
                    SharedSizeGroup = cd.SharedSizeGroup
                };
            }

            public ColumnDefinition ToColumnDefinition()
            {
                return new ColumnDefinition
                {
                    MinWidth = MinHeightWidth,
                    Width = HeightWidth,
                    MaxWidth = MaxHeightWidth,
                    SharedSizeGroup = SharedSizeGroup
                };
            }

            public RowDefinition ToRowDefinition()
            {
                return new RowDefinition
                {
                    MinHeight = MinHeightWidth,
                    Height = HeightWidth,
                    MaxHeight = MaxHeightWidth,
                    SharedSizeGroup = SharedSizeGroup
                };
            }
        }

        #endregion

    }
}
