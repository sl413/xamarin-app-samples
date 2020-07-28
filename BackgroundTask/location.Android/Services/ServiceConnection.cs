using System;
using Android.Content;
using Android.OS;
using Object = Java.Lang.Object;

namespace BackgroundCounter.Droid.Services
{
    public class ServiceConnection : Object, IServiceConnection
    {
        public ServiceConnection(ServiceBinder binder)
        {
            if (binder != null) Binder = binder;
        }

        public ServiceBinder Binder { get; set; }

        // This gets called when a client tries to bind to the Service with an Intent and an 
        // instance of the ServiceConnection. The system will locate a binder associated with the 
        // running Service 
        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            if (service is ServiceBinder serviceBinder)
            {
                Binder = serviceBinder;
                Binder.IsBound = true;
                ServiceConnected(this, new ServiceConnectedEventArgs {Binder = service});
            }
        }

        // This will be called when the Service unbinds, or when the app crashes
        public void OnServiceDisconnected(ComponentName name)
        {
            Binder.IsBound = false;
        }

        public event EventHandler<ServiceConnectedEventArgs> ServiceConnected = delegate { };
    }
}