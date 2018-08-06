using System;
using System.Collections.Generic;

namespace Rin.Core.Record
{
    public interface ITimelineScope : IDisposable
    {
        string Name { get; }
        string Category { get; }
        string Data { get; }
        IReadOnlyCollection<TimelineScope> Children { get; }
        void Complete();
    }
}
