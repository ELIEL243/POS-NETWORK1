using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using POS.Models;
using System.Data.SQLite;
using iTextSharp.text.pdf;
using static iTextSharp.awt.geom.Point2D;
using MySql.Data.MySqlClient;

namespace POS.Services
{
    
    public class CashDbService
    {
        public DbConfig dbConfig { get; set; }
        //public SQLiteConnection dbConfig.connection { get; set; }
        //public MySqlCommand dbConfig.command { get; set; }
        //public SQLiteDataReader dbConfig.reader { get; set; }
        public string query { get; set; }
        public string cs { get; set; }

        public CashDbService()
        {
            dbConfig = new DbConfig();
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
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    items.Add(new LineItem { Id = dbConfig.reader.GetInt32(0), No = count++, Desc = dbConfig.reader.GetString(2), Qts = dbConfig.reader.GetInt32(3), Price = dbConfig.reader.GetFloat(4), Total = dbConfig.reader.GetInt32(3) * dbConfig.reader.GetInt32(4), Unit = dbConfig.reader.GetString(5) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
            return items;

        }

        public void RefreshLineItems(Order order, ObservableCollection<LineItem> items)
        {
            query = $"select * from Line_Item where order_no={order.id}";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                items.Clear();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    items.Add(new LineItem { Id = dbConfig.reader.GetInt32(0), No = count++, Desc = dbConfig.reader.GetString(2), Qts = dbConfig.reader.GetInt32(3), Price = dbConfig.reader.GetFloat(4), Total = dbConfig.reader.GetInt32(3) * dbConfig.reader.GetInt32(4), Unit = dbConfig.reader.GetString(5) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
        }

        public Order GetOrder()
        {
            query = $"select * from SaleOrder where complete=0 limit 1";
            Order order = new Order();
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                
                dbConfig.reader = dbConfig.command.ExecuteReader();

                int cpt = 0;

                if (!dbConfig.reader.HasRows)
                {
                    query = $"Insert into SaleOrder (complete) values (0)";
                    dbConfig.connection.Close();
                    dbConfig.command = null;
                    dbConfig.connection.Open();
                    dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                    dbConfig.command.ExecuteNonQuery();
                    dbConfig.reader = null;
                    query = $"select * from SaleOrder where complete=0 limit 1";
                    dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                    dbConfig.reader = dbConfig.command.ExecuteReader();

                    while (dbConfig.reader.Read())
                    {
                        order.id = dbConfig.reader.GetInt32(0);
                    }
                    dbConfig.connection.Close();
                    return order;
                }
                else if (dbConfig.reader.HasRows)
                {
                    while (dbConfig.reader.Read())
                    {
                        order.id = dbConfig.reader.GetInt32(0);
                        cpt++;
                    }
                    dbConfig.connection.Close();
                    return order;
                }
            }
            catch
            {

            }
            return order;
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
            return total - (total * 16)/100;
        }

        public float GetTotalTaxLineItems(Order order)
        {
            float total = 0;
            try
            {
                total = GetTotalLineItems(order);
            }
            catch
            {

            }
            return (total * 16) / 100;
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

        public void AddLineItem(LineItem lineItem)
        {
            int id_order = GetOrder().id;
            query = $"Insert into Line_Item (order_no,item,qts,price,unit) values ({id_order}, '{lineItem.Desc}', {lineItem.Qts}, {lineItem.Price}, '{lineItem.Unit}')";
            try
            {
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.connection.Open();
                dbConfig.command.ExecuteNonQuery();
                dbConfig.connection.Close();
            }
            catch
            {

            }
        }

        public void CompleteOrder(Order order)
        {
            int id_order = order.id;
            query = $"Update SaleOrder set date='{order.date}', heure='{order.heure}', total={order.total}, tax={order.tax}, sub_total={order.sub_total}, complete=1 where id={id_order}";
            try
            {
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.connection.Open();
                dbConfig.command.ExecuteNonQuery();
                query = $"select * from Line_Item where order_no={id_order}";
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                MySqlConnection mySqlConnection = new MySqlConnection(dbConfig.cs);
                mySqlConnection.Open();
                while (dbConfig.reader.Read())
                {
                    query = $"update Item set qts=qts-{dbConfig.reader.GetInt32(3)} where description='{dbConfig.reader.GetString(2)}'";
                    dbConfig.command = new MySqlCommand(query, mySqlConnection);
                    dbConfig.command.ExecuteNonQuery();
                }

                mySqlConnection.Close();
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

        public bool IsDuplicated(LineItem lineItem)
        {
            query = $"select count(*) as nbr from Line_Item where item='{lineItem.Desc}' and order_no={GetOrder().id}";
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

        public void ResolveDuplicated(LineItem lineItem)
        {
            query = $"Update Line_Item set qts=qts+{lineItem.Qts} where order_no={GetOrder().id} and item='{lineItem.Desc}'";
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

        public void DeleteLineItem(LineItem lineItem)
        {
            query = $"Delete from Line_Item where order_no={GetOrder().id} and item='{lineItem.Desc}'";
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

        public void UpdateLineItem(LineItem lineItem)
        {
            query = $"Update Line_Item set qts={lineItem.Qts} where order_no={GetOrder().id} and item='{lineItem.Desc}'";
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
