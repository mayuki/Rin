using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Rin.Core.Record;

namespace Rin.Extensions.EntityFrameworkCore
{
    // https://github.com/dotnet/efcore/blob/release/3.1/src/EFCore.Relational/Diagnostics/RelationalEventId.cs
    // https://github.com/dotnet/efcore/blob/release/3.1/src/EFCore.Relational/Diagnostics/RelationalLoggerExtensions.cs
    public class EntityFrameworkCoreRelationalDiagnosticsListener : IObserver<KeyValuePair<string, object>>
    {
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            if (value.Key == "Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted" && value.Value is CommandExecutedEventData commandExecutedEventData)
            {
                var scope = TimelineScope.Create("CommandExecuted", TimelineEventCategory.Data, commandExecutedEventData.Command.CommandText);
                scope.Complete();
                scope.Duration = commandExecutedEventData.Duration;
            }
        }
    }
}
