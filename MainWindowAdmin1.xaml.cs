using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PetShop
{
    /// <summary>
    /// Interaction logic for MainWindowAdmin1.xaml
    /// </summary>
    public partial class MainWindowAdmin1 : Window
    {
        public MainWindowAdmin1()
        {
            InitializeComponent();
            MainContentFrame2.Navigate(new ArtikliAdmin());
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
               this.DragMove();
            }
        }
        private void Button_ClickA(object sender, RoutedEventArgs e)
        {
            Zaposleni.BorderBrush = null;
            Racun.BorderBrush = null;
            Sifarnici.BorderBrush = null;
            Podesavanja.BorderBrush = null;

            Artikli.BorderBrush = new SolidColorBrush(Colors.MediumSeaGreen); 
            Artikli.BorderThickness = new Thickness(0,0,0,1);


            MainContentFrame2.Navigate(new ArtikliAdmin());
            
        }

        private void Button_ClickR(object sender, RoutedEventArgs e)
        {
            
            Artikli.BorderBrush = null;
            Zaposleni.BorderBrush = null;
            Sifarnici.BorderBrush = null;
            Podesavanja.BorderBrush = null;

            Racun.BorderBrush = new SolidColorBrush(Colors.MediumSeaGreen); 
            Racun.BorderThickness = new Thickness(0, 0, 0, 1); 


            MainContentFrame2.Navigate(new Racuni());
        }

        private void Button_ClickP(object sender, RoutedEventArgs e)
        {
            Artikli.BorderBrush = null;
            Racun.BorderBrush = null;
            Sifarnici.BorderBrush = null;
            Zaposleni.BorderBrush = null;

            Podesavanja.BorderBrush = new SolidColorBrush(Colors.MediumSeaGreen);
            Podesavanja.BorderThickness = new Thickness(0, 0, 0, 1);

            MainContentFrame2.Navigate(new Podesavanja());
        }

        private void Button_ClickZ(object sender, RoutedEventArgs e)
        {
            Artikli.BorderBrush = null;
            Racun.BorderBrush = null;
            Sifarnici.BorderBrush = null;
            Podesavanja.BorderBrush = null;

            Zaposleni.BorderBrush = new SolidColorBrush(Colors.MediumSeaGreen);
            Zaposleni.BorderThickness = new Thickness(0, 0, 0, 1);


            MainContentFrame2.Navigate(new Zaposleni());
        }

       
        private void Button_ClickS(object sender, RoutedEventArgs e)
        {
            Artikli.BorderBrush = null;
            Racun.BorderBrush = null;
            Zaposleni.BorderBrush = null;
            Podesavanja.BorderBrush = null;

            Sifarnici.BorderBrush = new SolidColorBrush(Colors.MediumSeaGreen);
            Sifarnici.BorderThickness = new Thickness(0, 0, 0, 1);

            MainContentFrame2.Navigate(new Sifarnici());
        }
        private void Odjava_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
            
        }
    }
}

