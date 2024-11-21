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
    /// Логика взаимодействия для DoctorHomePage.xaml
    /// </summary>
    public partial class DoctorHomePage : Page
    {
        public static List<Personal> personals {  get; set; }
        public static List<Clients> clients { get; set; }
        public static List<Animals> animals { get; set; }
        public static List<Status> status { get; set; }
        public static List<Reason> reasons { get; set; }
        public static List<Appointments> appointments { get; set; }
        public static List<Services> services { get; set; }
        Personal contextDoctor;
        public DoctorHomePage(Personal personal)
        {
            InitializeComponent();
            contextDoctor = personal;
            appointments = new List<Appointments>(DBConnection.veterinary.Appointments.Where(x => x.DoctorId == contextDoctor.PersonalId).ToList());
            SheduleLV.ItemsSource = appointments;
            NameTB.Text = "Добро пожаловать," + " " + contextDoctor.FullName + "!";
            this.DataContext = this;
        }

        private void SheduleLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SheduleLV.SelectedItem is Appointments shedule)
            {
                if (shedule.StatusId == 1 && shedule.Date < DateTime.Now) 
                {
                    SheduleLV.SelectedItem = null;
                    NavigationService.Navigate(new MedicalRecordPage(shedule));
                }
                else if (shedule.StatusId == 2)
                {
                    MessageBox.Show("Прием уже завершен!");
                }
                else
                {
                    MessageBox.Show("К сожалению, данная запись недоступна!");
                }              
            }
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            DBConnection.loginedPersonal = null;
            NavigationService.Navigate(new AuthorizationPage());
        }
    }
}
