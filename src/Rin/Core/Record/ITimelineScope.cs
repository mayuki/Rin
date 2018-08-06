using System;
using System.Collections.Generic;

namespace Rin.Core.Record
{
    public interface ITimelineScope : IDisposable
    {
        string Name { get; }
        string Category { get; }
        string Data { get; }
        DateTime BeginTime { get; }
        TimeSpan Duration { get; }
        IReadOnlyCollection<TimelineScope> Children { get; }
        void Complete();
        ITimelineScope Create(string name, string category, string data);
    }
}
