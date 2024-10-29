using System;
using System.Collections.Generic;
using System.Data.Common;
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
    /// Логика взаимодействия для MedicalRecordPage.xaml
    /// </summary>
    public partial class MedicalRecordPage : Page
    {
        public static List<Appointments> appointments { get; set; }
        public static List<Services> services { get; set; }
        public static List<Inventory> inventory { get; set; }
        public static List<Diagnosis> diagnosis { get; set; }
        public static List<Recipe> recipes { get; set; }
        public static List<Tests> tests { get; set; }
        Appointments contextAppointment;
        public static Recipe recipe = new Recipe();
        public static Tests test = new Tests();
        public static MedicalRecords record = new MedicalRecords();
        public MedicalRecordPage(Appointments appointments)
        {
            InitializeComponent();
            contextAppointment = appointments;
            IdTB.Text = "Прием №" + (appointments.AppointmentId).ToString();
            ClientTB.Text = appointments.Animals.Clients.FullName;
            AnimalTB.Text = appointments.Animals.Name;
            ViewTB.Text = appointments.Animals.Breed.Species.Name;
            BreedTB.Text = appointments.Animals.Breed.Name;
            AgeTB.Text = (appointments.Animals.Birthday).ToString();
            DateTB.Text = (appointments.Date).ToString();
            ServiceTB.Text = appointments.Services.Name;
            diagnosis = new List<Diagnosis>(DBConnection.veterinary.Diagnosis.ToList());
            inventory = new List<Inventory>(DBConnection.veterinary.Inventory.ToList());
            services = new List<Services>(DBConnection.veterinary.Services.Where(i => i.TypeId == 2).ToList());
            record.AppointmentsId = appointments.AppointmentId;
            DBConnection.veterinary.MedicalRecords.Add(record);
            DBConnection.veterinary.SaveChanges();
            RecipeLV.ItemsSource = new List<Recipe>(DBConnection.veterinary.Recipe.Where(r => r.RecordId == record.RecordId).ToList());
            AnalisLV.ItemsSource = new List<Tests>(DBConnection.veterinary.Tests.Where(r => r.RecordId == record.RecordId).ToList());
            this.DataContext = this;
        }

        private void ItemCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var a = ItemCB.SelectedItem as Inventory;
            recipe.ItemId = a.ItemId;
            recipe.RecordId = record.RecordId;
            DBConnection.veterinary.Recipe.Add(recipe);
            DBConnection.veterinary.SaveChanges();
            RecipeLV.ItemsSource = new List<Recipe>(DBConnection.veterinary.Recipe.Where(r => r.RecordId == record.RecordId).ToList());
        }

        private void ServicesCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var a = ServicesCB.SelectedItem as Services;
            test.ServiceId = a.ServiceId;
            test.RecordId = record.RecordId;
            DBConnection.veterinary.Tests.Add(test);
            DBConnection.veterinary.SaveChanges();
            AnalisLV.ItemsSource = new List<Tests>(DBConnection.veterinary.Tests.Where(r => r.RecordId == record.RecordId).ToList());
        }

        private void RecipeLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecipeLV.SelectedItem is Recipe recipe)
            {
                DBConnection.veterinary.Recipe.Remove(recipe);
                DBConnection.veterinary.SaveChanges();
                RecipeLV.ItemsSource = new List<Recipe>(DBConnection.veterinary.Recipe.Where(r => r.RecordId == record.RecordId).ToList());
            }
        }

        private void SaveBT_Click(object sender, RoutedEventArgs e)
        {

            var a = DiagnosisCB.SelectedItem as Diagnosis;
            if (a == null || TreatmentTB.Text.Trim() == null)
            {
                MessageBox.Show("Заполните все поля!");
            }
            else
            {
                record.DiagnosisId = a.DiagnosisId;
                record.Treatment = TreatmentTB.Text.Trim();
                contextAppointment.StatusId = 2;
                DBConnection.veterinary.SaveChanges();
                NavigationService.Navigate(new DoctorHomePage(DBConnection.loginedPersonal));
            }
            
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DoctorHomePage(DBConnection.loginedPersonal));
        }

        private void AnalisLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AnalisLV.SelectedItem is Tests test)
            {
                DBConnection.veterinary.Tests.Remove(test);
                DBConnection.veterinary.SaveChanges();
                AnalisLV.ItemsSource = new List<Tests>(DBConnection.veterinary.Tests.Where(r => r.RecordId == record.RecordId).ToList());
            }
        }
    }
}
