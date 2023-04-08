using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace POS.Services
{
    public class DbConfig
    {
        public MySqlConnection connection { get; set; }
        public MySqlCommand command { get; set; }
        public MySqlDataReader reader { get; set; }
        public string query { get; set; }
        public string cs { get; set; }

        public DbConfig()
        {
            ReadConfig();
            if (CheckConnection(cs))
            {
                connection = new MySqlConnection(cs);
            }
            else
            {
                connection = null;
            }
            
            //Console.WriteLine(connection);
        }

        public void ReadConfig()
        {
            var list = new List<string>();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\DbConf" + @"\DbConfig.txt";
            if (File.Exists(path))
            {
                var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);
                        list.Add(line);
                    }
                }
                fileStream.Close();
                this.cs = $"SERVER={list[0]};DATABASE={list[1]};UID={list[2]};PASSWORD={list[3]};";
            }
            else
            {
                this.cs = $"SERVER={null};DATABASE={null};UID={null};PASSWORD={null};";
            }
            
        }

        public bool CheckConnection(string conn)
        {
            bool result = false;
            MySqlConnection mySqlConnection = new MySqlConnection(conn);
            try
            {
                mySqlConnection.Open();
                result = true;
                mySqlConnection.Close();
            }
            catch
            {
                result = false;
            }
            return result;
        }




    }
}
