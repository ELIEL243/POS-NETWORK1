using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;

namespace POS.Models
{
    public class Order
    {
        public int id { get; set; }
        public int no { get; set; }
        public string date { get; set; }
        public float total { get; set; }
        
        public string heure { get; set; }
        public float tax { get; set; }
        public float sub_total { get; set; }
        
        public float GetTotalOrders(ObservableCollection<Order> orders)
        {
            float __total = 0;

            foreach (Order order in orders)
            {
                __total+=order.total;
            }

            return __total;
        }
    }
}
