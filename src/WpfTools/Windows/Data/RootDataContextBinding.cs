using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;
using JetBrains.Annotations;
using Algel.WpfTools.Extensions;
using XamlParseException = System.Windows.Markup.XamlParseException;

namespace Algel.WpfTools.Windows.Data
{
    public class RootDataContextBinding : BindingDecoratorBase
    {
        /// <inheritdoc />
        public RootDataContextBinding()
        {
        }

        /// <inheritdoc />
        public RootDataContextBinding([CanBeNull] string path) : base(path)
        {
        }

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            CheckCanReceiveMarkupExtension(this, serviceProvider, out var targetDependencyObject, out var targetDependencyProperty);
            if (targetDependencyObject == null || targetDependencyProperty == null)
                return this;

            //// Retrieve target information
            //var target = serviceProvider.GetService<IProvideValueTarget>();
            //if (target?.TargetObject != null)
            //{
            //    // In a template the TargetObject is a SharedDp (internal WPF class)
            //    // In that case, the markup extension itself is returned to be re-evaluated later
            //    if (target.TargetObject.GetType().FullName == "System.Windows.SharedDp")
            //        return this;
            //}

            Binding.Source = serviceProvider.GetService<IRootObjectProvider>().RootObject;

            var oldPath = Path?.Path;

            var pathBuilder = new StringBuilder();
            pathBuilder.Append(FrameworkElement.DataContextProperty.Name);
            if (!string.IsNullOrEmpty(oldPath))
                pathBuilder.AppendFormat(".{0}", oldPath);

            Path = new PropertyPath(pathBuilder.ToString());

            return base.ProvideValue(serviceProvider);
        }

        // ReSharper disable once UnusedMember.Local
        /// <inheritdoc />
        [Browsable(false)]
        public override object Source
        {
            get => base.Source;
            set => throw new NotSupportedException();
        }

        #region Imported from MS.Internal.Helper in PresentationFramework 

        /// <summary>
        /// Checks if the given IProvideValueTarget can receive
        /// a DynamicResource or Binding MarkupExtension.
        /// </summary>
        internal static void CheckCanReceiveMarkupExtension(
            MarkupExtension markupExtension,
            IServiceProvider serviceProvider,
            out DependencyObject targetDependencyObject,
            out DependencyProperty targetDependencyProperty)
        {
            targetDependencyObject = null;
            targetDependencyProperty = null;

            IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (provideValueTarget == null)
            {
                return;
            }

            object targetObject = provideValueTarget.TargetObject;

            if (targetObject == null)
            {
                return;
            }

            Type targetType = targetObject.GetType();
            object targetProperty = provideValueTarget.TargetProperty;

            if (targetProperty != null)
            {
                targetDependencyProperty = targetProperty as DependencyProperty;
                if (targetDependencyProperty != null)
                {
                    // This is the DependencyProperty case

                    targetDependencyObject = targetObject as DependencyObject;
                    Debug.Assert(targetDependencyObject != null, "DependencyProperties can only be set on DependencyObjects");
                }
                else
                {
                    MemberInfo targetMember = targetProperty as MemberInfo;
                    if (targetMember != null)
                    {
                        // This is the Clr Property case
                        PropertyInfo propertyInfo = targetMember as PropertyInfo;

                        // Setters, Triggers, DataTriggers & Conditions are the special cases of
                        // Clr properties where DynamicResource & Bindings are allowed. Normally
                        // these cases are handled by the parser calling the appropriate
                        // ReceiveMarkupExtension method.  But a custom MarkupExtension
                        // that delegates ProvideValue will end up here (see Dev11 117372).
                        // So we handle it similarly to how the parser does it.

                        EventHandler<XamlSetMarkupExtensionEventArgs> setMarkupExtension
                            = LookupSetMarkupExtensionHandler(targetType);

                        if (setMarkupExtension != null && propertyInfo != null)
                        {
                            IXamlSchemaContextProvider scp = serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;
                            if (scp != null)
                            {
                                XamlSchemaContext sc = scp.SchemaContext;
                                XamlType xt = sc.GetXamlType(targetType);
                                if (xt != null)
                                {
                                    XamlMember member = xt.GetMember(propertyInfo.Name);
                                    if (member != null)
                                    {
                                        var eventArgs = new XamlSetMarkupExtensionEventArgs(member, markupExtension, serviceProvider);

                                        // ask the target object whether it accepts MarkupExtension
                                        setMarkupExtension(targetObject, eventArgs);
                                        if (eventArgs.Handled)
                                            return;     // if so, all is well
                                    }
                                }
                            }

                        }


                        // Find the MemberType

                        Debug.Assert(targetMember is PropertyInfo || targetMember is MethodInfo,
                            "TargetMember is either a Clr property or an attached static settor method");

                        Type memberType;

                        if (propertyInfo != null)
                        {
                            memberType = propertyInfo.PropertyType;
                        }
                        else
                        {
                            MethodInfo methodInfo = (MethodInfo)targetMember;
                            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                            Debug.Assert(parameterInfos.Length == 2, "The signature of a static settor must contain two parameters");
                            memberType = parameterInfos[1].ParameterType;
                        }

                        // Check if the MarkupExtensionType is assignable to the given MemberType
                        // This check is to allow properties such as the following
                        // - DataTrigger.Binding
                        // - Condition.Binding
                        // - HierarchicalDataTemplate.ItemsSource
                        // - GridViewColumn.DisplayMemberBinding

                        if (!typeof(MarkupExtension).IsAssignableFrom(memberType) ||
                            !memberType.IsAssignableFrom(markupExtension.GetType()))
                        {
                            throw new XamlParseException($"A '{markupExtension.GetType().Name}' cannot be set on the '{targetMember.Name}' property of type '{targetType.Name}'. A '{markupExtension.GetType().Name}' can only be set on a DependencyProperty of a DependencyObject.");
                        }
                    }
                    else
                    {
                        // This is the Collection ContentProperty case
                        // Example:
                        // <DockPanel>
                        //   <Button />
                        //   <DynamicResource ResourceKey="foo" />
                        // </DockPanel>

                        // Collection<BindingBase> used in MultiBinding is a special
                        // case of a Collection that can contain a Binding.

                        if (!typeof(BindingBase).IsAssignableFrom(markupExtension.GetType()) ||
                            !typeof(Collection<BindingBase>).IsAssignableFrom(targetProperty.GetType()))
                        {
                            throw new XamlParseException($"A '{markupExtension.GetType().Name}' cannot be used within a '{targetProperty.GetType().Name}' collection. A '{markupExtension.GetType().Name}' can only be set on a DependencyProperty of a DependencyObject.");
                        }
                    }
                }
            }
            else
            {
                // This is the explicit Collection Property case
                // Example:
                // <DockPanel>
                // <DockPanel.Children>
                //   <Button />
                //   <DynamicResource ResourceKey="foo" />
                // </DockPanel.Children>
                // </DockPanel>

                // Collection<BindingBase> used in MultiBinding is a special
                // case of a Collection that can contain a Binding.

                if (!typeof(BindingBase).IsAssignableFrom(markupExtension.GetType()) || !typeof(Collection<BindingBase>).IsAssignableFrom(targetType))
                {
                    throw new XamlParseException($"A '{markupExtension.GetType().Name}' cannot be used within a '{targetType.Name}' collection. A '{markupExtension.GetType().Name}' can only be set on a DependencyProperty of a DependencyObject.");
                }
            }
        }

        static EventHandler<XamlSetMarkupExtensionEventArgs> LookupSetMarkupExtensionHandler(Type type)
        {
            if (typeof(Setter).IsAssignableFrom(type))
                return Setter.ReceiveMarkupExtension;

            if (typeof(DataTrigger).IsAssignableFrom(type))
                return DataTrigger.ReceiveMarkupExtension;

            if (typeof(Condition).IsAssignableFrom(type))
                return Condition.ReceiveMarkupExtension;

            return null;
        }


        #endregion
    }
}
