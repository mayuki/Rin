using System;
using System.Collections.Generic;
using System.Text;

namespace Rin.Hubs.Payloads
{
    public class RinServerInfoPayload
    {
        public string Version { get; set; }
        public DateTimeOffset BuildDate { get; set; }
        public string[] FeatureFlags { get; set; }
    }
}
