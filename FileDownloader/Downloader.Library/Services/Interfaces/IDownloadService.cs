using System;
using System.Threading;
using System.Threading.Tasks;

namespace DownloaderLibrary.Services.Interfaces
{
    public interface IDownloadService
    {
        void SetAuthData(string userName, string password);
        public void SetDownloadRange(long from, long to);
        Task DownloadFileAsync(string url, IProgress<double> progress, CancellationToken token);
        string FilePath { get; set; }
    }
}
