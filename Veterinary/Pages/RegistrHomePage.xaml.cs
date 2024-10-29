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
    /// Логика взаимодействия для RegistrHomePage.xaml
    /// </summary>
    public partial class RegistrHomePage : Page
    {
        public static List<Personal> personals {  get; set; }
        public static List<Clients> clients { get; set; }
        Personal contextRegistr;

        public RegistrHomePage(Personal personal)
        {
            InitializeComponent();
            contextRegistr = personal;
            NameTB.Text = "Добро пожаловать, " + contextRegistr.FullName;
            ClientsLV.ItemsSource = new List<Clients>(DBConnection.veterinary.Clients.ToList());
            this.DataContext = this;
        }

        private void AddBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddClientPage());
        }

        private void ClientsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientsLV.SelectedItem is Clients client)
            {
                  ClientsLV.SelectedItem = null;
                  NavigationService.Navigate(new InfoClientPage(client));
            }
        }
    }
}
