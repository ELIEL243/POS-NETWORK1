using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POS.Services
{
    public class Backup
    {
        public void SaveUpdates()
        {
            string db_dir = Environment.CurrentDirectory + @"\Store.db";
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\backup-store";
            
            if (Directory.Exists(dir)) { 
                Directory.Delete(dir, true);
                Directory.CreateDirectory(dir);
            }
            else
            {
                Directory.CreateDirectory(dir);
            }
            string dir_save = dir + @"\Store.db";
            File.Copy(db_dir, dir_save);
            //MessageBox.Show(db_dir);

        }
    }
}
