using System.Windows.Markup;
using JetBrains.Annotations;

namespace Algel.WpfTools.Windows.Markup
{
    [PublicAPI]
    [MarkupExtensionReturnType(typeof(byte))]
    public class ByteExtension : PrimitiveTypeExtension<byte>
    {
        /// <summary>Initializes a new instance of a class derived from <see cref="T:System.Windows.Markup.MarkupExtension" />. </summary>
        public ByteExtension(byte value) : base(value)
        {
        }
    }
}