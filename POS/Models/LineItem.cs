using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class LineItem
    {
        
        public int Id { get; set; }
        public int No { get; set; }
        public int Order { get; set; }
        public string Desc { get; set; }
        public int Qts { get; set; }
        public float Price { get; set; }
        public float Total { get; set; }
        public string Unit { get; set; }

        public LineItem()
        {
            this.Total = Price * Qts;
        }
    }
}
