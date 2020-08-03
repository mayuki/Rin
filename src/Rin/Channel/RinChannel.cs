using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rin.Core;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rin.Channel
{
    public class RinChannel : IDisposable
    {
        private static readonly Encoding _encoding = new UTF8Encoding(false);
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ILogger _logger;

        public Dictionary<Type, ConcurrentDictionary<string, WebSocket>> ConnectionsByHub { get; } = new Dictionary<Type, ConcurrentDictionary<string, WebSocket>>();
        public ConcurrentDictionary<string, Tuple<Type, WebSocket>> Connections { get; } = new ConcurrentDictionary<string, Tuple<Type, WebSocket>>();

        public RinChannel(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RinChannel>();
        }

        public void Dispose()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public async Task ManageAsync(WebSocket socket, IHub hub)
        {
            var typeOfHub = hub.GetType();
            var connectionId = Guid.NewGuid().ToString();

            lock (ConnectionsByHub)
            {
                if (!ConnectionsByHub.TryGetValue(hub.GetType(), out var connectionsByHub))
                {
                    connectionsByHub = new ConcurrentDictionary<string, WebSocket>();
                    ConnectionsByHub[typeOfHub] = connectionsByHub;
                }
                connectionsByHub[connectionId] = socket;
            }
            Connections[connectionId] = Tuple.Create(typeOfHub, socket);

            var buffer = ArrayPool<byte>.Shared.Rent(1024);
            try
            {
                var establishConnectionAsync = typeof(RinChannel).GetMethod(nameof(EstablishConnectionAsync), BindingFlags.Instance | BindingFlags.NonPublic)!;
                var task = (Task)establishConnectionAsync.MakeGenericMethod(typeOfHub).Invoke(this, new object[] { socket, hub, connectionId, buffer })!;
                await task;
            }
            catch (TaskCanceledException) { }
            catch (WebSocketException) { }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private async Task EstablishConnectionAsync<THub>(WebSocket socket, THub hub, string connectionId, byte[] buffer)
            where THub : IHub
        {
            var memoryStream = new MemoryStream();
            var result = await socket.ReceiveAsync(buffer, _cancellationTokenSource.Token);

            while (!result.CloseStatus.HasValue && !_cancellationTokenSource.IsCancellationRequested)
            {
                if (result.Count != 0)
                {
                    memoryStream.Write(buffer, 0, result.Count);
                }
                if (result.EndOfMessage)
                {
                    var messageString = _encoding.GetString(memoryStream.ToArray());
                    memoryStream.SetLength(0);

                    try
                    {
                        var operation = JsonConvert.DeserializeAnonymousType(messageString, new { M = "", O = "", A = default(JToken[]) });
                        if (HubDispatcher<THub>.CanInvoke(operation.M))
                        {
                            try
                            {
                                var methodResult = await HubDispatcher<THub>.InvokeAsync(operation.M, hub, operation.A);
                                await SendResponseAsync(connectionId, operation.O, methodResult);
                            }
                            catch (Exception ex)
                            {
                                await SendResponseAsync(connectionId, operation.O, new { E = ex.GetType().Name, Detail = ex });
                                _logger.LogError(ex, "Exception was thrown until invoking a hub method: Method = {0}; Hub = {1}", operation.M, typeof(THub));
                            }
                        }
                        else
                        {
                            await SendResponseAsync(connectionId, operation.O, new { E = "MethodNotFound" });
                            _logger.LogWarning("Method not found: Method = {0}; Hub = {1}", operation.M, typeof(THub));
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        throw;
                    }
                    catch (WebSocketException)
                    {
                        throw;
                    }
                    catch
                    { }
                }

                result = await socket.ReceiveAsync(buffer, _cancellationTokenSource.Token);
            }

            if (_cancellationTokenSource.IsCancellationRequested) return;

            await socket.CloseAsync(result.CloseStatus ?? WebSocketCloseStatus.NormalClosure, result.CloseStatusDescription, _cancellationTokenSource.Token);
        }

        public TClient GetClient<THub, TClient>()
            where THub : IHub
            where TClient : IHubClient
        {
            return HubClientProxy<THub, TClient>.Create(this);
        }

        internal async Task SendAsync(string connectionId, object payload)
        {
            var message = JsonConvert.SerializeObject(payload);

            if (Connections.TryGetValue(connectionId, out var conn))
            {
                try
                {
                    await conn.Item2.SendAsync(_encoding.GetBytes(message), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
                }
                catch
                {
                    if (Connections.TryRemove(connectionId, out var item))
                    {
                        ConnectionsByHub[item.Item1].TryRemove(connectionId, out var _);
                    }
                }
            }
        }
        private Task SendAsync<THub>(object payload)
        {
            if (ConnectionsByHub.TryGetValue(typeof(THub), out var connectionsByTHub))
            {
                return Task.WhenAll(connectionsByTHub.Select(x => SendAsync(x.Key, payload)));
            }

            return Task.CompletedTask;
        }

        private Task SendResponseAsync(string connectionId, string operationId, object methodResult)
        {
            return SendAsync(connectionId, new { R = operationId, V = methodResult });
        }

        public Task InvokeAsync<THub>(string methodName, object? args = null)
        {
            return SendAsync<THub>(new { M = methodName, A = args });
        }
    }
}
