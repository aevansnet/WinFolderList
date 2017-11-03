using System;
using System.IO;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace WinFolderList.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// </summary>
    public class MainViewModel : ViewModelBase
    { 


        public RelayCommand Browse { get; set; }

        public RelayCommand Scan { get; set; }

        public RelayCommand Cancel { get; set; }

        public string Path { get; set; }



        private bool _pathEnabled;
        public bool PathEnabled {
            get { return _pathEnabled; }
            set
            {
                _pathEnabled = value;
                RaisePropertyChanged();
            }
        }



        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {

            Browse = new RelayCommand(OnBrowseClicked);
            Scan = new RelayCommand(OnScanClicked, ScanCanExecute);

            //var _mq = new FileInformationQueue();
            //var _fs = new FileSystemAccessService();

            ////var _q = new MockQueue();

            //var walker = new TreeWalker(_fs, _mq);


            //Task.Run(() =>walker.WalkTree(@"c:\"));



        }

        private bool ScanCanExecute()
        {

            // scan !inprogress
            // is path valid


            return true;
        }


        public void OnBrowseClicked()
        {
           
        }

        public void OnScanClicked()
        {

        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}