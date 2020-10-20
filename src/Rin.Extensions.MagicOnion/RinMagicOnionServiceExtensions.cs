using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Rin.Extensions;
using Rin.Extensions.MagicOnion;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class RinMagicOnionServiceExtensions
    {
        /// <summary>
        /// Adds Rin extension services for MagicOnion.
        /// </summary>
        /// <param name="builder"></param>
        public static IRinBuilder AddMagicOnionSupport(this IRinBuilder builder)
        {
            builder.AddRequestBodyDataTransformer<MagicOnionRequestBodyDataTransformer>();
            builder.AddResponseBodyDataTransformer<MagicOnionResponseBodyDataTransformer>();

            return builder;
        }
    }
}
