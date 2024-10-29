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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Veterinary.DB;

namespace Veterinary.Pages
{
    /// <summary>
    /// Логика взаимодействия для InfoClientPage.xaml
    /// </summary>
    public partial class InfoClientPage : Page
    {
        public static List<Clients> clients {  get; set; }
        public static List<Animals> animals { get; set; }
        Clients contextClient;
        public InfoClientPage(Clients client)
        {
            InitializeComponent();
            contextClient = client;
            ClientTB.Text = contextClient.FullName;
            PhoneTB.Text = contextClient.PhoneNumber;
            EmailTB.Text = contextClient.Email;
            AddressTB.Text = contextClient.Address;
            animals = new List<Animals>(DBConnection.veterinary.Animals.Where(a => a.ClientId == contextClient.ClientId).ToList());
            AnimalsLV.ItemsSource = animals;
            this.DataContext = this;
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrHomePage(DBConnection.loginedPersonal));
        }

        private void AnimalsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AnimalsLV.SelectedItem is Animals animal)
            {
                AnimalsLV.SelectedItem = null;
                NavigationService.Navigate(new InfoAnimalPage(animal, contextClient));
            }
        }

        private void AddBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddAnimalPage(contextClient));
        }

        private void DeleteBT_Click(object sender, RoutedEventArgs e)
        {
            DBConnection.veterinary.Clients.Remove(contextClient);
            DBConnection.veterinary.SaveChanges();
            NavigationService.Navigate(new RegistrHomePage(DBConnection.loginedPersonal));
        }
    }
}
