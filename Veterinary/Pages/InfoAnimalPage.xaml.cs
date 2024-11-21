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
    /// Логика взаимодействия для InfoAnimalPage.xaml
    /// </summary>
    public partial class InfoAnimalPage : Page
    {
        public static List<Animals> animals {  get; set; }
        public static List<Services> services { get; set; }
        public static List<Appointments> appointments { get; set; }
        Animals contextAnimal;
        Clients contextClient;
        public InfoAnimalPage(Animals animals, Clients clients)
        {
            InitializeComponent();
            contextAnimal = animals;
            contextClient = clients;
            appointments = new List<Appointments>(DBConnection.veterinary.Appointments.Where(a => a.AnimalId == contextAnimal.AnimalId).ToList());
            AppointmentsLV.ItemsSource = appointments;
            AnimalTB.Text = contextAnimal.Name;
            ViewTB.Text = contextAnimal.Breed.Species.Name;
            BreedTB.Text = contextAnimal.Breed.Name;
            AgeTB.Text = contextAnimal.Birthday.ToString();
            GenderTB.Text = contextAnimal.Gender.Name;
            this.DataContext = this;
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new InfoClientPage(contextClient));
        }

        private void DeleteBT_Click(object sender, RoutedEventArgs e)
        {
            DBConnection.veterinary.Animals.Remove(contextAnimal);
            DBConnection.veterinary.SaveChanges();
            NavigationService.Navigate(new InfoClientPage(contextClient));
        }

        private void AddRecordBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddRecordPage(contextAnimal, contextClient));
        }
    }
}
