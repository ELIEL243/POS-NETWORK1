using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using POS.Models;
using System.Data.SQLite;
using MySql.Data.MySqlClient;
using System.Windows;

namespace POS.Services
{
    public class ItemDbService
    {
        public DbConfig dbConfig { get; set; }
        //public SQLiteConnection dbConfig.connection { get; set; }
        //public MySqlCommand dbConfig.command { get; set; }
        //public SQLiteDataReader dbConfig.reader { get; set; }
        public string query { get; set; }
        public string cs { get; set; }

        public ItemDbService()
        {
            dbConfig = new DbConfig();

        }

        public ObservableCollection<Item> GetItems()
        {
            ObservableCollection<Item> items = new ObservableCollection<Item>();
            query = "select * from Item";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                int count = 1;

                while (dbConfig.reader.Read())
                {
                    items.Add(new Item { Id = dbConfig.reader.GetInt32(0), No = count++, Description = dbConfig.reader.GetString(1), Price = dbConfig.reader.GetFloat(2), Unit = dbConfig.reader.GetString(3), Qts = dbConfig.reader.GetInt32(4), Marque = dbConfig.reader.GetString(5), Category = dbConfig.reader.GetString(6) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
            return items;


        }

        public void Refresh(ObservableCollection<Item> items)
        {
            query = "select * from Item";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                items.Clear();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    items.Add(new Item { Id = dbConfig.reader.GetInt32(0), No = count++, Description = dbConfig.reader.GetString(1), Price = dbConfig.reader.GetFloat(2), Unit = dbConfig.reader.GetString(3), Qts = dbConfig.reader.GetInt32(4), Marque = dbConfig.reader.GetString(5), Category = dbConfig.reader.GetString(6) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
        }

        public Item GetItem(Item item)
        {
            query = $"select * from Item where description='{item.Description}'";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    item.Id = dbConfig.reader.GetInt32(0);
                    item.No = count++;
                    item.Price = dbConfig.reader.GetFloat(2);
                    item.Unit = dbConfig.reader.GetString(3);
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
            return item;

        }

        public void AddItem(Item item)
        {
            var items = GetItems().Where(i => i.Description.ToUpper() == item.Description.ToUpper());
            try
            {
                
                if (items.Count() == 0)
                {
                    query = $"Insert into Item (description,price,unit,qts,marque,category) values ('{item.Description}',{item.Price},'{item.Unit}',{item.Qts},'{item.Marque}','{item.Category}')";
                    dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                    dbConfig.connection.Open();
                    //dbConfig.command.Parameters.AddWithValue("@name", unit.Name);
                    dbConfig.command.ExecuteNonQuery();
                    dbConfig.connection.Close();
                    MessageBox.Show("Article ajouté avec succes !", "Good", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    MessageBox.Show("L'article existe deja !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }
            catch
            {

            }
        }

        public void DelItem(Item item)
        {
            query = $"Delete from Item where id={item.Id}";
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

        public void EditItem(Item item)
        {
            var items = GetItems().Where(i => i.Description.ToUpper() == item.Description.ToUpper());
            try
            {
                if (items.Count() == 0)
                {

                    query = $"Update Item set description='{item.Description}',price='{item.Price}',unit='{item.Unit}',qts={item.Qts},marque='{item.Marque}',category='{item.Category}' where id={item.Id}";
                    dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                    dbConfig.connection.Open();
                    //dbConfig.command.Parameters.AddWithValue("@name", unit.Name);
                    dbConfig.command.ExecuteNonQuery();
                    dbConfig.connection.Close();
                }
                else
                {
                    MessageBox.Show("L'article existe deja !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }
            catch
            {

            }
        }

        public void SearchItem(string text, ObservableCollection<Item> items)
        {
            query = $"select * from Item where description Like '%{text}%' OR id Like '%{text}%' OR category Like '%{text}%'";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                items.Clear();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    items.Add(new Item { Id = dbConfig.reader.GetInt32(0), No = count++, Description = dbConfig.reader.GetString(1), Price = dbConfig.reader.GetFloat(2), Unit = dbConfig.reader.GetString(3), Qts = dbConfig.reader.GetInt32(4), Marque = dbConfig.reader.GetString(5), Category = dbConfig.reader.GetString(6) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
        }

    }
}
