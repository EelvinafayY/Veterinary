using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для AddClientPage.xaml
    /// </summary>
    public partial class AddClientPage : Page
    {
        public static Clients clientNew = new Clients();
        public AddClientPage()
        {
            InitializeComponent();
        }

        private void AddBT_Click(object sender, RoutedEventArgs e)
        {
            string fullname = SurnameTB.Text.Trim() + " " + NameTB.Text.Trim() + " " + 
                PatronymicTB.Text.Trim();

            clientNew.FullName = fullname;
            clientNew.PhoneNumber = PhoneTB.Text.Trim();
            clientNew.Email = EmailTB.Text.Trim();
            clientNew.Address = AddressTB.Text.Trim();
            if (SurnameTB.Text != "" && NameTB.Text != "" && PatronymicTB.Text != ""
                && PhoneTB.Text != "" && EmailTB.Text != "" && AddressTB.Text != "")
            {

                DBConnection.veterinary.Clients.Add(clientNew);
                DBConnection.veterinary.SaveChanges();
                NavigationService.Navigate(new RegistrHomePage(DBConnection.loginedPersonal));
            }
            else
            {
                MessageBox.Show("Заполните все поля!");
            }

        }

        private void SurnameTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextOnly(e.Text);
        }
       
        private void PhoneTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDigitOnly(e.Text);
        }
        private void PhoneTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PhoneTB.Text.Length == 1 && PhoneTB.Text != "8")
            {
                PhoneTB.Text = "8";
                PhoneTB.CaretIndex = PhoneTB.Text.Length;
            }
            else if (PhoneTB.Text.Length > 11)
            {
                PhoneTB.Text = PhoneTB.Text.Substring(0, 11);
                PhoneTB.CaretIndex = PhoneTB.Text.Length;
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

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrHomePage(DBConnection.loginedPersonal));
        }
    }
}
