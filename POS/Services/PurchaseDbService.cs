using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using POS.Models;
using System.Data.SQLite;
using POS.Services;
using MySql.Data.MySqlClient;

namespace POS.Services
{
    public class PurchaseDbService
    {
        public DbConfig dbConfig { get; set; }
        //public SQLiteConnection dbConfig.connection { get; set; }
        //public MySqlCommand dbConfig.command { get; set; }
        //public MySqlCommand dbConfig.command2 { get; set; }
        //public SQLiteDataReader dbConfig.reader { get; set; }
        public string query { get; set; }
        public string cs { get; set; }

        public PurchaseDbService()
        {
            dbConfig = new DbConfig();
        }

        public ObservableCollection<Purchase> GetPurchases()
        {
            ObservableCollection<Purchase> purchases = new ObservableCollection<Purchase>();
            query = "select * from Purchase where completed=1";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    purchases.Add(new Purchase { Id = dbConfig.reader.GetInt32(0), No = count++, Item = dbConfig.reader.GetString(1), Qts = dbConfig.reader.GetInt32(2), Date = DateTime.Parse(dbConfig.reader.GetString(3)).ToShortDateString(), Purchase_price = dbConfig.reader.GetFloat(4), Total = dbConfig.reader.GetFloat(5) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
            return purchases;
            

        }

        public ObservableCollection<Purchase> GetUnCompletedPurchases()
        {
            ObservableCollection<Purchase> purchases = new ObservableCollection<Purchase>();
            query = "select * from Purchase where completed=0";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    purchases.Add(new Purchase { Id = dbConfig.reader.GetInt32(0), No = count++, Item = dbConfig.reader.GetString(1), Qts = dbConfig.reader.GetInt32(2), Date = DateTime.Parse(dbConfig.reader.GetString(3)).ToShortDateString(), Purchase_price = dbConfig.reader.GetFloat(4), Total = dbConfig.reader.GetFloat(5) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
            return purchases;


        }
        public void Refresh(ObservableCollection<Purchase> purchases)
        {
            query = "select * from Purchase where completed=1";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                purchases.Clear();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    purchases.Add(new Purchase { Id = dbConfig.reader.GetInt32(0), No = count++, Item = dbConfig.reader.GetString(1), Qts = dbConfig.reader.GetInt32(2), Date = DateTime.Parse(dbConfig.reader.GetString(3)).ToShortDateString(), Purchase_price = dbConfig.reader.GetFloat(4), Total = dbConfig.reader.GetFloat(5) });

                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
        }

        public void RefreshUnCompleted(ObservableCollection<Purchase> purchases)
        {
            query = "select * from Purchase where completed=0";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                purchases.Clear();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    purchases.Add(new Purchase { Id = dbConfig.reader.GetInt32(0), No = count++, Item = dbConfig.reader.GetString(1), Qts = dbConfig.reader.GetInt32(2), Date = DateTime.Parse(dbConfig.reader.GetString(3)).ToShortDateString(), Purchase_price = dbConfig.reader.GetFloat(4), Total = dbConfig.reader.GetFloat(5) });

                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
        }

        public void AddPurchase(Purchase purchase)
        { 
            query = $"Insert into Purchase (item,qts,date,purchase_price,total,completed) values ('{purchase.Item}',{purchase.Qts},'{purchase.Date}',{purchase.Purchase_price},{purchase.Total}, 0)";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                //dbConfig.command.Parameters.AddWithValue("@name", unit.Name);
                dbConfig.command.ExecuteNonQuery();
                dbConfig.connection.Close();
            }
            catch
            {

            }
        }

        public void DelPurchase(Purchase purchase)
        {
            query = $"Delete from Purchase where id={purchase.Id}";
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

        public void EditPurchase(Purchase purchase)
        {
            query = $"Update Purchase set qts={purchase.Qts},purchase_price={purchase.Purchase_price} where id={purchase.Id}";
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

        public void CompletePurchase(ObservableCollection<Purchase> purchases)
        {
            int id=0;
            try
            {
                ItemDbService itemDbService = new ItemDbService();
                foreach (Purchase p in purchases)

                {
                    Console.WriteLine(p.Item);

                    query = $"Update Purchase set completed=1 where id={p.Id}";
                    dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                    dbConfig.connection.Open();
                    //dbConfig.command.Parameters.AddWithValue("@name", unit.Name);
                    dbConfig.command.ExecuteNonQuery();
                    dbConfig.connection.Close();
                    dbConfig.connection.Open();
                    foreach (Item i in itemDbService.GetItems())
                    {
                        if (i.Description == p.Item)
                        {
                            id = i.Id;
                        }
                    }
                    string query2 = $"Update Item set qts=qts+{p.Qts} where id='{id}'";
                    dbConfig.command = new MySqlCommand(query2, dbConfig.connection);
                    dbConfig.command.ExecuteNonQuery();
                    dbConfig.connection.Close();
                }
            }
            catch
            {

            }
        }

        public void SearchPurchases(ObservableCollection<Purchase> purchases, string date)
        {
            query = $"select * from Purchase where completed=1 and date='{date}'";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                purchases.Clear();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    //Console.WriteLine($"{date} = {dbConfig.reader.GetString(3)}");
                    purchases.Add(new Purchase { Id = dbConfig.reader.GetInt32(0), No = count++, Item = dbConfig.reader.GetString(1), Qts = dbConfig.reader.GetInt32(2), Date = DateTime.Parse(dbConfig.reader.GetString(3)).ToShortDateString(), Purchase_price = dbConfig.reader.GetFloat(4), Total = dbConfig.reader.GetFloat(5) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }

        }

        public void SearchPurchasesBetween(ObservableCollection<Purchase> purchases, string date, string date2)
        {
            query = $"select * from Purchase where completed=1 and date between '{date}' and '{date2}'";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                purchases.Clear();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    //Console.WriteLine($"{date} = {dbConfig.reader.GetString(3)}");
                    purchases.Add(new Purchase { Id = dbConfig.reader.GetInt32(0), No = count++, Item = dbConfig.reader.GetString(1), Qts = dbConfig.reader.GetInt32(2), Date = DateTime.Parse(dbConfig.reader.GetString(3)).ToShortDateString(), Purchase_price = dbConfig.reader.GetFloat(4), Total = dbConfig.reader.GetFloat(5) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }

        }

        public bool IsDuplicated(Purchase purchase)
        {
            query = $"select count(*) as nbr from Purchase where item='{purchase.Item}' and completed=0";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                var nbr = 0;
                while (dbConfig.reader.Read())
                {
                    nbr = dbConfig.reader.GetInt32(0);
                }
                if (nbr > 0)
                {
                    dbConfig.connection.Close();
                    return true;
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
            return false;

        }

        public void ResolveDuplicated(Purchase purchase)
        {
            query = $"Update Purchase set qts=qts+{purchase.Qts} where completed=0 and item='{purchase.Item}'";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.command.ExecuteNonQuery();
                dbConfig.connection.Close();
            }
            catch
            {

            }
        }

    }
}
