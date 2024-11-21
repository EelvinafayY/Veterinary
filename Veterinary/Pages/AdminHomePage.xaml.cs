using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Veterinary.DB;

namespace Veterinary.Pages
{
    public partial class AdminHomePage : Page
    {
        public static List<Personal> personals { get; set; }
        Personal contextPersonal;
        public static List<Clients> clients { get; set; }
        public static List<Animals> animals { get; set; }
        public static List<Status> status { get; set; }
        public static List<Reason> reasons { get; set; }
        public static List<Appointments> appointments { get; set; }
        public static List<Services> services { get; set; }

        public AdminHomePage(Personal personal)
        {
            InitializeComponent();
            contextPersonal = personal;
            appointments = DBConnection.veterinary.Appointments.ToList();
            SheduleLV.ItemsSource = appointments;
            status = DBConnection.veterinary.Status.ToList();
            this.DataContext = this;
        }

        private void SortOrderCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void DateFilterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void StatusCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void ApplyFiltersAndSort()
        {
            DateTime now = DateTime.Now.Date;
            IEnumerable<Appointments> filteredAppointments = appointments;

            // Фильтрация по дате
            switch (DateFilterCB.SelectedIndex)
            {
                case 0: // Сегодня
                    filteredAppointments = filteredAppointments.Where(a => a.Date.HasValue && a.Date.Value.Date == now);
                    break;
                case 1: // Вчера
                    filteredAppointments = filteredAppointments.Where(a => a.Date.HasValue && a.Date.Value.Date == now.AddDays(-1));
                    break;
                case 2: // Последние 7 дней
                    DateTime sevenDaysAgo = now.AddDays(-7);
                    filteredAppointments = filteredAppointments.Where(a => a.Date.HasValue && a.Date.Value.Date >= sevenDaysAgo && a.Date.Value.Date <= now);
                    break;
                case 3: // Последний месяц
                    DateTime oneMonthAgo = now.AddMonths(-1);
                    filteredAppointments = filteredAppointments.Where(a => a.Date.HasValue && a.Date.Value.Date >= oneMonthAgo && a.Date.Value.Date <= now);
                    break;
                case 4: // Без фильтра по дате
                    break;
                default:
                    break;
            }

            // Фильтрация по статусу
            switch (StatusCB.SelectedIndex)
            {
                case 0:
                filteredAppointments = filteredAppointments.Where(a => a.StatusId == 1);
                    break;
                case 1:
                filteredAppointments = filteredAppointments.Where(a => a.StatusId == 2);
                    break;
                case 2:
                filteredAppointments = filteredAppointments.Where(a => a.StatusId == 3);
                    break;
                case 3:
                    break;
                default :
                    break;
            }

            // Сортировка
            switch (SortOrderCB.SelectedIndex)
            {
                case 0: // По возрастанию
                    filteredAppointments = filteredAppointments.OrderBy(a => a.Date ?? DateTime.MaxValue);
                    break;
                case 1: // По убыванию
                    filteredAppointments = filteredAppointments.OrderByDescending(a => a.Date ?? DateTime.MinValue);
                    break;
                default:
                    break;
            }

            // Обновление источника данных
            SheduleLV.ItemsSource = filteredAppointments.ToList();
        }

        private void DiagnosisBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DiagnosisChartPage());
        }

        private void SpeciasesBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AnimalPopularityPage());
        }

        private void ServicesBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ServicesChartPage());
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            DBConnection.loginedPersonal = null;
            NavigationService.Navigate(new AuthorizationPage());
        }
    }
}
