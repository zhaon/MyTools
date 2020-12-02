using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenamFile
{
    public class FileRename
    {
      

        public bool Rename(string fileName, string targetName)
        {
            bool res = true;
            try
            {
                File.Move(fileName, targetName);
            }
            catch (Exception ex)
            {

                res = false;
            }
            return res;
          
        }



        public List<string> GetFiles(string path)
        {
            if(!string.IsNullOrEmpty(path))
            {
                return Directory.GetFiles(path).ToList();

            }
            return new List<string>();
        }

        public string AnalysisDir(List<string> files)
        {
            int jpg = 0;
            int png = 0;

            for (int i = 0; i < files.Count; i++)
            {
                FileInfo fi = new FileInfo(files[i]);
                if (fi.Extension == ".jpg" || fi.Extension == ".jpeg")
                {
                    jpg++;
                }
                if (fi.Extension == ".png")
                {
                    png++;
                }
            }
            return string.Format("共有{0}个文件，其中{1}个jpg；{2}个png;", files.Count, jpg, png);
        }

    }
}
