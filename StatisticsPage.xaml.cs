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

                    // Общее количество заявок
                    var totalQuery = "SELECT COUNT(*) FROM Requests";
                    using (var command = new SqlCommand(totalQuery, connection))
                    {
                        var count = command.ExecuteScalar();
                        TotalRequestsCount.Text = count.ToString();
                    }

                    // Статистика по статусам
                    var statusQuery = @"SELECT 
                                    S.Name as Status,
                                    COUNT(R.RequestID) as Count
                                    FROM RequestStatuses S
                                    LEFT JOIN Requests R ON S.StatusID = R.StatusID
                                    GROUP BY S.Name, S.StatusID
                                    ORDER BY S.StatusID";

                    using (var command = new SqlCommand(statusQuery, connection))
                    {
                        var statusStats = new List<StatusStat>();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                statusStats.Add(new StatusStat
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
                                      P.Name as Priority,
                                      COUNT(R.RequestID) as Count
                                      FROM RequestPriorities P
                                      LEFT JOIN Requests R ON P.PriorityID = R.PriorityID
                                      GROUP BY P.Name, P.PriorityID
                                      ORDER BY P.PriorityID";

                    using (var command = new SqlCommand(priorityQuery, connection))
                    {
                        var priorityStats = new List<PriorityStat>();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                priorityStats.Add(new PriorityStat
                                {
                                    Priority = reader.GetString(0),
                                    Count = reader.GetInt32(1)
                                });
                            }
                        }
                        PriorityStatsList.ItemsSource = priorityStats;
                    }

                    // Топ исполнителей
                    var executorsQuery = @"SELECT 
                                       CONCAT(U.LastName, ' ', U.FirstName, ' ', ISNULL(U.MiddleName, '')) as Name,
                                       COUNT(R.RequestID) as CompletedCount
                                       FROM Users U
                                       JOIN Requests R ON U.UserID = R.ExecutorID
                                       JOIN RequestStatuses S ON R.StatusID = S.StatusID
                                       WHERE S.Name = N'Завершена'
                                       GROUP BY U.UserID, U.LastName, U.FirstName, U.MiddleName
                                       ORDER BY CompletedCount DESC";

                    using (var command = new SqlCommand(executorsQuery, connection))
                    {
                        var executorStats = new List<ExecutorStat>();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                executorStats.Add(new ExecutorStat
                                {
                                    Name = reader.GetString(0),
                                    CompletedCount = reader.GetInt32(1)
                                });
                            }
                        }
                        TopExecutorsList.ItemsSource = executorStats;
                    }

                    // Среднее время выполнения заявок
                    var avgTimeQuery = @"SELECT 
                                     ISNULL(AVG(DATEDIFF(HOUR, CreatedDate, CompletionDate)), 0) as AvgHours
                                     FROM Requests 
                                     WHERE CompletionDate IS NOT NULL";

                    using (var command = new SqlCommand(avgTimeQuery, connection))
                    {
                        var hours = Convert.ToDouble(command.ExecuteScalar());
                        AverageCompletionTime.Text = $"{Math.Round(hours, 1)} часов";
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


