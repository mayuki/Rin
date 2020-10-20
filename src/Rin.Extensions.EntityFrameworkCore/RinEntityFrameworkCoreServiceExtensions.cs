using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Rin.Extensions;
using Rin.Extensions.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class RinEntityFrameworkCoreServiceExtensions
    {
        /// <summary>
        /// Adds Rin extension services for Entity Framework Core diagnostics.
        /// </summary>
        /// <param name="builder"></param>
        public static IRinBuilder AddEntityFrameworkCoreDiagnostics(this IRinBuilder builder)
        {
            builder.Services.AddHostedService<EntityFrameworkCoreRelationalDiagnosticsHostedService>();

            return builder;
        }
    }
}
