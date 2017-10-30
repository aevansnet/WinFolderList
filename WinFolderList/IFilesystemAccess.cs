using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFolderList
{

    /// <summary>
    /// Interface describing basic functionality to be provided by a filesystem access service.
    /// </summary>
    public interface IFilesystemAccess
    {
        FileInformation GetFileInformation(string path);
        IEnumerable<string> GetFilesInDir(string path);
        IEnumerable<string> GetDirsInDir(string path);
    }
}
