using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFolderList
{
    /// <summary>
    /// Wrapper around some basic OS level filesystem calls to provide directrory structure and file information
    /// </summary>
    public class FileSystemAccessService : IFilesystemAccess
    {
        public IEnumerable<string> GetFilesInDir(string path)
        {
            return Directory.EnumerateFiles(path);
        }

        public IEnumerable<string> GetDirsInDir(string path)
        {
            return Directory.EnumerateDirectories(path);
        }

        public FileInformation GetFileInformation(string path)
        {
            var fInfo = new FileInfo(path);

            return new FileInformation()
            {
                Filename = path,
                LastModified = fInfo.LastWriteTime
            };
        }
    }
}
