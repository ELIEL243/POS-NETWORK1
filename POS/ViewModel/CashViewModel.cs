using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;
using POS.Services;
using Prism.Commands;
using POS.Views.DataEntry;
using POS.Views.DataInfo;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POS.ViewModel
{
    public class CashViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<Item> items { get; set; }
        public ObservableCollection<LineItem> line_items { get; set; }
        public Item selected_item { get; set; }
        public DelegateCommand search_box { get; set; }
        public DelegateCommand del_btn { get; set; }
        public DelegateCommand edit_btn { get; set; }
        public DelegateCommand change_devise { get; set; }
        public DelegateCommand complete_order { get; set; }
        public DelegateCommand SelectItem { get; set; }
        public DelegateCommand remainder_calculate { get;set; }
        public DelegateCommand valider_reste { get; set; }
        public string search_item { get; set; }
        public ItemDbService dbService_item { get; set; }
        public TauxDbService dbService_taux { get; set; }
        public CashDbService dbService { get;set; }
        public LineItem selected_line_item { get;set;}
        public string sub_total { get; set; }
        public string total { get; set; }
        public string total2 { get; set; }
        public string total_dol { get; set; }
        public string reste_dol { get; set; }
        public string tax { get; set; }
        public string qts { get; set; }
        public string unit { get; set; }
        public string amount_dumped { get; set; }
        public string amount_dumped_dol { get; set; }
        public string reste { get; set; }
        public string[] devises { get; set; }
        public string selected_devise { get; set; }
        public Order current_order { get; set; }
        public Backup backup { get; set; }
        RemainderDataEntry remainderDataEntry { get; set; } 
        public event PropertyChangedEventHandler PropertyChanged;
        public CashViewModel()
        {
            items = new ObservableCollection<Item>();
            dbService_item = new ItemDbService();
            dbService_taux = new TauxDbService();
            items = dbService_item.GetItems();
            dbService = new CashDbService();
            line_items = new ObservableCollection<LineItem>();
            line_items = dbService.GetLineItems(dbService.GetOrder());
            search_box = new DelegateCommand(SearchItem);
            SelectItem = new DelegateCommand(AddCart);
            del_btn = new DelegateCommand(DelLineItem);
            edit_btn = new DelegateCommand(EditLineItem);
            complete_order = new DelegateCommand(CompleteOrder);
            remainder_calculate = new DelegateCommand(GetRemainder);
            change_devise = new DelegateCommand(ChangeDevise);
            valider_reste = new DelegateCommand(CompleteReste);
            current_order = new Order();
            devises = new string[] {"Fc", "$"};
            current_order = dbService.GetOrder();
            total = dbService.GetTotalLineItems(current_order).ToString() + " Fc";
            sub_total = dbService.GetSubTotalLineItems(current_order).ToString() + " Fc";
            tax = dbService.GetTotalTaxLineItems(current_order).ToString() + " Fc";
            backup = new Backup();
        }

        private void CompleteReste()
        {
            remainderDataEntry.Close();
            Order order = dbService.GetOrder();
            order.heure = DateTime.Now.ToString("HH:mm");
            order.date = DateTime.Now.Date.ToString("yyyy-M-d");
            order.total = float.Parse(total.Split(' ')[0]);
            order.tax = float.Parse(tax.Split(' ')[0]);
            order.sub_total = float.Parse(sub_total.Split(' ')[0]);
            if(float.Parse(reste) > -1)
            {
                PrintService printService = new PrintService();
                printService.PrintInvoiceOrder(line_items, DateTime.Now.Date.ToString("dd/M/yyyy") + " "+order.heure, dbService.GetOrder().id.ToString(), total, sub_total, tax, true);
                backup.SaveUpdates();
                dbService.CompleteOrder(order);
                MessageBox.Show("Vendue(s) avec succes !", "Good", MessageBoxButton.OK, MessageBoxImage.Information);
                line_items.Clear();
                current_order = dbService.GetOrder();
                SetTotal();

            }
        }

        private void ChangeDevise()
        {
            GetRemainder();
        }

        private void GetRemainder()
        {
            float rst = 0;
            float rst_dol = 0;
            Taux taux1 = dbService_taux.GetTaux();
            int taux = taux1.Tx;
            if (amount_dumped != "" && amount_dumped_dol == null || amount_dumped != "" && amount_dumped_dol == "")
            {
                rst = float.Parse(amount_dumped) - float.Parse(total2);
                rst_dol = rst / taux;
                reste = rst.ToString();
                reste_dol = rst_dol.ToString();
                OnPropertyChanged("reste");
                OnPropertyChanged("reste_dol");

            }

            else if (!string.IsNullOrEmpty(amount_dumped_dol) && amount_dumped == null || !string.IsNullOrEmpty(amount_dumped_dol) && amount_dumped == "")
            {
                amount_dumped = "0";
                rst = (float.Parse(amount_dumped)*taux) - float.Parse(total2);
                rst_dol = float.Parse(amount_dumped_dol) - float.Parse(total_dol);
                reste = rst.ToString();
                reste_dol = rst_dol.ToString();
                OnPropertyChanged("reste");
                OnPropertyChanged("reste_dol");
            }

            else if(amount_dumped != "" && amount_dumped_dol != "")
            {
                rst = (float.Parse(amount_dumped) + float.Parse(amount_dumped_dol) * taux) - float.Parse(total2);
                rst_dol = ((float.Parse(amount_dumped)/taux)+float.Parse(amount_dumped_dol)) - float.Parse(total_dol);
                reste = rst.ToString();
                reste_dol = rst_dol.ToString();
                OnPropertyChanged("reste");
                OnPropertyChanged("reste_dol");

            }
        }

        private void CompleteOrder()
        {
            remainderDataEntry = new RemainderDataEntry();
            remainderDataEntry.DataContext = this;
            Taux taux1 = dbService_taux.GetTaux();
            int taux = taux1.Tx;

            if (line_items.Count > 0)
            {
                total2 = total.Split(' ')[0];
                if (taux == 1)
                {
                    total_dol = "0";

                }
                else
                {
                    total_dol = (float.Parse(total2) / taux).ToString();
                }
                
                remainderDataEntry.ShowDialog();

            }
        }

        private void EditLineItem()
        {
            Qts qts1 = new Qts();
            qts1.DataContext = this;
            qts = selected_line_item.Qts.ToString();
            qts1.ShowDialog();
            
            if (int.Parse(qts) > 0)
            {
                selected_line_item.Qts = int.Parse(qts);
                dbService.UpdateLineItem(selected_line_item);
                backup.SaveUpdates();
                dbService.RefreshLineItems(current_order, line_items);
                MessageBox.Show("Quantité modifiée avec succes !", "Good", MessageBoxButton.OK, MessageBoxImage.Information);
                SetTotal();
            }
        }

        private void AddCart()
        {
            Qts qts1 = new Qts();
            qts1.DataContext = this;
            qts1.ShowDialog();
            if (!string.IsNullOrEmpty(qts))
            {
                if (int.Parse(qts) > 0)
                {
                    if (int.Parse(qts) <= selected_item.Qts)
                    {
                        LineItem lineItem = new LineItem();
                        lineItem.Order = current_order.id;
                        lineItem.Desc = selected_item.Description;
                        lineItem.Price = selected_item.Price;
                        lineItem.Unit = selected_item.Unit;
                        lineItem.Qts = int.Parse(qts);
                        if (dbService.IsDuplicated(lineItem))
                        {
                            dbService.ResolveDuplicated(lineItem);
                            backup.SaveUpdates();
                            dbService.RefreshLineItems(current_order, line_items);
                            MessageBox.Show("Ajouté avec succes !", "Good", MessageBoxButton.OK, MessageBoxImage.Information);
                            SetTotal();
                        }
                        else
                        {
                            dbService.AddLineItem(lineItem);
                            backup.SaveUpdates();
                            dbService.RefreshLineItems(current_order, line_items);
                            MessageBox.Show("Ajouté avec succes !", "Good", MessageBoxButton.OK, MessageBoxImage.Information);
                            SetTotal();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Quantité insuffisante en stock !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }

            }




        }

        private void SearchItem()
        {
            dbService_item.SearchItem(search_item, items);
        }

        private void DelLineItem()
        {
            if (selected_line_item != null)
            {
                if (MessageBox.Show($"Voulez-vous supprimer : {selected_line_item.Desc} ?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    dbService.DeleteLineItem(selected_line_item);
                    backup.SaveUpdates();
                    line_items.Remove(selected_line_item);
                    MessageBox.Show("Supprimé avec succes !", "Good", MessageBoxButton.OK, MessageBoxImage.Information);
                    SetTotal();
                }

            }
        }

        private void SetTotal()
        {
            total = dbService.GetTotalLineItems(current_order).ToString() + " Fc";
            sub_total = dbService.GetSubTotalLineItems(current_order).ToString() + " Fc";
            tax = dbService.GetTotalTaxLineItems(current_order).ToString() + " Fc";
            OnPropertyChanged("total");
            OnPropertyChanged("sub_total");
            OnPropertyChanged("tax");
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
