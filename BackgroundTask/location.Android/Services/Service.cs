using System;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.V4.App;

namespace BackgroundCounter.Droid.Services
{
    [Service]
    public class Service : Android.App.Service
    {
        private const int SERVICE_RUNNING_NOTIFICATION_ID = 1337;
        private const string NOTIFICATION_CHANNEL_ID = "xyz.sl413.backgroundcounter.channel";
        private IBinder binder;
        private int counter;
        private Timer timer;

        public override void OnCreate()
        {
            base.OnCreate();
            ResetTimer();
            SetupTimer();
        }

        [Obsolete("deprecated in base class")]
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // Check if device is running Android 8.0 or higher and call StartForeground() if so
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var notification = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID)
                    .SetContentTitle(Resources.GetString(Resource.String.app_name))
                    .SetContentText(Resources.GetString(Resource.String.notification_text))
                    .SetSmallIcon(Resource.Drawable.notification_icon_background)
                    .SetOngoing(true)
                    .Build();

                var notificationManager =
                    GetSystemService(NotificationService) as NotificationManager;

                var chan = new NotificationChannel(NOTIFICATION_CHANNEL_ID, "On-going Notification",
                    NotificationImportance.Min);

                notificationManager.CreateNotificationChannel(chan);

                StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
            }

            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            binder = new ServiceBinder(this);
            return binder;
        }

        public void StopTimer()
        {
            timer.Stop();
        }

        public void StartTimer()
        {
            timer.Start();
        }

        public void ResetTimer()
        {
            counter = 0;
            SaveTimerCounterInShPref(0);
        }

        public event EventHandler<ServiceCounterEventArgs> CounterChanged = delegate { };

        private void SetupTimer()
        {
            timer = new Timer(1000);
            timer.Elapsed += (sender, args) => { SaveTimerCounterInShPref(++counter); };
            timer.AutoReset = true;
            timer.Enabled = false;
        }

        private void OnCounterDataChanged(int count)
        {
            CounterChanged(this, new ServiceCounterEventArgs {Counter = count});
        }

        private void SaveTimerCounterInShPref(int value)
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            var editor = prefs.Edit();
            editor.PutInt("counter", value);
            editor.Apply();
            OnCounterDataChanged(value);
        }
    }
}