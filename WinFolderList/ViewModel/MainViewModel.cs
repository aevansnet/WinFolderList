using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace WinFolderList.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// </summary>
    public class MainViewModel : ViewModelBase
    { 



        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {

            //var _mq = new FileInformationQueue();
            //var _fs = new FileSystemAccessService();

            ////var _q = new MockQueue();

            //var walker = new TreeWalker(_fs, _mq);


            //Task.Run(() =>walker.WalkTree(@"c:\"));



        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}