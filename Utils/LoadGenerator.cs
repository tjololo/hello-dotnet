using System.Diagnostics;

namespace hello_dotnet.Utils
{
    public static class LoadGenerator
    {
        public static float ConsumeCpu(int percentage, int secondsToRun)
        {

            float counter = 0;
            if (secondsToRun <= 0)
            {
                return counter;
            }
            if (percentage < 0 || percentage > 100 || secondsToRun > 30)
                throw new ArgumentException("percentage/time");
            Stopwatch loadWatch = new Stopwatch();
            loadWatch.Start();
            Stopwatch runWatch = new Stopwatch();
            runWatch.Start();
            while (true)
            {
                // Make the loop go on for "percentage" milliseconds then sleep the 
                // remaining percentage milliseconds. So 40% utilization means work 40ms and sleep 60ms
                counter++;
                if (loadWatch.ElapsedMilliseconds > percentage)
                {
                    Thread.Sleep(100 - percentage);
                    loadWatch.Reset();
                    loadWatch.Start();
                }
                if (runWatch.ElapsedMilliseconds / 1000 > secondsToRun)
                    break;
            }
            return counter;
        }
    }
}