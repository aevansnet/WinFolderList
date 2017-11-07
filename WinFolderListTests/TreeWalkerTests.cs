using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinFolderList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Moq;

namespace WinFolderList.Tests
{
    [TestClass()]
    public class TreeWalkerTests
    {
        private Mock<IFilesystemAccess> _fileSystemMock;
        private Mock<IMessageQueue<FileInformation>> _messageQueueMock;
        private CancellationTokenSource _cancelationTokenSource;
        private Mock<Action<string>>  _errorHandlerMock;

        [TestInitialize]
        public void Init()
        {
            // test\File1
            // test\File2
            // test\File3
            // test\Dir1\File4
            // test\Dir1\File5
            // test\Dir1\Dir2\File6
            // test\Dir3\


            _fileSystemMock = new Mock<IFilesystemAccess>();

            _fileSystemMock.Setup(fs => fs.GetDirsInDir("test")).Returns(new List<string>() { "Dir1", "Dir3" });
            _fileSystemMock.Setup(fs => fs.GetFilesInDir("test")).Returns(new List<string>() { "File1", "File2", "File3" });

            _fileSystemMock.Setup(fs => fs.GetFilesInDir("Dir1")).Returns(new List<string>() { "File4", "File5" });
            _fileSystemMock.Setup(fs => fs.GetDirsInDir("Dir1")).Returns(new List<string>() { "Dir2" });

            _fileSystemMock.Setup(fs => fs.GetFilesInDir("Dir2")).Returns(new List<string>() { "File6" });
            _fileSystemMock.Setup(fs => fs.GetDirsInDir("Dir2")).Returns(new List<string>() { });

            _fileSystemMock.Setup(fs => fs.GetFilesInDir("Dir3")).Returns(new List<string>() { });
            _fileSystemMock.Setup(fs => fs.GetDirsInDir("Dir3")).Returns(new List<string>() { });

            long size = 1;

            _fileSystemMock.Setup(fs => fs.GetFileInformation(It.IsAny<string>()))
                .Returns((string path) => new FileInformation() { Filename = path, Size = size++ });

            _fileSystemMock.Setup(fs => fs.DirectoryExists(It.IsAny<string>())).Returns(true);


            _fileSystemMock.Setup(fs => fs.GetFilesInDir("SecurePath")).Throws<UnauthorizedAccessException>();
            _fileSystemMock.Setup(fs => fs.GetDirsInDir("SecurePath")).Throws<UnauthorizedAccessException>();


            _messageQueueMock = new Mock<IMessageQueue<FileInformation>>();

            _cancelationTokenSource = new CancellationTokenSource();

            _errorHandlerMock = new Mock<Action<string>>();
           

        }

        [TestMethod()]
        public void CheckRecursion()
        {
            var walker = new TreeWalker(_fileSystemMock.Object, _messageQueueMock.Object);
            var fileCount = walker.WalkTree("test", _cancelationTokenSource.Token, _errorHandlerMock.Object);

            _fileSystemMock.Verify(fs => fs.GetDirsInDir(It.IsAny<string>()), Times.Exactly(4));
            _messageQueueMock.Verify(mq => mq.Enqueue(It.IsAny<FileInformation>()), Times.Exactly(7)); // 6 files with one end of list message

            Assert.AreEqual(fileCount, 6);
        }

        [TestMethod()]
        public void CheckCancelation()
        {
            _fileSystemMock.Setup(fs => fs.GetFilesInDir("Dir1")).Returns(new List<string>() { "File4", "File5" }).Callback(() => _cancelationTokenSource.Cancel());
            var walker = new TreeWalker(_fileSystemMock.Object, _messageQueueMock.Object);
            var fileCount = walker.WalkTree("test", _cancelationTokenSource.Token, _errorHandlerMock.Object);

            _fileSystemMock.Verify(fs => fs.GetDirsInDir(It.IsAny<string>()), Times.Exactly(3));
            _messageQueueMock.Verify(mq => mq.Enqueue(It.IsAny<FileInformation>()), Times.Exactly(3)); // 3 files with no end of file list message

            Assert.AreEqual(fileCount, 0);  // we do not expect to get a file count if we were cancelled

        }


        [TestMethod()]
        public void CheckFileAccessExceptions()
        {
            var walker = new TreeWalker(_fileSystemMock.Object, _messageQueueMock.Object);
            walker.WalkTree("SecurePath", new System.Threading.CancellationToken(), _errorHandlerMock.Object);
            _errorHandlerMock.Verify(a => a(It.IsAny<string>()), Times.AtLeastOnce);
        }


    }
}