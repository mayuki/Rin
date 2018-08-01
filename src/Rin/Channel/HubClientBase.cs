using System.Threading.Tasks;

namespace Rin.Channel
{
    public abstract class HubClientBase<THub, TClient>
    {
        private RinChannel _channel;
        public HubClientBase(RinChannel channel)
        {
            _channel = channel;
        }

        protected Task InvokeAsync(string methodName, object[] args)
        {
            return _channel.InvokeAsync<THub>(methodName, args);
        }
    }

}
