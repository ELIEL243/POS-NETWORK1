using Org.BouncyCastle.Utilities;
using POS.Models;
using POS.Services;
using POS.Views.DataEntry;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static iTextSharp.text.pdf.AcroFields;

namespace POS.ViewModel
{
    public class UserViewModel
    {
        public ObservableCollection<User> users { get; set; }
        public ObservableCollection<Role> roles { get; set; }
        public User user { get; set; }
        public Role role { get; set; }
        public UserDbService DbService { get; set; }
        public DelegateCommand add_btn { get; set; }
        public DelegateCommand del_btn { get; set; }
        public DelegateCommand edit_btn { get; set; }
        public Backup backup { get; set; }
        public string visibility { get; set; }
        public UserViewModel()
        {
            users = new ObservableCollection<User>();
            DbService = new UserDbService();
            users = DbService.GetUsers();
            roles = DbService.GetRoles();
            add_btn = new DelegateCommand(AddUser);
            edit_btn = new DelegateCommand(EditUser);
            del_btn = new DelegateCommand(DelUser);
            backup = new Backup();



        }

        private void EditUser()
        {
            UserDataEntry userDataEntry = new UserDataEntry();
            visibility = "Visible";
            if(user != null)
            {
                role = new Role();
                role.Name = user.role;

            }
            userDataEntry.DataContext = this;
            

            if (user != null)
            {
                userDataEntry.ShowDialog();

                if (MessageBox.Show("Voulez-vous modifier ?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    //MessageBox.Show(role.Name+" "+user.name);
                    if (!string.IsNullOrEmpty(user.name)  && userDataEntry.validate == true && !string.IsNullOrEmpty(role.Name))
                    {
                        user.role = role.Name;
                        user.password = userDataEntry.password;
                        DbService.EditUser(user);
                        backup.SaveUpdates();

                        DbService.Refresh(users);
                    }
                    else
                    {
                        MessageBox.Show("Veuillez remplir le champs !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        DbService.Refresh(users);

                    }

                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner un objet !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            //user = null;
        }

        private void DelUser()
        {
            if (user != null)
            {
                if (MessageBox.Show("Voulez-vous supprimer ?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                        DbService.DelUser(user);
                        backup.SaveUpdates();
                        MessageBox.Show("Utilisateur supprimé avec succes !", "Good", MessageBoxButton.OK, MessageBoxImage.Information);

                        DbService.Refresh(users);
                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner un objet !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            user = null;
        }

        private void AddUser()
        {
            user = new User();
            UserDataEntry userDataEntry = new UserDataEntry();
            visibility = "Visible";
            userDataEntry.DataContext = this;
            userDataEntry.ShowDialog();
            user.password = userDataEntry.password;
            try
            {
                if(!string.IsNullOrEmpty(user.name) && !string.IsNullOrEmpty(user.password) && userDataEntry.validate == true && !string.IsNullOrEmpty(role.Name))
                {
                    user.role = role.Name;
                    DbService.AddUSer(user);
                    DbService.Refresh(users);
                }
            }
            catch (Exception)
            {

            }
            user = null;
        }
    }
}
