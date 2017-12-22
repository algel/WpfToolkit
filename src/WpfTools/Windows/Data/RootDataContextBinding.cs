using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Xaml;
using JetBrains.Annotations;
using Algel.WpfTools.Extensions;

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
        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            Binding.Source = serviceProvider.GetService<IRootObjectProvider>().RootObject;

            var oldPath = Path?.Path;

            var pathBuilder = new StringBuilder();
            pathBuilder.Append(FrameworkElement.DataContextProperty.Name);
            if (!string.IsNullOrEmpty(oldPath))
                pathBuilder.Append($".{oldPath}");

            Path = new PropertyPath(pathBuilder.ToString());

            return base.ProvideValueInternal(serviceProvider);
        }



        // ReSharper disable once UnusedMember.Local
        /// <inheritdoc />
        [Browsable(false)]
        public override object Source
        {
            get => base.Source;
            set => throw new NotSupportedException();
        }

    }
}
