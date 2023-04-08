using POS.ViewModel;
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
    /// Logique d'interaction pour UCLogin.xaml
    /// </summary>
    public partial class UCLogin : Window
    {
        public LoginViewModel log_view{get;set;}
        public UCLogin()
        {
            InitializeComponent();
            CheckDbConnection();
            username.Focus();
            log_view = new LoginViewModel();
            this.DataContext = log_view;

        }

        public void CheckDbConnection() {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\DbConf";
            string path2 = path + @"\DbConfig.txt";
            if (!Directory.Exists(path) || !File.Exists(path2))
            {
                Directory.CreateDirectory(path);
                UCDbConfig uCDbConfig = new UCDbConfig();
                uCDbConfig.Show();
                this.Close();
                
                
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            log_view.password = pass.Password;
            log_view.Login();
        }

        private void pass_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                log_view.password = pass.Password;
                log_view.Login();
            }
        }
    }
}
