using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using POS.Models;
using System.Data.SQLite;
using System.Threading;
using System.Windows;
using MySql.Data.MySqlClient;

namespace POS.Services
{
    public class UserDbService
    {
        //public SQLiteConnection connection { get; set; }
        //public MySqlCommand command { get; set; }
        //public SQLiteDataReader reader { get; set; }
        public string query { get; set; }
        public string cs { get; set; }
        public DbConfig dbConfig { get; set; }
        public UserDbService()
        {
            dbConfig = new DbConfig();
            
        }

        public ObservableCollection<User> GetUsers()
        {
            
            ObservableCollection<User> items = new ObservableCollection<User>();
            query = "select * from User";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                int count = 1;

                while (dbConfig.reader.Read())
                {
                    items.Add(new User { id = dbConfig.reader.GetInt32(0), no = count++, name = dbConfig.reader.GetString(1), password = dbConfig.reader.GetString(3), role = dbConfig.reader.GetString(2) });
                }
                dbConfig.connection.Close();
                

            }
            catch
            {
                MessageBox.Show("Erreur de connexion", "Echec", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return items;

        }

        public ObservableCollection<Role> GetRoles()
        {
            ObservableCollection<Role> items = new ObservableCollection<Role>();
            query = "select * from Role";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();

                while (dbConfig.reader.Read())
                {
                    items.Add(new Role { Id = dbConfig.reader.GetInt32(0), Name = dbConfig.reader.GetString(1) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
            return items;


        }

        public bool CheckUser(User user)
        {
            query = $"select * from User where name='{user.name}' and password='{user.password}'";
            dbConfig.connection.Open();
            bool check = false;
            dbConfig.command = new MySqlCommand(query, dbConfig.connection);
            dbConfig.reader = dbConfig.command.ExecuteReader();
            if (dbConfig.reader.HasRows)
            {
                check = true;
            }
            else
            {
                check = false;
            }

            dbConfig.connection.Close();
            return check;
        }

        public void Refresh(ObservableCollection<User> items)
        {
            query = "select * from User";
            dbConfig.connection.Open();
            dbConfig.command = new MySqlCommand(query, dbConfig.connection);
            dbConfig.reader = dbConfig.command.ExecuteReader();
            items.Clear();
            int count = 1;
            while (dbConfig.reader.Read())
            {
                items.Add(new User { id = dbConfig.reader.GetInt32(0), no = count++, name = dbConfig.reader.GetString(1), password = dbConfig.reader.GetString(3), role = dbConfig.reader.GetString(2) });
            }
            dbConfig.connection.Close();
        }

        public void AddUSer(User item)
        {
            var users = GetUsers().Where(u => u.name.ToUpper() == item.name.ToUpper());
            try
            {
                if (users.Count() == 0)
                {
                    dbConfig.connection.Open();

                    query = $"Insert into User(name,role,password) values ('{item.name}','{item.role}','{item.password}')";
                    dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                    //dbConfig.command.Parameters.AddWithValue("@name", unit.Name);
                    dbConfig.command.ExecuteNonQuery();
                    dbConfig.connection.Close();
                    MessageBox.Show("Utilisateur ajouté avec succes !", "Good", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    MessageBox.Show("L'utilisateur existe deja !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void EditUser(User item)
        {
            var users = GetUsers().Where(u => u.name.ToUpper() == item.name.ToUpper());

            try
            {
               
                if (users.Count() == 0)
                {
                    query = $"Update User set name='{item.name}',role='{item.role}' where id={item.id}";

                    if (!string.IsNullOrEmpty(item.password))
                    {
                        query = $"Update User set name='{item.name}',role='{item.role}',password='{item.password}' where id={item.id}";

                    }
                    dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                    dbConfig.connection.Open();
                    //dbConfig.command.Parameters.AddWithValue("@name", unit.Name);
                    dbConfig.command.ExecuteNonQuery();
                    dbConfig.connection.Close();
                    MessageBox.Show("Utilisateur modifié avec succes !", "Good", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    MessageBox.Show("L'utilisateur existe deja !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }
            catch
            {

            }
        }

        public void DelUser(User item)
        {
            query = $"Delete from User where id={item.id}";
            try
            {
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.connection.Open();
                //dbConfig.command.Parameters.AddWithValue("@name", unit.Name);
                dbConfig.command.ExecuteNonQuery();
                dbConfig.connection.Close();
            }
            catch
            {

            }
        }
    }
}
