using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rin
{
    internal static class TaskExtensions
    {
        public static void Forget(this Task task)
        {
            task.ContinueWith(_ => { }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
