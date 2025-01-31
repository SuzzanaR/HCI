using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PetShop
{
    public partial class Racuni : Page
    {

        public List<Bill> Bills { get; }
      


        public static class BillManager
{
    private static string connectionString = "Server=localhost;Database=mydb;User ID=root;Password=suzana;";

    public static List<Bill> GetAllBills()
    {
        var bills = new List<Bill>();

        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                string query = "SELECT RACUN.RacunID, RACUN.DatumVrijeme, RACUN.CijenaUkupna, ZAPOSLENI.Ime, ZAPOSLENI.Prezime " +
                               "FROM RACUN " +
                               "INNER JOIN ZAPOSLENI ON RACUN.ZAPOSLENI_ZaposleniID = ZAPOSLENI.ZaposleniID";

                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var bill = new Bill
                            {
                                Id = reader.GetInt32("RacunID"),
                                Date = reader.GetDateTime("DatumVrijeme").ToString("yyyy-MM-dd HH:mm:ss"),
                                Employee = $"{reader.GetString("Ime")} {reader.GetString("Prezime")}",
                                TotalPrice = (double)reader.GetDecimal("CijenaUkupna"),
                                Items = new List<BillItem>()  
                            };
                            bills.Add(bill);
                        }
                    }
                }

                foreach (var bill in bills)
                {
                    using (var itemConnection = new MySqlConnection(connectionString))
                    {
                        itemConnection.Open();

                        string itemQuery = "SELECT STAVKA.Cijena, STAVKA.Kolicina, ARTIKAL.NazivArtikla " +
                                           "FROM STAVKA " +
                                           "INNER JOIN ARTIKAL ON STAVKA.ARTIKAL_ArtikalID = ARTIKAL.ArtikalID " +
                                           "WHERE STAVKA.RACUN_RacunID = @RacunID";

                        using (var command = new MySqlCommand(itemQuery, itemConnection))
                        {
                            command.Parameters.AddWithValue("@RacunID", bill.Id);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var billItem = new BillItem
                                    {
                                        Article = new Article
                                        {
                                            Name = reader.GetString("NazivArtikla"),
                                            Price = (double)reader.GetDecimal("Cijena") 
                                        },
                                        Quantity = reader.GetInt32("Kolicina"),
                                        TotalPrice = (double)(reader.GetDecimal("Cijena") * reader.GetInt32("Kolicina"))
                                    };

                                    bill.Items.Add(billItem);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        return bills;
    }
}





        public Racuni()
        {
            InitializeComponent();

            Bills = BillManager.GetAllBills();

            Bills = Bills.OrderByDescending(b => DateTime.Parse(b.Date)).ToList();

            for (int i = 0; i < Bills.Count; i++)
            {
                Bills[i].Id = i + 1;  
            }

            BillsListBox.ItemsSource = Bills;  
        }










        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DateFilterPicker.SelectedDate = null;
            var bills = BillManager.GetAllBills();

            bills = bills.OrderByDescending(b => DateTime.Parse(b.Date)).ToList();

            for (int i = 0; i < bills.Count; i++)
            {
                bills[i].Id = i + 1;  
            }

            BillsListBox.ItemsSource = bills;
        }








        private void DateFilterPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? selectedDate = DateFilterPicker.SelectedDate;

            if (selectedDate.HasValue)
            {
                var filteredBills = BillManager.GetAllBills()
                                                .Where(b => DateTime.Parse(b.Date).Date == selectedDate.Value.Date)
                                                .ToList();

                for (int i = 0; i < filteredBills.Count; i++)
                {
                    filteredBills[i].Id = i + 1;
                }

                BillsListBox.ItemsSource = filteredBills;  
            }
            else
            {
                BillsListBox.ItemsSource = BillManager.GetAllBills();
            }
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var bill = button?.DataContext as Bill;

            if (bill != null)
            {
                string billDetails = $"{bill.Date}\nEmployee: {bill.Employee}\nTotal: {bill.TotalPrice:N2} KM\n\n";

                foreach (var item in bill.Items)
                {
                    billDetails += $"{item.Article.Name} - {item.Quantity} x {item.Article.Price:N2} KM = {item.TotalPrice:N2} KM\n";
                }


                BillDetailsTextBlock.Text = billDetails;
                BillDetailsTextBlock.Visibility = Visibility.Visible; 

                CloseDetailsButton.Visibility = Visibility.Visible;
            }
        }

       
        private void CloseDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            BillDetailsTextBlock.Visibility = Visibility.Collapsed;
            CloseDetailsButton.Visibility = Visibility.Collapsed;
        }
    }

    public class Bill
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Employee { get; set; }
        public double TotalPrice { get; set; }
        public List<BillItem> Items { get; set; }
    }

    public static class BillManager
    {
        private static List<Bill> _bills = new List<Bill>();

        public static void AddBill(Bill bill)
        {
            _bills.Add(bill);
        }

        public static List<Bill> GetAllBills()
        {
            return _bills;
        }
    }



}
