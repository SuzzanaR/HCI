using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetShop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContentFrame.Navigate(new Artikli());
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            
            Racuni.BorderBrush = null;
            Podesavanja.BorderBrush = null;

            Artikli.BorderBrush = new SolidColorBrush(Colors.MediumSeaGreen);
            Artikli.BorderThickness = new Thickness(0, 0, 0, 1);

            MainContentFrame.Navigate(new Artikli());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            Artikli.BorderBrush = null;
            Podesavanja.BorderBrush = null;

            Racuni.BorderBrush = new SolidColorBrush(Colors.MediumSeaGreen);
            Racuni.BorderThickness = new Thickness(0, 0, 0, 1);

            MainContentFrame.Navigate(new Racuni());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            Artikli.BorderBrush = null;
            Racuni.BorderBrush = null;

            Podesavanja.BorderBrush = new SolidColorBrush(Colors.MediumSeaGreen);
            Podesavanja.BorderThickness = new Thickness(0, 0, 0, 1);

            MainContentFrame.Navigate(new Podesavanja());
        }


        private void Odjava_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
            
        }
    }
}