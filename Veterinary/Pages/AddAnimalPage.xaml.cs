using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddAnimalPage.xaml
    /// </summary>
    public partial class AddAnimalPage : Page
    {
        public static List<Animals> animals {  get; set; }
        public static List<Gender> genders { get; set; }
        public static List<Breed> breeds { get; set; }
        public static List<Clients> clients { get; set; }
        public static Animals animalNew = new Animals();
        Clients contextClient;
        public AddAnimalPage(Clients clients)
        {
            InitializeComponent();
            contextClient = clients;
            breeds = new List<Breed>(DBConnection.veterinary.Breed.ToList());
            genders = new List<Gender>(DBConnection.veterinary.Gender.ToList());
            this.DataContext = this;
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new InfoClientPage(contextClient));
        }

        private void AddBT_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AnimalTB.Text))
            {
                MessageBox.Show("Введите кличку животного.");
                return;
            }

            else if (string.IsNullOrWhiteSpace(AgeTB.Text))
            {
                MessageBox.Show("Введите возраст животного.");
                return;
            }

            // Проверка комбобоксов
            else if (BreedCB.SelectedItem == null)
            {
                MessageBox.Show("Выберите породу.");
                return;
            }

            else if (GenderCB.SelectedItem == null)
            {
                MessageBox.Show("Выберите пол.");
                return;
            }
            else if (AgeTB.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("Неккоректная дата рождения!");
                return;
            }
            else
            {
                var a = BreedCB.SelectedItem as Breed;
                animalNew.BreedId = a.BreedId;
                animalNew.Name = AnimalTB.Text.Trim();
                var b = GenderCB.SelectedItem as Gender;
                animalNew.GenderId = b.GenderId;
                animalNew.Birthday = AgeTB.SelectedDate;
                animalNew.ClientId = contextClient.ClientId;
                DBConnection.veterinary.Animals.Add(animalNew);
                DBConnection.veterinary.SaveChanges();
                NavigationService.Navigate(new InfoClientPage(contextClient));
            }
        }

        private bool IsTextOnly(string text)
        {
            return Regex.IsMatch(text, @"^[а-яА-ЯёЁa-zA-Z]+$");
        }

        private bool IsDigitOnly(string text)
        {
            return Regex.IsMatch(text, @"^\d+$");
        }

        private void AnimalTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextOnly(e.Text);
        }

        private void AgeTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDigitOnly(e.Text);
        }
    }
}
