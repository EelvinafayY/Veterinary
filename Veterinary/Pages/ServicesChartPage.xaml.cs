using LiveCharts.Wpf;
using LiveCharts;
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
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;
using System.IO;

namespace Veterinary.Pages
{
    /// <summary>
    /// Логика взаимодействия для ServicesChartPage.xaml
    /// </summary>
    public partial class ServicesChartPage : Page
    {
        public SeriesCollection ServicesSeriesCollection { get; set; } = new SeriesCollection();
        public List<string> ServicesLabels { get; set; } = new List<string>();

        public ServicesChartPage()
        {
            InitializeComponent();
            DataContext = this; // Устанавливаем контекст данных для привязки
            LoadServicesData(); // Загружаем данные для графика
        }

        private void LoadServicesData()
        {
            // Получаем данные о количестве назначений, сгруппированных по услугам
            var serviceData = DBConnection.veterinary.Appointments
                .GroupBy(a => a.ServiceId) // Группируем по ServiceId
                .Select(g => new
                {
                    ServiceId = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // Подготавливаем данные для графика
            ServicesSeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Услуги",
                    Values = new ChartValues<int>(),
                    Fill = new SolidColorBrush(Color.FromRgb(0x6E, 0x7D, 0x64)) // Цвет столбцов #FF6E7D64
                }
            };
            ServicesLabels = new List<string>();

            // Подгружаем названия услуг и их количества
            foreach (var service in serviceData)
            {
                var serviceName = DBConnection.veterinary.Services
                    .Where(s => s.ServiceId == service.ServiceId)
                    .Select(s => s.Name)
                    .FirstOrDefault();


                if (serviceName != null)
                {
                    //if (serviceName.Length > 25)
                    //{
                    //    serviceName = serviceName.Substring(0, 25) + "...";
                    //}
                    ServicesLabels.Add(serviceName);
                    ServicesSeriesCollection[0].Values.Add(service.Count);
                
                }
            }
        }

        private void ExitBT_Click(object sender, object e)
        {
            NavigationService.GoBack();
        }

        private void SavePdfBT_Click(object sender, RoutedEventArgs e)
        {
            // Создаем новый PDF документ
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Услуги - Статистика";

            // Добавляем страницу в документ
            PdfPage page = document.AddPage();
            page.Width = 595; // A4 ширина
            page.Height = 842; // A4 высота

            // Устанавливаем белый фон и рендерим график в изображение
            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, ServicesChart.ActualWidth, ServicesChart.ActualHeight));
                dc.DrawRectangle(new VisualBrush(ServicesChart), null, new Rect(0, 0, ServicesChart.ActualWidth, ServicesChart.ActualHeight));
            }

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)ServicesChart.ActualWidth, (int)ServicesChart.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(dv);

            // Конвертируем изображение в байты
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Save(stream);
                stream.Position = 0; // Обязательно переместите на начало потока
                XImage img = XImage.FromStream(stream);

                // Рисуем изображение на PDF странице, с учетом масштабирования для страницы
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Рассчитываем масштаб, чтобы график поместился на странице
                double scaleX = page.Width / bmp.Width;
                double scaleY = page.Height / bmp.Height;
                double scale = Math.Min(scaleX, scaleY);

                // Вычисляем позицию для центрирования
                double x = (page.Width - bmp.Width * scale) / 2;
                double y = (page.Height - bmp.Height * scale) / 2;

                gfx.DrawImage(img, x, y, bmp.Width * scale, bmp.Height * scale);
            }

            // Сохранение PDF файла
            string filename = "ServicesStatistics.pdf";
            document.Save(filename);
            Process.Start(filename); // Открываем PDF после сохранения
        }

        private void ServicesChart_DataHover(object sender, ChartPoint chartPoint)
        {
            
        }
    }
}
