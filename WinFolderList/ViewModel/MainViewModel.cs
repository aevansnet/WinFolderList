using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using WinFolderList.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;

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

        public ObservableCollection<FileInformationModel> FileList { get; set; }

        private TreeWalker _treeWalker;
        private IMessageQueue<FileInformation> _subscribeQueue;
        private CancellationTokenSource _scanCancelationTokenSource;

        private string _scanPath = "";
        public string ScanPath
        {
            get
            {
                return _scanPath;
            }
            set
            {
                _scanPath = value;
                RaisePropertyChanged();
                Scan.RaiseCanExecuteChanged();
            }
        }



        private bool _pathEnabled;
        public bool PathEnabled {
            get { return _pathEnabled; }
            set
            {
                _pathEnabled = value;
                RaisePropertyChanged();
            }
        }


        private bool _scanInProgress;
        public bool ScanInProgress
        {
            get { return _scanInProgress; }
            set
            {
                _scanInProgress = value;
                RaisePropertyChanged();
                Cancel.RaiseCanExecuteChanged();
            }
        }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(TreeWalker treeWalker, IMessageQueue<FileInformation> subscribeQueue)
        {
            _treeWalker = treeWalker;
            _subscribeQueue = subscribeQueue;

            Browse = new RelayCommand(OnBrowseClicked);
            Scan = new RelayCommand(OnScanClicked, ScanCanExecute);
            Cancel = new RelayCommand(OnCancelClicked, CancelCanExecute);
            FileList = new ObservableCollection<FileInformationModel>();

            //var _mq = new FileInformationQueue();




        }

        private bool ScanCanExecute()
        {

            if (ScanInProgress)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(ScanPath) && Directory.Exists(ScanPath))
            {
                return true;
            }


            return false;
        }


        private bool CancelCanExecute()
        {
            return ScanInProgress;
        }


        private void OnCancelClicked()
        {
            if(_scanCancelationTokenSource != null)
            {
                _scanCancelationTokenSource.Cancel();
            }

        }
        


        public void OnBrowseClicked()
        {
           
        }

        private void OnScanClicked()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => FileList.Clear());
            Task.Run(() =>
            {
                while (true)
                {
                    var fileInfo = _subscribeQueue.Dequeue();
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        FileList.Add(new FileInformationModel() { FileName = fileInfo.Filename, FilePath= fileInfo.FilePath, FileSize = fileInfo.Size, LastModified = fileInfo.LastModified });
                        //Console.WriteLine(fileInfo.Filename);
                    });
                }
            });


            var cts = new CancellationTokenSource();
            _scanCancelationTokenSource = cts;
           
            


            try
            {
                Task.Run(() =>
                {
                    cts.Token.ThrowIfCancellationRequested();
                    DispatcherHelper.CheckBeginInvokeOnUI(() => ScanInProgress = true);                   
                    _treeWalker.WalkTree(ScanPath, cts.Token);
                    cts.Token.ThrowIfCancellationRequested();
                },

            cts.Token).ContinueWith(t =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => ScanInProgress = false);
                if (t.IsFaulted)
                {
                    // log the exception
                }
                if (t.IsCanceled)
                {
                    // log the cancelation
                }
            });
            }
            catch (AggregateException e)
            {
                foreach (var v in e.InnerExceptions)
                    Console.WriteLine(e.Message + " " + v.Message);
            }
            finally
            {
                //_scanCancelationTokenSource.Dispose();
            }

        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}