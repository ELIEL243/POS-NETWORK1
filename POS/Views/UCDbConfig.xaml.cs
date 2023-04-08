using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POS.Views
{
    /// <summary>
    /// Logique d'interaction pour UCDbConfig.xaml
    /// </summary>
    public partial class UCDbConfig : Window
    {
        public UCDbConfig()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\DbConf"+@"\DbConfig.txt";

            //StreamWriter sw = new StreamWriter(@"DbConfig.txt");

            if (!string.IsNullOrEmpty(user.Text) && !string.IsNullOrEmpty(password.Text) && !string.IsNullOrEmpty(database.Text) && !string.IsNullOrEmpty(server.Text))
            {
                string[] details = { server.Text, database.Text, user.Text, password.Text};
                foreach(string detail in details)
                {
                    //Write a line of text
                    //sw.WriteLine(detail);
                    //Write a second line of text
                    File.WriteAllLines(path, details);
                }
                
                
                //sw.Close();
                this.Close();
                   
            }

        }
    }
}
