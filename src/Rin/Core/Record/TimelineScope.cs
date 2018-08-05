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

        public IReadOnlyCollection<TimelineScope> Children => _children.IsValueCreated ? _children.Value : (IReadOnlyCollection<TimelineScope>)Array.Empty<TimelineScope>();

        public static TimelineScope Prepare()
        {
            CurrentScope.Value = new TimelineScope("Root");
            return CurrentScope.Value;
        }

        public TimelineScope([CallerMemberName]string name = "")
        {
            BeginTime = DateTime.Now;
            Name = name;
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

        public void Dispose()
        {
            Duration = DateTime.Now - BeginTime;
            CurrentScope.Value = Parent;
        }
    }
}
