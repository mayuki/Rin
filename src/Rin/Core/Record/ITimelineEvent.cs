using System;

namespace Rin.Core.Record
{
    public interface ITimelineEvent
    {
        /// <summary>
        /// Timeline event type name.
        /// </summary>
        string EventType { get; }

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
        string? Data { get; set; }

        /// <summary>
        /// Timestamp of operation started at.
        /// </summary>
        DateTimeOffset Timestamp { get; set; }
    }
}
