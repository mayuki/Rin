using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Rin.Channel
{
    internal class HubClientProxy<THub, TClient>
        where THub : IHub
        where TClient : IHubClient
    {
        public static Type ProxyType { get; }
        public static Func<RinChannel, TClient> Create { get; }

        static HubClientProxy()
        {
            ProxyType = HubClientProxyBuilder.Instance.CreateProxyType<THub, TClient>();
            Create = CreateFactory();
        }

        private static Func<RinChannel, TClient> CreateFactory()
        {
            var method = ProxyType.GetMethod("Create", BindingFlags.Static | BindingFlags.Public); // 'ProxyOfHub.Create' static method
            return (Func<RinChannel, TClient>)method.CreateDelegate(typeof(Func<RinChannel, TClient>));
        }
    }

}
