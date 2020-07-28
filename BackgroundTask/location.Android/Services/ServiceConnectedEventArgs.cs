using System;
using Android.OS;

namespace BackgroundCounter.Droid.Services
{
    public class ServiceConnectedEventArgs : EventArgs
    {
        public IBinder Binder { get; set; }
    }
}
