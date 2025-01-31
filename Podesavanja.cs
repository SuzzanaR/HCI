using System;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MySql.Data.MySqlClient;

namespace PetShop
{
    public partial class Podesavanja : Page
    {

        private string connectionString = "Server=localhost;Database=mydb;User ID=root;Password=suzana;";

        public Podesavanja()
        {
           InitializeComponent();
           LoadUserData();



        }


     

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }


        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedItem = myComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem;
            if (selectedItem != null)
            {
                string selectedContent = selectedItem.Content.ToString();

                CallFunctionBasedOnSelection(selectedContent);
            }
        }

        private void CallFunctionBasedOnSelection(string selectedOption)
        {
            switch (selectedOption)
            {
                case "Svijetla tema":
                    AppTheme.ChangeTheme(new Uri("Themes/LightTheme.xaml", UriKind.Relative));
                    break;
                case "Tamna tema":
                    AppTheme.ChangeTheme(new Uri("Themes/DarkTheme.xaml", UriKind.Relative));
                    break;
                case "Zemljana tema":
                    AppTheme.ChangeTheme(new Uri("Themes/Theme3.xaml", UriKind.Relative));
                    break;

                default:
                    MessageBox.Show("Nepoznata opcija.");
                    break;
            }
        }


        private void LoadUserData()
        {
            FirstNameTextBox.Text = SessionManager.FirstName;
            LastNameTextBox.Text = SessionManager.LastName;
            PhoneNumberTextBox.Text = SessionManager.PhoneNumber;
            UsernameTextBox.Text = SessionManager.Username;
            PasswordBox.Password = SessionManager.Password;
        }

        private void UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text.Trim();
            string lastName = LastNameTextBox.Text.Trim();
            string phoneNumber = PhoneNumberTextBox.Text.Trim();
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Sva polja su obavezna!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    Console.WriteLine("IsAdmin: " + SessionManager.IsAdmin);

                    if (SessionManager.IsAdmin)
                    {
                        Console.WriteLine("Updating ADMINISTRATOR table...");

                        string queryadmin = "UPDATE ADMINISTRATOR SET Ime = @firstName, Prezime = @lastName, " +
                                            "BrojTelefona = @phoneNumber, KorisnickoIme = @username, Lozinka = @password " +
                                            "WHERE AdministratorID = @userId";

                        using (MySqlCommand command = new MySqlCommand(queryadmin, connection))
                        {
                            command.Parameters.AddWithValue("@firstName", firstName);
                            command.Parameters.AddWithValue("@lastName", lastName);
                            command.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                            command.Parameters.AddWithValue("@username", username);
                            command.Parameters.AddWithValue("@password", string.IsNullOrEmpty(password) ? SessionManager.Password : password);
                            command.Parameters.AddWithValue("@userId", SessionManager.UserId);

                            command.ExecuteNonQuery();
                        }

                        SessionManager.FirstName = firstName;
                        SessionManager.LastName = lastName;
                        SessionManager.PhoneNumber = phoneNumber;
                        SessionManager.Username = username;
                        SessionManager.Password = password;

                        MessageBox.Show("Podaci su uspešno ažurirani!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Console.WriteLine("Updating ZAPOSLENI table...");

                        string queryzaposleni = "UPDATE ZAPOSLENI SET Ime = @firstName, Prezime = @lastName, " +
                                                "BrojTelefona = @phoneNumber, KorisnickoIme = @username, Lozinka = @password " +
                                                "WHERE ZaposleniID = @userId";

                        using (MySqlCommand command = new MySqlCommand(queryzaposleni, connection))
                        {
                            command.Parameters.AddWithValue("@firstName", firstName);
                            command.Parameters.AddWithValue("@lastName", lastName);
                            command.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                            command.Parameters.AddWithValue("@username", username);
                            command.Parameters.AddWithValue("@password", string.IsNullOrEmpty(password) ? SessionManager.Password : password);
                            command.Parameters.AddWithValue("@userId", SessionManager.UserId);

                            command.ExecuteNonQuery();
                        }

                        SessionManager.FirstName = firstName;
                        SessionManager.LastName = lastName;
                        SessionManager.PhoneNumber = phoneNumber;
                        SessionManager.Username = username;
                        SessionManager.Password = password;

                        MessageBox.Show("Podaci su uspešno ažurirani!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }














    }



}
