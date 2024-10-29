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
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public static Personal personal = new Personal();
        public static List<Specialization> specializations {  get; set; }
        public RegistrationPage()
        {
            InitializeComponent();
            specializations = new List<Specialization>(DBConnection.veterinary.Specialization.ToList());
            SpecializationCB.ItemsSource = specializations;
            this.DataContext = this;

        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddBT_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SurnameTB.Text))
            {
                MessageBox.Show("Введите фамилию.");
                return;
            }

            else if (string.IsNullOrWhiteSpace(NameTB.Text))
            {
                MessageBox.Show("Введите имя.");
                return;
            }

            // Проверка комбобоксов
            else if (SpecializationCB.SelectedItem == null)
            {
                MessageBox.Show("Выберите специальность.");
                return;
            }
            else if (string.IsNullOrWhiteSpace(PhoneTB.Text))
            {
                MessageBox.Show("Введите телефон.");
                return;
            }
            else if (string.IsNullOrWhiteSpace(EmailTB.Text))
            {
                MessageBox.Show("Введите email.");
                return;
            }
            else if (string.IsNullOrWhiteSpace(LoginTB.Text))
            {
                MessageBox.Show("Введите логин.");
                return;
            }
            else if (string.IsNullOrWhiteSpace(PasswordTB.Text))
            {
                MessageBox.Show("Введите пароль.");
                return;
            }
            else
            {
                string fullname = SurnameTB.Text.Trim() + " " + NameTB.Text.Trim() + " " +
                    PatronymicTB.Text.Trim();

                personal.FullName = fullname;
                personal.PhoneNumber = PhoneTB.Text.Trim();
                personal.Email = EmailTB.Text.Trim();
                var a = SpecializationCB.SelectedItem as Specialization;
                personal.SpecializationId = a.SpecializationId;
                personal.Login = LoginTB.Text.Trim();
                personal.Password = PasswordTB.Text.Trim();
                DBConnection.veterinary.Personal.Add(personal);
                DBConnection.veterinary.SaveChanges();
                NavigationService.Navigate(new AuthorizationPage());
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

        private void LoginTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDigitOnly(e.Text);
        }

        private void PasswordTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDigitOnly(e.Text);
        }

        private void LoginTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LoginTB.Text.Length > 3)
            {
                LoginTB.Text = LoginTB.Text.Substring(0, 3);
                LoginTB.CaretIndex = LoginTB.Text.Length;
            }
        }

        private void PasswordTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PasswordTB.Text.Length > 5)
            {
                PasswordTB.Text = PasswordTB.Text.Substring(0, 5);
                PasswordTB.CaretIndex = PasswordTB.Text.Length;
            }
        }
    }
}
