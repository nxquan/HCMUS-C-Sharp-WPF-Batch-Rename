using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename
{
    public class FileItem : INotifyPropertyChanged
    {
        public string FileName { get; set; }
        public string NewFileName { get; set; }
        public string FilePath { get; set; }
        public string Error { get; set; }

        public FileItem() {
            FileName = "";
            NewFileName = "";
            FilePath = "";
            Error = "";
        }

        public string renameFile()
        {
            string temp = (string)FilePath.Clone();
            string result = temp.Replace(FileName, NewFileName);
            return result;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
