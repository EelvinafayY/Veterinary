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
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        public static List<Personal> personals {  get; set; }
        public static List<Role> roles { get; set; }
        public static List<Specialization> specializations { get; set; }
        public AuthorizationPage()
        {
            InitializeComponent();
        }

        private void VxodBT_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text.Trim();
            string password = PasswordPB.Password.Trim();

            personals = new List<Personal>(DBConnection.veterinary.Personal.ToList());
            Personal currentPersonal = personals.FirstOrDefault(i => i.Login == login && i.Password == password);
            DBConnection.loginedPersonal = currentPersonal;


            if (currentPersonal != null && currentPersonal.Specialization.RoleId == 2)
            {
                NavigationService.Navigate(new DoctorHomePage(currentPersonal));
            }
            if (currentPersonal != null && currentPersonal.Specialization.RoleId == 1)
            {
                //NavigationService.Navigate(new AdminHomePage(currentPersonal));
            }
            if (currentPersonal != null && currentPersonal.Specialization.RoleId == 3)
            {
                NavigationService.Navigate(new RegistrHomePage(currentPersonal));
            }
            
            if (currentPersonal == null)
            {
                MessageBox.Show("Такого пользователя не существует(((");
            }
        }

        private void RegistrBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
        }
    }
}
