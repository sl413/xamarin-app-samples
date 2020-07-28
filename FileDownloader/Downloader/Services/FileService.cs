using System;
using DownloaderLibrary.Services.Interfaces;

namespace Downloader.Services
{
    public class FileService : IFileService
    {
        public string GetStorageFolderPath()
        {
            // return Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment
            // .DirectoryDownloads); //for saving in Downloads folder, but needs WRITE storage perm
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal); //for saving in app storage
        }
    }
}