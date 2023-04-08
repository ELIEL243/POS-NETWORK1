using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;
using System.Data.SQLite;
using MySql.Data.MySqlClient;

namespace POS.Services
{
    public class TauxDbService
    {
        public DbConfig dbConfig { get; set; }
        //public SQLiteConnection dbConfig.connection { get; set; }
        //public SQLiteCommand command { get; set; }
        //public SQLiteDataReader reader { get; set; }
        public string query { get; set; }
        public string cs { get; set; }

        public TauxDbService()
        {
            dbConfig = new DbConfig();
        }

        public Taux GetTaux()
        {
            Taux taux = new Taux();
            query = $"select * from Taux";
            try
            {
                dbConfig.connection.Open();
                dbConfig.command = new MySqlCommand(query, dbConfig.connection);
                dbConfig.reader = dbConfig.command.ExecuteReader();

                if (dbConfig.reader.HasRows)
                {

                    while (dbConfig.reader.Read())
                    {
                        taux.Id = dbConfig.reader.GetInt32(0);
                        taux.Tx = dbConfig.reader.GetInt32(1);
                    }
                }
                else
                {
                    taux.Id = 1;
                    taux.Tx = 1;
                }
                dbConfig.connection.Close();
            }
            catch
            {

            }
            return taux;
        }

        public void SaveTaux(Taux taux)
        {
            query = $"Update Taux set taux={taux.Tx}";
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

    }
}
