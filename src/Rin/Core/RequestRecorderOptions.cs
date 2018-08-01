using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rin.Core
{
    public class RequestRecorderOptions
    {
        public Func<PathString, bool> Excludes { get; set; }

        public int RetentionMaxRequests { get; set; } = 100;

        public bool EnableBodyCapturing { get; set; } = true;
    }
}
