using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using POS.Models;
using System.Data.SQLite;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace POS.Services
{
    public class SaleDBService
    {
        public DbConfig dbConfig { get; set; }
        //public SQLiteConnection dbConfig.connection { get; set; }
        //public MySqlCommand dbConfig.command { get; set; }
        //public SQLiteDataReader dbConfig.reader { get; set; }
        public string query { get; set; }
        public string cs { get; set; }

        public SaleDBService()
        {
            dbConfig = new DbConfig();
        }

        public ObservableCollection<Order> GetOrders()
        {
            ObservableCollection<Order> orders = new ObservableCollection<Order>();
            query = "select * from SaleOrder where complete=1";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                int count = 1;
                while (dbConfig.reader.Read())
                {

                    orders.Add(new Order { id = dbConfig.reader.GetInt32(0), no = count++, date = dbConfig.reader.GetString(1).Replace(" ", "-"), heure = dbConfig.reader.GetString(2), total = dbConfig.reader.GetFloat(3), tax = dbConfig.reader.GetFloat(4), sub_total = dbConfig.reader.GetFloat(5) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
            return orders;


        }
        public void DelOrder(Order order)
        {
            query = $"Delete from SaleOrder where id={order.id}";
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
        
        public void RefreshOrders(ObservableCollection<Order> orders)
        {
            query = "select * from SaleOrder where complete=1";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                orders.Clear();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    orders.Add(new Order { id = dbConfig.reader.GetInt32(0), no = count++, date = dbConfig.reader.GetString(1), heure = dbConfig.reader.GetString(2), total = dbConfig.reader.GetFloat(3), tax = dbConfig.reader.GetFloat(4), sub_total = dbConfig.reader.GetFloat(5) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }

        }

        public void SearchOrders(ObservableCollection<Order> orders, string date)
        {
            query = $"select * from SaleOrder where complete=1 and date='{date}'";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                orders.Clear();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    //Console.WriteLine($"{date} = {dbConfig.reader.GetString(3)}");
                    orders.Add(new Order { id = dbConfig.reader.GetInt32(0), no = count++, date = dbConfig.reader.GetString(1), heure = dbConfig.reader.GetString(2), total = dbConfig.reader.GetFloat(3), tax = dbConfig.reader.GetFloat(4), sub_total = dbConfig.reader.GetFloat(5) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }

        }

        public void SearchOrdersBetween(ObservableCollection<Order> orders, string date, string date2)
        {
            query = $"select * from SaleOrder where complete=1 and date between '{date}' and '{date2}'";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                orders.Clear();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    //Console.WriteLine($"{date} = {dbConfig.reader.GetString(3)}");
                    orders.Add(new Order { id = dbConfig.reader.GetInt32(0), no = count++, date = dbConfig.reader.GetString(1), heure = dbConfig.reader.GetString(2), total = dbConfig.reader.GetFloat(3), tax = dbConfig.reader.GetFloat(4), sub_total = dbConfig.reader.GetFloat(5) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }

        }

        public ObservableCollection<LineItem> GetLineItems(Order order)
        {
            query = $"select * from Line_Item where order_no={order.id}";
            ObservableCollection<LineItem> items = new ObservableCollection<LineItem>();
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                
                while (dbConfig.reader.Read())
                {
                    items.Add(new LineItem { Desc = dbConfig.reader.GetString(2), Qts = dbConfig.reader.GetInt32(3), Price = dbConfig.reader.GetFloat(4), Total = dbConfig.reader.GetInt32(3) * dbConfig.reader.GetInt32(4) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
            return items;

        }

        public float GetTotalLineItems(Order order)
        {
            float total = 0;
            query = $"select * from Line_Item where order_no={order.id}";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                while (dbConfig.reader.Read())
                {
                    total += dbConfig.reader.GetInt32(3) * dbConfig.reader.GetFloat(4);

                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
            return total;
        }

        public float GetSubTotalLineItems(Order order)
        {
            float total = 0;
            try
            {
                total = GetTotalLineItems(order);
            }
            catch
            {

            }
            return total - ((total * 16) / 100);
        }

        public float GetTaxTotalLineItems(Order order)
        {
            float sub_total = 0;
            try
            {
                sub_total = GetTotalLineItems(order);
            }
            catch
            {

            }
            return (sub_total * 16) / 100;
        }


    }
}
