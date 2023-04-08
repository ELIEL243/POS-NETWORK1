using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using POS.Views;
using POS.ViewModel;
using Stubble.Core.Loaders;
using System.Diagnostics;

namespace POS
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Button> buttonList { get; set; }
        public string current_role { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            buttonList = new List<Button>() { user_btn,setting_btn,unit_btn,category_btn,item_btn,sale_btn,invoice_btn };
            SelectMenu(stock_btn, buttonList);
            UCStock uCStock = new UCStock();
            uCStock.DataContext = new PurchaseViewModel();
            container.Content = uCStock;
        }

        public void CheckRole(string role)
        {
            if(role == "caissier")
            {
                user_btn.IsEnabled = false;
                setting_btn.IsEnabled = false;
                stock_btn.IsEnabled = false;
                unit_btn.IsEnabled = false;
                category_btn.IsEnabled = false;
                sale_btn.IsEnabled=false;
                item_btn.IsEnabled = false;
                setting_db_btn.IsEnabled = false;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            buttonList = new List<Button>() { user_btn, setting_btn, stock_btn, category_btn, item_btn, sale_btn, invoice_btn };
            SelectMenu(unit_btn, buttonList);

            UCUnit uCUnit = new UCUnit();
            uCUnit.DataContext = new UnitViewModel();
            container.Content = uCUnit;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            buttonList = new List<Button>() { user_btn, setting_btn, stock_btn, unit_btn, item_btn, sale_btn, invoice_btn };
            SelectMenu(category_btn, buttonList);

            UCCategory uCCategory = new UCCategory();
            uCCategory.DataContext = new CategoryViewModel();
            container.Content = uCCategory;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            buttonList = new List<Button>() { user_btn, setting_btn, stock_btn, unit_btn, category_btn, sale_btn, invoice_btn };
            SelectMenu(item_btn, buttonList);

            UCItem uCItem = new UCItem();
            uCItem.DataContext = new ItemViewModel();
            container.Content = uCItem;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            buttonList = new List<Button>() { user_btn, setting_btn, stock_btn, unit_btn, category_btn, sale_btn, item_btn };
            SelectMenu(invoice_btn, buttonList);

            UCCash uCCash = new UCCash();
            uCCash.DataContext = new CashViewModel();
            container.Content = uCCash;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            buttonList = new List<Button>() { user_btn, setting_btn, stock_btn, unit_btn, category_btn, invoice_btn, item_btn };
            SelectMenu(sale_btn, buttonList);

            UCSaleReport uCSaleReport = new UCSaleReport();
            uCSaleReport.DataContext = new SaleReportViewModel();
            container.Content = uCSaleReport;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            buttonList = new List<Button>() { setting_btn, sale_btn, stock_btn, unit_btn, category_btn, invoice_btn, item_btn };
            SelectMenu(user_btn, buttonList);

            UCSettings uCSSettings = new UCSettings();
            uCSSettings.DataContext = new UserViewModel();
            container.Content = uCSSettings;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            buttonList = new List<Button>() { user_btn, sale_btn, stock_btn, unit_btn, category_btn, invoice_btn, item_btn };
            SelectMenu(setting_btn, buttonList);
            UCTaux uCTaux = new UCTaux();
            uCTaux.DataContext = new TauxViewModel();
            container.Content = uCTaux;
        }
        private void SelectMenu(Button selected_menu, List<Button> buttons)
        {
            BrushConverter bc = new BrushConverter();
            Brush selected_color = (Brush)bc.ConvertFrom("#FF5873BA");
            Brush unselected_color = (Brush)bc.ConvertFrom("#FF11182A");
            selected_menu.Background = selected_color;

            foreach(Button b in buttons)
            {
                b.Background = unselected_color;
            }
        }

        private void logout_btn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
            
        }

        private void setting_db_btn_Click(object sender, RoutedEventArgs e)
        {
            UCDbConfig db_conf = new UCDbConfig();
            db_conf.Show();
        }
    }
}
