using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rin.Core.Event
{
    public class MessageEventBus<T> : IMessageEventBus<T> where T: IMessage
    {
        private IMessageSubscriber<T>[] _subscribers;
        private System.Threading.Channels.Channel<T> _channel;
        private Task _readerTask;
        private bool _disposed;
        private CancellationTokenSource _cancellationTokenSource;

        public MessageEventBus(IMessageSubscriber<T>[] subscribers)
        {
            _subscribers = subscribers;
            _channel = System.Threading.Channels.Channel.CreateUnbounded<T>(new System.Threading.Channels.UnboundedChannelOptions()
            {
                SingleReader = true,
            });
            _readerTask = Task.Run(RunLoopAsync);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private async Task RunLoopAsync()
        {
            var reader = _channel.Reader;
            while (await reader.WaitToReadAsync(_cancellationTokenSource.Token) && !_cancellationTokenSource.IsCancellationRequested)
            {
                var item = await reader.ReadAsync(_cancellationTokenSource.Token);

                foreach (var subscriber in _subscribers)
                {
                    subscriber.Publish(item);
                }
            }
        }

        public async Task PostAsync(T item)
        {
            await _channel.Writer.WaitToWriteAsync(_cancellationTokenSource.Token);
            await _channel.Writer.WriteAsync(item, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _cancellationTokenSource.Cancel();
            _readerTask.Wait();

            _disposed = true;
        }
    }
}
