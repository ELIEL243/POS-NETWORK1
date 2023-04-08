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
    public class UnitDbService
    {
        public DbConfig dbConfig { get; set; }
        //public SQLiteConnection connection { get; set; }
        //public SQLiteCommand command { get; set; }
        //public SQLiteDataReader reader { get; set; }
        public string query { get; set; }
        //public string cs { get; set; }
        public UnitDbService()
        {
            dbConfig = new DbConfig();

        }

        public ObservableCollection<Models.Unit> GetUnits()
        {
            ObservableCollection<Models.Unit> units = new ObservableCollection<Models.Unit>();
            query = "select * from Unit";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    units.Add(new Models.Unit { Id = dbConfig.reader.GetInt32(0), No = count++, Name = dbConfig.reader.GetString(1) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
            return units;


        }

        public void Refresh(ObservableCollection<Unit> units)
        {
            query = "select * from Unit";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();
                units.Clear();
                int count = 1;
                while (dbConfig.reader.Read())
                {
                    units.Add(new Models.Unit { Id = dbConfig.reader.GetInt32(0), No = count++, Name = dbConfig.reader.GetString(1) });
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
        }

        public Unit GetUnit(Unit unit)
        {
            query = $"select * from Unit where name='{unit.Name}'";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();

                while (dbConfig.reader.Read())
                {
                    unit.Id = dbConfig.reader.GetInt32(0);
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
            return unit;

        }

        public bool CheckDuplicate(Unit unit)
        {
            bool check = false;
            var units = GetUnits();

            foreach (var u in units)
            {
                if (u.Name == unit.Name)
                {
                    check = true;
                }
            }

            return check;
        }

        public void AddUnit(Unit unit)
        {
            
            var units = GetUnits().Where(u => u.Name.ToUpper() == unit.Name.ToUpper());
            try
            {
                if (units.Count() == 0)
                {
                    query = $"Insert into Unit (name) values ('{unit.Name}')";
                    dbConfig.connection.Open();
                    dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                    //dbConfig.command.Parameters.AddWithValue("@name", unit.Name);
                    dbConfig.command.ExecuteNonQuery();
                    dbConfig.connection.Close();
                    MessageBox.Show("Unité ajouté avec succes !", "Good", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    MessageBox.Show("L'unité existe deja !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }
            catch
            {

            }
            
        }

        public void DelUnit(Unit unit)
        {
            query = $"Delete from Unit where id={unit.Id}";
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

        public void EditUnit(Unit unit)
        {
            var units = GetUnits().Where(u => u.Name.ToUpper() == unit.Name.ToUpper());

            try
            {
                if (units.Count() == 0)
                {
                    query = $"Update Unit set name='{unit.Name}' where id={unit.Id}";
                    dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                    dbConfig.connection.Open();
                    //dbConfig.command.Parameters.AddWithValue("@name", unit.Name);
                    dbConfig.command.ExecuteNonQuery();
                    dbConfig.connection.Close();
                }
                else
                {
                    MessageBox.Show("L'unité existe deja !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }
            catch
            {

            }
        }


    }
}
