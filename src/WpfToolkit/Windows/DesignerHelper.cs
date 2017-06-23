using System.ComponentModel;
using System.Windows;

namespace WpfToolkit.Windows
{
    /// <summary>
    /// The designer helper.
    /// </summary>
    public static class DesignerHelper
    {
        /// <summary>
        /// Value indicating whether control in design mode.
        /// </summary>
        private static bool? _designMode;

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend or Visual Studio).
        /// </summary>
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!_designMode.HasValue)
                {
//#if DEBUG
#if SILVERLIGHT
                    designMode = DesignerProperties.IsInDesignTool;
#else
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    _designMode = (bool)DependencyPropertyDescriptor
                        .FromProperty(prop, typeof(DependencyObject))
                        .Metadata.DefaultValue;
#endif
//#else
//                    designMode = false;
//#endif
                }

                return _designMode.GetValueOrDefault();
            }
        }
    }
}
