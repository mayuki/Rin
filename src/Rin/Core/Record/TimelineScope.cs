using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Rin.Core.Record
{
    [DebuggerDisplay("TimelineScope: {Name}")]
    public class TimelineScope : ITimelineScope
    {
        private static readonly AsyncLocal<TimelineScope> CurrentScope = new AsyncLocal<TimelineScope>();

        private Lazy<ConcurrentQueue<TimelineScope>> _children { get; } = new Lazy<ConcurrentQueue<TimelineScope>>(() => new ConcurrentQueue<TimelineScope>(), LazyThreadSafetyMode.PublicationOnly);
        private TimelineScope _parent { get; }

        public DateTime BeginTime { get; private set; }
        public TimeSpan Duration { get; private set; }

        public string Name { get; }
        public string Category { get; }
        public string Data { get; set; }

        public IReadOnlyCollection<TimelineScope> Children => _children.IsValueCreated ? _children.Value : (IReadOnlyCollection<TimelineScope>)Array.Empty<TimelineScope>();

        /// <summary>
        /// Prepare a TimelineScope for current ExecutionContext (async execution flow).
        /// </summary>
        /// <returns></returns>
        internal static TimelineScope Prepare()
        {
            CurrentScope.Value = new TimelineScope("Root", TimelineScopeCategory.Root, null);
            return CurrentScope.Value;
        }

        private TimelineScope(string name, string category, string data)
        {
            BeginTime = DateTime.Now;
            Category = category;
            Name = name;
            Data = data;
            _parent = CurrentScope.Value;

            if (_parent != null)
            {
                _parent.AddChild(this);
            }

            CurrentScope.Value = this;
        }

        /// <summary>
        /// Create a instance of TimelineScope. When Rin is disabled on production environment, this method returns immutable NullScope.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ITimelineScope Create([CallerMemberName]string name = "", string category = TimelineScopeCategory.Method, string data = null)
        {
            if (CurrentScope.Value == null) return NullTimelineScope.Instance;

            return ((ITimelineScope)CurrentScope.Value).Create(name, category, data);
        }

        ITimelineScope ITimelineScope.Create(string name, string category, string data)
        {
            return new TimelineScope(name, category, data);
        }

        private void AddChild(TimelineScope s)
        {
            _children.Value.Enqueue(s);
        }

        public void Complete()
        {
            if (CurrentScope.Value != this) return;

            Duration = DateTime.Now - BeginTime;
            CurrentScope.Value = _parent;
        }

        void IDisposable.Dispose()
        {
            Complete();
        }
    }

    /// <summary>
    /// Pre-defined TimelineScope categories
    /// </summary>
    public static class TimelineScopeCategory
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
