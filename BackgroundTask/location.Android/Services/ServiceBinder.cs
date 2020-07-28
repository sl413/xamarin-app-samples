using Android.OS;

namespace BackgroundCounter.Droid.Services
{
    public class ServiceBinder : Binder
    {
        protected Service service;

        // constructor
        public ServiceBinder(Service service)
        {
            this.service = service;
        }

        public Service Service => service;

        public bool IsBound { get; set; }
    }
}