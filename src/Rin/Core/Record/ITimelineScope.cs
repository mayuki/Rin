using System;
using System.Collections.Generic;

namespace Rin.Core.Record
{
    public interface ITimelineScope : ITimelineEvent, IDisposable
    {
        /// <summary>
        /// Duration of operation process.
        /// </summary>
        TimeSpan Duration { get; set; }

        /// <summary>
        /// Child operations.
        /// </summary>
        IReadOnlyCollection<ITimelineEvent> Children { get; }

        /// <summary>
        /// Mark the timeline scope as completed.
        /// </summary>
        void Complete();
    }

    public interface ITimelineScopeCreatable : ITimelineScope
    {
        /// <summary>
        /// Create a new TimelineScope as a child.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        ITimelineScope Create(string name, string category, string? data);
    }
}
