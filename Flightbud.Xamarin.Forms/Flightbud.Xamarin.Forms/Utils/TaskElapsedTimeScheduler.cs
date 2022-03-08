using Flightbud.Xamarin.Forms.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Flightbud.Xamarin.Forms.Utils
{
    public enum TaskElapsedTimeSchedulerBehavior
    {
        TriggerOnce,
        Recurring
    }

    /// <summary>
    /// Utility object that starts a callback function after a defined elapsed millisecond time.
    /// </summary>
    public class TaskElapsedTimeScheduler
    {
        TaskElapsedTimeSchedulerBehavior _behavior;

        Stopwatch _stopwatch;
        CancellationTokenSource _cancellationTokenSource;
        Func<Task> _callback;
        int _elapsedMillisecondsToStart;

        public TaskElapsedTimeScheduler(Func<Task> callback,
            int elapsedMillisecondsToStart,
            TaskElapsedTimeSchedulerBehavior behavior = TaskElapsedTimeSchedulerBehavior.TriggerOnce)
        {
            _callback = callback;
            _elapsedMillisecondsToStart = elapsedMillisecondsToStart;
            _behavior = behavior;

            _stopwatch = new Stopwatch();

            _cancellationTokenSource = new CancellationTokenSource();
        }

        bool _isRunning;
        public void Start()
        {
            _stopwatch.Start();

            _isRunning = true;

            Device.StartTimer(TimeSpan.FromMilliseconds(Constants.ELAPSED_TIME_SCHEDULER_FREQUENCY_MILLISECONDS),
                () =>
                {
                    Task.Run(async () =>
                    {
                        if (_stopwatch.ElapsedMilliseconds > _elapsedMillisecondsToStart)
                        {
                            try
                            {
                                _stopwatch.Reset();
                                await _callback();

                                if (_behavior == TaskElapsedTimeSchedulerBehavior.Recurring)
                                {
                                    Start();
                                }
                            }
                            catch (OperationCanceledException oce)
                            {
                                _isRunning = false;
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }, _cancellationTokenSource.Token);

                    return _isRunning;
                });
        }

        public void Restart()
        {
            _cancellationTokenSource.Cancel(true);
            _cancellationTokenSource = new CancellationTokenSource();
            Start();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel(true);
        }
    }
}
