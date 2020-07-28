using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using BackgroundCounter.Droid.Services;

namespace BackgroundCounter.Droid
{
    [Activity(Label = "Counter", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout)]
    public class MainActivity : AppCompatActivity
    {
        private readonly string sharedPrefKeyCounter = "counter";
        private TextView counterTextView;
        private Button resetButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            App.StartService();
            SetContentView(Resource.Layout.Main);
            SetupViews();
            App.Current.ServiceConnected += (sender, e) =>
            {
                App.Current.Service.CounterChanged += HandleCounterChanged;
            };
        }

        private void SetupViews()
        {
            counterTextView = FindViewById<TextView>(Resource.Id.counter_text_view);
            resetButton = FindViewById<Button>(Resource.Id.reset_button);
            counterTextView.Text = "Count: 0";
            resetButton.Click += delegate
            {
                if (App.ServiceConnection.Binder == null) return;
                if (App.ServiceConnection.Binder.IsBound) App.Current.Service.ResetTimer();
            };
        }

        private void HandleCounterChanged(object sender, ServiceCounterEventArgs e)
        {
            counterTextView.Text = $"Count: {e.Counter}";
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (App.ServiceConnection.Binder == null) return;
            if (!App.ServiceConnection.Binder.IsBound) return;
            App.Current.Service.StopTimer();
        }

        protected override void OnPause()
        {
            base.OnPause();
            App.Current.Service.StartTimer();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            App.StopLocationService();
        }
    }
}