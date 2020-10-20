using System;
using System.Collections.Generic;
using System.Text;
using Rin.Core;
using Rin.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for the Rin services.
    /// </summary>
    public static class RinBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="IBodyDataTransformer"/> implementation to Rin services.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IRinBuilder AddBodyDataTransformer<T>(this IRinBuilder builder) where T : class, IBodyDataTransformer
        {
            builder.Services.AddSingleton<IBodyDataTransformer, T>();
            return builder;
        }

        /// <summary>
        /// Adds a <see cref="IRequestBodyDataTransformer"/> implementation to Rin services.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IRinBuilder AddRequestBodyDataTransformer<T>(this IRinBuilder builder) where T : class, IRequestBodyDataTransformer
        {
            builder.Services.AddSingleton<IRequestBodyDataTransformer, T>();
            return builder;
        }

        /// <summary>
        /// Adds a <see cref="IResponseBodyDataTransformer"/> implementation to Rin services.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IRinBuilder AddResponseBodyDataTransformer<T>(this IRinBuilder builder) where T : class, IResponseBodyDataTransformer
        {
            builder.Services.AddSingleton<IResponseBodyDataTransformer, T>();
            return builder;
        }
    }
}
