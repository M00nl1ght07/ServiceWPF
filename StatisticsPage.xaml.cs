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
using System.Data.SqlClient;
using LiveCharts;
using LiveCharts.Wpf;
using ClosedXML.Excel;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;

namespace ServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage : Page
    {
        public class StatusStat
        {
            public string Status { get; set; }
            public int Count { get; set; }
        }

        public class PriorityStat
        {
            public string Priority { get; set; }
            public int Count { get; set; }
        }

        public class ExecutorStat
        {
            public string Name { get; set; }
            public int CompletedCount { get; set; }
        }

        public StatisticsPage()
        {
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    LoadGeneralStatistics(connection);
                    LoadStatusStatistics(connection);
                    LoadPriorityStatistics(connection);
                    LoadExecutorStatistics(connection);
                    LoadCompletionTimeStatistics(connection);
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при загрузке статистики: {ex.Message}", NotificationType.Error);
            }
        }

        private void LoadGeneralStatistics(SqlConnection connection)
        {
            // Общее количество заявок
            using (var command = new SqlCommand("SELECT COUNT(*) FROM Requests", connection))
            {
                TotalRequestsCount.Text = command.ExecuteScalar().ToString();
            }

            // Количество активных заявок
            using (var command = new SqlCommand(@"SELECT COUNT(*) 
                  FROM Requests R 
                  JOIN RequestStatuses S ON R.StatusID = S.StatusID 
                  WHERE S.Name IN (N'В работе', N'На проверке')", connection))
            {
                ActiveRequestsCount.Text = command.ExecuteScalar().ToString();
            }

            // Среднее время выполнения
            using (var command = new SqlCommand(@"SELECT AVG(DATEDIFF(HOUR, CreatedDate, LastModifiedDate))
                  FROM Requests R
                  JOIN RequestStatuses S ON R.StatusID = S.StatusID
                  WHERE S.Name = N'Завершена'", connection))
            {
                var result = command.ExecuteScalar();
                int hours = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                AverageCompletionTime.Text = $"{hours} часов";
            }

            // Количество завершенных заявок за неделю
            using (var command = new SqlCommand(
                @"SELECT COUNT(*) 
                  FROM Requests R
                  JOIN RequestStatuses S ON R.StatusID = S.StatusID
                  WHERE S.Name = N'Завершена'
                  AND R.LastModifiedDate >= DATEADD(day, -7, GETDATE())", connection))
            {
                CompletedThisWeek.Text = command.ExecuteScalar().ToString();
            }
        }

        private void LoadStatusStatistics(SqlConnection connection)
        {
            var statusData = new Dictionary<string, int>();
            using (var command = new SqlCommand(@"SELECT S.Name, COUNT(*) as Count
                  FROM Requests R
                  JOIN RequestStatuses S ON R.StatusID = S.StatusID
                  GROUP BY S.Name, S.StatusID
                  ORDER BY S.StatusID", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        statusData.Add(reader.GetString(0), reader.GetInt32(1));
                    }
                }
            }

            // Добавим цвета для статусов
            var colors = new[]
            {
                "#2196F3", // Основной цвет приложения
                "#4CAF50", // Зеленый для завершенных
                "#FFC107", // Желтый для проверки
                "#F44336"  // Красный для отмененных
            };

            var pieSeriesCollection = new SeriesCollection();
            var i = 0;
            foreach (var status in statusData)
            {
                pieSeriesCollection.Add(new PieSeries
                {
                    Title = status.Key,
                    Values = new ChartValues<int> { status.Value },
                    DataLabels = true,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[i % colors.Length]))
                });
                i++;
            }
            StatusPieChart.Series = pieSeriesCollection;

            // Настройка столбчатой диаграммы
            var barSeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Количество заявок",
                    Values = new ChartValues<int>(statusData.Values),
                    DataLabels = true,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3"))
                }
            };
            StatusBarChart.Series = barSeriesCollection;
            StatusBarChart.AxisX[0].Labels = statusData.Keys.ToList();
            
            // Добавим настройки для лучшего отображения
            StatusBarChart.AxisY[0].MinValue = 0;
            StatusBarChart.AxisY[0].MaxValue = statusData.Values.Max() + 1;
            StatusBarChart.AxisX[0].Separator.Step = 1;
        }

        private void LoadPriorityStatistics(SqlConnection connection)
        {
            var priorityData = new Dictionary<string, int>();
            using (var command = new SqlCommand(@"SELECT P.Name, COUNT(*) as Count
                  FROM Requests R
                  JOIN RequestPriorities P ON R.PriorityID = P.PriorityID
                  GROUP BY P.Name", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        priorityData.Add(reader.GetString(0), reader.GetInt32(1));
                    }
                }
            }

            var seriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Количество заявок",
                    Values = new ChartValues<int>(priorityData.Values)
                }
            };
            PriorityBarChart.Series = seriesCollection;
            PriorityBarChart.AxisX[0].Labels = priorityData.Keys.ToList();
        }

        private void LoadExecutorStatistics(SqlConnection connection)
        {
            var executorData = new Dictionary<string, int>();
            using (var command = new SqlCommand(
                @"SELECT 
                    CONCAT(U.LastName, ' ', LEFT(U.FirstName, 1), '.') as ExecutorName,
                    COUNT(*) as CompletedCount
                  FROM Requests R
                  JOIN Users U ON R.ExecutorID = U.UserID
                  JOIN RequestStatuses S ON R.StatusID = S.StatusID
                  WHERE S.Name = N'Завершена'
                  GROUP BY U.UserID, U.LastName, U.FirstName
                  ORDER BY CompletedCount DESC", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        executorData.Add(reader.GetString(0), reader.GetInt32(1));
                    }
                }
            }

            var seriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Выполненные заявки",
                    Values = new ChartValues<int>(executorData.Values),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3"))
                }
            };
            ExecutorsChart.Series = seriesCollection;
            ExecutorsChart.AxisX[0].Labels = executorData.Keys.ToList();
        }

        private void LoadCompletionTimeStatistics(SqlConnection connection)
        {
            var completionTimeData = new Dictionary<string, double>();
            using (var command = new SqlCommand(
                @"SELECT 
                    P.Name,
                    AVG(CAST(DATEDIFF(HOUR, R.CreatedDate, R.LastModifiedDate) as float)) as AvgTime
                  FROM Requests R
                  JOIN RequestPriorities P ON R.PriorityID = P.PriorityID
                  JOIN RequestStatuses S ON R.StatusID = S.StatusID
                  WHERE S.Name = N'Завершена'
                  GROUP BY P.Name, P.PriorityID
                  ORDER BY P.PriorityID", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        completionTimeData.Add(reader.GetString(0), reader.GetDouble(1));
                    }
                }
            }

            var seriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Среднее время выполнения",
                    Values = new ChartValues<double>(completionTimeData.Values),
                    Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3")),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3F2FD")),
                    LineSmoothness = 0
                }
            };
            CompletionTimeChart.Series = seriesCollection;
            CompletionTimeChart.AxisX[0].Labels = completionTimeData.Keys.ToList();
        }

        // Добавьте новый метод для сохранения графика как изображения
        private MemoryStream ChartToImage(FrameworkElement chart)
        {
            // Создаем контейнер для графика
            var container = new Grid();
            container.Width = 800;
            container.Height = 600;
            container.Background = Brushes.White;

            // Создаем визуальную копию графика
            var visual = new VisualBrush(chart);
            var rectangle = new Rectangle
            {
                Width = 800,
                Height = 600,
                Fill = visual
            };

            container.Children.Add(rectangle);

            // Рендерим в окне нужного размера
            container.Measure(new Size(800, 600));
            container.Arrange(new Rect(0, 0, 800, 600));

            // Создаем RenderTargetBitmap
            var renderBitmap = new RenderTargetBitmap(
                800, 600, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(container);

            // Сохраняем в поток
            var stream = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            encoder.Save(stream);
            stream.Position = 0;
            
            return stream;
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    // Лист с общей статистикой
                    var worksheetGeneral = workbook.Worksheets.Add("Общая статистика");
                    
                    // Заголовки
                    worksheetGeneral.Cell("A1").Value = "Показатель";
                    worksheetGeneral.Cell("B1").Value = "Значение";
                    
                    // Данные
                    worksheetGeneral.Cell("A2").Value = "Всего заявок";
                    worksheetGeneral.Cell("B2").Value = int.Parse(TotalRequestsCount.Text);
                    
                    worksheetGeneral.Cell("A3").Value = "Среднее время";
                    worksheetGeneral.Cell("B3").Value = AverageCompletionTime.Text;
                    
                    worksheetGeneral.Cell("A4").Value = "Заявок в работе";
                    worksheetGeneral.Cell("B4").Value = int.Parse(ActiveRequestsCount.Text);
                    
                    worksheetGeneral.Cell("A5").Value = "Завершено за неделю";
                    worksheetGeneral.Cell("B5").Value = int.Parse(CompletedThisWeek.Text);

                    // Лист со статистикой по статусам
                    var worksheetStatus = workbook.Worksheets.Add("Статистика по статусам");
                    worksheetStatus.Cell("A1").Value = "Статус";
                    worksheetStatus.Cell("B1").Value = "Количество";

                    var statusSeries = StatusBarChart.Series[0] as ColumnSeries;
                    var statusLabels = StatusBarChart.AxisX[0].Labels;
                    for (int i = 0; i < statusLabels.Count; i++)
                    {
                        worksheetStatus.Cell(i + 2, 1).Value = statusLabels[i];
                        worksheetStatus.Cell(i + 2, 2).Value = (int)statusSeries.Values[i];
                    }

                    // Лист со статистикой по исполнителям
                    var worksheetExecutors = workbook.Worksheets.Add("Исполнители");
                    worksheetExecutors.Cell("A1").Value = "Исполнитель";
                    worksheetExecutors.Cell("B1").Value = "Выполнено заявок";

                    var executorSeries = ExecutorsChart.Series[0] as ColumnSeries;
                    var executorLabels = ExecutorsChart.AxisX[0].Labels;
                    for (int i = 0; i < executorLabels.Count; i++)
                    {
                        worksheetExecutors.Cell(i + 2, 1).Value = executorLabels[i];
                        worksheetExecutors.Cell(i + 2, 2).Value = (int)executorSeries.Values[i];
                    }

                    // Добавляем лист с графиками
                    var worksheetCharts = workbook.Worksheets.Add("Графики");
                    
                    // Сохраняем графики как изображения
                    using (var pieChartStream = ChartToImage(StatusPieChart))
                    using (var barChartStream = ChartToImage(StatusBarChart))
                    using (var priorityChartStream = ChartToImage(PriorityBarChart))
                    using (var executorsChartStream = ChartToImage(ExecutorsChart))
                    using (var completionTimeChartStream = ChartToImage(CompletionTimeChart))
                    {
                        // Статистика по статусам (круговая)
                        worksheetCharts.Cell("A1").Value = "Статистика по статусам (круговая диаграмма)";
                        worksheetCharts.Cell("A1").Style.Font.Bold = true;
                        var pieImage = worksheetCharts.AddPicture(pieChartStream)
                            .MoveTo(worksheetCharts.Cell("A2"))
                            .WithSize(800, 400);

                        // Статистика по статусам (столбчатая)
                        worksheetCharts.Cell("A27").Value = "Статистика по статусам (столбчатая диаграмма)";
                        worksheetCharts.Cell("A27").Style.Font.Bold = true;
                        var barImage = worksheetCharts.AddPicture(barChartStream)
                            .MoveTo(worksheetCharts.Cell("A28"))
                            .WithSize(800, 400);

                        // Статистика по приоритетам
                        worksheetCharts.Cell("A53").Value = "Статистика по приоритетам";
                        worksheetCharts.Cell("A53").Style.Font.Bold = true;
                        var priorityImage = worksheetCharts.AddPicture(priorityChartStream)
                            .MoveTo(worksheetCharts.Cell("A54"))
                            .WithSize(800, 400);

                        // Статистика по исполнителям
                        worksheetCharts.Cell("A79").Value = "Статистика по исполнителям";
                        worksheetCharts.Cell("A79").Style.Font.Bold = true;
                        var executorsImage = worksheetCharts.AddPicture(executorsChartStream)
                            .MoveTo(worksheetCharts.Cell("A80"))
                            .WithSize(800, 400);

                        // Среднее время выполнения
                        worksheetCharts.Cell("A105").Value = "Среднее время выполнения по приоритетам";
                        worksheetCharts.Cell("A105").Style.Font.Bold = true;
                        var completionTimeImage = worksheetCharts.AddPicture(completionTimeChartStream)
                            .MoveTo(worksheetCharts.Cell("A106"))
                            .WithSize(800, 400);

                        // Настраиваем высоту строк для заголовков
                        worksheetCharts.Row(1).Height = 30;   // Заголовок 1
                        worksheetCharts.Row(27).Height = 30;  // Заголовок 2
                        worksheetCharts.Row(53).Height = 30;  // Заголовок 3
                        worksheetCharts.Row(79).Height = 30;  // Заголовок 4
                        worksheetCharts.Row(105).Height = 30; // Заголовок 5
                    }

                    // Форматирование
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        if (worksheet.Name != "Графики") // Пропускаем лист с графиками
                        {
                            var usedRange = worksheet.RangeUsed();
                            usedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            usedRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        }
                        worksheet.Columns().AdjustToContents();
                    }

                    // Сохранение файла
                    var saveDialog = new SaveFileDialog
                    {
                        Filter = "Excel Files|*.xlsx",
                        DefaultExt = "xlsx",
                        FileName = $"Статистика_{DateTime.Now:yyyy-MM-dd}"
                    };

                    if (saveDialog.ShowDialog() == true)
                    {
                        workbook.SaveAs(saveDialog.FileName);
                        NotificationManager.Show("Статистика успешно экспортирована", NotificationType.Success);
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при экспорте: {ex.Message}", NotificationType.Error);
            }
        }
    }
}


