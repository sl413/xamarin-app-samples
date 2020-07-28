using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using BackgroundCounter.Droid.Services;
using Service = BackgroundCounter.Droid.Services.Service;

namespace BackgroundCounter.Droid
{
    public class App
    {
        public static ServiceConnection ServiceConnection { get; set; }

        static App()
        {
            Current = new App();
        }

        protected App()
        {
            ServiceConnection = new ServiceConnection(null);
            ServiceConnection.ServiceConnected += (sender, e) => { ServiceConnected(this, e); };
        }

        public static App Current { get; }

        public Service Service
        {
            get
            {
                if (ServiceConnection.Binder == null)
                {
                    throw new Exception("Service not bound yet");
                }

                return ServiceConnection.Binder.Service;
            }
        }

        public event EventHandler<ServiceConnectedEventArgs> ServiceConnected = delegate { };

        public static void StartService()
        {
            new Task(() =>
            {
                // Start our main service
                Log.Debug("App", "Calling StartService");
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    Application.Context.StartForegroundService(new Intent(Application.Context, typeof(Service)));
                }
                else // For older versions, use the traditional StartService() method
                {
                    Application.Context.StartService(new Intent(Application.Context, typeof(Service)));
                }

                var locationServiceIntent = new Intent(Application.Context, typeof(Service));
                Log.Debug("App", "Calling service binding");
                Application.Context.BindService(locationServiceIntent, ServiceConnection, Bind.AutoCreate);
            }).Start();
        }

        public static void StopLocationService()
        {
            if (ServiceConnection != null)
            {
                Application.Context.UnbindService(ServiceConnection);
            }

            Current.Service?.StopSelf();
        }
    }
}