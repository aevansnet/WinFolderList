using System;


namespace WinFolderList
{
    /// <summary>
    /// Class describing some basic file information during a directory 'walk'
    /// </summary>
    public class FileInformation
    {
        public string Filename { get; set; }
        public string FilePath { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
    }
}
