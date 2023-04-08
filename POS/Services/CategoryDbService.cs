using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;
using System.Data.SQLite;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Runtime.InteropServices;

namespace POS.Services
{
    public class CategoryDbService
    {
        public DbConfig dbConfig { get; set; }
        //public SQLiteConnection dbConfig.connection { get; set; }
        //public MySqlCommand dbConfig.command { get; set; }
        //public SQLiteDataReader dbConfig.reader { get; set; }
        public string query { get; set; }
        public string cs { get; set; }

        public CategoryDbService()
        {
            dbConfig = new DbConfig();
        }

        public ObservableCollection<Category> GetCategories()
        {
            ObservableCollection<Category> categories = new ObservableCollection<Category>();
            try
            {
                query = "select * from Category";
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                int count = 1;

                while (dbConfig.reader.Read())
                {
                    categories.Add(new Category { Id = dbConfig.reader.GetInt32(0), No = count++, Name = dbConfig.reader.GetString(1) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
            return categories;


        }

        public void Refresh(ObservableCollection<Category> categories)
        {
            query = "select * from Category";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                categories.Clear();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    categories.Add(new Category { Id = dbConfig.reader.GetInt32(0), No = count++, Name = dbConfig.reader.GetString(1) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
        }

        public Category GetCategory(Category category)
        {
            query = $"select * from Category where name='{category.Name}'";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();

                while (dbConfig.reader.Read())
                {
                    category.Id = dbConfig.reader.GetInt32(0);
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
            return category;
           
        }

        public void AddCategory(Category category)
        {
            var categories = GetCategories().Where(c => c.Name.ToUpper() == category.Name.ToUpper());
            try
            {
                if (categories.Count() == 0)
                {
                    query = $"Insert into Category (name) values ('{category.Name}')";
                    dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                    dbConfig.connection.Open();
                    //dbConfig.command.Parameters.AddWithValue("@name", unit.Name);
                    dbConfig.command.ExecuteNonQuery();
                    dbConfig.connection.Close();
                    MessageBox.Show("Categorie ajouté avec succes !", "Good", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    MessageBox.Show("La catégorie existe deja !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }
            catch
            {

            }
        }

        public void DelCategory(Category category)
        {
            query = $"Delete from Category where id={category.Id}";
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

        public void EditCategory(Category category)
        {
            var categories = GetCategories().Where(c => c.Name.ToUpper() == category.Name.ToUpper());

            query = $"Update Category set name='{category.Name}' where id={category.Id}";
            try
            {
                if (categories.Count() == 0)
                {
                    dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                    dbConfig.connection.Open();
                    //dbConfig.command.Parameters.AddWithValue("@name", unit.Name);
                    dbConfig.command.ExecuteNonQuery();
                    dbConfig.connection.Close();
                }
                else
                {
                    MessageBox.Show("La catégorie existe deja !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }
            catch
            {

            }
        }

    }
}
