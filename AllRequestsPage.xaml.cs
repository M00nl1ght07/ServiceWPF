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

    /// Логика взаимодействия для AllRequestsPage.xaml

    /// </summary>

    public class AdminRequest

    {

        public int RequestID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string CreatedDate { get; set; }

        public string CreatedDateFull { get; set; }

        public string Status { get; set; }

        public string Priority { get; set; }

        public string Executor { get; set; }

        public int? ExecutorID { get; set; }

    }



    public partial class AllRequestsPage : Page

    {

        private List<AdminRequest> _allRequests;



        public AllRequestsPage()

        {

            InitializeComponent();

            LoadExecutors();

            LoadAllRequests();

        }



        private void LoadAllRequests()

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

                                FORMAT(R.CreatedDate, 'dd.MM.yyyy') as CreatedDateDisplay,

                                FORMAT(R.CreatedDate, 'dd.MM.yyyy HH:mm:ss') as CreatedDateFull,

                                S.Name as Status,

                                P.Name as Priority,

                                CASE 

                                    WHEN E.UserID IS NULL THEN 'Не назначен'

                                    ELSE CONCAT(E.LastName, ' ', LEFT(E.FirstName, 1), '.', 

                                        CASE WHEN E.MiddleName IS NOT NULL 

                                        THEN CONCAT(LEFT(E.MiddleName, 1), '.') 

                                        ELSE '' END)

                                END as Executor,

                                E.UserID as ExecutorID

                                FROM Requests R

                                JOIN RequestStatuses S ON R.StatusID = S.StatusID

                                JOIN RequestPriorities P ON R.PriorityID = P.PriorityID

                                LEFT JOIN Users E ON R.ExecutorID = E.UserID

                                ORDER BY R.CreatedDate DESC";



                    using (var command = new SqlCommand(query, connection))

                    {

                        using (var reader = command.ExecuteReader())

                        {

                            _allRequests = new List<AdminRequest>();

                            while (reader.Read())

                            {

                                _allRequests.Add(new AdminRequest

                                {

                                    RequestID = reader.GetInt32(0),

                                    Title = reader.GetString(1),

                                    Description = reader.GetString(2),

                                    CreatedDate = reader.GetString(3),

                                    CreatedDateFull = reader.GetString(4),

                                    Status = reader.GetString(5),

                                    Priority = reader.GetString(6),

                                    Executor = reader.GetString(7),

                                    ExecutorID = reader.IsDBNull(8) ? null : (int?)reader.GetInt32(8)

                                });

                            }

                            ApplyFilters();

                        }

                    }

                }

            }

            catch (Exception ex)

            {

                NotificationManager.Show($"Ошибка при загрузке заявок: {ex.Message}", NotificationType.Error);

            }

        }



        private void LoadExecutors()

        {

            try

            {

                using (var connection = DatabaseManager.GetConnection())

                {

                    connection.Open();

                    var query = @"SELECT 

                                CONCAT(U.LastName, ' ', LEFT(U.FirstName, 1), '.', 

                                    CASE WHEN U.MiddleName IS NOT NULL 

                                    THEN CONCAT(LEFT(U.MiddleName, 1), '.') 

                                    ELSE '' END) as ExecutorName

                                FROM Users U

                                WHERE U.RoleID = 2  -- ID роли исполнителя

                                ORDER BY U.LastName, U.FirstName";



                    ExecutorFilter.Items.Clear();

                    ExecutorFilter.Items.Add(new ComboBoxItem { Content = "Все исполнители" });

                    ExecutorFilter.Items.Add(new ComboBoxItem { Content = "Без исполнителя" });



                    using (var command = new SqlCommand(query, connection))

                    {

                        using (var reader = command.ExecuteReader())

                        {

                            while (reader.Read())

                            {

                                ExecutorFilter.Items.Add(new ComboBoxItem 

                                { 

                                    Content = reader.GetString(0)

                                });

                            }

                        }

                    }

                    ExecutorFilter.SelectedIndex = 0;

                }

            }

            catch (Exception ex)

            {

                NotificationManager.Show($"Ошибка при загрузке списка исполнителей: {ex.Message}", NotificationType.Error);

            }

        }



        private void ApplyFilters()

        {

            var filteredRequests = _allRequests;



            // Поиск

            var searchText = SearchBox.Text.Trim().ToLower();

            if (!string.IsNullOrEmpty(searchText))

            {

                filteredRequests = filteredRequests.Where(r => 

                    r.Title.ToLower().Contains(searchText) || 

                    r.Description.ToLower().Contains(searchText)).ToList();

            }



            // Фильтр по статусу

            if (StatusFilter.SelectedIndex > 0)

            {

                var selectedStatus = (StatusFilter.SelectedItem as ComboBoxItem).Content.ToString();

                filteredRequests = filteredRequests.Where(r => r.Status == selectedStatus).ToList();

            }



            // Фильтр по исполнителю

            if (ExecutorFilter.SelectedIndex > 0)

            {

                var selectedExecutor = (ExecutorFilter.SelectedItem as ComboBoxItem).Content.ToString();

                if (selectedExecutor == "Без исполнителя")

                {

                    filteredRequests = filteredRequests.Where(r => r.Executor == "Не назначен").ToList();

                }

                else

                {

                    filteredRequests = filteredRequests.Where(r => r.Executor == selectedExecutor).ToList();

                }

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



        private void ExecutorFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)

        {

            if (IsLoaded) ApplyFilters();

        }



        private void RequestsList_SelectionChanged(object sender, SelectionChangedEventArgs e)

        {

            if (RequestsList.SelectedItem != null)

            {

                var selectedRequest = RequestsList.SelectedItem as AdminRequest;

                NavigationService?.Navigate(new RequestDetailsPage(selectedRequest.RequestID));

                RequestsList.SelectedItem = null;

            }

        }



        private void AssignExecutor_Click(object sender, RoutedEventArgs e)

        {

            if (sender is Button button && button.Tag != null)

            {

                var request = button.Tag as AdminRequest;
                
                // Создаем окно выбора исполнителя
                var window = new Window
                {
                    Title = "Назначение исполнителя",
                    Width = 400,
                    Height = 500,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Background = Brushes.White,
                    WindowStyle = WindowStyle.ToolWindow,
                    ResizeMode = ResizeMode.NoResize
                };

                var panel = new StackPanel { Margin = new Thickness(20) };
                var title = new TextBlock
                {
                    Text = "Выберите исполнителя",
                    FontSize = 16,
                    FontWeight = FontWeights.Medium,
                    Margin = new Thickness(0, 0, 0, 15)
                };

                var executorsList = new ListView
                {
                    Margin = new Thickness(0, 0, 0, 15)
                };

                // Загружаем список исполнителей
                try
                {
                    using (var connection = DatabaseManager.GetConnection())
                    {
                        connection.Open();
                        var query = @"SELECT U.UserID, 
                                    CONCAT(U.LastName, ' ', U.FirstName, ' ', ISNULL(U.MiddleName, '')) as FullName
                            FROM Users U
                            JOIN UserRoles UR ON U.UserID = UR.UserID
                            JOIN Roles R ON UR.RoleID = R.RoleID
                            WHERE R.Name = N'Исполнитель'
                            ORDER BY U.LastName, U.FirstName";

                        using (var command = new SqlCommand(query, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var item = new ListViewItem
                                    {
                                        Content = reader.GetString(1),
                                        Tag = reader.GetInt32(0)
                                    };
                                    executorsList.Items.Add(item);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    NotificationManager.Show($"Ошибка при загрузке списка исполнителей: {ex.Message}", NotificationType.Error);
                    return;
                }

                var assignButton = new Button
                {
                    Content = "Назначить",
                    Height = 35,
                    Margin = new Thickness(0, 10, 0, 0),
                    IsEnabled = false
                };

                executorsList.SelectionChanged += (s, args) =>
                {
                    assignButton.IsEnabled = executorsList.SelectedItem != null;
                };

                assignButton.Click += async (s, args) =>
                {
                    if (executorsList.SelectedItem is ListViewItem selectedItem)
                    {
                        var executorId = (int)selectedItem.Tag;
                        try
                        {
                            using (var connection = DatabaseManager.GetConnection())
                            {
                                connection.Open();
                                using (var transaction = connection.BeginTransaction())
                                {
                                    try
                                    {
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
                                            command.Parameters.AddWithValue("@ExecutorID", executorId);
                                            command.Parameters.AddWithValue("@RequestID", request.RequestID);
                                            command.ExecuteNonQuery();
                                        }

                                        // Добавляем запись в историю
                                        var historyQuery = @"INSERT INTO RequestHistory 
                                                           (RequestID, StatusID, ChangedByUserID, ChangeDate, Comment)
                                                           VALUES 
                                                           (@RequestID, @StatusID, @UserID, GETDATE(), @Comment)";

                                        var currentUserLogin = "";
                                        if (Application.Current.MainWindow is MainWindow mainWindow)
                                        {
                                            currentUserLogin = mainWindow.CurrentUserLogin;
                                        }

                                        int userId;
                                        using (var command = new SqlCommand("SELECT UserID FROM Users WHERE Login = @Login", connection, transaction))
                                        {
                                            command.Parameters.AddWithValue("@Login", currentUserLogin);
                                            userId = (int)command.ExecuteScalar();
                                        }

                                        using (var command = new SqlCommand(historyQuery, connection, transaction))
                                        {
                                            command.Parameters.AddWithValue("@RequestID", request.RequestID);
                                            command.Parameters.AddWithValue("@StatusID", inProgressStatusId);
                                            command.Parameters.AddWithValue("@UserID", userId);
                                            command.Parameters.AddWithValue("@Comment", "Назначен исполнитель");
                                            command.ExecuteNonQuery();
                                        }

                                        transaction.Commit();
                                        NotificationManager.Show("Исполнитель успешно назначен", NotificationType.Success);
                                        window.Close();
                                        LoadAllRequests();
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
                            NotificationManager.Show($"Ошибка при назначении исполнителя: {ex.Message}", NotificationType.Error);
                        }
                    }
                };

                panel.Children.Add(title);
                panel.Children.Add(executorsList);
                panel.Children.Add(assignButton);
                window.Content = panel;
                window.ShowDialog();
            }
        }



        private void CancelRequest_Click(object sender, RoutedEventArgs e)

        {

            if (sender is Button button && button.Tag is AdminRequest request)

            {

                try

                {

                    using (var connection = DatabaseManager.GetConnection())

                    {

                        connection.Open();

                        using (var transaction = connection.BeginTransaction())

                        {

                            try

                            {

                                var currentUserLogin = "";
                                if (Application.Current.MainWindow is MainWindow mainWindow)
                                {
                                    currentUserLogin = mainWindow.CurrentUserLogin;
                                }

                                // Получаем ID статуса "Отменена"
                                int cancelledStatusId;
                                using (var command = new SqlCommand("SELECT StatusID FROM RequestStatuses WHERE Name = N'Отменена'", connection, transaction))
                                {
                                    cancelledStatusId = (int)command.ExecuteScalar();
                                }

                                // Обновляем статус заявки
                                var updateQuery = @"UPDATE Requests 
                                                  SET StatusID = @StatusID, 
                                                      LastModifiedDate = GETDATE() 
                                                  WHERE RequestID = @RequestID";

                                using (var command = new SqlCommand(updateQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@StatusID", cancelledStatusId);
                                    command.Parameters.AddWithValue("@RequestID", request.RequestID);
                                    command.ExecuteNonQuery();
                                }

                                // Получаем ID текущего пользователя
                                int userId;
                                using (var command = new SqlCommand("SELECT UserID FROM Users WHERE Login = @Login", connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@Login", currentUserLogin);
                                    userId = (int)command.ExecuteScalar();
                                }

                                // Добавляем запись в историю
                                var historyQuery = @"INSERT INTO RequestHistory 
                                                   (RequestID, StatusID, ChangedByUserID, ChangeDate, Comment)
                                                   VALUES 
                                                   (@RequestID, @StatusID, @UserID, GETDATE(), N'Заявка отменена администратором')";

                                using (var command = new SqlCommand(historyQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@RequestID", request.RequestID);
                                    command.Parameters.AddWithValue("@StatusID", cancelledStatusId);
                                    command.Parameters.AddWithValue("@UserID", userId);
                                    command.ExecuteNonQuery();
                                }

                                // Получаем автора заявки
                                string requestAuthorLogin;
                                using (var command = new SqlCommand(
                                    @"SELECT U.Login 
                                      FROM Requests R 
                                      JOIN Users U ON R.CreatedByUserID = U.UserID 
                                      WHERE R.RequestID = @RequestID", connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@RequestID", request.RequestID);
                                    requestAuthorLogin = (string)command.ExecuteScalar();
                                }

                                // Получаем исполнителя заявки (если есть)
                                string executorLogin = null;
                                using (var command = new SqlCommand(
                                    @"SELECT U.Login 
                                      FROM Requests R 
                                      JOIN Users U ON R.ExecutorID = U.UserID 
                                      WHERE R.RequestID = @RequestID", connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@RequestID", request.RequestID);
                                    var result = command.ExecuteScalar();
                                    if (result != null)
                                        executorLogin = (string)result;
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

                                // Отправляем уведомления
                                // Автору заявки
                                if (requestAuthorLogin != currentUserLogin)
                                {
                                    NotificationManager.CreateNotification(
                                        requestAuthorLogin,
                                        "Заявка отменена",
                                        $"Ваша заявка #{request.RequestID} была отменена администратором",
                                        NotificationType.Warning
                                    );
                                }

                                // Исполнителю, если он назначен
                                if (executorLogin != null && executorLogin != currentUserLogin)
                                {
                                    NotificationManager.CreateNotification(
                                        executorLogin,
                                        "Заявка отменена",
                                        $"Заявка #{request.RequestID} была отменена администратором",
                                        NotificationType.Warning
                                    );
                                }

                                // Другим администраторам
                                foreach (var adminLogin in adminLogins)
                                {
                                    NotificationManager.CreateNotification(
                                        adminLogin,
                                        "Заявка отменена администратором",
                                        $"Заявка #{request.RequestID} была отменена администратором {currentUserLogin}",
                                        NotificationType.Info
                                    );
                                }

                                transaction.Commit();
                                NotificationManager.Show("Заявка успешно отменена", NotificationType.Success);
                                LoadAllRequests(); // Перезагружаем список
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

                    NotificationManager.Show($"Ошибка при отмене заявки: {ex.Message}", NotificationType.Error);

                }

            }

        }

    }

}


