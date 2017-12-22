using System.Windows.Markup;
using JetBrains.Annotations;

namespace Algel.WpfTools.Windows.Markup
{
    [PublicAPI]
    [MarkupExtensionReturnType(typeof(long))]
    public class Int64Extension : PrimitiveTypeExtension<long>
    {
        /// <summary>Initializes a new instance of a class derived from <see cref="T:System.Windows.Markup.MarkupExtension" />. </summary>
        public Int64Extension(long value) : base(value)
        {
        }
    }
}