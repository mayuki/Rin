using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Rin.Core;

namespace Rin.Extensions
{
    public interface IRinBuilder
    {
        IServiceCollection Services { get; }
    }

    internal class RinBuilder : IRinBuilder
    {
        public IServiceCollection Services { get; }

        public RinBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
