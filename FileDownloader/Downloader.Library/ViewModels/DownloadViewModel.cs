using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DownloaderLibrary.Services.Interfaces;
using GalaSoft.MvvmLight;

namespace DownloaderLibrary.ViewModels
{
    public class DownloadViewModel : ViewModelBase
    {
        private readonly IDownloadService _downloadService;

        private bool _isDownloading;
        private double _progressValue;

        public DownloadViewModel(IDownloadService downloadService)
        {
            _downloadService = downloadService;
        }

        private double ProgressValue
        {
            get => _progressValue;
            set => Set(ref _progressValue, value);
        }

        private bool IsDownloading
        {
            get => _isDownloading;
            set => Set(ref _isDownloading, value);
        }

        public async Task StartDownloadAsync(string url)
        {
            var progressIndicator = new Progress<double>(ReportProgress);
            var cts = new CancellationTokenSource();
            try
            {
                IsDownloading = true;
                await _downloadService.DownloadFileAsync(url, progressIndicator, cts.Token);
            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                IsDownloading = false;
            }
        }

        /// <summary>
        ///     Reports the progress status for the downlaod.
        /// </summary>
        /// <param name="value">Value.</param>
        private void ReportProgress(double value)
        {
            ProgressValue = value;
            OnThresholdReached(new DownloadProgressEventArgs {Progress = Convert.ToInt32(value)});
        }

        protected virtual void OnThresholdReached(DownloadProgressEventArgs e)
        {
            var handler = ProgressChanged;
            handler?.Invoke(this, e);
        }

        public event EventHandler<DownloadProgressEventArgs> ProgressChanged;


        public string GetDownloadedFilePath()
        {
            return _downloadService.FilePath;
        }

        public void SetAuthData(string userName, string password)
        {
            _downloadService.SetAuthData(userName, password);
        }
    }

    public class DownloadProgressEventArgs : EventArgs
    {
        public int Progress { get; set; }
    }
}