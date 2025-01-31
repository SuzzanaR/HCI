using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PetShop
{
    public partial class Artikli : Page
    {
        private List<Article> articles;
        private List<Kategorija> categories;
        private List<BillItem> billItems = new List<BillItem>();
        
        private string connectionString = "Server=localhost;Database=mydb;User ID=root;Password=suzana;";


        public Artikli()
        {
            InitializeComponent();
            LoadArticles();
            LoadCategoriesFilter();



        ArticlesListBox.ItemsSource = articles;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryPopup.IsOpen = !CategoryPopup.IsOpen;
        }


        private void LoadCategoriesFilter()
        {
            categories = new List<Kategorija>();

            string query = "SELECT KategorijaID, NazivKategorije FROM KATEGORIJA";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Kategorija
                            {
                                KategorijaID = reader.GetInt32("KategorijaID"),
                                NazivKategorije = reader.GetString("NazivKategorije")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading categories: " + ex.Message);
                }
            }

            foreach (var category in categories)
            {
                CheckBox checkBox = new CheckBox
                {
                    Content = category.NazivKategorije,
                    Tag = category.KategorijaID,
                    Margin = new Thickness(2)
                };

                CategoryCheckBoxes.Items.Add(checkBox);
            }
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            List<int> selectedCategoryIds = new List<int>();

            foreach (CheckBox checkBox in CategoryCheckBoxes.Items)
            {
                if (checkBox.IsChecked == true)
                {
                    selectedCategoryIds.Add((int)checkBox.Tag);
                }
            }

            LoadArticles(selectedCategoryIds);

            CategoryPopup.IsOpen = false;
        }



        private void LoadArticles(List<int> selectedCategoryIds = null)
        {
            articles = new List<Article>();

            string query = @"
            SELECT a.ArtikalID, a.NazivArtikla, a.Opis, a.Cijena, a.ImageUrl, a.KATEGORIJA_KategorijaID, 
            k.NazivKategorije, p.NazivProizvodjaca
            FROM ARTIKAL a
            JOIN KATEGORIJA k ON a.KATEGORIJA_KategorijaID = k.KategorijaID
            JOIN PROIZVODJAC p ON a.PROIZVODJAC_ProizvodjacID = p.ProizvodjacID";

            if (selectedCategoryIds != null && selectedCategoryIds.Any())
            {
                string categoryFilter = string.Join(",", selectedCategoryIds);
                query += $" WHERE a.KATEGORIJA_KategorijaID IN ({categoryFilter})";
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            articles.Add(new Article
                            {
                                ID = reader.GetInt32("ArtikalID"),
                                Name = reader.GetString("NazivArtikla"),
                                Description = reader.GetString("Opis"),
                                Price = reader.GetDouble("Cijena"),
                                ImageSource = reader.GetString("ImageUrl"),
                                Category = reader.GetString("NazivKategorije"),
                                Manufacturer = reader.GetString("NazivProizvodjaca")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading articles: " + ex.Message);
                }
            }

            ArticlesListBox.ItemsSource = articles;
        }




        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var selectedArticle = button?.DataContext as Article;

            if (selectedArticle != null)
            {
                var billItem = billItems.FirstOrDefault(b => b.Article.Name == selectedArticle.Name);

                if (billItem == null)
                {
                    billItem = new BillItem
                    {
                        Article = selectedArticle,
                        Quantity = 1,
                        TotalPrice = selectedArticle.Price,
                        ImageUrl = selectedArticle.ImageSource,
                        
                    };
                    billItems.Add(billItem);
                }
                else
                {
                    billItem.Quantity++;
                    billItem.TotalPrice = billItem.Article.Price * billItem.Quantity;
                }

                BillListBox.ItemsSource = null;  
                BillListBox.ItemsSource = billItems;

                UpdateTotalPrice();
            }
        }

        private void UpdateTotalPrice()
        {
            double total = billItems.Sum(b => b.TotalPrice);
            TotalPriceText.Text = $"{total:N2} KM";
        }

        private void CreateBillButton_Click(object sender, RoutedEventArgs e)
        {
            string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string employeeName = $"{SessionManager.FirstName} {SessionManager.LastName}";

            var bill = new Bill
            {
                Date = currentDateTime,
                Employee = employeeName,  
                TotalPrice = billItems.Sum(b => b.TotalPrice),
                Items = billItems.ToList()  
            };

           InsertBillIntoDatabase(bill);

            string billContent = $"Bill generated on {currentDateTime}\nEmployee: {employeeName}\n\n";
            foreach (var item in billItems)
            {
                billContent += $"{item.Article.Name} - {item.Quantity} x {item.Article.Price:N2} KM = {item.TotalPrice:N2} KM\n";
            }
            billContent += $"\nTotal: {TotalPriceText.Text}";

            MessageBox.Show(billContent, "Bill Created");

            this.NavigationService.Navigate(new Racuni());
        }


        private void InsertBillIntoDatabase(Bill bill)
        {
            string connectionString = "Server=localhost;Database=mydb;User ID=root;Password=suzana;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string insertBillQuery = @"
                    INSERT INTO RACUN (DatumVrijeme, CijenaUkupna, ZAPOSLENI_ZaposleniID)
                    VALUES (@Date, @TotalPrice, @EmployeeId);
                    SELECT LAST_INSERT_ID();"; 

                    using (MySqlCommand command = new MySqlCommand(insertBillQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Date", bill.Date);
                        command.Parameters.AddWithValue("@TotalPrice", bill.TotalPrice);
                        command.Parameters.AddWithValue("@EmployeeId", SessionManager.UserId); 

                        int billId = Convert.ToInt32(command.ExecuteScalar());

                         foreach (var item in bill.Items)
                        {
                            string insertBillItemQuery = @"
                            INSERT INTO STAVKA (RACUN_RacunID, ARTIKAL_ArtikalID, Cijena, Kolicina)
                            VALUES (@BillId, @ArticleId, @Price, @Quantity);";

                            using (MySqlCommand itemCommand = new MySqlCommand(insertBillItemQuery, connection))
                            {
                                itemCommand.Parameters.AddWithValue("@BillId", billId);
                                itemCommand.Parameters.AddWithValue("@ArticleId", item.Article.ID);
                                itemCommand.Parameters.AddWithValue("@Price", item.Article.Price);
                                itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);

                                itemCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while creating the bill: " + ex.Message);
                }
            }
        }





        private void IncreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            
            Button button = sender as Button;
            var billItem = button?.DataContext as BillItem;

            if (billItem != null)
            {
                billItem.Quantity++;
                billItem.TotalPrice = billItem.Article.Price * billItem.Quantity;

               
                BillListBox.ItemsSource = null;
                BillListBox.ItemsSource = billItems;

               
                UpdateTotalPrice();
            }
        }

        
        private void DecreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            
            Button button = sender as Button;
            var billItem = button?.DataContext as BillItem;

            if (billItem != null && billItem.Quantity > 1)
            {
                billItem.Quantity--;
                billItem.TotalPrice = billItem.Article.Price * billItem.Quantity;

                
                BillListBox.ItemsSource = null;
                BillListBox.ItemsSource = billItems;

                
                UpdateTotalPrice();
            }
        }

        
        private void DeleteItemButton_Click(object sender, RoutedEventArgs e)
        {
             Button button = sender as Button;
            var billItem = button?.DataContext as BillItem;

            if (billItem != null)
            {
                billItems.Remove(billItem);

                BillListBox.ItemsSource = null;
                BillListBox.ItemsSource = billItems;

                UpdateTotalPrice();
            }
        }



        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
           PlaceholderText.Visibility = Visibility.Collapsed;
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
            {
                PlaceholderText.Visibility = Visibility.Visible;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                ArticlesListBox.ItemsSource = articles;
            }
            else
            {
                var filteredArticles = articles.Where(a => a.Name.ToLower().Contains(searchText)).ToList();
                ArticlesListBox.ItemsSource = filteredArticles;
            }
        }
        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null || comboBox.SelectedItem == null)
            {
                return; 
            }

            
            ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null)
            {
                return; 
            }

            
            string selectedCategory = selectedItem.Content as string;

            
            if (articles == null)
            {
                return; 
            }

           
            if (selectedCategory == "All")
            {
                ArticlesListBox.ItemsSource = articles;
            }
            else
            {
                var filteredArticles = articles.Where(a => a.Category == selectedCategory).ToList();
                ArticlesListBox.ItemsSource = filteredArticles;
            }
        }



  

    }


    public class Article
    {
        public string Name { get; set; }

        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string ImageSource { get; set; }
        public string Category { get; set; }

        public string Category2 { get; set; }
        public int ID { get; internal set; }
        public string ImageUrl { get; internal set; }
        public string ManufacturerName { get; internal set; }
        public object ArtikalID { get; internal set; }
        public int CategoryID { get; internal set; }
        public int ProizvodjacID { get; internal set; }
        public object ArticleID { get; internal set; }
        public object Pet { get; internal set; }
    }

    public class BillItem
    {
        public int Id { get; set; }
        public Article Article { get; set; }

        public string ImageUrl {  set; get; }
        
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }

    }

    

}
