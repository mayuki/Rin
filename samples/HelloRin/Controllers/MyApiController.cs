using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HelloRin.Controllers
{
    public class MyApiController : ControllerBase
    {
        private readonly ILogger<MyApiController> _logger;

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

        // Custom Content-Type response body is transformed/viewable on Inspector
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

            return File(data, "application/x-rin-custom");
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