using System;

namespace Algel.WpfTools.Extensions
{
    internal static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            return (T) serviceProvider.GetService(typeof(T));
        }
    }
}
