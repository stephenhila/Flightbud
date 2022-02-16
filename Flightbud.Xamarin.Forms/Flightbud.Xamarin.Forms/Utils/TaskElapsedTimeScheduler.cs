using Flightbud.Xamarin.Forms.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Flightbud.Xamarin.Forms.Utils
{
    public class TaskElapsedTimeScheduler
    {
        Stopwatch _stopwatch;

        public TaskElapsedTimeScheduler()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public void StartTask(Func<Task> callback, int elapsedMillisecondsToRestart)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(Constants.ELAPSED_TIME_SCHEDULER_FREQUENCY_MILISECONDS),
                () =>
                {
                    Task.Run(async () =>
                    {
                        if (_stopwatch.ElapsedMilliseconds > elapsedMillisecondsToRestart)
                        {
                            
                            _stopwatch.Stop();
                            _stopwatch.Reset();
                            await callback();
                            _stopwatch.Start();
                        }
                    });

                    return true;
                });
        }
    }
}
