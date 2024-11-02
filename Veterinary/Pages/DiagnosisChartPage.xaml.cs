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
    /// Логика взаимодействия для DiagnosisChartPage.xaml
    /// </summary>
    public partial class DiagnosisChartPage : Page
    {
        public SeriesCollection DiagnosisSeriesCollection { get; set; }
        public List<string> DiagnosisLabels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public DiagnosisChartPage()
        {
            InitializeComponent();
            YFormatter = value => value.ToString("N0");
            LoadDiagnosisData();
            DataContext = this;
        }

        private void LoadDiagnosisData()
        {
            // Получаем данные из базы данных
            var diagnosisData = DBConnection.veterinary.MedicalRecords
                .GroupBy(m => m.DiagnosisId)
                .Select(g => new
                {
                    DiagnosisId = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // Подготавливаем данные для графика
            DiagnosisSeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Диагнозы",
                    Values = new ChartValues<int>(),
                    Fill = new SolidColorBrush(Color.FromRgb(0x6E, 0x7D, 0x64)) // Цвет столбцов #FF6E7D64

                }
            };
            DiagnosisLabels = new List<string>();

            // Подгружаем названия диагнозов и их количества
            foreach (var diagnosis in diagnosisData)
            {
                var diagnosisName = DBConnection.veterinary.Diagnosis
                    .Where(d => d.DiagnosisId == diagnosis.DiagnosisId)
                    .Select(d => d.Name)
                    .FirstOrDefault();

                DiagnosisLabels.Add(diagnosisName);
                DiagnosisSeriesCollection[0].Values.Add(diagnosis.Count);
            }
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void SavePdfBT_Click(object sender, RoutedEventArgs e)
        {
            // Создаем новый PDF документ
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Диагнозы - Статистика";

            // Добавляем страницу в документ
            PdfPage page = document.AddPage();
            page.Width = 595; // A4 ширина
            page.Height = 842; // A4 высота

            // Устанавливаем белый фон и рендерим график в изображение
            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, DiagnosisChart.ActualWidth, DiagnosisChart.ActualHeight));
                dc.DrawRectangle(new VisualBrush(DiagnosisChart), null, new Rect(0, 0, DiagnosisChart.ActualWidth, DiagnosisChart.ActualHeight));
            }

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)DiagnosisChart.ActualWidth, (int)DiagnosisChart.ActualHeight, 96, 96, PixelFormats.Pbgra32);
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
            string filename = "DiagnosisStatistics.pdf";
            document.Save(filename);
            Process.Start(filename); // Открываем PDF после сохранения
        }
    }
}
