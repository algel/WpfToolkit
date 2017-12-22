using System.Windows.Markup;
using JetBrains.Annotations;

namespace Algel.WpfTools.Windows.Markup
{
    [PublicAPI]
    [MarkupExtensionReturnType(typeof(bool))]
    public class BoolExtension : PrimitiveTypeExtension<bool>
    {
        /// <summary>Initializes a new instance of a class derived from <see cref="T:System.Windows.Markup.MarkupExtension" />. </summary>
        public BoolExtension(bool value) : base(value)
        {
        }
    }
}