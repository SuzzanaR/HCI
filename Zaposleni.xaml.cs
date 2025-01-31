using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace PetShop
{
    public partial class Zaposleni : Page
    {
        public ObservableCollection<Employee> Employees { get; set; }

        private string connectionString = "Server=localhost;Database=mydb;User ID=root;Password=suzana;";

        public Zaposleni()
        {
            InitializeComponent();
            Employees = new ObservableCollection<Employee>();

            LoadEmployeesFromDatabase();

            EmployeeListBox.ItemsSource = Employees;
        }

        private void LoadEmployeesFromDatabase()
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ZaposleniID, Ime, Prezime, BrojTelefona, KorisnickoIme FROM ZAPOSLENI";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            ZaposleniID = reader.GetInt32("ZaposleniID"),
                            Ime = reader.GetString("Ime"),
                            Prezime = reader.GetString("Prezime"),
                            BrojTelefona = reader.GetString("BrojTelefona"),
                            KorisnickoIme = reader.GetString("KorisnickoIme")
                        };
                        Employees.Add(employee);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data from database: " + ex.Message);
            }
        }

        private void DeleteEmployeeFromDatabase(int zaposleniID)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string deleteRacunQuery = "DELETE FROM RACUN WHERE ZAPOSLENI_ZaposleniID = @ZaposleniID";
                    MySqlCommand deleteRacunCommand = new MySqlCommand(deleteRacunQuery, connection);
                    deleteRacunCommand.Parameters.AddWithValue("@ZaposleniID", zaposleniID);
                    deleteRacunCommand.ExecuteNonQuery();

                    string deleteEmployeeQuery = "DELETE FROM ZAPOSLENI WHERE ZaposleniID = @ZaposleniID";
                    MySqlCommand deleteEmployeeCommand = new MySqlCommand(deleteEmployeeQuery, connection);
                    deleteEmployeeCommand.Parameters.AddWithValue("@ZaposleniID", zaposleniID);
                    deleteEmployeeCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri brisanju zaposlenog: " + ex.Message);
            }
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }

      


        private Employee employeeToDelete;

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeListBox.SelectedItem is Employee selectedEmployee)
            {
                employeeToDelete = selectedEmployee;
                DeleteConfirmationPopup.IsOpen = true;
            }
            else
            {
                MessageBox.Show("Izaberite zaposlenog za brisanje.");
            }
        }

        private void ConfirmDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (employeeToDelete != null)
            {
                DeleteEmployeeFromDatabase(employeeToDelete.ZaposleniID);

                Employees.Remove(employeeToDelete);
                employeeToDelete = null;

                DeleteConfirmationPopup.IsOpen = false;

                MessageBox.Show("Zaposleni je obrisan.");
            }
        }

        private void CancelDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteConfirmationPopup.IsOpen = false;
        }









        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;
            string phoneNumber = PhoneNumberTextBox.Text;
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;  

           
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Popunite sva polja.");
                return;
            }

            int newEmployeeId = 1; 
            if (Employees.Count > 0)
            {
                newEmployeeId = Employees.Max(emp => emp.ZaposleniID) + 1;
            }

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO ZAPOSLENI (ZaposleniID, Ime, Prezime, BrojTelefona, KorisnickoIme, Lozinka) " +
                                   "VALUES (@ZaposleniID, @Ime, @Prezime, @BrojTelefona, @KorisnickoIme, @Lozinka)";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ZaposleniID", newEmployeeId);  
                    command.Parameters.AddWithValue("@Ime", firstName);
                    command.Parameters.AddWithValue("@Prezime", lastName);
                    command.Parameters.AddWithValue("@BrojTelefona", phoneNumber);
                    command.Parameters.AddWithValue("@KorisnickoIme", username);
                    command.Parameters.AddWithValue("@Lozinka", password);

                    command.ExecuteNonQuery();
                }

                Employee newEmployee = new Employee
                {
                    ZaposleniID = newEmployeeId,
                    Ime = firstName,
                    Prezime = lastName,
                    BrojTelefona = phoneNumber,
                    KorisnickoIme = username,
                    Lozinka = password
                };

                Employees.Add(newEmployee);

                FirstNameTextBox.Clear();
                LastNameTextBox.Clear();
                PhoneNumberTextBox.Clear();
                UsernameTextBox.Clear();
                PasswordBox.Clear();

                MessageBox.Show("Zaposleni je dodan!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri dodavanju zaposlenog: " + ex.Message);
            }
        }





















    }

    public class Employee
    {
        public int ZaposleniID { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string BrojTelefona { get; set; }
        public string KorisnickoIme { get; set; }
        public string Lozinka { get; set; }


        public override string ToString()
        {
            return ZaposleniID + " " + Ime + " " + Prezime + " " + BrojTelefona + " " + KorisnickoIme + " " + Lozinka;
        }
    }
}
