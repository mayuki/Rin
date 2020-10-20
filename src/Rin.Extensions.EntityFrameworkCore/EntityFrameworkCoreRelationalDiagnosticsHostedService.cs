using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Rin.Extensions.EntityFrameworkCore
{
    public class EntityFrameworkCoreRelationalDiagnosticsHostedService : IObserver<DiagnosticListener>, IHostedService, IDisposable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _disposables.Add(DiagnosticListener.AllListeners.Subscribe(this));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(DiagnosticListener value)
        {
            lock (_disposables)
            {
                if (value.Name == "Microsoft.EntityFrameworkCore")
                {
                    _disposables.Add(value.Subscribe(new EntityFrameworkCoreRelationalDiagnosticsListener()));
                }
            }
        }

        public void Dispose()
        {
            lock (_disposables)
            {
                foreach (var disposable in _disposables)
                {
                    disposable.Dispose();
                }

                _disposables.Clear();
            }
        }
    }
}
