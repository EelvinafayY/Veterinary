using LiveCharts;
using LiveCharts.Wpf;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Veterinary.DB;

namespace Veterinary.Pages
{
    public partial class AnimalPopularityPage : Page
    {
        public SeriesCollection AnimalSeriesCollection { get; set; }
        public List<string> AnimalLabels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public AnimalPopularityPage()
        {
            InitializeComponent();
            YFormatter = value => value.ToString("N0");
            LoadAnimalData();
            DataContext = this;
        }

        private void LoadAnimalData()
        {
            // Получаем данные о количестве животных, сгруппированных по видам
            var speciesData = DBConnection.veterinary.Animals
                .GroupBy(a => a.Breed.SpeciesId) // Группируем по SpeciesId
                .Select(g => new
                {
                    SpeciesId = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // Подготавливаем данные для графика
            AnimalSeriesCollection = new SeriesCollection
    {
        new ColumnSeries
        {
            Title = "Виды",
            Values = new ChartValues<int>(),
            Fill = new SolidColorBrush(Color.FromRgb(0x6E, 0x7D, 0x64)) // Цвет столбцов #FF6E7D64
        }
    };
            AnimalLabels = new List<string>();

            // Подгружаем названия видов и их количества
            foreach (var species in speciesData)
            {
                var speciesName = DBConnection.veterinary.Species
                    .Where(s => s.SpeciesId == species.SpeciesId)
                    .Select(s => s.Name)
                    .FirstOrDefault();

                if (speciesName != null)
                {
                    AnimalLabels.Add(speciesName);
                    AnimalSeriesCollection[0].Values.Add(species.Count);
                }
            }
        }

        private void SavePdfBT_Click(object sender, RoutedEventArgs e)
        {
            // Создаем новый PDF документ
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Популярность видов животных";

            // Добавляем страницу в документ
            PdfPage page = document.AddPage();
            page.Width = 595; // A4 ширина
            page.Height = 842; // A4 высота

            // Устанавливаем белый фон и рендерим график в изображение
            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, AnimalPopularityChart.ActualWidth, AnimalPopularityChart.ActualHeight));
                dc.DrawRectangle(new VisualBrush(AnimalPopularityChart), null, new Rect(0, 0, AnimalPopularityChart.ActualWidth, AnimalPopularityChart.ActualHeight));
            }

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)AnimalPopularityChart.ActualWidth, (int)AnimalPopularityChart.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(dv);

            // Конвертируем изображение в байты
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Save(stream);
                stream.Position = 0;
                XImage img = XImage.FromStream(stream);

                // Рисуем изображение на PDF странице, с учетом масштабирования для страницы
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Рассчитываем масштаб, чтобы график поместился на странице
                double scaleX = page.Width / bmp.Width;
                double scaleY = page.Height / bmp.Height;
                double scale = Math.Min(scaleX, scaleY);

                // Центрирование изображения на странице
                double x = (page.Width - bmp.Width * scale) / 2;
                double y = (page.Height - bmp.Height * scale) / 2;

                gfx.DrawImage(img, x, y, bmp.Width * scale, bmp.Height * scale);
            }

            // Сохранение PDF файла
            string filename = "AnimalPopularityStatistics.pdf";
            document.Save(filename);
            System.Diagnostics.Process.Start(filename); // Открываем PDF после сохранения
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            // Логика возврата на предыдущую страницу
            NavigationService.GoBack();
        }
    }
}
