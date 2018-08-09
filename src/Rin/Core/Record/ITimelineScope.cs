using System;
using System.Collections.Generic;

namespace Rin.Core.Record
{
    public interface ITimelineScope : IDisposable
    {
        /// <summary>
        /// Operation name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Operation category.
        /// </summary>
        string Category { get; set; }

        /// <summary>
        /// Custom operation data.
        /// </summary>
        string Data { get; set; }

        /// <summary>
        /// Timestamp of operation started at.
        /// </summary>
        DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Duration of operation process.
        /// </summary>
        TimeSpan Duration { get; set; }

        /// <summary>
        /// Child operations.
        /// </summary>
        IReadOnlyCollection<TimelineScope> Children { get; }

        /// <summary>
        /// Mark the timeline scope as completed.
        /// </summary>
        void Complete();

        /// <summary>
        /// Create a new TimelineScope as a child.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        ITimelineScope Create(string name, string category, string data);
    }
}
