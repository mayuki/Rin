using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Rin.Core.Record
{
    [DebuggerDisplay("TimelineScope: {Name,nq}")]
    public class TimelineScope : ITimelineScopeCreatable
    {
        internal static readonly AsyncLocal<TimelineScope> CurrentScope = new AsyncLocal<TimelineScope>();

        private readonly Lazy<ConcurrentQueue<ITimelineEvent>> _children;
        private readonly TimelineScope _parent;

        private bool _completed;
        private string _name;
        private string _category;

        public string EventType => nameof(TimelineScope);
        public DateTimeOffset Timestamp { get; set; }
        public TimeSpan Duration { get; set; }

        public string Name
        {
            get => _name;
            set => _name = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Category
        {
            get => _category;
            set => _category = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Data { get; set; }

        public IReadOnlyCollection<ITimelineEvent> Children => _children.IsValueCreated ? _children.Value : (IReadOnlyCollection<ITimelineEvent>)Array.Empty<ITimelineEvent>();

        /// <summary>
        /// Prepare a TimelineScope for current ExecutionContext (async execution flow).
        /// </summary>
        /// <returns></returns>
        public static TimelineScope Prepare()
        {
            if (CurrentScope.Value != null) throw new InvalidOperationException("TimelineScope is already prepared in current execution.");
            CurrentScope.Value = new TimelineScope("Root", TimelineEventCategory.Root, null);
            return CurrentScope.Value;
        }

        private TimelineScope(string name, string category, string data)
        {
            Timestamp = DateTimeOffset.Now;
            Category = category;
            Name = name;
            Data = data;

            _parent = CurrentScope.Value;
            _children = new Lazy<ConcurrentQueue<ITimelineEvent>>(() => new ConcurrentQueue<ITimelineEvent>(), LazyThreadSafetyMode.PublicationOnly);

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
        public static ITimelineScope Create([CallerMemberName]string name = "", string category = TimelineEventCategory.Method, string data = null)
        {
            if (CurrentScope.Value == null) return NullTimelineScope.Instance;

            return ((ITimelineScopeCreatable)CurrentScope.Value).Create(name, category, data);
        }

        ITimelineScope ITimelineScopeCreatable.Create(string name, string category, string data)
        {
            return new TimelineScope(name, category, data);
        }

        internal void AddChild(ITimelineEvent timelineEvent)
        {
            _children.Value.Enqueue(timelineEvent);
        }

        public void Complete()
        {
            if (_completed) return;

            _completed = true;
            Duration = DateTimeOffset.Now - Timestamp;
            CurrentScope.Value = _parent;
        }

        void IDisposable.Dispose()
        {
            Complete();
        }
    }
}
