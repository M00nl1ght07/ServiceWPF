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

namespace ServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage : Page
    {
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

                    // Общее количество заявок
                    var totalQuery = "SELECT COUNT(*) FROM Requests";
                    using (var command = new SqlCommand(totalQuery, connection))
                    {
                        TotalRequestsCount.Text = command.ExecuteScalar().ToString();
                    }

                    // Статистика по статусам
                    var statusQuery = @"SELECT 
                                    S.Name,
                                    COUNT(R.RequestID) as Count
                                    FROM RequestStatuses S
                                    LEFT JOIN Requests R ON S.StatusID = R.StatusID
                                    GROUP BY S.Name
                                    ORDER BY Count DESC";

                    using (var command = new SqlCommand(statusQuery, connection))
                    {
                        var statusStats = new List<dynamic>();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                statusStats.Add(new
                                {
                                    Status = reader.GetString(0),
                                    Count = reader.GetInt32(1)
                                });
                            }
                        }
                        StatusStatsList.ItemsSource = statusStats;
                    }

                    // Статистика по приоритетам
                    var priorityQuery = @"SELECT 
                                      P.Name,
                                      COUNT(R.RequestID) as Count
                                      FROM RequestPriorities P
                                      LEFT JOIN Requests R ON P.PriorityID = R.PriorityID
                                      GROUP BY P.Name
                                      ORDER BY Count DESC";

                    using (var command = new SqlCommand(priorityQuery, connection))
                    {
                        var priorityStats = new List<dynamic>();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                priorityStats.Add(new
                                {
                                    Priority = reader.GetString(0),
                                    Count = reader.GetInt32(1)
                                });
                            }
                        }
                        PriorityStatsList.ItemsSource = priorityStats;
                    }

                    // Топ исполнителей
                    var executorsQuery = @"SELECT TOP 5
                                       CONCAT(U.LastName, ' ', LEFT(U.FirstName, 1), '.', 
                                           CASE WHEN U.MiddleName IS NOT NULL 
                                           THEN CONCAT(LEFT(U.MiddleName, 1), '.') 
                                           ELSE '' END) as ExecutorName,
                                       COUNT(R.RequestID) as CompletedCount
                                       FROM Users U
                                       JOIN Requests R ON U.UserID = R.ExecutorID
                                       JOIN RequestStatuses S ON R.StatusID = S.StatusID
                                       WHERE S.Name = 'Завершена'
                                       GROUP BY U.UserID, U.LastName, U.FirstName, U.MiddleName
                                       ORDER BY CompletedCount DESC";

                    using (var command = new SqlCommand(executorsQuery, connection))
                    {
                        var executorStats = new List<dynamic>();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                executorStats.Add(new
                                {
                                    Name = reader.GetString(0),
                                    CompletedCount = reader.GetInt32(1)
                                });
                            }
                        }
                        TopExecutorsList.ItemsSource = executorStats;
                    }

                    // Среднее время выполнения заявок (в часах)
                    var avgTimeQuery = @"SELECT 
                                     AVG(DATEDIFF(HOUR, CreatedDate, CompletionDate)) as AvgHours
                                     FROM Requests 
                                     WHERE CompletionDate IS NOT NULL";

                    using (var command = new SqlCommand(avgTimeQuery, connection))
                    {
                        var result = command.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            var hours = Convert.ToDouble(result);
                            AverageCompletionTime.Text = $"{Math.Round(hours, 1)} часов";
                        }
                        else
                        {
                            AverageCompletionTime.Text = "Нет данных";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при загрузке статистики: {ex.Message}", NotificationType.Error);
            }
        }
    }
}


