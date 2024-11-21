using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для AddRecordPage.xaml
    /// </summary>
    public partial class AddRecordPage : Page
    {
        public static List<Animals> animals {  get; set; }
        public static List<Personal> personals { get; set; }
        public static List<Services> services { get; set; }
        public static List<Clients> clients { get; set; }
        public static Appointments appointments = new Appointments();
        Animals contextAnimal;
        Clients clientsAnimal;
        public AddRecordPage(Animals animals, Clients client)
        {
            InitializeComponent();
            contextAnimal = animals;
            PersonalsCB.ItemsSource = new List<Personal>(DBConnection.veterinary.Personal.Where(p => p.Specialization.RoleId == 2).ToList());
            AnimalTB.Text = contextAnimal.Name;
            clientsAnimal = client;
            ServicesCB.ItemsSource = new List<Services>(DBConnection.veterinary.Services.Where(s => s.TypeId == 1).ToList());
            this.DataContext = this;
        }

        private async void SaveBT_Click(object sender, RoutedEventArgs e)
        {
            var a = PersonalsCB.SelectedItem as Personal;
            var b = ServicesCB.SelectedItem as Services;

            if (a == null || b == null || DateDP.SelectedDate == null || string.IsNullOrWhiteSpace(TimeTB.Text))
            {
                MessageBox.Show("Заполните все поля!");
            }
            else
            {
                // Получаем выбранную дату и время
                DateTime appointmentDateTime = DateDP.SelectedDate.Value + DateTime.Parse(TimeTB.Text).TimeOfDay;

                // Проверяем, свободно ли время
                bool isAvailable = await IsTimeAvailable(appointmentDateTime, a.PersonalId);

                if (!isAvailable)
                {
                    // Если время занято, показываем следующее доступное время
                    var nextAvailableTime = appointmentDateTime.AddMinutes(30);
                    MessageBox.Show($"Время занято. Следующее доступное время: {nextAvailableTime:HH:mm}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    // Если время свободно, сохраняем запись
                    appointments.DoctorId = a.PersonalId;
                    appointments.ServiceId = b.ServiceId;
                    appointments.StatusId = 1;  // Можно добавить больше статусов
                    appointments.AnimalId = contextAnimal.AnimalId;
                    appointments.Date = appointmentDateTime;

                    // Добавляем запись в базу данных и сохраняем изменения
                    DBConnection.veterinary.Appointments.Add(appointments);
                    DBConnection.veterinary.SaveChanges();

                    // Навигация на страницу с информацией о животном
                    NavigationService.Navigate(new InfoAnimalPage(contextAnimal, clientsAnimal));
                }
            }
        }

        private async Task<bool> IsTimeAvailable(DateTime appointmentDateTime, int doctorId)
        {
            // Получаем все записи для этого врача и на указанную дату
            var startDate = appointmentDateTime.Date;
            var endDate = startDate.AddDays(1);

            var appointmentsList = await DBConnection.veterinary.Appointments
                .Where(a => a.DoctorId == doctorId && a.Date >= startDate && a.Date < endDate)
                .ToListAsync();


            // Проверяем, пересекаются ли времена
            foreach (var existingAppointment in appointmentsList)
            {
                var existingDateTime = existingAppointment.Date;

                // Если временная метка назначения или уже существующая запись null, пропускаем
                if (!existingDateTime.HasValue)
                    continue;

                // Проверяем, пересекаются ли времена
                DateTime existingStartTime = existingDateTime.Value;
                DateTime existingEndTime = existingStartTime.AddMinutes(30); // Предполагаем, что время записи - 30 минут

                // Проверяем, не пересекаются ли интервалы времени
                if ((appointmentDateTime < existingEndTime) && (appointmentDateTime.AddMinutes(30) > existingStartTime))
                {
                    return false; // Время занято
                }
            }

            return true; // Время свободно
        }




        private void TimeTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, что вводится только цифра
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true; // Блокируем ввод, если это не цифра
            }
        }

        private void TimeTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            // Если длина текста больше 2 и меньше 5, вставляем двоеточие
            if (textBox != null && textBox.Text.Length == 2)
            {
                textBox.Text = textBox.Text.Insert(2, ":");
                textBox.SelectionStart = textBox.Text.Length; // Перемещаем курсор в конец
            }

            // Проверка на корректность времени
            if (textBox != null && textBox.Text.Length == 5)
            {
                string timeInput = textBox.Text;
                if (!IsValidTime(timeInput))
                {
                    // Ввод неверного времени (меньше 10:00 или больше 18:00) - можно выводить ошибку
                    textBox.Text = ""; // Устанавливаем корректное значение (например, минимальное время)
                    textBox.SelectionStart = textBox.Text.Length; // Перемещаем курсор в конец
                    MessageBox.Show("Введите время в пределах с 10:00 до 18:00.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }
        }

        // Функция для проверки времени
        private bool IsValidTime(string timeInput)
        {
            // Проверяем, что введенный текст имеет корректный формат и находится в допустимом диапазоне времени
            if (timeInput.Length == 5 && timeInput[2] == ':')
            {
                int hours = int.Parse(timeInput.Substring(0, 2)); // Читаем часы
                int minutes = int.Parse(timeInput.Substring(3, 2)); // Читаем минуты

                // Проверяем, что время находится в пределах от 10:00 до 18:00
                if (hours >= 10 && hours <= 18)
                {
                    if (hours == 18 && minutes > 0)
                        return false; // 18:00 — это максимальное время, 18:01 уже не допустимо

                    return true;
                }
            }
            return false;
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new InfoAnimalPage(contextAnimal, clientsAnimal));
        }

        private void PersonalsCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var a = PersonalsCB.SelectedItem as Personal;
            if (a.SpecializationId == 1) 
            {
                ServicesCB.ItemsSource = new List<Services>(DBConnection.veterinary.Services.Where(s => s.ServiceId == 1 ||
                s.ServiceId == 6 || s.ServiceId == 1 || s.ServiceId == 12).ToList());
            }
            if (a.SpecializationId == 2)
            {
                ServicesCB.ItemsSource = new List<Services>(DBConnection.veterinary.Services.Where(s => s.ServiceId == 5 ||
                s.ServiceId == 10 || s.ServiceId == 13).ToList());
            }
            if (a.SpecializationId == 3)
            {
                ServicesCB.ItemsSource = new List<Services>(DBConnection.veterinary.Services.Where(s => s.ServiceId == 2 
                || s.ServiceId == 7).ToList());
            }
            if (a.SpecializationId == 4)
            {
                ServicesCB.ItemsSource = new List<Services>(DBConnection.veterinary.Services.Where(s => s.ServiceId == 4 ||
                s.ServiceId == 9).ToList());
            }
            if (a.SpecializationId == 5)
            {
                ServicesCB.ItemsSource = new List<Services>(DBConnection.veterinary.Services.Where(s => s.ServiceId == 3 ||
                s.ServiceId == 8).ToList());
            }
        }

        private void ServicesCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s = ServicesCB.SelectedItem as Services;
            if(s.ServiceId == 1 ||
                s.ServiceId == 6 || s.ServiceId == 1 || s.ServiceId == 12)
            {
                PersonalsCB.ItemsSource = new List<Personal>(DBConnection.veterinary.Personal.Where(
                    p => p.SpecializationId == 1).ToList());
            }
            if (s.ServiceId == 5 ||
                s.ServiceId == 10 || s.ServiceId == 13)
            {
                PersonalsCB.ItemsSource = new List<Personal>(DBConnection.veterinary.Personal.Where(
                    p => p.SpecializationId == 2).ToList());
            }
            if(s.ServiceId == 2
                || s.ServiceId == 7)
            {
                PersonalsCB.ItemsSource = new List<Personal>(DBConnection.veterinary.Personal.Where(
                   p => p.SpecializationId == 3).ToList());
            }
            if (s.ServiceId == 4
                || s.ServiceId == 9)
            {
                PersonalsCB.ItemsSource = new List<Personal>(DBConnection.veterinary.Personal.Where(
                   p => p.SpecializationId == 4).ToList());
            }
            if (s.ServiceId == 3
                || s.ServiceId == 8)
            {
                PersonalsCB.ItemsSource = new List<Personal>(DBConnection.veterinary.Personal.Where(
                   p => p.SpecializationId == 5).ToList());
            }
        }
    }
}
