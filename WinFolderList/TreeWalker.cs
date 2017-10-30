using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFolderList
{
    /// <summary>
    /// Class to provide a 'tree walk' recursivley over a directory structure. File information will be published to a queue for processing    
    /// </summary>
    public class TreeWalker
    {
        private readonly IFilesystemAccess _fs;
        private readonly IMessageQueue<FileInformation> _mq;
        public TreeWalker(IFilesystemAccess fileSystem, IMessageQueue<FileInformation> publishQueue)
        {
            _fs = fileSystem;
            _mq = publishQueue;
        }   

        public void WalkTree(string path)
        {
            try
            {
                var fileInfo = _fs.GetFilesInDir(path).Select(f => _fs.GetFileInformation(f));
                foreach (var info in fileInfo)
                {
                    _mq.Enqueue(info);
                }

                var directories = _fs.GetDirsInDir(path);
                foreach (var directory in directories)
                {
                    WalkTree(directory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
