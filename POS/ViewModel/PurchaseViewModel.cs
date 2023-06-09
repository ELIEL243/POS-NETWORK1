﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using POS.Models;
using POS.Services;
using Prism.Commands;
using POS.Views.DataEntry;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System.Threading;


namespace POS.ViewModel
{
    public class PurchaseViewModel: INotifyPropertyChanged
    {
        #region Properties
        public ObservableCollection<Item> items { get; set; }
        public ObservableCollection<Purchase> purchases { get; set; }
        public ObservableCollection<Purchase> unpurchases { get; set; }
        public ItemDbService dbService_item { get; set; }
        public PurchaseDbService dbService { get; set; }
        public Item selected_item { get; set; }
        public Purchase selected_purchase { get; set; }
        public DelegateCommand add_btn { get; set; }
        public DelegateCommand del_purchase { get; set; }
        public DelegateCommand del_btn { get; set; }
        public DelegateCommand edit_btn { get; set; }
        public DelegateCommand ref_btn { get; set; }
        public DelegateCommand print_btn { get; set; }
        public DelegateCommand complete_btn { get; set; }
        public DelegateCommand search_box { get; set; }
        public DelegateCommand search_date { get; set; }
        public DelegateCommand SelectItem { get; set; }
        public Purchase purchase { get; set; }
        public string qts { get; set; }
        public string price { get; set; }
        public string date1 { get; set; }
        public string date2 { get; set; }
        public string unit = "$";
        public string search_item { get; set; }
        public string total_purchase { get; set; }
        public Backup backup { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public PurchaseViewModel()
        {
            items = new ObservableCollection<Item>();
            dbService_item = new ItemDbService();
            items = dbService_item.GetItems();
            unpurchases = new ObservableCollection<Purchase>();
            purchases = new ObservableCollection<Purchase>();
            dbService = new PurchaseDbService();
            purchases = dbService.GetPurchases();
            unpurchases = dbService.GetUnCompletedPurchases();
            add_btn = new DelegateCommand(AddPurchase);
            del_btn = new DelegateCommand(DelPurchase);
            del_purchase = new DelegateCommand(DelPurchase);
            edit_btn = new DelegateCommand(EditPurchase);
            ref_btn = new DelegateCommand(RefreshPurchases);
            print_btn = new DelegateCommand(PrintPurchases);
            search_box = new DelegateCommand(SearchPurchase);
            SelectItem = new DelegateCommand(SelectProd);
            complete_btn = new DelegateCommand(CompletePurchase);
            search_date = new DelegateCommand(SearchDate);
            backup = new Backup();
            
            
        }

        private void PrintPurchases()
        {
            PrintService printService = new PrintService();

            try
            {
                if (date2 == null && date1 != null && purchases.Count() > 0)
                {
                    string fir_date = ConvertDateFormat(date1);
                    printService.PrintPurchaseReport(purchases, "du "+fir_date);

                }

                else if (date2 != null && date1 == null && purchases.Count() > 0)
                {
                    string sec_date = ConvertDateFormat(date2);
                    printService.PrintPurchaseReport(purchases, "du "+sec_date);
                }

                else if (date2 != null && date1 != null && purchases.Count() > 0)
                {
                    string fir_date = ConvertDateFormat(date1);
                    string sec_date = ConvertDateFormat(date2);
                    printService.PrintPurchaseReport(purchases, "du "+fir_date+" au "+sec_date);

                }

                else
                {
                    if (purchases.Count() > 0)
                    {
                        printService.PrintPurchaseReport(purchases, "");
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

        private void RefreshPurchases()
        {
            dbService.Refresh(purchases);
            total_purchase = "";
            OnPropertyChanged("total_purchase");

        }

        private void SearchDate()
        {
            purchase = new Purchase();
            
            try
            {
                if (date2 == null && date1 != null)
                {
                    string fir_date = ConvertDateToSave(date1);
                    dbService.SearchPurchases(purchases, fir_date);
                    total_purchase = purchase.GetTotalPurchase(purchases).ToString()+" Fc";
                    OnPropertyChanged("total_purchase");

                }

                else if (date2 != null && date1 == null)
                {
                    string sec_date = ConvertDateToSave(date2);
                    dbService.SearchPurchases(purchases, sec_date);
                    total_purchase = purchase.GetTotalPurchase(purchases).ToString() + " Fc";
                    OnPropertyChanged("total_purchase");
                }

                else if(date2 != null && date1 != null)
                {
                    string fir_date = ConvertDateToSave(date1);
                    string sec_date = ConvertDateToSave(date2);
                    dbService.SearchPurchasesBetween(purchases, fir_date, sec_date);
                    total_purchase = purchase.GetTotalPurchase(purchases).ToString() + " Fc";
                    OnPropertyChanged("total_purchase");
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void CompletePurchase()
        {
            dbService.CompletePurchase(unpurchases);
            dbService.RefreshUnCompleted(unpurchases);
            dbService_item.Refresh(items);
            dbService.Refresh(purchases);
            MessageBox.Show("Achat(s) validés avec succes !", "Good", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SelectProd()
        {
            purchase = new Purchase();
            QtsPrice qtsPrice = new QtsPrice();
            qtsPrice.DataContext = this;
            qtsPrice.ShowDialog();
            if (!string.IsNullOrEmpty(qts) && !string.IsNullOrEmpty(price)) { 
                if (int.Parse(qts) > 0 && int.Parse(price) > 0)
                {
                    purchase.Qts = int.Parse(qts);
                    purchase.Purchase_price = float.Parse(price);
                    purchase.Item = selected_item.Description;
                    purchase.Total = purchase.Purchase_price;
                    purchase.Date = DateTime.Now.ToString("yyyy-M-d");
                    
                    if (dbService.IsDuplicated(purchase) == false)
                    {
                       
                        dbService.AddPurchase(purchase);
                        backup.SaveUpdates();
                        dbService.RefreshUnCompleted(unpurchases);
                        
                    }
                    else
                    {
                        dbService.ResolveDuplicated(purchase);
                        backup.SaveUpdates();
                        dbService.RefreshUnCompleted(unpurchases);
                    }
                    purchase = null;
                    price = null;
                    qts = null;
                    MessageBox.Show("Achat ajouté avec succes !", "Good", MessageBoxButton.OK, MessageBoxImage.Information);
                } 
             }
        }

        private void AddPurchase()
        {
            throw new NotImplementedException();
        }

        private void DelPurchase()
        {
            if (selected_purchase != null)
            {
                if (MessageBox.Show($"Voulez-vous supprimer  l'arcticle : {selected_purchase.Item} ?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    dbService.DelPurchase(selected_purchase);
                    backup.SaveUpdates();
                    unpurchases.Remove(selected_purchase);
                    purchases.Remove(selected_purchase);
                    //selected_purchase = null;
                }

            }
        }

        private void EditPurchase()
        {
            if (selected_purchase != null)
            {
                QtsPrice qtsPrice = new QtsPrice();
                qts = selected_purchase.Qts.ToString();
                price = selected_purchase.Purchase_price.ToString();
                qtsPrice.DataContext = this;
                qtsPrice.ShowDialog();

                if (MessageBox.Show("Voulez-vous modifier ?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (!string.IsNullOrEmpty(qts) && !string.IsNullOrEmpty(price))
                    {
                        selected_purchase.Qts = int.Parse(qts);
                        selected_purchase.Purchase_price = float.Parse(price);
                        dbService.EditPurchase(selected_purchase);
                        backup.SaveUpdates();
                        dbService.RefreshUnCompleted(unpurchases);
                    }
                    else
                    {
                        MessageBox.Show("Veuillez remplir le champs !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        dbService.RefreshUnCompleted(unpurchases);
                    }


                }
                else
                {
                    //units.Clear();
                    dbService.RefreshUnCompleted(unpurchases);

                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner un objet !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchPurchase()
        {
            dbService_item.SearchItem(search_item, items);
        }

        private string ConvertDate(string date)
        {
            try
            {
                if (date != null)
                {
                    string[] brut_date = date.Split(' ');
                    string[] nums_date = brut_date[0].Split('/');
                    string temp = nums_date[1];
                    nums_date[1] = nums_date[0];
                    nums_date[0] = temp;

                    return nums_date[0] + " " + nums_date[1] + " " + nums_date[2];
                }
            }
            catch (Exception)
            {

            }
            return " ";
        }

        private string ConvertDateToSave(string date)
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
                    return nums_date[0] + "-" + nums_date[1] + "-" + nums_date[2];
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
