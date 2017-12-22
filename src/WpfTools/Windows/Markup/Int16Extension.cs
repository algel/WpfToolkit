using System.Windows.Markup;
using JetBrains.Annotations;

namespace Algel.WpfTools.Windows.Markup
{
    [PublicAPI]
    [MarkupExtensionReturnType(typeof(short))]
    public class Int16Extension : PrimitiveTypeExtension<short>
    {
        /// <summary>Initializes a new instance of a class derived from <see cref="T:System.Windows.Markup.MarkupExtension" />. </summary>
        public Int16Extension(short value) : base(value)
        {
        }
    }
}