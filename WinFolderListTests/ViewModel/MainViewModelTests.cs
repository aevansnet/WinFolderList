using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinFolderList.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace WinFolderList.ViewModel.Tests
{
    [TestClass()]
    public class MainViewModelTests
    {
        private Mock<IFilesystemAccess> _fsMock;
        private Mock<IMessageQueue<FileInformation>> _mqMock;
        private Mock<Action<string>> _errorHandlerMock;
        private Mock<TreeWalker> _twMock;

        private MainViewModel _vm;

  

        [TestInitialize]
        public void Init()
        {
            
            _fsMock = new Mock<IFilesystemAccess>();
            _mqMock = new Mock<IMessageQueue<FileInformation>>();
            _twMock = new Mock<TreeWalker>(_fsMock.Object, _mqMock.Object);

            _vm = new MainViewModel(_twMock.Object, _fsMock.Object, _mqMock.Object );


            _fsMock.Setup(f => f.DirectoryExists("InvalidPath")).Returns(false);
            _fsMock.Setup(f => f.DirectoryExists("ValidPath")).Returns(true);
            _fsMock.Setup(fs => fs.GetDirsInDir("test")).Returns(new List<string>() { "Dir1", "Dir3" });
            _fsMock.Setup(fs => fs.GetFilesInDir("test")).Returns(new List<string>() { "File1", "File2", "File3" });

            _fsMock.Setup(fs => fs.GetFilesInDir("Dir1")).Returns(new List<string>() { "File4", "File5" });
            _fsMock.Setup(fs => fs.GetDirsInDir("Dir1")).Returns(new List<string>() { "Dir2" });

            _fsMock.Setup(fs => fs.GetFilesInDir("Dir2")).Returns(new List<string>() { "File6" });
            _fsMock.Setup(fs => fs.GetDirsInDir("Dir2")).Returns(new List<string>() { });

            _fsMock.Setup(fs => fs.GetFilesInDir("Dir3")).Returns(new List<string>() { });
            _fsMock.Setup(fs => fs.GetDirsInDir("Dir3")).Returns(new List<string>() { });

        }


        [TestMethod()]
        public void MainViewModelTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void ScanCanExecuteTest()
        {
          

            _vm.ScanPath = "InvalidPath";
            Assert.AreEqual(_vm.Scan.CanExecute(null), false);

            _vm.ScanPath = "ValidPath";
            Assert.AreEqual(_vm.Scan.CanExecute(null), true);
        }




    



        [TestMethod()]
        public void CleanupTest()
        {
            Assert.Fail();
        }
    }
}