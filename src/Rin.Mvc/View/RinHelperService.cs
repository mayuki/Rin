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
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="scriptPath"></param>
        /// <returns></returns>
        public HtmlString Render(OnViewInspectorPosition position = OnViewInspectorPosition.Top, string scriptPath = null)
        {
            var feature = _httpContextAccessor.HttpContext.Features.Get<Rin.Features.IRinRequestRecordingFeature>();
            if (feature == null) return HtmlString.Empty;

            var pathBase = _rinOptions.Inspector.MountPath;
            var requestId = feature.Record.Id;

            return new HtmlString(
                $"<div id=\"rinOnViewInspectorRoot\" data-rin-on-view-inspector-config='{{ \"Position\": \"{position.ToString()}\", \"PathBase\": \"{pathBase}\", \"RequestId\": \"{requestId}\" }}'></div>" +
                $"<script src=\"{(scriptPath ?? (pathBase + Constants.MvcStaticResourcesRoot + "main.js"))}\"></script>"
            );
        }
    }

    public enum OnViewInspectorPosition
    {
        Top,
        Bottom
    }
}
