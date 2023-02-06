using System;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace CheatLib
{
    public class DelayWorker
    {
        private static Timer _timer = new Timer(100);
        private readonly Action _action;
        private DateTime? _next;

        static DelayWorker()
        {
            _timer.Start();
        }

        public DelayWorker(Action action)
        {
            _action = action;
            _timer.Elapsed += (sender, args) =>
            {
                if (_action != null && _next != null && DateTime.Now >= _next)
                {
                    _next = null;
                    _action?.Invoke();
                }
            };
        }

        public void Schedule(TimeSpan span)
        {
            _next = DateTime.Now + span;
        }
    }
}