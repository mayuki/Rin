namespace Rin.Core.Record
{
    /// <summary>
    /// Pre-defined TimelineScope categories
    /// </summary>
    public static class TimelineEventCategory
    {
        internal const string Root = "Rin.Timeline.Root";

        /// <summary>
        /// Method Call events.
        /// </summary>
        public const string Method = "Rin.Timeline.Method";

        /// <summary>
        /// Data access events.
        /// </summary>
        public const string Data = "Rin.Timeline.Data";

        /// <summary>
        /// Log/Trace events.
        /// </summary>
        public const string Trace = "Rin.Timeline.Trace";

        /// <summary>
        /// ASP.NET Core common events.
        /// </summary>
        public const string AspNetCoreCommon = "Rin.Timeline.AspNetCore.Common";

        /// <summary>
        /// ASP.NET MVC Core view events.
        /// </summary>
        public const string AspNetCoreMvcView = "Rin.Timeline.AspNetCore.Mvc.View";

        /// <summary>
        /// ASP.NET MVC Core result events.
        /// </summary>
        public const string AspNetCoreMvcResult = "Rin.Timeline.AspNetCore.Mvc.Result";

        /// <summary>
        /// ASP.NET MVC Core action events.
        /// </summary>
        public const string AspNetCoreMvcAction = "Rin.Timeline.AspNetCore.Mvc.Action";
    }
}
