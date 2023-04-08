using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


namespace POS.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int No { get; set; }
        public string Item { get; set; }
        public int Qts { get; set; }
        public string Date { get; set; }
        public float Purchase_price { get; set; }
        public float Total { get; set; }

        public float GetTotalPurchase(ObservableCollection<Purchase> purchases)
        {
            float total = 0;

            foreach (Purchase purchase in purchases)
            {
                total+=purchase.Purchase_price;
            }
            return total;
        }
        
    }
}
