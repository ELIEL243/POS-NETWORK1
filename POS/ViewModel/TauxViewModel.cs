using POS.Models;
using POS.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POS.ViewModel
{
    public class TauxViewModel
    {
        public TauxDbService dbService { get; set; }
        public Taux current_taux { get; set; }
        public Taux taux { get; set; }
        public DelegateCommand val_btn { get; set; }

        public TauxViewModel()
        {
            dbService = new TauxDbService();
            current_taux = dbService.GetTaux();
            val_btn = new DelegateCommand(SaveTaux);
        }

        private void SaveTaux()
        {
            if(current_taux.Tx > 0 && current_taux != null)
            {
                dbService.SaveTaux(current_taux);
                MessageBox.Show("Taux modifié avec succes !", "Good", MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }
    }
}
