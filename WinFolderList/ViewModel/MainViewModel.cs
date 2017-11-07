using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;
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

        // DI injected fields
        private TreeWalker _treeWalker;
        private IMessageQueue<FileInformation> _subscribeQueue;
        private IFilesystemAccess _fs;

        // producer/consumer fields
        private Task _msmqConsumer;
        private CancellationTokenSource _msmqConsumerCancellationTokenSource;
        private CancellationTokenSource _scanCancelationTokenSource;
        private long _producerFileCount;


        #region Bindable properties

        /// <summary>
        /// Command to initiate a directory scan. Is executable for a given 'ScanPath'
        /// </summary>
        public RelayCommand Scan { get; set; }

        /// <summary>
        /// Command to cancel an in-progress scan. Is executable while scan in progress.
        /// </summary>
        public RelayCommand Cancel { get; set; }

        /// <summary>
        /// An observable list show directory scan results.
        /// </summary>
        public ObservableCollection<FileInformationModel> FileList { get; set; }

        /// <summary>
        /// An observable list of log items
        /// </summary>
        public ObservableCollection<string> ErrorList { get; set; }

        /// <summary>
        /// Indicator of progress though the directory scan
        ///  </summary>
        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged();

            }
        }

        /// <summary>
        /// Root path to initiate the scan from.
        ///  </summary>
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


        /// <summary>
        /// Boolean to show that a scan is in progress
        /// </summary>
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

        #endregion


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// <param name="fs">Filesystem accessor</param>
        /// <param name="treeWalker">Filesystem accessor</param>
        public MainViewModel(TreeWalker treeWalker,IFilesystemAccess fs, IMessageQueue<FileInformation> subscribeQueue)
        {
            // accept injected dependencies
            _treeWalker = treeWalker;
            _subscribeQueue = subscribeQueue;
            _fs = fs;

            Scan = new RelayCommand(OnScanClicked, ScanCanExecute);
            Cancel = new RelayCommand(OnCancelClicked, CancelCanExecute);
            ErrorList = new ObservableCollection<string>();

            FileList = new ObservableCollection<FileInformationModel>();            
            FileList.CollectionChanged += FileListOnCollectionChanged;


        }

        private void FileListOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            // Update on the progress if we can
            if (_producerFileCount > 0)
            {
                float progress = (float)FileList.Count / (float)_producerFileCount;
                // should already be back onto the UI thread by the updater
                Progress = (int)(progress * 100);
            }
        }


        private bool CancelCanExecute()
        {
            return ScanInProgress;
        }


        private void OnCancelClicked()
        {
            // cancel the producer 
            _scanCancelationTokenSource?.Cancel();

            // now clear the consumer queue
            _subscribeQueue.Clear();

            DispatcherHelper.CheckBeginInvokeOnUI(() => ScanInProgress = false);
        }

        /// <summary>
        /// Delegate to report on if a scan can be executed
        /// </summary>
        /// <returns>Boolean describing can execute status</returns>
        private bool ScanCanExecute()
        {
            // Do we have a valid path, and are we ready to initiate a scan?
            if (ScanInProgress)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(ScanPath) && _fs.DirectoryExists(ScanPath))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Initiate a scan
        /// </summary>
        private void OnScanClicked()
        {
            // User has executed a scan.

            // Clear the current file list, and log that we are starting a scan.
            DispatcherHelper.CheckBeginInvokeOnUI(() => FileList.Clear());
            OnLog($"Scan Starting for {ScanPath}");

            // Check that our msmq consumer is running.
            _msmqConsumerCancellationTokenSource = new CancellationTokenSource();  // todo: move to field?
            CheckStartMsmqConsumer(_msmqConsumerCancellationTokenSource.Token);

            // Signal to the UI scan is in progress.
            DispatcherHelper.CheckBeginInvokeOnUI(() => ScanInProgress = true);

            // Now start the msmq producer
            _producerFileCount = 0;
            _scanCancelationTokenSource = new CancellationTokenSource();
            CreateMsmqScanProducer(_scanCancelationTokenSource);

        }


        /// <summary>
        /// Accecpt a string to log, then use the UI thread to push it onto the log list.
        /// </summary>
        /// <param name="s">The string to log</param>
        private void OnLog(string s)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => ErrorList.Add(s));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cts"></param>
        private void CreateMsmqScanProducer(CancellationTokenSource cts)
        {
            try
            {
                Task.Run(() =>
                    {
                        // exit if we are already cancelled
                        cts.Token.ThrowIfCancellationRequested();
                        _producerFileCount = _treeWalker.WalkTree(ScanPath, cts.Token, OnLog);
                        cts.Token.ThrowIfCancellationRequested();
                    },

                    cts.Token).ContinueWith(t =>
                {
                    if (t.IsCanceled)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() => ScanInProgress = false);
                        OnLog("Scan Cancelled");
                    }
                    else if (t.IsCompleted)
                    {
                        OnLog($"Producer walk Completed for {_producerFileCount} files.");
                    }
                    else if(t.IsFaulted)
                    {
                        OnLog("Scan Faulted");
                    }
                });
            }
            catch (AggregateException e)
            {
                foreach (var v in e.InnerExceptions)
                {
                    OnLog(e.Message + " " + v.Message);
                }
            }

        }

        private void CheckStartMsmqConsumer(CancellationToken ct)
        {
            // attempt to start the msmq consumer if its not already created and running.
            // (by means of a 'never-ending' task.
       
            if (_msmqConsumer != null && _msmqConsumer.Status == TaskStatus.Running)
            {
                return;
            }

            _msmqConsumer = new Task(() =>
            {
                while (true)
                {
                    var fileInfo = _subscribeQueue.Dequeue();
                    if (fileInfo.LastFile)
                    {
                        OnLog("Scan Completed");
                        DispatcherHelper.CheckBeginInvokeOnUI(() => ScanInProgress = false); // Scan fully completed. 
                    }
                    else
                    {
                        DispatcherHelper.UIDispatcher.Invoke(() =>
                        {
                            FileList.Add(new FileInformationModel()
                            {
                                FileName = fileInfo.Filename,
                                FilePath = fileInfo.FilePath,
                                FileSize = fileInfo.Size,
                                LastModified = fileInfo.LastModified
                            });
                        }, DispatcherPriority.ApplicationIdle, ct);

                        ct.ThrowIfCancellationRequested();
                    }
                }
            }, ct, TaskCreationOptions.LongRunning);

            _msmqConsumer.ContinueWith(x =>
            {
                OnLog("Msmq Consumer finished");
            });

            _msmqConsumer.Start();

        }

        public override void Cleanup()
        { 
            // stop the consumer loop.
            _msmqConsumerCancellationTokenSource.Cancel();
            base.Cleanup();
        }
    }
}