using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Love.Utils
{
    public class AsyncHelper
    {
        private static readonly TaskFactory _myTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return TaskExtensions.Unwrap<TResult>(_myTaskFactory.StartNew<Task<TResult>>(func)).GetAwaiter().GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            TaskExtensions.Unwrap(_myTaskFactory.StartNew<Task>(func)).GetAwaiter().GetResult();
        }
    }
}
