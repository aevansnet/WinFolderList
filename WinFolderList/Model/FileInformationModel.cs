using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFolderList.Model
{
    public class FileInformationModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime LastModified { get; set; }
        public long FileSize { get; set; }
    }
}
