using System;
using System.Collections.Concurrent;

namespace CodeFirstConfig
{
    public static class AppFinalizator
    {
        public static ConcurrentQueue<IDisposable> CleanupQueue { get; private set; }

        static AppFinalizator()
        {
            CleanupQueue = new ConcurrentQueue<IDisposable>();            
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => PerformCleanup();
        }

        public static void PerformCleanup()
        {
            lock (CleanupQueue)
            {
                if (CleanupQueue.IsEmpty) return;
                IDisposable value;
                while (CleanupQueue.TryDequeue(out value))
                {
                    value.Dispose();
                }
            }            
        }
    }
}
