using RenamFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RenameFile.ViewModel
{
    public class Form1ViewModel : INotifyPropertyChanged
    {

        public Form1ViewModel(Form View)
        {
            this.View = View;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private FileRename renamer = new FileRename();

        private Form View;
        private string dirPath;
        private List<string> files = new List<string>();
        private string startNO = "1";
        private string msg;
        private int startNum;
        private IList<RowItem> dataList = new BindingList<RowItem>();

        private string[] fileType = { ".jpg", ".jpeg", ".png" };
        public string DirPath
        {
            get => dirPath;
            set
            {
               
                dirPath = value;
                this.OnPropertyChanged("DirPath");

                this.Files = renamer.GetFiles(DirPath);

            }
        }
        public List<string> Files
        {
            get => files;
            set
            {
                files = value;
                this.OnPropertyChanged("File");
                UpdateMsg(renamer.AnalysisDir(this.Files));
                foreach (var fileName in files)
                {
                    var rowItem = new RowItem()
                    {
                        OldName = Path.GetFileName(fileName),
                        OldFullName = fileName,
                        NewName = ""
                    };
                    this.DataList.Add(rowItem);
                }

            }
        }
        public string StartNO
        {
            get => startNO;
            set
            {
                startNO = value;
                this.OnPropertyChanged("StartNO");
            }
        }
        public string Msg
        {
            get => msg;
            set
            {
                msg = value;
                this.OnPropertyChanged("Msg");
            }
        }

        public IList<RowItem> DataList
        {
            get => dataList;
            set
            {
                dataList = value;
                this.OnPropertyChanged("DataList");
            }
        }


        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                View.Invoke((Action)(() =>
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }));

            }
        }

        public void GenerateNewName()
        {

            this.startNum = Convert.ToInt32(this.startNO);
 

            foreach (var row in this.DataList)
            {
                row.NewName = (startNum++) + Path.GetExtension(row.OldName);
                row.NewFullName = Path.Combine(Path.GetDirectoryName(row.OldFullName), row.NewName);
            }

            this.UpdateMsg("生成成功。");



        }
        /// <summary>
        /// 校验DataList数据
        /// </summary>
        /// <returns></returns>
        public int ValidateGridData()
        {
            int res = -1;
            for (int i = 0; i < this.DataList.Count; i++)
            {
                var curItem = this.DataList[i];
                if (string.IsNullOrEmpty(curItem.NewName))
                {
                    res = i;
                    break;
                }
            }
            return res;
        }
        public void CommitChanged()
        {
            this.startNum = Convert.ToInt32(this.startNO);

            this.UpdateMsg("=================================");
            this.UpdateMsg("开始提交");
           foreach(var item in DataList)
            {
                FileInfo fi = new FileInfo(item.NewFullName);
                if (fileType.Contains(fi.Extension))
                {
                    if (File.Exists(item.NewFullName))
                    {
                        this.UpdateMsg(string.Format("文件 [{0}] 已经存在！", item.NewFullName));
                        continue;
                    }

                    try
                    {
                        if (renamer.Rename(item.OldFullName, item.NewFullName))
                        {
                            this.UpdateMsg(item.OldName + " 改为 " + item.NewName);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.UpdateMsg("发生错误：" + ex.Message);

                    }
                }
            }

            this.UpdateMsg("完成");
            this.UpdateMsg("=================================\n\n");


        }

        public void Clear()
        {
            this.DirPath = "";
            this.Files.Clear();
            this.DataList.Clear();
            this.Msg = "";
        }

        public void UpdateMsg(string msg)
        {

            this.Msg +=
        string.Format("【{0}】：{1}\n", DateTime.Now.ToString("HH:mm:ss"), msg);


        }
    }
    public class RowItem
    {
        private string oldName;
        public string OldName
        {
            get
            {
                return Path.GetFileName(OldFullName);
            }
            set 
            {
                oldName = value;
            }
        }
        public string OldFullName { get; set; }
        public string NewName { get; set; }

        private string newFullName;
        public string NewFullName
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(OldFullName), Path.GetFileNameWithoutExtension(NewName) + Path.GetExtension(OldFullName));
            }
            set
            {
                newFullName = value;
            }
        }


    }
}
