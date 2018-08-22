using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelloRin.Models;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rin.Core.Record;

namespace HelloRin.Controllers
{
    public class MyApiController : ControllerBase
    {
        private readonly ILogger<MyApiController> _logger;
        private readonly ILog _loggerLog4Net = log4net.LogManager.GetLogger(typeof(MyApiController));

        public MyApiController(ILogger<MyApiController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return Content("Index");
        }

        public async Task<string> Delay(int ms)
        {
            await Task.Delay(ms);
            return ms.ToString();
        }

        public IActionResult AsOctetStream()
        {
            return File(new byte[] { 1, 2, 3, 4, 5 }, "application/octet-stream");
        }

        [Produces("application/json")]
        public MyClass AsJson()
        {
            return new MyClass
            {
                ValueA = "ValueA12345",
                ValueB = 123,
                ValueC = new MyClass.InnerClass
                {
                    ValueD = 37564,
                    ValueE = new[] { "Hauhau", "Maumau" }
                },
                ValueF = Enumerable.Range(0, 100).Select(x => new MyClass()
                {
                    ValueA = x.ToString(),
                    ValueB = x,
                    ValueC = new MyClass.InnerClass
                    {
                        ValueD = x * 10,
                        ValueE = new[] { x.ToString() }
                    }
                }).ToArray()
            };
        }

        public IActionResult Throw()
        {
            _logger.LogCritical("Critical message");
            _logger.LogDebug("Debug message");
            _logger.LogError("Error message");
            _logger.LogWarning("Warning message");
            throw new Exception("Nanka yabai exception");
        }

        [HttpPost]
        [Produces("application/json")]
        public object PostWithFormBody([FromForm]string bodyValueA, [FromForm]int bodyValueB)
        {
            return new { ValueA = bodyValueA, ValueB = bodyValueB };
        }

        [HttpPost]
        [Produces("application/json")]
        public object PostWithFormData([FromBody]string bodyValueA, [FromBody]int bodyValueB)
        {
            return new { ValueA = bodyValueA, ValueB = bodyValueB };
        }

        // Custom Content-Type response body is transformed and viewable on Inspector
        // See RinCustomContentTypeTransformer class in this project.
        public IActionResult CustomContentType()
        {
            var data = MessagePack.LZ4MessagePackSerializer.Serialize(new MyClass
            {
                ValueA = "ValueA12345",
                ValueB = 123,
                ValueC = new MyClass.InnerClass
                {
                    ValueD = 37564,
                    ValueE = new[] { "Hauhau", "Maumau" }
                },
                ValueF = Enumerable.Range(0, 100).Select(x => new MyClass()
                {
                    ValueA = x.ToString(),
                    ValueB = x,
                    ValueC = new MyClass.InnerClass
                    {
                        ValueD = x * 10,
                        ValueE = new[] { x.ToString() }
                    }
                }).ToArray()
            }, MessagePack.Resolvers.ContractlessStandardResolver.Instance);

            return File(data, "application/x-msgpack");
        }

        public IActionResult LongResult()
        {
            var stream = new MemoryStream(new byte[1024 * 1024 * 1]);
            return File(new SlowStream(stream), "application/octet-stream");
        }

        public async Task<IActionResult> Timeline()
        {
            _loggerLog4Net.Warn("Begin Timeline \r\n(logging via log4net)");

            using (TimelineScope.Create("First"))
            {
                await Task.Delay(50);
                using (TimelineScope.Create("Second"))
                {
                    await Task.Delay(100);
                    var t1 = HogeAsync(12);
                    var t2 = HogeAsync(34);
                    var t3 = HogeAsync(56);
                    await Task.WhenAll(t1, t2, t3);
                }
                await Task.Delay(85);

                var sql = @"SELECT
    *
FROM
    SugoiTable
WHERE
    Nantoka = 1 AND Kantoka IN (1, 2, 3, 4, 5)
ORDER BY
    Id DESC";
                using (TimelineScope.Create("Third", TimelineEventCategory.Data, sql))
                {
                    await Task.Delay(120);
                    await MogeAsync(22);
                    await Task.Delay(80);
                    await MogeAsync(33);
                }
            }

            NewThread();

            var scope = TimelineScope.Create("Manual", TimelineEventCategory.Data);
            {
                var scope2 = TimelineScope.Create("Manual.Inner", TimelineEventCategory.Data);
                scope2.Complete();
                scope2.Duration = TimeSpan.FromMilliseconds(24);
            }
            scope.Complete();
            scope.Duration = TimeSpan.FromMilliseconds(12);

            for (var i = 0; i < 30; i++)
            {
                var scope2 = TimelineScope.Create("Manual." + i, TimelineEventCategory.Data);
                scope2.Complete();
                scope2.Duration = TimeSpan.FromMilliseconds(24 + i);
            }

            return Content("OK");
        }

        void NewThread()
        {
            var t = new Thread(() =>
            {
                using (TimelineScope.Create())
                {
                    Thread.Sleep(120);
                }
            });

            t.Start();
            t.Join();
        }

        async Task HogeAsync(int delay)
        {
            using (TimelineScope.Create())
            {
                await Task.Delay(delay);
                await MogeAsync(delay * 2);
            }
        }
        async Task MogeAsync(int delay)
        {
            using (TimelineScope.Create())
            {
                await Task.Delay(delay);
            }
        }

        public class MyClass
        {
            public string ValueA { get; set; }
            public int ValueB { get; set; }

            public InnerClass ValueC { get; set; }

            public MyClass[] ValueF { get; set; }

            public class InnerClass
            {
                public long ValueD { get; set; }
                public string[] ValueE { get; set; }
            }
        }
    }
}