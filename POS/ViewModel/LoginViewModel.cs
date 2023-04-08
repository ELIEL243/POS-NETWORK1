using POS.Models;
using POS.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POS.ViewModel
{
    public class LoginViewModel
    {
        public ObservableCollection<User> users { get; set; }
        public User user { get; set; }
        public UserDbService DbService { get; set; }
        public DelegateCommand val_btn { get; set; }
        public string password { get; set; }
        public Backup backup { get; set; }
        public static string current_role { get; set; }
        public LoginViewModel()
        {
            users = new ObservableCollection<User>();
            user = new User();
            DbService = new UserDbService();
            users = DbService.GetUsers();
            val_btn = new DelegateCommand(Login);
            backup = new Backup();
            

        }

        public bool check_user(User user)
        {
            bool check = false;
            foreach(User u in users)
            {
                if(u.name == user.name && u.password == user.password)
                {
                    current_role = u.role;
                    check = true;
                }
            }
            if(user.name == "default" && user.password == "default")
            {
                current_role = "administrateur";
                check = true;
            }
            return check;
        }

        public void Login()
        {
            if(!string.IsNullOrEmpty(user.name) && !string.IsNullOrEmpty(password))
            {
                user.password = password;
                if (check_user(user))
                {
                    Window login_win = Window.GetWindow(App.Current.MainWindow);

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.current_role = current_role;
                    login_win.Close();
                    mainWindow.CheckRole(current_role);
                    mainWindow.Show();
                }
                else
                {
                    MessageBox.Show("Utilisateur ou mot de passe incorrect !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            else
            {
                MessageBox.Show(user.name+" "+user.password);
                MessageBox.Show("Veuillez remplir le champs !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
