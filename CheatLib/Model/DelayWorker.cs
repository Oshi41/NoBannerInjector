using System;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace CheatLib
{
    public class DelayWorker
    {
        private static Timer _timer = new Timer(100);
        private readonly Func<Task> _action;
        private DateTime? _next;
        private bool _executing;

        static DelayWorker()
        {
            _timer.Start();
        }

        public DelayWorker(Func<Task> action)
        {
            _action = action;
            _timer.Elapsed += async (sender, args) =>
            {
                if (!_executing && _action != null && _next != null && DateTime.Now >= _next)
                {
                    _next = null;
                    _executing = true;
                    var task = _action?.Invoke();
                    task.Start();
                    await task;
                    _executing = false;
                }
            };
        }

        public DelayWorker(Action action)
            : this(() => new Task(action))
        {
        }

        public void Schedule(TimeSpan span)
        {
            _next = DateTime.Now + span;
        }
    }
}