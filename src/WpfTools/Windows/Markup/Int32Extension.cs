using System.Windows.Markup;
using JetBrains.Annotations;

namespace Algel.WpfTools.Windows.Markup
{
    [PublicAPI]
    [MarkupExtensionReturnType(typeof(int))]
    public class Int32Extension : PrimitiveTypeExtension<int>
    {
        /// <summary>Initializes a new instance of a class derived from <see cref="T:System.Windows.Markup.MarkupExtension" />. </summary>
        public Int32Extension(int value) : base(value)
        {
        }
    }
}