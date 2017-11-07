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
        /// <summary>
        /// Gets some basic file information for file at given path
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>file information</returns>
        FileInformation GetFileInformation(string path);

        /// <summary>
        /// Gets a list of files within directory
        /// </summary>
        /// <param name="path">Path to query</param>
        /// <returns>IEnumerable of file paths withing directory</returns>
        IEnumerable<string> GetFilesInDir(string path);

        /// <summary>
        /// Get a list of directories within a directory
        /// </summary>
        /// <param name="path">Path to query</param>
        /// <returns>IEnumerable of directorys in directory</returns>
        IEnumerable<string> GetDirsInDir(string path);

        /// <summary>
        /// Checks to see if directory exists at given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Boolean indicating presence</returns>
        bool DirectoryExists(string path);
    }
}
