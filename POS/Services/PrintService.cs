using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System.Collections.ObjectModel;
using POS.Models;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using Microsoft.Win32;
using Stubble.Core.Settings;
using System.Security.Principal;
using System.ComponentModel;

namespace POS.Services
{
    public class PrintService: INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;

        public void AddCell(string str, Font f, BaseColor c, PdfPTable t)
        {
            PdfPCell cell1 = new PdfPCell(new Phrase(str, f));
            cell1.BackgroundColor = c;
            cell1.Padding = 7;
            t.AddCell(cell1);

        }

        public iTextSharp.text.pdf.PdfWriter PdfWriter_GetInstance(iTextSharp.text.Document document, System.IO.FileStream filestrm)
        {
            iTextSharp.text.pdf.PdfWriter pdfwr = null;

            for (int repeat = 0; repeat < 6; repeat++)
            {
                try
                {
                    pdfwr = iTextSharp.text.pdf.PdfWriter.GetInstance(document, filestrm); 
                  break; //created, then exit loop
                }
                catch // instantiation of PdfWriter failed, then pause
                {
                    System.Threading.Thread.Sleep(300);
                }
            }
            if (pdfwr == null)
            {
                throw new Exception("iTextSharp PdfWriter was not instantiated");
            }

            return pdfwr;
        }

        public void PrintPurchaseReport(ObservableCollection<Purchase> purchases, string date)
        {
            Purchase purchase1 = new Purchase();
            var id = DateTime.Now.Ticks;
            string outFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"/facture{id}.pdf";
            FileStream fs = new FileStream(outFile, FileMode.Create);

            Document doc = new Document(PageSize.A4, 25, 25, 25, 15);
            //Thread.Sleep(1000);
            
            Console.WriteLine(outFile);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            //

            BaseColor blue = new BaseColor(0, 75, 155);
            BaseColor gris = BaseColor.DARK_GRAY;
            BaseColor blanc = new BaseColor(255, 255, 255);

            Font police = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14f, iTextSharp.text.Font.NORMAL, blue);
            Font police2 = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18f, iTextSharp.text.Font.NORMAL, gris);
            Font police3 = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18f, iTextSharp.text.Font.NORMAL, blanc);

            string logo_path = @"medicine.png";

            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logo_path);
            logo.Alignment = Element.ALIGN_CENTER;
            logo.ScaleAbsoluteWidth(45);
            logo.ScaleAbsoluteHeight(45);
            //logo.Height = 45;
            doc.Add(logo);

            iTextSharp.text.Paragraph p = new Paragraph("Pharmacie de l'infirmière, 1101, Av.IKUKU\nC/DILALA\nKOLWEZI, LUALABA\nCD/K'ZI/RCCM/14-A-062\nID NAT 441/01492\n", police);
            p.Alignment = Element.ALIGN_LEFT;
            doc.Add(p);
            

            Paragraph p5 = new Paragraph(DateTime.Now.ToString("dddd dd MMMM yyyy") + "\n\n", police);
            p5.Alignment = Element.ALIGN_LEFT;
            doc.Add(p5);


            Paragraph p3 = new Paragraph($"INVENTAIRE D'ENTREES {date}\n\n", police);
            p3.Alignment = Element.ALIGN_CENTER;
            doc.Add(p3);

            iTextSharp.text.pdf.draw.LineSeparator line = new LineSeparator(1f, 90f, blue, Element.ALIGN_CENTER, 1);
            doc.Add(line);
            doc.Add(new Paragraph("\n\n"));

            //Creation de la table

            PdfPTable table = new PdfPTable(4);
            table.WidthPercentage = 100;

            AddCell("No", police3, blue, table);
            AddCell("Article", police3, blue, table);
            AddCell("Qts", police3, blue, table);
            AddCell("Total", police3, blue, table);

            //Lister les produits

            foreach (Purchase purchase in purchases)
            {
                AddCell(purchase.Id.ToString(), police2, blanc, table);
                AddCell(purchase.Item, police2, blanc, table);
                AddCell(purchase.Qts.ToString(), police2, blanc, table);
                AddCell(purchase.Total.ToString(), police2, blanc, table);

            }
            doc.Add(table);
            doc.Add(new Phrase("\n\n"));
            Paragraph p4 = new Paragraph($"Total: {purchase1.GetTotalPurchase(purchases)} FC\n", police);
            p4.Alignment = Element.ALIGN_RIGHT;
            doc.Add(p4);
            doc.Close();
            Console.WriteLine(outFile);
            Process.Start(outFile);
        }

        public void PrintOrderReport(ObservableCollection<Order> orders, string date)
        {
            Order order1 = new Order();
            var id = DateTime.Now.Ticks;
            string outFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"/facture{id}.pdf";
            FileStream fs = new FileStream(outFile, FileMode.Create);

            Document doc = new Document(PageSize.A4, 25, 25, 25, 15);
            //Thread.Sleep(1000);

            Console.WriteLine(outFile);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            //

            BaseColor blue = new BaseColor(0, 75, 155);
            BaseColor gris = BaseColor.DARK_GRAY;
            BaseColor blanc = new BaseColor(255, 255, 255);

            Font police = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18f, iTextSharp.text.Font.NORMAL, blue);
            Font police2 = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18f, iTextSharp.text.Font.NORMAL, gris);
            Font police3 = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18f, iTextSharp.text.Font.NORMAL, blanc);
            string logo_path = @"medicine.png";

            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logo_path);
            logo.Alignment = Element.ALIGN_CENTER;
            logo.ScaleAbsoluteWidth(45);
            logo.ScaleAbsoluteHeight(45);
            //logo.Height = 45;
            doc.Add(logo);

            iTextSharp.text.Paragraph p = new Paragraph("Pharmacie de l'infirmière, 1101, Av.IKUKU\nC/DILALA\nKOLWEZI, LUALABA\nCD/K'ZI/RCCM/14-A-062\nID NAT 441/01492\n", police);
            p.Alignment = Element.ALIGN_LEFT;
            doc.Add(p);

            Paragraph p5 = new Paragraph(DateTime.Now.ToString("dddd dd MMMM yyyy") + "\n\n", police);
            p5.Alignment = Element.ALIGN_LEFT;
            doc.Add(p5);


            Paragraph p3 = new Paragraph($"INVENTAIRE DES VENTES {date}\n\n", police);
            p3.Alignment = Element.ALIGN_CENTER;
            doc.Add(p3);

            iTextSharp.text.pdf.draw.LineSeparator line = new LineSeparator(1f, 90f, blue, Element.ALIGN_CENTER, 1);
            doc.Add(line);
            doc.Add(new Paragraph("\n\n"));

            //Creation de la table

            PdfPTable table = new PdfPTable(4);
            table.WidthPercentage = 100;

            AddCell("No", police3, blue, table);
            AddCell("Date", police3, blue, table);
            AddCell("Heure", police3, blue, table);
            AddCell("Total", police3, blue, table);

            //Lister les produits

            foreach (Order order in orders)
            {
                AddCell(order.id.ToString(), police2, blanc, table);
                AddCell(order.date, police2, blanc, table);
                AddCell(order.heure.ToString(), police2, blanc, table);
                AddCell(order.total.ToString(), police2, blanc, table);

            }
            doc.Add(table);
            doc.Add(new Phrase("\n\n"));
            Paragraph p4 = new Paragraph($"Total: {order1.GetTotalOrders(orders)} FC\n", police);
            p4.Alignment = Element.ALIGN_RIGHT;
            doc.Add(p4);
            doc.Close();
            Console.WriteLine(outFile);
            Process.Start(outFile);
        }

        public void PrintInvoiceOrder(ObservableCollection<LineItem> lineItems, string date, string order_no, string total, string sub_total, string tax, bool open_nav=true)
        {
            var id = DateTime.Now.Ticks;
            string outFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"/facture{id}.pdf";
            FileStream fs = new FileStream(outFile, FileMode.Create);

            Document doc = new Document(PageSize.A6, 25, 25, 25, 15);
            //Thread.Sleep(1000);

            Console.WriteLine(outFile);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            //

            BaseColor blue = new BaseColor(0, 75, 155);
            BaseColor gris = BaseColor.DARK_GRAY;
            BaseColor blanc = new BaseColor(255, 255, 255);

            Font police = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, blue);
            Font police2 = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, gris);
            Font police3 = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, blanc);
            string logo_path = @"medicine.png";

            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logo_path);
            logo.Alignment = Element.ALIGN_CENTER;
            logo.ScaleAbsoluteWidth(45);
            logo.ScaleAbsoluteHeight(45);
            //logo.Height = 45;
            doc.Add(logo);

            iTextSharp.text.Paragraph p = new Paragraph("Pharmacie de l'infirmière, 1101, Av.IKUKU\nC/DILALA\nKOLWEZI, LUALABA\nCD/K'ZI/RCCM/14-A-062\nID NAT 441/01492\n", police);
            p.Alignment = Element.ALIGN_LEFT;
            doc.Add(p);

            Paragraph p5 = new Paragraph(DateTime.Now.ToString("dddd dd MMMM yyyy") + "\n\n", police);
            p5.Alignment = Element.ALIGN_LEFT;
            doc.Add(p5);


            Paragraph p3 = new Paragraph($"FACTURE N°{order_no} DU {date}\n\n", police);
            p3.Alignment = Element.ALIGN_CENTER;
            doc.Add(p3);

            iTextSharp.text.pdf.draw.LineSeparator line = new LineSeparator(1f, 90f, blue, Element.ALIGN_CENTER, 1);
            doc.Add(line);
            doc.Add(new Paragraph("\n\n"));

            //Creation de la table

            PdfPTable table = new PdfPTable(4);
            table.WidthPercentage = 100;

            AddCell("Article", police3, blue, table);
            AddCell("Qts", police3, blue, table);
            AddCell("Prix", police3, blue, table);
            AddCell("Total", police3, blue, table);

            //Lister les produits

            foreach (LineItem line1 in lineItems)
            {
                AddCell(line1.Desc, police2, blanc, table);
                AddCell(line1.Qts.ToString(), police2, blanc, table);
                AddCell(line1.Price.ToString(), police2, blanc, table);
                AddCell(line1.Total.ToString(), police2, blanc, table);

            }
            doc.Add(table);
            doc.Add(new Phrase("\n\n"));
            Paragraph p4 = new Paragraph($"Total: {total.Split(' ')[0]} FC\n", police);
            Paragraph p7 = new Paragraph($"Sous-Total: {sub_total.Split(' ')[0]} FC\n", police);
            Paragraph p8 = new Paragraph($"Tax: {tax.Split(' ')[0]} FC\n", police);
            p4.Alignment = Element.ALIGN_RIGHT;
            p7.Alignment = Element.ALIGN_RIGHT;
            p8.Alignment = Element.ALIGN_RIGHT;
            doc.Add(p4);
            doc.Add(p7);
            doc.Add(p8);
            doc.Close();
            Console.WriteLine(outFile);
            if (open_nav)
            {
                Process.Start(outFile);
            }


        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }


    }
}
