using Microsoft.Extensions.DiagnosticAdapter;
using Rin.Core.Record;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rin.DiagnosticListeners
{
    internal class AspNetMvcCoreDiagnosticListener
    {
        private readonly AsyncLocal<ITimelineScope> _onActionScope = new AsyncLocal<ITimelineScope>();
        private readonly AsyncLocal<ITimelineScope> _onResultScope = new AsyncLocal<ITimelineScope>();

        [DiagnosticName("Microsoft.AspNetCore.Mvc.BeforeOnActionExecuting")]
        public void OnBeforeOnActionExecuting()
        {
            _onActionScope.Value = TimelineScope.Create("ActionExecuting", TimelineScopeCategory.AspNetCoreMvcAction);
        }

        [DiagnosticName("Microsoft.AspNetCore.Mvc.AfterOnActionExecuted")]
        public void OnAfterOnActionExecuted()
        {
            _onActionScope?.Value?.Complete();
        }

        [DiagnosticName("Microsoft.AspNetCore.Mvc.BeforeOnResultExecuting")]
        public void OnBeforeOnResultExecuting()
        {
            Console.WriteLine("BeforeOnResultExecuting");
            _onResultScope.Value = TimelineScope.Create("ResultExecuting", TimelineScopeCategory.AspNetCoreMvcView);
        }

        [DiagnosticName("Microsoft.AspNetCore.Mvc.AfterOnResultExecuting")]
        public void OnAfterOnResultExecuting()
        {
            Console.WriteLine("AfterOnResultExecuting");
            _onResultScope?.Value?.Complete();
        }
    }
}
