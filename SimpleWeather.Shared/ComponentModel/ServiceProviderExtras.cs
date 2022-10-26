using Microsoft.Extensions.DependencyInjection;
using System;

namespace SimpleWeather.ComponentModel
{
    public static class ServiceProviderExtras
    {
        /// <summary>
        /// Creates a new instance of a ViewModel class using the provided IServiceProvider
        /// </summary>
        /// <typeparam name="T">IViewModel implementation</typeparam>
        /// <param name="provider">Service provider (ex. Ioc.Default)</param>
        /// <param name="parameters">Constructor arguments to initiate instance if needed</param>
        /// <returns></returns>
        public static T GetViewModel<T>(this IServiceProvider provider, params object[] parameters) where T : IViewModel
        {
            return ActivatorUtilities.CreateInstance<T>(provider, parameters);
        }
    }
}
