using MySql.Data.MySqlClient;
using System;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Input;

namespace PetShop
{
    public partial class Login : Window
    {
        private string connectionString = "Server=localhost;Database=mydb;User ID=root;Password=suzana;";

        public Login()
        {
            InitializeComponent();
        }

        private void Button_Exit(object sender, EventArgs e)
        {
            this.Close();  
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }



        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            

            string username = UsernameTextBox.Text.Trim();
            string password = passwordBox1.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Molimo da unesete korisničko ime i lozinku.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        string queryZaposleni = "SELECT * FROM ZAPOSLENI WHERE KorisnickoIme = @username AND Lozinka = @password";
                        using (MySqlCommand command = new MySqlCommand(queryZaposleni, connection))
                        {
                            command.Parameters.AddWithValue("@username", username);
                            command.Parameters.AddWithValue("@password", password);

                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        SessionManager.UserId = reader.GetInt32("ZaposleniID");
                                        SessionManager.FirstName = reader.GetString("Ime");
                                        SessionManager.LastName = reader.GetString("Prezime");
                                        SessionManager.PhoneNumber = reader.GetString("BrojTelefona");
                                        SessionManager.Username = reader.GetString("KorisnickoIme");
                                        SessionManager.Password = reader.GetString("Lozinka");

                                    }
                                    reader.Close();
                                    MainWindow mainWindow = new MainWindow();
                                    mainWindow.Show();
                                    this.Close();
                                    return;
                                }

                            }
                        }

                        
                        string queryAdministrator = "SELECT * FROM ADMINISTRATOR WHERE KorisnickoIme = @username AND Lozinka = @password";
                        using (MySqlCommand command = new MySqlCommand(queryAdministrator, connection))
                        {
                            command.Parameters.AddWithValue("@username", username);
                            command.Parameters.AddWithValue("@password", password);

                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        SessionManager.UserId = reader.GetInt32("AdministratorID");
                                        SessionManager.FirstName = reader.GetString("Ime");
                                        SessionManager.LastName = reader.GetString("Prezime");
                                        SessionManager.PhoneNumber = reader.GetString("BrojTelefona");
                                        SessionManager.Username = reader.GetString("KorisnickoIme");
                                        SessionManager.Password = reader.GetString("Lozinka");

                                    }
                                    reader.Close();
                                    MainWindowAdmin1 mainWindow = new MainWindowAdmin1();
                                    mainWindow.Show();
                                    this.Close();
                                    return;
                                }

                            }
                        }


                        MessageBox.Show("Neispravno korisničko ime ili lozinka.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                

            }

            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        


    }




    public static class SessionManager
    {
        public static bool IsAdmin { get; set; } 
        public static string FirstName { get; set; }
        public static string LastName { get; set; }
        public static string PhoneNumber { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static int UserId { get; set; }
    }



}
