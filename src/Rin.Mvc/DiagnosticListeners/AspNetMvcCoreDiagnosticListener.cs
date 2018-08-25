using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DiagnosticAdapter;
using Rin.Core.Record;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rin.Mvc.DiagnosticListeners
{
    internal class AspNetMvcCoreDiagnosticListener
    {
        private readonly AsyncLocal<ITimelineScope> _onActionMethodScope = new AsyncLocal<ITimelineScope>();
        private readonly AsyncLocal<ITimelineScope> _onActionResultScope = new AsyncLocal<ITimelineScope>();
        private readonly AsyncLocal<ITimelineScope> _onViewPageScope = new AsyncLocal<ITimelineScope>();

        #region ActionMethod
        // https://github.com/aspnet/Mvc/blob/rel/2.0.0/src/Microsoft.AspNetCore.Mvc.Core/Internal/ControllerActionInvoker.cs#L318
        [DiagnosticName("Microsoft.AspNetCore.Mvc.BeforeActionMethod")]
        public void OnBeforeActionMethod(Microsoft.AspNetCore.Mvc.ActionContext actionContext, System.Collections.Generic.IDictionary<string, object> actionArguments, object controller)
        {
            _onActionMethodScope.Value = TimelineScope.Create("ActionMethod", TimelineEventCategory.AspNetCoreMvcAction, actionContext.ActionDescriptor.DisplayName);
        }

        [DiagnosticName("Microsoft.AspNetCore.Mvc.AfterActionMethod")]
        public void OnAfterActionMethod()
        {
            _onActionMethodScope?.Value?.Complete();
        }
        #endregion


        #region View
        // https://github.com/aspnet/Mvc/blob/rel/2.0.0/src/Microsoft.AspNetCore.Mvc.Razor/Internal/MvcRazorDiagnosticSourceExtensions.cs
        // https://github.com/aspnet/Mvc/blob/rel/2.0.0/src/Microsoft.AspNetCore.Mvc.Razor/RazorView.cs#L170
        [DiagnosticName("Microsoft.AspNetCore.Mvc.Razor.BeforeViewPage")]
        public void OnBeforeViewPage(IRazorPage page, ViewContext viewContext)
        {
            _onViewPageScope.Value = TimelineScope.Create("Page", TimelineEventCategory.AspNetCoreMvcView, page.Path);
        }

        [DiagnosticName("Microsoft.AspNetCore.Mvc.Razor.AfterViewPage")]
        public void OnAfterViewPage(IRazorPage page, ViewContext viewContext)
        {
            _onViewPageScope?.Value?.Complete();
        }
        #endregion

        #region ActionResult
        // https://github.com/aspnet/Mvc/blob/rel/2.0.0/src/Microsoft.AspNetCore.Mvc.Core/Internal/ResourceInvoker.cs#L133
        [DiagnosticName("Microsoft.AspNetCore.Mvc.BeforeActionResult")]
        public void OnBeforeActionResult(Microsoft.AspNetCore.Mvc.ActionContext actionContext, Microsoft.AspNetCore.Mvc.IActionResult result)
        {
            _onActionResultScope.Value = TimelineScope.Create("ActionResult", TimelineEventCategory.AspNetCoreMvcResult);
        }

        [DiagnosticName("Microsoft.AspNetCore.Mvc.AfterActionResult")]
        public void OnAfterActionResult()
        {
            _onActionResultScope?.Value?.Complete();
        }
        #endregion
    }
}
