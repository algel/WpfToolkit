using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using JetBrains.Annotations;

namespace Algel.WpfTools.Windows.Data
{
    /// <summary>
    /// A base class for custom markup extension which provides properties
    /// that can be found on regular <see cref="Binding"/> markup extension.<br/>
    /// See: http://www.hardcodet.net/2008/04/wpf-custom-binding-class 
    /// </summary>
    [PublicAPI]
    [MarkupExtensionReturnType(typeof(object))]
    public abstract class BindingDecoratorBase : MarkupExtension
    {
        /// <inheritdoc />
        protected BindingDecoratorBase()
        {
            Binding = new Binding();
        }

        /// <inheritdoc />
        protected BindingDecoratorBase([CanBeNull] string path)
        {
            Binding = new Binding(path);
        }

        //check documentation of the Binding class for property information
        #region properties

        /// <summary>
        /// The decorated binding class.
        /// </summary>
        [NotNull]
        [Browsable(false)]
        public Binding Binding { get; }


        /// <inheritdoc cref="System.Windows.Data.Binding.AsyncState"/>
        [DefaultValue(null)]
        public object AsyncState
        {
            get => Binding.AsyncState;
            set => Binding.AsyncState = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.BindsDirectlyToSource"/>
        [DefaultValue(false)]
        public bool BindsDirectlyToSource
        {
            get => Binding.BindsDirectlyToSource;
            set => Binding.BindsDirectlyToSource = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.Converter"/>
        [CanBeNull]
        [DefaultValue(null)]
        public IValueConverter Converter
        {
            get => Binding.Converter;
            set => Binding.Converter = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.TargetNullValue"/>
        [CanBeNull]
        [DefaultValue(null)]
        public object TargetNullValue
        {
            get => Binding.TargetNullValue;
            set => Binding.TargetNullValue = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.ConverterCulture"/>
        [CanBeNull]
        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter)), DefaultValue(null)]
        public CultureInfo ConverterCulture
        {
            get => Binding.ConverterCulture;
            set => Binding.ConverterCulture = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.ConverterParameter"/>
        [CanBeNull]
        [DefaultValue(null)]
        public object ConverterParameter
        {
            get => Binding.ConverterParameter;
            set => Binding.ConverterParameter = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.ElementName"/>
        [CanBeNull]
        [DefaultValue(null)]
        public string ElementName
        {
            get => Binding.ElementName;
            set => Binding.ElementName = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.FallbackValue"/>
        [CanBeNull]
        [DefaultValue(null)]
        public object FallbackValue
        {
            get => Binding.FallbackValue;
            set => Binding.FallbackValue = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.IsAsync"/>
        [DefaultValue(false)]
        public bool IsAsync
        {
            get => Binding.IsAsync;
            set => Binding.IsAsync = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.Mode"/>
        [DefaultValue(BindingMode.Default)]
        public BindingMode Mode
        {
            get => Binding.Mode;
            set => Binding.Mode = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.NotifyOnSourceUpdated"/>
        [DefaultValue(false)]
        public bool NotifyOnSourceUpdated
        {
            get => Binding.NotifyOnSourceUpdated;
            set => Binding.NotifyOnSourceUpdated = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.NotifyOnTargetUpdated"/>
        [DefaultValue(false)]
        public bool NotifyOnTargetUpdated
        {
            get => Binding.NotifyOnTargetUpdated;
            set => Binding.NotifyOnTargetUpdated = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.NotifyOnValidationError"/>
        [DefaultValue(false)]
        public bool NotifyOnValidationError
        {
            get => Binding.NotifyOnValidationError;
            set => Binding.NotifyOnValidationError = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.Path"/>
        [CanBeNull]
        [DefaultValue(null)]
        [ConstructorArgument("path")]
        public PropertyPath Path
        {
            get => Binding.Path;
            set => Binding.Path = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.RelativeSource"/>
        [CanBeNull]
        [DefaultValue(null)]
        public RelativeSource RelativeSource
        {
            get => Binding.RelativeSource;
            set => Binding.RelativeSource = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.Source"/>
        [CanBeNull]
        [DefaultValue(null)]
        public virtual object Source
        {
            get => Binding.Source;
            set => Binding.Source = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.UpdateSourceExceptionFilter"/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter
        {
            get => Binding.UpdateSourceExceptionFilter;
            set => Binding.UpdateSourceExceptionFilter = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.UpdateSourceTrigger"/>
        [DefaultValue(UpdateSourceTrigger.Default)]
        public UpdateSourceTrigger UpdateSourceTrigger
        {
            get => Binding.UpdateSourceTrigger;
            set => Binding.UpdateSourceTrigger = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.ValidatesOnDataErrors"/>
        [DefaultValue(false)]
        public bool ValidatesOnDataErrors
        {
            get => Binding.ValidatesOnDataErrors;
            set => Binding.ValidatesOnDataErrors = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.ValidatesOnExceptions"/>
        [DefaultValue(false)]
        public bool ValidatesOnExceptions
        {
            get => Binding.ValidatesOnExceptions;
            set => Binding.ValidatesOnExceptions = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.XPath"/>
        [CanBeNull]
        [DefaultValue(null)]
        public string XPath
        {
            get => Binding.XPath;
            set => Binding.XPath = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.ValidationRules"/>
        [CanBeNull]
        [DefaultValue(null)]
        public Collection<ValidationRule> ValidationRules => Binding.ValidationRules;

        /// <inheritdoc cref="System.Windows.Data.Binding.StringFormat"/>
        [CanBeNull]
        [DefaultValue(null)]
        public string StringFormat
        {
            get => Binding.StringFormat;
            set => Binding.StringFormat = value;
        }

        /// <inheritdoc cref="System.Windows.Data.Binding.BindingGroupName"/>
        [DefaultValue("")]
        public string BindingGroupName
        {
            get => Binding.BindingGroupName;
            set => Binding.BindingGroupName = value;
        }

        /// <summary>
        /// Additional data
        /// </summary>
        [CanBeNull]
        [DefaultValue(null)]
        public string Tag { get; set; }

        #endregion

        /// <summary>
        /// This basic implementation just sets a binding on the targeted
        /// <see cref="DependencyObject"/> and returns the appropriate
        /// <see cref="BindingExpressionBase"/> instance.<br/>
        /// All this work is delegated to the decorated <see cref="Binding"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// The object value to set on the property where the extension is applied. 
        /// In case of a valid binding expression, this is a <see cref="BindingExpressionBase"/>
        /// instance.
        /// </returns>
        /// <param name="provider">Object that can provide services for the markup
        /// extension.</param>
        public override object ProvideValue(IServiceProvider provider)
        {
            // workaround:  .Net 4, that the serviceProvider dose not get the correct target property.
            var target = provider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            if (target?.TargetObject is Setter)
            {
                return Binding;
            }

            //create a binding and associate it with the target
            return Binding.ProvideValue(provider);
        }

        /// <summary>
        /// Validates a service provider that was submitted to the <see cref="ProvideValue"/>
        /// method. This method checks whether the provider is null (happens at design time),
        /// whether it provides an <see cref="IProvideValueTarget"/> service, and whether
        /// the service's <see cref="IProvideValueTarget.TargetObject"/> and
        /// <see cref="IProvideValueTarget.TargetProperty"/> properties are valid
        /// <see cref="DependencyObject"/> and <see cref="DependencyProperty"/>
        /// instances.
        /// </summary>
        /// <param name="provider">The provider to be validated.</param>
        /// <param name="target">The binding target of the binding.</param>
        /// <param name="dp">The target property of the binding.</param>
        /// <returns>True if the provider supports all that's needed.</returns>
        protected virtual bool TryGetTargetItems(IServiceProvider provider, out DependencyObject target, out DependencyProperty dp)
        {
            target = null;
            dp = null;

            //create a binding and assign it to the target
            var service = (IProvideValueTarget)provider?.GetService(typeof(IProvideValueTarget));
            if (service == null) return false;

            //we need dependency objects / properties
            target = service.TargetObject as DependencyObject;
            dp = service.TargetProperty as DependencyProperty;
            return target != null && dp != null;
        }

    }
}
