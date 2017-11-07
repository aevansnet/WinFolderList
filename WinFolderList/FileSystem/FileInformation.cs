using System;


namespace WinFolderList
{
    /// <summary>
    /// Class describing some basic file information during a directory 'walk'
    /// </summary>
    public class FileInformation
    {
        /// <summary>
        /// The actual file name of the file including extension
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// Path of the file, not including the filename
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// size of the file in bytes
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// last 'touched' or modified time
        /// </summary>
        public DateTime LastModified { get; set; }
        /// <summary>
        /// is this the last file in this batch of messages
        /// </summary>
        public bool LastFile { get; set; }
    }
}
