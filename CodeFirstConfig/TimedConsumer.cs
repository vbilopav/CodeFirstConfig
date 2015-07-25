using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CodeFirstConfig
{
    public class TimedConsumer : IDisposable
    {
        protected Action ConsumerAction;
        protected Timer Timer;
        protected readonly TaskFactory Factory;
        protected readonly int PeriodMilliseconds;
        protected ManualResetEvent TimerDisposed;

        public TimedConsumer(int maxDegreeOfParallelism, int periodMilliseconds)
        {
            PeriodMilliseconds = periodMilliseconds;
            var token = new CancellationToken();
            Factory = new TaskFactory(
                token,
                TaskCreationOptions.PreferFairness,
                TaskContinuationOptions.None,
                new LimitedConcurrencyLevelTaskScheduler(maxDegreeOfParallelism));
            Timer = null;
        }

        public void Start(Action consumerAction)
        {
            ConsumerAction = consumerAction;
            if (Timer == null)
            {
                Timer = new Timer(state => Factory.StartNew(() =>
                {
                    ConsumerAction();
                    Timer.Change(PeriodMilliseconds, Timeout.Infinite);
                }), null, PeriodMilliseconds, Timeout.Infinite);
                TimerDisposed = new ManualResetEvent(false);
            }
            else
                throw new ArgumentException("TimedConsumer already started!");
        }

        public void Dispose()
        {
            if (Timer == null) return;
            Timer.Dispose(TimerDisposed);
            TimerDisposed.WaitOne();
            TimerDisposed.Dispose();
        }
    }

    public class TimedConsumer<TModel> : TimedConsumer
    {                       
        public ConcurrentQueue<TModel> Queue { get; private set; }

        public TimedConsumer(int maxDegreeOfParallelism, int periodMilliseconds)
            : base(maxDegreeOfParallelism, periodMilliseconds)
        {            
            Queue = new ConcurrentQueue<TModel>();        
        }     
    }
}
