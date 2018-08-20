using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Rin.Core.Record
{
    [DebuggerDisplay("TimelineStamp: {Name}")]
    public class TimelineStamp : ITimelineEvent
    {
        private string _name;
        private string _category;

        public string EventType => nameof(TimelineStamp);

        public string Name
        {
            get => _name;
            set => SetValue(ref _name, value);
        }

        public string Category
        {
            get => _category;
            set => SetValue(ref _category, value);
        }

        public string Data { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        private TimelineStamp(string name, string category, string data, DateTimeOffset timestamp)
        {
            Name = name;
            Category = category;
            Data = data;
            Timestamp = timestamp;
        }

        private void SetValue<T>(ref T field, T value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            field = value;
        }

        /// <summary>
        /// Stamp a event point in the timeline.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <param name="data"></param>
        /// <param name="timestamp"></param>
        public static void Stamp(string name, string category, string data = null, DateTimeOffset? timestamp = null)
        {
            if (TimelineScope.CurrentScope.Value == null) return;

            var stamp = new TimelineStamp(name, category, data, timestamp ?? DateTimeOffset.Now);
            TimelineScope.CurrentScope.Value.AddChild(stamp);
        }
    }
}
