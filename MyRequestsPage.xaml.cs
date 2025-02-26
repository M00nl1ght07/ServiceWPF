using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Threading;
using ServiceWPF;

namespace ServiceWPF
{
    public partial class MyRequestsPage : Page
    {
        public MyRequestsPage()
        {
            InitializeComponent();
            
            // Подписываемся на событие загрузки страницы
            this.Loaded += MyRequestsPage_Loaded;
        }

        private void MyRequestsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRequests();
        }

        private void LoadRequests()
        {
            try
            {
                var currentUserLogin = "";
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    currentUserLogin = mainWindow.CurrentUserLogin;
                }
                else
                {
                    // Если окно еще не установлено, пробуем через 100мс
                    Dispatcher.BeginInvoke(new Action(() => LoadRequests()), System.Windows.Threading.DispatcherPriority.Loaded);
                    return;
                }

                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    var query = @"SELECT R.RequestID, R.Title, R.Description, 
                                FORMAT(R.CreatedDate, 'dd.MM.yyyy') as CreatedDate, 
                                S.Name as Status, P.Name as Priority
                             FROM Requests R
                             JOIN RequestStatuses S ON R.StatusID = S.StatusID
                             JOIN RequestPriorities P ON R.PriorityID = P.PriorityID
                             JOIN Users U ON R.CreatedByUserID = U.UserID
                             WHERE U.Login = @Login
                             ORDER BY R.CreatedDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Login", currentUserLogin);

                        using (var reader = command.ExecuteReader())
                        {
                            var requests = new List<dynamic>();
                            while (reader.Read())
                            {
                                requests.Add(new
                                {
                                    RequestID = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    CreatedDate = reader.GetString(3),
                                    Status = reader.GetString(4),
                                    Priority = reader.GetString(5)
                                });
                            }
                            RequestsList.ItemsSource = requests;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при загрузке заявок: {ex.Message}", NotificationType.Error);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Здесь будет логика поиска
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Здесь будет логика фильтрации по статусу
        }

        private void SortFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Здесь будет логика сортировки
        }

        private void RequestsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RequestsList.SelectedItem != null)
            {
                dynamic selectedRequest = RequestsList.SelectedItem;
                NavigationService?.Navigate(new RequestDetailsPage(selectedRequest.RequestID));
                RequestsList.SelectedItem = null;
            }
        }
    }
}