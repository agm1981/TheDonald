using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DonaldTracker
{
    class Program
    {
        static ManualResetEvent completedEvent = new ManualResetEvent(false);
        static void Main(string[] args)
        {
            TimerHandler.StartProcess();
            completedEvent.WaitOne();
        }
    }
}
