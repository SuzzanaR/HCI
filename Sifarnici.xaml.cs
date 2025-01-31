using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PetShop
{
    public partial class Sifarnici : Page
    {
        public ObservableCollection<CategoryItem> Categories { get; set; }
        public ObservableCollection<ManufacturerItem> Manufacturers { get; set; }

        private string connectionString = "Server=localhost;Database=mydb;User ID=root;Password=suzana;";

        public Sifarnici()
        {
            InitializeComponent();
            Categories = new ObservableCollection<CategoryItem>();
            Manufacturers = new ObservableCollection<ManufacturerItem>();

            LoadCategories();
            LoadManufacturers();

            CategoryListBox.ItemsSource = Categories;
            ManufacturerListBox.ItemsSource = Manufacturers;
        }

        private void LoadCategories()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT KategorijaID, NazivKategorije FROM KATEGORIJA"; 
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Categories.Add(new CategoryItem
                            {
                                KategorijaID = reader.GetInt32("KategorijaID"),
                                NazivKategorije = reader.GetString("NazivKategorije")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading categories: {ex.Message}");
                }
            }
        }

        private void LoadManufacturers()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ProizvodjacID, NazivProizvodjaca FROM PROIZVODJAC"; 
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Manufacturers.Add(new ManufacturerItem
                            {
                                ProizvodjacID = reader.GetInt32("ProizvodjacID"),
                                NazivProizvodjaca = reader.GetString("NazivProizvodjaca")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading manufacturers: {ex.Message}");
                }
            }





        }










        private void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var categoryItem = button.DataContext as CategoryItem;
            if (categoryItem == null) return;

            SelectedCategoryToDelete = categoryItem;

            DeleteConfirmationPopup.IsOpen = true;
        }

        private void DeleteManufacturerButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var manufacturerItem = button.DataContext as ManufacturerItem;
            if (manufacturerItem == null) return;

            SelectedManufacturerToDelete = manufacturerItem;

            DeleteConfirmationPopup2.IsOpen = true;
        }



        private CategoryItem SelectedCategoryToDelete;
        private ManufacturerItem SelectedManufacturerToDelete;


        private void ConfirmDeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCategoryToDelete != null)
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "DELETE FROM KATEGORIJA WHERE KategorijaID = @KategorijaID";
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@KategorijaID", SelectedCategoryToDelete.KategorijaID);
                        cmd.ExecuteNonQuery();

                        Categories.Remove(SelectedCategoryToDelete);
                        MessageBox.Show("Kategorija je obrisana.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Greška pri brisanju kategorije: {ex.Message}");
                    }
                }
            }

            DeleteConfirmationPopup.IsOpen = false;
        }

        private void CancelDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteConfirmationPopup.IsOpen = false;
        }

        private void ConfirmDeleteManufacturerButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedManufacturerToDelete != null)
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "DELETE FROM PROIZVODJAC WHERE ProizvodjacID = @ProizvodjacID";
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@ProizvodjacID", SelectedManufacturerToDelete.ProizvodjacID);
                        cmd.ExecuteNonQuery();

                        Manufacturers.Remove(SelectedManufacturerToDelete);
                        MessageBox.Show("Proizvođač je obrisan.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Greška pri brisanju proizvođača: {ex.Message}");
                    }
                }
            }

            DeleteConfirmationPopup2.IsOpen = false;
        }

        private void CancelDeleteButton_Click2(object sender, RoutedEventArgs e)
        {
            DeleteConfirmationPopup2.IsOpen = false;
        }





        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            string categoryName = CategoryTextBox.Text;
            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Unesite ime kategorije.");
                return;
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();  
                    string query = "INSERT INTO KATEGORIJA (NazivKategorije) VALUES (@NazivKategorije)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@NazivKategorije", categoryName);

                    int rowsAffected = cmd.ExecuteNonQuery();  

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Kategorija je dodana.");
                        CategoryTextBox.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Greška pri dodavanju kategorije.");
                    }

                    Categories.Clear();
                    LoadCategories();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri dodavanju kategorije: {ex.Message}\n{ex.StackTrace}");
                }
            }

        }
        private void AddManufacturerButton_Click(object sender, RoutedEventArgs e)
        {
            string manufacturerName = ManufacturerTextBox.Text;
            if (string.IsNullOrEmpty(manufacturerName))
            {
                MessageBox.Show("Unesite ime proizvođača.");
                return;
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO PROIZVODJAC (NazivProizvodjaca) VALUES (@NazivProizvodjaca)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@NazivProizvodjaca", manufacturerName);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Proizvođač je dodan.");
                    ManufacturerTextBox.Clear();

                    Manufacturers.Clear();
                    LoadManufacturers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri dodavanju proizvođača: {ex.Message}");
                }
            }
        }


























        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }

    
    }

    public class CategoryItem
    {
        public int KategorijaID { get; set; }
        public string NazivKategorije { get; set; }
    }

    public class ManufacturerItem
    {
        public int ProizvodjacID { get; set; }
        public string NazivProizvodjaca { get; set; }
    }
}
