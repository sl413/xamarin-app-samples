using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Text;
using Android.Webkit;
using Android.Widget;
using Downloader.Services;
using DownloaderLibrary;
using DownloaderLibrary.Services.Interfaces;
using DownloaderLibrary.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Xamarin.Essentials;
using FileProvider = Android.Support.V4.Content.FileProvider;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Downloader
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Switch authSwitch;
        private Button downloadButton;
        private DownloadViewModel downloadViewModel;
        private EditText loginEditText;
        private Button openFileButton;
        private EditText passwordEditText;
        private ProgressBar progressBar;
        private EditText urlEditText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bootstrap.Instance.Setup();
            Platform.Init(this, savedInstanceState);
            SimpleIoc.Default.Register<IFileService, FileService>();
            downloadViewModel = SimpleIoc.Default.GetInstance<DownloadViewModel>();
            downloadViewModel.ProgressChanged += OnProgressChanged;
            SetContentView(Resource.Layout.activity_main);
            SetupViews();
        }

        private void SetupViews()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            downloadButton = FindViewById<Button>(Resource.Id.download_button);
            openFileButton = FindViewById<Button>(Resource.Id.open_file_button);
            authSwitch = FindViewById<Switch>(Resource.Id.auth_switch);
            urlEditText = FindViewById<EditText>(Resource.Id.url_edit_text);
            loginEditText = FindViewById<EditText>(Resource.Id.login_edit_text);
            passwordEditText = FindViewById<EditText>(Resource.Id.password_edit_text);
            DisableAuthEditTexts();
            authSwitch.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                if (e.IsChecked)
                    ActivateAuthEditText();
                else
                    DisableAuthEditTexts();
            };
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            openFileButton.Enabled = false;
            downloadButton.Click += DownloadButtonOnClick;
            openFileButton.Click += OpenFileButtonOnClick;
        }

        private void ActivateAuthEditText()
        {
            ToggleEditText(loginEditText, true);
            ToggleEditText(passwordEditText, true);
        }

        private void DisableAuthEditTexts()
        {
            ToggleEditText(loginEditText, false);
            ToggleEditText(passwordEditText, false);
        }

        private static void ToggleEditText(TextView editText, bool activate)
        {
            editText.Enabled = activate;
            editText.Focusable = activate;
            editText.FocusableInTouchMode = activate;
            editText.InputType = activate ? InputTypes.ClassText : InputTypes.Null;
        }


        private void OnProgressChanged(object sender, DownloadProgressEventArgs e)
        {
            RunOnUiThread(() => { progressBar.SetProgress(e.Progress, true); });
        }

        private void DownloadButtonOnClick(object sender, EventArgs eventArgs)
        {
            Toast.MakeText(this, "File downloading started", ToastLength.Short).Show();
            if (authSwitch.Checked) downloadViewModel.SetAuthData(loginEditText.Text, passwordEditText.Text);
            downloadViewModel.StartDownloadAsync(urlEditText.Text).ContinueWith(task => { OnFileDownloaded(); });
        }

        private void OnFileDownloaded()
        {
            RunOnUiThread(() => { openFileButton.Enabled = true; };
            Toast.MakeText(this, "File downloaded", ToastLength.Short).Show();
            // Notify the user about the completed "download"
            var downloadManager = DownloadManager.FromContext(Application.Context);
            var filePath = downloadViewModel.GetDownloadedFilePath();
            downloadManager.AddCompletedDownload(filePath, "myDescription", true, 
                GetFileType(filePath), filePath, File.ReadAllBytes(filePath).Length, true);
        }

        private void OpenFileButtonOnClick(object sender, EventArgs eventArgs)
        {
            var filePath = new Java.IO.File(downloadViewModel.GetDownloadedFilePath());
            //todo: change provider name
            var contentUri = FileProvider.GetUriForFile(this, "121212.provider", filePath);
            var mime = GetFileType(filePath.Path);
            var intent = new Intent();
            intent.AddFlags(ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
            intent.SetAction(Intent.ActionView);
            intent.SetDataAndType(contentUri, mime);
            StartActivity(intent);
        }

        private static string GetFileType(string url)
        {
            string type = null;
            var extension = MimeTypeMap.GetFileExtensionFromUrl(url);
            if (extension == null) return type;
            var mime = MimeTypeMap.Singleton;
            type = mime.GetMimeTypeFromExtension(extension);

            return type;
        }
    }
}