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
    }
}


