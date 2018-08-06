using System;
using System.Collections.Generic;

namespace Rin.Core.Record
{
    public sealed class NullTimelineScope : ITimelineScope
    {
        public static NullTimelineScope Instance { get; } = new NullTimelineScope();

        public string Name => String.Empty;

        public string Category => String.Empty;

        public string Data => null;

        public IReadOnlyCollection<TimelineScope> Children => Array.Empty<TimelineScope>();

        public DateTime BeginTime => default;

        public TimeSpan Duration => default;

        public void Complete()
        {
        }

        public void Dispose()
        {
        }

        ITimelineScope ITimelineScope.Create(string name, string category, string data) => this;
    }
}
