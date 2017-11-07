using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WinFolderList
{
    /// <summary>
    /// Class to provide a 'tree walk' traversely over a directory structure. File information will be published to a queue for processing    
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


        /// <summary>
        /// Walk the directory tree using the traverse method
        /// </summary>
        /// <param name="root">The root directory to start the traverse from</param>
        /// <param name="cancelationToken">A cancellation token that should be used to cancel the traverse</param>
        public long WalkTree(string root, CancellationToken cancelationToken, Action<string> onError)
        {
            long fileCount = 0;
            Stack<string> dirs = new Stack<string>();

            if (!_fs.DirectoryExists(root))
            {
                onError("Root directory does not exist");
            }
            dirs.Push(root);

            while (dirs.Count > 0 && !cancelationToken.IsCancellationRequested)
            {
                
                try
                {
                    string currentDir = dirs.Pop();

                    // scan through files in current directory                    

                    var fileInfo = _fs.GetFilesInDir(currentDir).Select(f => _fs.GetFileInformation(f));
                    foreach (var info in fileInfo)
                    {
                        if (cancelationToken.IsCancellationRequested)
                            break;

                        _mq.Enqueue(info);
                        fileCount++;
                    }

                    // now scan through nested directories, and push them onto the stack for processing
                    var subDirs = _fs.GetDirsInDir(currentDir);

                    foreach (string str in subDirs)
                    {
                        if (cancelationToken.IsCancellationRequested)
                            break;

                        dirs.Push(str);
                    }
                }


                catch (UnauthorizedAccessException e)
                {
                    // todo: need to raise this up the call stack to log
                    onError(e.Message);
                    continue;
                }

                catch (DirectoryNotFoundException e)
                {
                    // todo: need to raise this up the call stack to log
                    onError(e.Message);
                    continue;
                }                       
            }
            if (cancelationToken.IsCancellationRequested)
                return 0;
            else
            {
                _mq.Enqueue(new FileInformation
                {
                    LastFile = true
                }); // effectively a blank file message to signify end of list. Dont like doing this - re-address
                return fileCount;
            }
        }     
    }
}
