using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rin.Core.Record;
using Rin.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rin.Core
{
    public class RequestRecorderOptions
    {
        public List<Func<HttpRequest, bool>> Excludes { get; set; } = new List<Func<HttpRequest, bool>>();

        public int RetentionMaxRequests { get; set; } = 100;

        public bool EnableBodyCapturing { get; set; } = true;

        public bool AllowRunningOnProduction { get; set; } = false;

        public Func<IServiceProvider, IRecordStorage> StorageFactory { get; set; } = (services) =>
        {
            return new InMemoryRecordStorage(services.GetService<RinOptions>().RequestRecorder.RetentionMaxRequests);
        };
    }
}
