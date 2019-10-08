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
        private static readonly AsyncLocal<IRinRequestRecordingFeature> _current = new AsyncLocal<IRinRequestRecordingFeature>();

        public IRinRequestRecordingFeature Feature
        {
            get => _current.Value;
            set => _current.Value = value;
        }

        public void SetValue(IRinRequestRecordingFeature feature)
        {
            _current.Value = feature;
        }
    }
}
