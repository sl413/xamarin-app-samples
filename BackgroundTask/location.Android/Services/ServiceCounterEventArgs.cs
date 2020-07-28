using System;

namespace BackgroundCounter.Droid.Services
{
    public class ServiceCounterEventArgs : EventArgs
    {
        public int Counter { get; set; }
    }
}