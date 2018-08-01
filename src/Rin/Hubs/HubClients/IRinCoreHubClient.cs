using Rin.Channel;
using Rin.Hubs;
using Rin.Hubs.Payloads;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rin.Hubs.HubClients
{
    public interface IRinCoreHubClient : IHubClient
    {
        Task RequestBegin(RequestEventPayload payload);
        Task RequestEnd(RequestEventPayload payload);
    }
}
