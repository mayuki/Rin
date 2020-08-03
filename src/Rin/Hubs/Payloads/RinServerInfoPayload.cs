using System;
using System.Collections.Generic;
using System.Text;

namespace Rin.Hubs.Payloads
{
    public class RinServerInfoPayload
    {
        public string Version { get; }
        public DateTimeOffset BuildDate { get; }
        public IReadOnlyList<string> FeatureFlags { get; }

        public RinServerInfoPayload(string version, DateTimeOffset buildDate, IReadOnlyList<string> featureFlags)
        {
            Version = version;
            BuildDate = buildDate;
            FeatureFlags = featureFlags;
        }
    }
}
