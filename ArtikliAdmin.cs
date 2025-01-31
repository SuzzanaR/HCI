using System;
using System.Windows;

using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Controls.Primitives;



namespace PetShop
{
    public partial class ArtikliAdmin : Page
    {


        private List<Kategorija> categories;
        private List<ArticleAdmin> articles = new List<ArticleAdmin>();

        private string connectionString = "Server=localhost;Database=mydb;User ID=root;Password=suzana;";

        public object ProizvodjacID { get; private set; }
        public object selectedArticleId { get; private set; }
      

        public ArtikliAdmin()
        {

            InitializeComponent();
            LoadArticlesFromDatabase();
            LoadManufacturers();
            LoadCategories();
            LoadCategoriesFilter();

           
        }


       

        private void LoadManufacturers()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM PROIZVODJAC";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var manufacturer = new
                            {
                                ProizvodjacID = reader.GetInt32("ProizvodjacID"),
                                NazivProizvodjaca = reader.GetString("NazivProizvodjaca")
                            };
                            ProizvodjaciComboBox.Items.Add(new ComboBoxItem
                            {
                                Content = manufacturer.NazivProizvodjaca,
                                Tag = manufacturer.ProizvodjacID
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Greška pri učitavanju proizvođača: " + ex.Message);
                }
            }
        }

        private void LoadCategories()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM KATEGORIJA";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var category = new
                            {
                                KategorijaID = reader.GetInt32("KategorijaID"),
                                NazivKategorije = reader.GetString("NazivKategorije")
                            };
                            LjubimciComboBox.Items.Add(new ComboBoxItem
                            {
                                Content = category.NazivKategorije,
                                Tag = category.KategorijaID
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Greška pri učitavanju kategorija: " + ex.Message);
                }
            }
        }


        private void AddArticleButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string description = Description.Text;
            decimal price;
            if (!decimal.TryParse(PriceTextBox.Text, out price))
            {
                MessageBox.Show("Cijena je nevažeća.");
                return;
            }
            string imageUrl = ImageUrl.Text;

            ComboBoxItem selectedCategory = TipArtiklaComboBox.SelectedItem as ComboBoxItem;
            int categoryId = selectedCategory != null ? Convert.ToInt32(selectedCategory.Tag) : 0;

            ComboBoxItem selectedManufacturer = ProizvodjaciComboBox.SelectedItem as ComboBoxItem;
            int manufacturerId = selectedManufacturer != null ? Convert.ToInt32(selectedManufacturer.Tag) : 0;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                   
                    string insertArticleQuery = "INSERT INTO ARTIKAL (NazivArtikla, Opis, Cijena, KATEGORIJA_KategorijaID, PROIZVODJAC_ProizvodjacID, ImageUrl) " +
                                                "VALUES (@name, @description, @price, @categoryId, @manufacturerId, @imageUrl)";
                    MySqlCommand cmd = new MySqlCommand(insertArticleQuery, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@categoryId", categoryId);
                    cmd.Parameters.AddWithValue("@manufacturerId", manufacturerId);
                    cmd.Parameters.AddWithValue("@imageUrl", imageUrl);
                    cmd.ExecuteNonQuery();

                    int artikalId = (int)cmd.LastInsertedId;

                    switch (categoryId)
                    {
                        case 1: 
                            InsertIntoHRANA(conn, artikalId);
                            break;
                        case 2: 
                            InsertIntoIGRACKE(conn, artikalId);
                            break;
                        case 3: 
                            InsertIntoOPREMA(conn, artikalId);
                            break;
                        case 4: 
                            InsertIntoHIGIJENA(conn, artikalId);
                            break;
                        default:
                            MessageBox.Show("Invalid category selected.");
                            return;
                    }

                    MessageBox.Show("Novi artikal je dodan.");
                    LoadArticlesFromDatabase(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Greška u dodavanju artikla: " + ex.Message);
                }
            }
        }

        private void InsertIntoHRANA(MySqlConnection conn, int artikalId)
        {
            string insertQuery = "INSERT INTO HRANA (ARTIKAL_ArtikalID) VALUES (@artikalId)";
            MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@artikalId", artikalId);
            cmd.ExecuteNonQuery();
        }

        private void InsertIntoIGRACKE(MySqlConnection conn, int artikalId)
        {
            string insertQuery = "INSERT INTO IGRACKE (ARTIKAL_ArtikalID) VALUES (@artikalId)";
            MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@artikalId", artikalId);
            cmd.ExecuteNonQuery();
        }

        private void InsertIntoOPREMA(MySqlConnection conn, int artikalId)
        {
            string insertQuery = "INSERT INTO OPREMA (ARTIKAL_ArtikalID) VALUES (@artikalId)";
            MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@artikalId", artikalId);
            cmd.ExecuteNonQuery();
        }

        private void InsertIntoHIGIJENA(MySqlConnection conn, int artikalId)
        {
            string insertQuery = "INSERT INTO HIGIJENA (ARTIKAL_ArtikalID) VALUES (@artikalId)";
            MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@artikalId", artikalId);
            cmd.ExecuteNonQuery();
        }




        //UPDATE

        private ArticleAdmin selectedArticle;

        private void ArticlesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            selectedArticle = (ArticleAdmin)ArticlesListBox.SelectedItem;

            if (selectedArticle != null)
            {
                
                NameTextBox.Text = selectedArticle.Name;
                Description.Text = selectedArticle.Description;
                PriceTextBox.Text = selectedArticle.Price.ToString("N2");
                ImageUrl.Text = selectedArticle.ImageUrl;
            }
        }

        private void IzmijeniButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedArticle != null)
            {
                
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(Description.Text) ||
                    !decimal.TryParse(PriceTextBox.Text, out decimal newPrice))
                {
                    MessageBox.Show("Pravilno popunite polja.");
                    return;
                }

                selectedArticle.Name = NameTextBox.Text;
                selectedArticle.Description = Description.Text;
                selectedArticle.Price = newPrice;
                selectedArticle.ImageUrl= ImageUrl.Text;

                
                UpdateArticleInDatabase(selectedArticle);

                MessageBox.Show("Artikal je uspješno izmijenjen!");

                
                ArticlesListBox.Items.Refresh();
            }
        }

       

    private void UpdateArticleInDatabase(ArticleAdmin article)
    {
        string connectionString = "Server=localhost;Database=mydb;User ID=root;Password=suzana;";

            using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "UPDATE ARTIKAL SET NazivArtikla = @Name, Opis = @Description, ImageUrl = @ImageUrl, Cijena = @Price WHERE ArtikalID = @ArtikalID";

            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Name", article.Name);
                cmd.Parameters.AddWithValue("@Description", article.Description);
                cmd.Parameters.AddWithValue("@Price", article.Price);
                cmd.Parameters.AddWithValue("@ArtikalID", article.ArtikalID);
                cmd.Parameters.AddWithValue("@ImageUrl", article.ImageUrl);

                cmd.ExecuteNonQuery();
            }
        }
    }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && (textBox.Text == "Naziv artikla" || textBox.Text == "Opis artikla" || textBox.Text == "Cijena" || textBox.Text == "URL slike"))
            {
                textBox.Clear();  
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrEmpty(textBox.Text))
            {
                if (textBox.Name == "NameTextBox")
                {
                    textBox.Text = "Naziv artikla";
                }
                else if (textBox.Name == "Description")
                {
                    textBox.Text = "Opis artikla";
                }
                else if (textBox.Name == "PriceTextBox")
                {
                    textBox.Text = "Cijena";
                }
                else if (textBox.Name == "ImageUrl")
                {
                    textBox.Text = "URL slike";
                }
            }
        }



        private void ObrisiButton_Click(object sender, RoutedEventArgs e)
        {

            NameTextBox.Clear();
            Description.Clear();
            PriceTextBox.Clear();
            LjubimciComboBox.SelectedIndex = -1;
            TipArtiklaComboBox.SelectedIndex = -1;
            ProizvodjaciComboBox.SelectedIndex = -1;
            ImageUrl.Clear();
        }






        private void LoadCategoriesFilter()
        {
            categories = new List<Kategorija>();

            string query = "SELECT KategorijaID, NazivKategorije FROM KATEGORIJA";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
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
                }

                foreach (var category in categories)
                {
                    CheckBox checkBox = new CheckBox
                    {
                        Content = category.NazivKategorije,
                        Tag = category.KategorijaID  
                    };
                    CategoryCheckBoxes.Items.Add(checkBox);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message);
            }
        }







        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryPopup.IsOpen = true;
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            
            var selectedCategoryIds = new List<int>();
            foreach (CheckBox checkBox in CategoryCheckBoxes.Items)
            {
                if (checkBox.IsChecked == true)
                {
                    selectedCategoryIds.Add((int)checkBox.Tag);
                }
            }

            LoadArticlesFromDatabase();
          
            CategoryPopup.IsOpen = false;
        }






        private void LoadArticlesFromDatabase()
        {
            List<ArticleAdmin> articlesAdmin = new List<ArticleAdmin>();

            string query = @"
        SELECT a.ArtikalID, a.NazivArtikla, a.Opis, a.Cijena, a.ImageUrl, p.NazivProizvodjaca
        FROM ARTIKAL a
        JOIN PROIZVODJAC p ON a.PROIZVODJAC_ProizvodjacID = p.ProizvodjacID";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ArticleAdmin articleAdmin = new ArticleAdmin
                                {
                                    ArtikalID = reader.GetInt32("ArtikalID"),
                                    Name = reader.GetString("NazivArtikla"),
                                    Description = reader.GetString("Opis"),
                                    Price = reader.GetDecimal("Cijena"),
                                    ImageUrl = reader.GetString("ImageUrl"),
                                    ManufacturerName = reader.GetString("NazivProizvodjaca")
                                };

                                articlesAdmin.Add(articleAdmin);
                            }
                        }
                    }
                }

                
                articles = articlesAdmin;

               
                ArticlesListBox.ItemsSource = articles;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading articles: " + ex.Message);
            }
        }





        //DELETE

        private void DeleteArticle(int artikalId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        DeleteFromAssociatedTables(artikalId, connection, transaction);

                        string deleteQuery = "DELETE FROM ARTIKAL WHERE ArtikalID = @ArtikalID";
                        MySqlCommand cmd = new MySqlCommand(deleteQuery, connection, transaction);
                        cmd.Parameters.AddWithValue("@ArtikalID", artikalId);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error deleting article: {ex.Message}");
                    }
                }
            }
        }


       
        private void DeleteFromAssociatedTables(int artikalId, MySqlConnection connection, MySqlTransaction transaction)
        {
            string[] tables = { "HRANA", "OPREMA", "IGRACKE", "HIGIJENA" };

            foreach (var table in tables)
            {
                string deleteQuery = $"DELETE FROM {table} WHERE ARTIKAL_ArtikalID = @ArtikalID";
                MySqlCommand cmd = new MySqlCommand(deleteQuery, connection, transaction);
                cmd.Parameters.AddWithValue("@ArtikalID", artikalId);
                cmd.ExecuteNonQuery();
            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedArticle = (ArticleAdmin)((Button)sender).DataContext;

            if (selectedArticle != null)
            {
                DeleteArticle(selectedArticle.ArtikalID);

                List<ArticleAdmin> articlesAdmin = ArticlesListBox.ItemsSource as List<ArticleAdmin>;
                if (articlesAdmin != null)
                {
                    articlesAdmin.Remove(selectedArticle);

                    
                    ArticlesListBox.ItemsSource = null; 
                    ArticlesListBox.ItemsSource = articlesAdmin; 
                }

                MessageBox.Show("Article deleted successfully.");
            }
        }




        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
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
            if (articles == null)
            {
                MessageBox.Show("Articles list is not initialized.");
                return;
            }

            string searchText = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                ArticlesListBox.ItemsSource = articles;
            }
            else
            {
                var filteredArticles = articles.Where(a =>
                    a.Name.ToLower().Contains(searchText) ||
                    a.Description.ToLower().Contains(searchText) ||
                    a.ManufacturerName.ToLower().Contains(searchText)).ToList();
                ArticlesListBox.ItemsSource = filteredArticles;
            }
        }

        
    }


 

    public class ArticleAdmin
    {
        public int ArtikalID { get; set; }
        public string Name { get; set; }  
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryID { get; set; }
        public string ImageUrl { get; set; }
        public string ManufacturerName { get; set; }
        public string Category { get; internal set; }
        public string TipArtiklaComboBox { get; internal set; }
    }




    public class Kategorija
    {
        public int KategorijaID { get; set; }
        public string NazivKategorije { get; set; }
    }


}




