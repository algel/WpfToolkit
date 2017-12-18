using System;
using System.Windows.Markup;

namespace Algel.WpfTools.Windows.Markup
{
    public abstract class PrimitiveTypeExtension<T> : MarkupExtension
    {
        /// <summary>When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension. </summary>
        /// <returns>The object value to set on the property where the extension is applied. </returns>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Value;
        }

        [ConstructorArgument("value")]
        public T Value { get; set; }

        /// <summary>Initializes a new instance of a class derived from <see cref="T:System.Windows.Markup.MarkupExtension" />. </summary>
        protected PrimitiveTypeExtension(T value)
        {
            Value = value;
        }

    }
}
