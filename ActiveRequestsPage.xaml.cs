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
using ServiceWPF; // Добавляем для NotificationManager
using System.Data.SqlClient;

namespace ServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для ActiveRequestsPage.xaml
    /// </summary>
    public partial class ActiveRequestsPage : Page
    {
        private List<ActiveRequest> _allRequests;

        public class ActiveRequest
        {
            public int RequestID { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string CreatedDate { get; set; }
            public string Status { get; set; }
            public string CreatedDateFull { get; set; }
        }

        public ActiveRequestsPage()
        {
            InitializeComponent();
            LoadActiveRequests();
            StatusFilter.SelectedIndex = 0;
        }

        private void LoadActiveRequests()
        {
            try
            {
                var currentUserLogin = "";
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    currentUserLogin = mainWindow.CurrentUserLogin;
                }

                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    var query = @"SELECT 
                                R.RequestID,
                                R.Title,
                                R.Description,
                                FORMAT(R.CreatedDate, 'dd.MM.yyyy') as CreatedDate,
                                FORMAT(R.CreatedDate, 'dd.MM.yyyy HH:mm:ss') as CreatedDateFull,
                                S.Name as Status
                                FROM Requests R
                                JOIN RequestStatuses S ON R.StatusID = S.StatusID
                                JOIN Users U ON R.ExecutorID = U.UserID
                                WHERE U.Login = @Login
                                AND S.Name IN (N'В работе', N'На проверке', N'Завершена')
                                ORDER BY R.CreatedDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Login", currentUserLogin);
                        using (var reader = command.ExecuteReader())
                        {
                            _allRequests = new List<ActiveRequest>();
                            while (reader.Read())
                            {
                                _allRequests.Add(new ActiveRequest
                                {
                                    RequestID = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    CreatedDate = reader.GetString(3),
                                    CreatedDateFull = reader.GetString(4),
                                    Status = reader.GetString(5)
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

            // Применяем фильтр по статусу
            if (StatusFilter.SelectedIndex > 0)
            {
                var selectedStatus = (StatusFilter.SelectedItem as ComboBoxItem).Content.ToString();
                filteredRequests = filteredRequests.Where(r => r.Status == selectedStatus).ToList();
            }

            RequestsList.ItemsSource = filteredRequests;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded) ApplyFilters();
        }

        private void RequestsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RequestsList.SelectedItem != null)
            {
                var selectedRequest = RequestsList.SelectedItem as ActiveRequest;
                NavigationService?.Navigate(new RequestDetailsPage(selectedRequest.RequestID));
                RequestsList.SelectedItem = null;
            }
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.Tag != null)
            {
                var requestId = (int)comboBox.Tag;
                var newStatus = ((ComboBoxItem)comboBox.SelectedItem).Content.ToString();

                try
                {
                    using (var connection = DatabaseManager.GetConnection())
                    {
                        connection.Open();
                        using (var transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                // Получаем ID нового статуса
                                var statusQuery = "SELECT StatusID FROM RequestStatuses WHERE Name = @StatusName";
                                int statusId;
                                using (var command = new SqlCommand(statusQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@StatusName", newStatus);
                                    statusId = (int)command.ExecuteScalar();
                                }

                                // Обновляем статус заявки
                                var updateQuery = @"UPDATE Requests 
                                                SET StatusID = @StatusID,
                                                    LastModifiedDate = GETDATE(),
                                                    CompletionDate = CASE WHEN @StatusName = N'Завершена' 
                                                                   THEN GETDATE() 
                                                                   ELSE CompletionDate END
                                                WHERE RequestID = @RequestID";

                                using (var command = new SqlCommand(updateQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@StatusID", statusId);
                                    command.Parameters.AddWithValue("@StatusName", newStatus);
                                    command.Parameters.AddWithValue("@RequestID", requestId);
                                    command.ExecuteNonQuery();
                                }

                                // Добавляем запись в историю
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
                                    userId = (int)command.ExecuteScalar();
                                }

                                var historyQuery = @"INSERT INTO RequestHistory 
                                                 (RequestID, StatusID, ChangedByUserID, ChangeDate, Comment)
                                                 VALUES 
                                                 (@RequestID, @StatusID, @UserID, GETDATE(), @Comment)";

                                using (var command = new SqlCommand(historyQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@RequestID", requestId);
                                    command.Parameters.AddWithValue("@StatusID", statusId);
                                    command.Parameters.AddWithValue("@UserID", userId);
                                    command.Parameters.AddWithValue("@Comment", $"Статус изменен на '{newStatus}'");
                                    command.ExecuteNonQuery();
                                }

                                // Получаем автора заявки и заголовок
                                string requestAuthorLogin, requestTitle;
                                using (var command = new SqlCommand(
                                    @"SELECT U.Login, R.Title
                                      FROM Requests R 
                                      JOIN Users U ON R.CreatedByUserID = U.UserID 
                                      WHERE R.RequestID = @RequestID", connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@RequestID", requestId);
                                    using (var reader = command.ExecuteReader())
                                    {
                                        reader.Read();
                                        requestAuthorLogin = reader.GetString(0);
                                        requestTitle = reader.GetString(1);
                                    }
                                }

                                // Отправляем уведомление автору заявки
                                if (requestAuthorLogin != currentUserLogin)
                                {
                                    NotificationManager.CreateNotification(
                                        requestAuthorLogin,
                                        "Статус заявки изменен",
                                        $"Статус вашей заявки '{requestTitle}' изменен на '{newStatus}'",
                                        NotificationType.Info
                                    );
                                }

                                // Получаем всех администраторов
                                List<string> adminLogins = new List<string>();
                                using (var command = new SqlCommand(
                                    @"SELECT U.Login 
                                      FROM Users U 
                                      JOIN Roles R ON U.RoleID = R.RoleID 
                                      WHERE R.Name = 'Admin' AND U.Login != @CurrentUser", connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@CurrentUser", currentUserLogin);
                                    using (var reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            adminLogins.Add(reader.GetString(0));
                                        }
                                    }
                                }

                                // Отправляем уведомления всем администраторам
                                foreach (var adminLogin in adminLogins)
                                {
                                    NotificationManager.CreateNotification(
                                        adminLogin,
                                        "Изменение статуса заявки",
                                        $"Исполнитель {currentUserLogin} изменил статус заявки '{requestTitle}' на '{newStatus}'",
                                        NotificationType.Info
                                    );
                                }

                                transaction.Commit();
                                NotificationManager.Show($"Статус заявки успешно изменен на '{newStatus}'", NotificationType.Success);
                                LoadActiveRequests(); // Перезагружаем список
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
                    NotificationManager.Show($"Ошибка при изменении статуса: {ex.Message}", NotificationType.Error);
                    LoadActiveRequests(); // Перезагружаем список для отмены изменений в UI
                }
            }
        }
    }
}


