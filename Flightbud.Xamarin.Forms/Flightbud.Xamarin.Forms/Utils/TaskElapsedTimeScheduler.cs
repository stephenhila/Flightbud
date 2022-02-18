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

        public TaskElapsedTimeScheduler(Func<Task> callback,
            int elapsedMillisecondsToStart,
            TaskElapsedTimeSchedulerBehavior behavior = TaskElapsedTimeSchedulerBehavior.TriggerOnce)
        {
            _callback = callback;
            _behavior = behavior;

            _stopwatch = new Stopwatch();

            _cancellationTokenSource = new CancellationTokenSource();

            Device.StartTimer(TimeSpan.FromMilliseconds(Constants.ELAPSED_TIME_SCHEDULER_FREQUENCY_MILISECONDS),
                () =>
                {
                    Task.Run(async () =>
                    {
                        if (_stopwatch.ElapsedMilliseconds > elapsedMillisecondsToStart)
                        {
                            try
                            {
                                await _callback();
                                _stopwatch.Reset();
                            }
                            catch (OperationCanceledException oce)
                            {
                                _stopwatch.Reset();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }

                            if (_behavior == TaskElapsedTimeSchedulerBehavior.Recurring)
                            {
                                _stopwatch.Start();
                            }
                        }
                    }, _cancellationTokenSource.Token);

                    return true;
                });

        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Restart()
        {
            _cancellationTokenSource.Cancel(true);
            _cancellationTokenSource = new CancellationTokenSource();

            _stopwatch.Start();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel(true);
            _cancellationTokenSource = new CancellationTokenSource();
        }
    }
}
