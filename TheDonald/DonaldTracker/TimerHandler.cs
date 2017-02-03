using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Tweetinvi.Models;

namespace DonaldTracker
{
    public static class TimerHandler
    {
        private static Timer timer;

        static TimerHandler()
        {
            timer = new Timer();
        }

        public static void StartProcess()
        {
            timer.Elapsed += TimerOnElapsed;
            timer.Interval = new TimeSpan(0, 1, 0).TotalMilliseconds;
            timer.Start();
        }

        private static void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            try
            {
                timer.Stop();
                TweeterWorker ts = new TweeterWorker();
                ts.GetAndPublish();
               
            }
            catch (Exception ex)
            {
                File.AppendAllText("errorlog.txt", ex.Message);
            }
            finally
            {
                timer.Start();
            }
        }
    }
}
