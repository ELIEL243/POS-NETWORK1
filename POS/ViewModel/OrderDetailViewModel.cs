using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.Collections.ObjectModel;
using POS.Models;
using POS.Services;
using System.Windows;

namespace POS.ViewModel
{
    public class OrderDetailViewModel
    {
        public string no_order { get; set; }
        public string sec_no_order { get; set; }
        public string total_order { get; set; }
        public string sub_total_sales { get; set; }
        public string tax_total_sales { get; set; }
        public string date { get; set; }
        public DelegateCommand print_btn { get; set; }
        public ObservableCollection<LineItem> line_items { get; set; }
        public OrderDetailViewModel()
        {
            line_items = new ObservableCollection<LineItem>();
            print_btn = new DelegateCommand(PrintInvoice);
        }

        private void PrintInvoice()
        {
            if(line_items.Count > 0)
            {
                PrintService printService = new PrintService();
                printService.PrintInvoiceOrder(line_items, date, sec_no_order, total_order, sub_total_sales, tax_total_sales );
            }

            else
            {
                MessageBox.Show("Rien à imprimer !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
    }
}
