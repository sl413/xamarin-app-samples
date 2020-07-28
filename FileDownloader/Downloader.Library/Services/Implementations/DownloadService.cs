using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using DownloaderLibrary.Services.Interfaces;

namespace DownloaderLibrary.Services.Implementations
{
    public class DownloadService : IDownloadService
    {
        private readonly HttpClient _client;
        private readonly IFileService _fileService;
        private const int _bufferSize = 4095;

        private readonly HttpClientHandler handler;

        public DownloadService(IFileService fileService)
        {
            handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip, Credentials = new NetworkCredential()
            };
            _client = new HttpClient(handler);
            _fileService = fileService;
        }

        public void SetAuthData(string userName, string password)
        {
            handler.Credentials = new NetworkCredential(userName, password);
        }

        public void SetDownloadRange(long from, long to)
        {
            _client.DefaultRequestHeaders.Range = new RangeHeaderValue(from, to);
        }

        public async Task DownloadFileAsync(string url, IProgress<double> progress, CancellationToken token)
        {
            try
            {
                var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"The request returned with HTTP status code {response.StatusCode}");

                var fileName = Path.GetFileName(new Uri(url).LocalPath);
                var totalData = response.Content.Headers.ContentLength.GetValueOrDefault(-1L);
                var canSendProgress = totalData != -1L && progress != null;

                FilePath = Path.Combine(_fileService.GetStorageFolderPath(), fileName);

                await using var fileStream = OpenStream(FilePath);
                await using var stream = await response.Content.ReadAsStreamAsync();
                var totalRead = 0L;
                var buffer = new byte[_bufferSize];
                var isMoreDataToRead = true;

                do
                {
                    token.ThrowIfCancellationRequested();

                    var read = await stream.ReadAsync(buffer, 0, buffer.Length, token);

                    if (read == 0)
                    {
                        isMoreDataToRead = false;
                    }
                    else
                    {
                        await fileStream.WriteAsync(buffer, 0, read, token);

                        totalRead += read;

                        if (canSendProgress) progress.Report(totalRead * 1d / (totalData * 1d) * 100);
                    }
                } while (isMoreDataToRead);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        public string FilePath { get; set; }

        private Stream OpenStream(string path)
        {
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, _bufferSize);
        }
    }
}