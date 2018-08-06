using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Rin.Core.Record
{
    [DebuggerDisplay("TimelineScope: {Name}")]
    public class TimelineScope : IDisposable
    {
        private static readonly AsyncLocal<TimelineScope> CurrentScope = new AsyncLocal<TimelineScope>();

        private Lazy<ConcurrentQueue<TimelineScope>> _children { get; } = new Lazy<ConcurrentQueue<TimelineScope>>(() => new ConcurrentQueue<TimelineScope>(), LazyThreadSafetyMode.PublicationOnly);

        public DateTime BeginTime { get; private set; }
        public TimeSpan Duration { get; private set; }

        public TimelineScope Parent { get; }
        public string Name { get; }
        public string Category { get; }
        public string Data { get; set; }

        public IReadOnlyCollection<TimelineScope> Children => _children.IsValueCreated ? _children.Value : (IReadOnlyCollection<TimelineScope>)Array.Empty<TimelineScope>();

        public static TimelineScope Prepare()
        {
            CurrentScope.Value = new TimelineScope("Root", TimelineScopeCategory.Root);
            return CurrentScope.Value;
        }

        public TimelineScope([CallerMemberName]string name = "", string category = TimelineScopeCategory.Method, string data = null)
        {
            BeginTime = DateTime.Now;
            Category = category;
            Name = name;
            Data = data;
            Parent = CurrentScope.Value;

            if (Parent != null)
            {
                Parent.AddChild(this);
            }

            CurrentScope.Value = this;
        }

        private void AddChild(TimelineScope s)
        {
            _children.Value.Enqueue(s);
        }

        public void Complete()
        {
            if (CurrentScope.Value != this) return;

            Duration = DateTime.Now - BeginTime;
            CurrentScope.Value = Parent;
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
        /// ASP.NET MVC Core action events.
        /// </summary>
        public const string AspNetCoreMvcAction = "Rin.Timeline.AspNetCore.Mvc.Action";
    }
}
