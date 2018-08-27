using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Rin.Features;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rin.Mvc.Filters
{
    public class SetRequestIdActionFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ViewResult)
            {
                var feature = context.HttpContext.Features.Get<IRinRequestRecordingFeature>();
                if (feature == null) return;

                feature.Record.ParentId = null;
                context.HttpContext.Response.Cookies.Append(Constants.CookieName, feature.Record.Id);
            }
        }
    }
}
