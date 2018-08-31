using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Rin.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rin.Mvc.View
{
    public class RinHelperService
    {
        private IHttpContextAccessor _httpContextAccessor;
        private RinOptions _rinOptions;

        public RinHelperService(IHttpContextAccessor httpContextAccessor, RinOptions options)
        {
            _httpContextAccessor = httpContextAccessor;
            _rinOptions = options;
        }

        /// <summary>
        /// Render in-view Rin inspector scripts.
        /// </summary>
        /// <param name="position">Show position of inspector</param>
        /// <param name="scriptPath">Path for in-view Rin inspector bundle script</param>
        /// <returns></returns>
        public HtmlString RenderInViewInspector(InViewInspectorPosition position = InViewInspectorPosition.Top, string scriptPath = null)
        {
            var feature = _httpContextAccessor.HttpContext.Features.Get<Rin.Features.IRinRequestRecordingFeature>();
            if (feature == null) return HtmlString.Empty;

            var pathBase = _rinOptions.Inspector.MountPath;
            var requestId = feature.Record.Id;

            return new HtmlString(
                $"<script src=\"{(scriptPath ?? (pathBase + Constants.MvcStaticResourcesRoot + "main.js"))}\" data-rin-in-view-inspector-config='{{ \"Position\": \"{position.ToString()}\", \"PathBase\": \"{pathBase}\", \"RequestId\": \"{requestId}\" }}'></script>"
            );
        }
    }

    public enum InViewInspectorPosition
    {
        Top,
        Bottom
    }
}
