namespace WinFolderList
{
    /// <summary>
    /// Implementaion of a msmq message queue for FileInformation items
    /// </summary>
    public class FileInformationQueue : MsmqMessageQueue<FileInformation>
    {
        protected override string QueuePath => @".\Private$\FileListQueue";
    }
}
