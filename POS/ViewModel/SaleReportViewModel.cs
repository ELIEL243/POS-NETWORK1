using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;
using POS.Services;
using Prism.Commands;
using POS.Views.DataEntry;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using POS.Views.DataInfo;

namespace POS.ViewModel
{
    public class SaleReportViewModel: INotifyPropertyChanged
    {
        #region Properties
        public DelegateCommand print_btn { get; set; }
        public DelegateCommand ref_btn { get; set; }
        public DelegateCommand del_btn { get; set; }
        public DelegateCommand search_date { get; set; }
        public DelegateCommand SelectOrder { get; set; }
        public ObservableCollection<Order> orders { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public SaleDBService dBService { get; set; }
        public string date1 { get; set; }
        public string date2 { get; set; }
        public string total_sales { get; set; }
        public Backup backup { get; set; }

        public Order selected_order { get; set; }
        #endregion

        public SaleReportViewModel()
        {
            orders = new ObservableCollection<Order>();
            dBService = new SaleDBService();
            orders = dBService.GetOrders();
            print_btn = new DelegateCommand(PrintOrders);
            ref_btn = new DelegateCommand(RefOrders);
            del_btn = new DelegateCommand(DelOrder);
            search_date = new DelegateCommand(SearchDate);
            SelectOrder = new DelegateCommand(CheckOrder);
            backup = new Backup();
        }

        private void DelOrder()
        {
            if(selected_order != null)
            {
                if (MessageBox.Show($"Voulez-vous supprimer cette vente ?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    dBService.DelOrder(selected_order);
                    orders.Remove(selected_order);
                    MessageBox.Show("Vente supprimée avec succes !", "Good", MessageBoxButton.OK, MessageBoxImage.Information);

                }


            }
            else
            {
                MessageBox.Show("Veuillez selectionner un objet !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void CheckOrder()
        {
            OrderDetail orderDetail = new OrderDetail();
            OrderDetailViewModel orderDetailViewModel = new OrderDetailViewModel();
            orderDetailViewModel.no_order = "Vente n°"+selected_order.id.ToString();
            orderDetailViewModel.line_items = dBService.GetLineItems(selected_order);
            orderDetailViewModel.total_order = dBService.GetTotalLineItems(selected_order).ToString()+" FC";
            orderDetailViewModel.sub_total_sales = dBService.GetSubTotalLineItems(selected_order).ToString() + " FC";
            orderDetailViewModel.tax_total_sales = dBService.GetTaxTotalLineItems(selected_order).ToString();
            orderDetailViewModel.date = selected_order.date;
            orderDetailViewModel.sec_no_order = selected_order.id.ToString();
            orderDetail.DataContext = orderDetailViewModel;
            
            orderDetail.ShowDialog();


        }

        private void SearchDate()
        {
            Order order = new Order();
            try
            {
                if (date2 == null && date1 != null)
                {
                    string fir_date = ConvertDateToSave(date1);
                    dBService.SearchOrders(orders, fir_date);
                    total_sales = order.GetTotalOrders(orders).ToString() + " Fc";
                    OnPropertyChanged("total_sales");

                }

                else if (date2 != null && date1 == null)
                {
                    string sec_date = ConvertDateToSave(date2);
                    dBService.SearchOrders(orders, sec_date);
                    total_sales = order.GetTotalOrders(orders).ToString() + " Fc";
                    OnPropertyChanged("total_sales");

                }

                else if (date2 != null && date1 != null)
                {
                    string fir_date = ConvertDateToSave(date1);
                    string sec_date = ConvertDateToSave(date2);
                    dBService.SearchOrdersBetween(orders, fir_date, sec_date);
                    total_sales = order.GetTotalOrders(orders).ToString() + " Fc";
                    OnPropertyChanged("total_sales");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void PrintOrders()
        {
            PrintService printService = new PrintService();

            try
            {
                if (date2 == null && date1 != null && orders.Count() > 0)
                {
                    string fir_date = ConvertDateFormat(date1);
                    printService.PrintOrderReport(orders, "du " + fir_date);

                }

                else if (date2 != null && date1 == null && orders.Count() > 0)
                {
                    string sec_date = ConvertDateFormat(date2);
                    printService.PrintOrderReport(orders, "du " + sec_date);
                }

                else if (date2 != null && date1 != null && orders.Count() > 0)
                {
                    string fir_date = ConvertDateFormat(date1);
                    string sec_date = ConvertDateFormat(date2);
                    printService.PrintOrderReport(orders, "du " + fir_date + " au " + sec_date);

                }

                else
                {
                    if (orders.Count() > 0)
                    {
                        printService.PrintOrderReport(orders, "");
                    }

                    else
                    {
                        MessageBox.Show("Rien à imprimer !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void RefOrders()
        {

            dBService.RefreshOrders(orders);

        }

        private string ConvertDateToSave(string date)
        {
            try
            {
                if (date != null)
                {
                    string[] brut_date = date.Split(' ');
                    string[] nums_date = brut_date[0].Split('/');
                    string month = nums_date[1];
                    string year = nums_date[2];
                    string day = nums_date[0];
                    
                    nums_date[0] = year;
                    nums_date[1] = month;
                    nums_date[2] = day;
                    return nums_date[0] + "-" + nums_date[2] + "-" + nums_date[1];
                }
            }
            catch (Exception)
            {

            }
            return " ";
        }

         private string ConvertDateFormat(string date)
        {
            try
            {
                if (date != null)
                {
                    string[] brut_date = date.Split(' ');
                    string[] nums_date = brut_date[0].Split('/');
                    string month = nums_date[0];
                    string year = nums_date[2];
                    string day = nums_date[1];
                    nums_date[0] = year;
                    nums_date[1] = month;
                    nums_date[2] = day;

                    return nums_date[2] + "-" + nums_date[1] + "-" + nums_date[0];
                }
            }
            catch (Exception)
            {

            }
            return " ";
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
