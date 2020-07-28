using DownloaderLibrary.Services.Implementations;
using DownloaderLibrary.Services.Interfaces;
using DownloaderLibrary.ViewModels;
using GalaSoft.MvvmLight.Ioc;

namespace DownloaderLibrary
{
    public class Bootstrap
    {
        private static Bootstrap instance;

        /// <summary>
        ///     This is a singleton instance for bootstraping the application.
        /// </summary>
        /// <value>The instance.</value>
        public static Bootstrap Instance => instance ??= new Bootstrap();

        /// <summary>
        ///     Setup all injections
        /// </summary>
        public void Setup()
        {
            SimpleIoc.Default.Register<IDownloadService, DownloadService>();
            SimpleIoc.Default.Register<DownloadViewModel>();
        }
    }
}