using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rin.Features
{
    public interface IRinRequestRecordingFeatureAccessor
    {
        IRinRequestRecordingFeature Feature { get; }

        void SetValue(IRinRequestRecordingFeature feature);
    }

    // NOTE: When Rin is integrated with non-ASP.NET Core application. We can't use HttpContext (or HttpContextAccessor).
    // Currently, Rin depends IRinRequestRecordingFeature, so we use this instead of HttpContext at this time.
    public class RinRequestRecordingFeatureAccessor : IRinRequestRecordingFeatureAccessor
    {
        private static readonly AsyncLocal<Holder> _current = new AsyncLocal<Holder>();

        public IRinRequestRecordingFeature Feature => _current.Value?.Value;

        public void SetValue(IRinRequestRecordingFeature feature)
        {
            if (_current.Value == null)
            {
                _current.Value = new Holder();
            }

            _current.Value.Value = feature;
        }

        private class Holder
        {
            public IRinRequestRecordingFeature Value { get; set; }
        }
    }
}
