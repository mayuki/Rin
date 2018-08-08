using Rin.Core;
using Rin.DiagnosticListeners;
using Rin.Middlewares;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class RinMvcExtensions
    {
        public static void UseRinMvcSupport(this IApplicationBuilder app)
        {
            var diagnosticListener = app.ApplicationServices.GetService(typeof(DiagnosticListener)) as DiagnosticListener;
            diagnosticListener.SubscribeWithAdapter(new AspNetMvcCoreDiagnosticListener());
        }
    }
}
