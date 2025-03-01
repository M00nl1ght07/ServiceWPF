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
    /// Логика взаимодействия для AvailableRequestsPage.xaml
    /// </summary>
    public partial class AvailableRequestsPage : Page
    {
        private List<AvailableRequest> _allRequests;

        public class AvailableRequest
        {
            public int RequestID { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string CreatedDate { get; set; }
            public string Priority { get; set; }
            public string CreatedDateFull { get; set; }
        }

        public AvailableRequestsPage()
        {
            InitializeComponent();
            LoadAvailableRequests();
            PriorityFilter.SelectedIndex = 0;
        }

        private void LoadAvailableRequests()
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    var query = @"SELECT 
                                R.RequestID,
                                R.Title,
                                R.Description,
                                FORMAT(R.CreatedDate, 'dd.MM.yyyy') as CreatedDate,
                                FORMAT(R.CreatedDate, 'dd.MM.yyyy HH:mm:ss') as CreatedDateFull,
                                P.Name as Priority
                                FROM Requests R
                                JOIN RequestPriorities P ON R.PriorityID = P.PriorityID
                                JOIN RequestStatuses S ON R.StatusID = S.StatusID
                                WHERE S.Name = N'Новая'
                                ORDER BY R.CreatedDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            _allRequests = new List<AvailableRequest>();
                            while (reader.Read())
                            {
                                _allRequests.Add(new AvailableRequest
                                {
                                    RequestID = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    CreatedDate = reader.GetString(3),
                                    CreatedDateFull = reader.GetString(4),
                                    Priority = reader.GetString(5)
                                });
                            }
                        }
                    }
                }
                ApplyFilters();
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при загрузке заявок: {ex.Message}", NotificationType.Error);
            }
        }

        private void ApplyFilters()
        {
            var filteredRequests = _allRequests;

            // Применяем поиск
            var searchText = SearchBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredRequests = filteredRequests.Where(r =>
                    r.Title.ToLower().Contains(searchText) ||
                    r.Description.ToLower().Contains(searchText)).ToList();
            }

            // Применяем фильтр по приоритету
            if (PriorityFilter.SelectedIndex > 0)
            {
                var selectedPriority = (PriorityFilter.SelectedItem as ComboBoxItem).Content.ToString();
                filteredRequests = filteredRequests.Where(r => r.Priority == selectedPriority).ToList();
            }

            RequestsList.ItemsSource = filteredRequests;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void PriorityFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded) ApplyFilters();
        }

        private void TakeRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                var request = button.Tag as AvailableRequest;
                try
                {
                    using (var connection = DatabaseManager.GetConnection())
                    {
                        connection.Open();
                        using (var transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                // Получаем ID текущего пользователя
                                var currentUserLogin = "";
                                if (Application.Current.MainWindow is MainWindow mainWindow)
                                {
                                    currentUserLogin = mainWindow.CurrentUserLogin;
                                }

                                var getUserIdQuery = "SELECT UserID FROM Users WHERE Login = @Login";
                                int userId;
                                using (var command = new SqlCommand(getUserIdQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@Login", currentUserLogin);
                                    var result = command.ExecuteScalar();
                                    if (result == null)
                                    {
                                        throw new Exception("Пользователь не найден");
                                    }
                                    userId = (int)result;
                                }

                                // Получаем ID статуса "В работе"
                                int inProgressStatusId;
                                var statusQuery = "SELECT StatusID FROM RequestStatuses WHERE Name = N'В работе'";
                                using (var command = new SqlCommand(statusQuery, connection, transaction))
                                {
                                    var result = command.ExecuteScalar();
                                    if (result == null)
                                    {
                                        throw new Exception("Статус 'В работе' не найден");
                                    }
                                    inProgressStatusId = (int)result;
                                }

                                // Обновляем заявку
                                var updateQuery = @"UPDATE Requests 
                                                  SET StatusID = @StatusID,
                                                      ExecutorID = @ExecutorID,
                                                      LastModifiedDate = GETDATE()
                                                  WHERE RequestID = @RequestID";

                                using (var command = new SqlCommand(updateQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@StatusID", inProgressStatusId);
                                    command.Parameters.AddWithValue("@ExecutorID", userId);
                                    command.Parameters.AddWithValue("@RequestID", request.RequestID);
                                    command.ExecuteNonQuery();
                                }

                                // Добавляем запись в историю
                                var historyQuery = @"INSERT INTO RequestHistory 
                                                   (RequestID, StatusID, ChangedByUserID, ChangeDate, Comment)
                                                   VALUES 
                                                   (@RequestID, @StatusID, @UserID, GETDATE(), N'Заявка взята в работу')";

                                using (var command = new SqlCommand(historyQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@RequestID", request.RequestID);
                                    command.Parameters.AddWithValue("@StatusID", inProgressStatusId);
                                    command.Parameters.AddWithValue("@UserID", userId);
                                    command.ExecuteNonQuery();
                                }

                                transaction.Commit();
                                NotificationManager.Show("Заявка успешно взята в работу", NotificationType.Success);
                                LoadAvailableRequests(); // Перезагружаем список
                            }
                            catch (Exception)
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    NotificationManager.Show($"Ошибка при взятии заявки в работу: {ex.Message}", NotificationType.Error);
                }
            }
        }
    }
}
