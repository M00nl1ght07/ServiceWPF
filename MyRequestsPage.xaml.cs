using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Threading;
using ServiceWPF;
using System.Linq;

namespace ServiceWPF
{
    public partial class MyRequestsPage : Page
    {
        private List<dynamic> _allRequests; // Храним все заявки для фильтрации

        public MyRequestsPage()
        {
            InitializeComponent();
            StatusFilter.SelectedIndex = 0;
            SortFilter.SelectedIndex = 0;
            
            // Возвращаем обработчик загрузки
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
                    Dispatcher.BeginInvoke(new Action(() => LoadRequests()), DispatcherPriority.Loaded);
                    return;
                }

                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    var query = @"SELECT R.RequestID, R.Title, R.Description, 
                                FORMAT(R.CreatedDate, 'dd.MM.yyyy') as CreatedDateDisplay,
                                FORMAT(R.CreatedDate, 'dd.MM.yyyy HH:mm:ss') as CreatedDateFull, 
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
                            _allRequests = new List<dynamic>();
                            while (reader.Read())
                            {
                                _allRequests.Add(new
                                {
                                    RequestID = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    CreatedDate = reader.GetString(3), // Для отображения
                                    CreatedDateFull = reader.GetString(4), // Для сортировки
                                    Status = reader.GetString(5),
                                    Priority = reader.GetString(6)
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

        private void ApplyFilters()
        {
            if (_allRequests == null) return; // Защита от null

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

            // Сортировка с учетом времени
            switch (SortFilter.SelectedIndex)
            {
                case 0: // Сначала новые
                    filteredRequests = filteredRequests
                        .OrderByDescending(r => DateTime.ParseExact(r.CreatedDateFull, "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                        .ToList();
                    break;
                case 1: // Сначала старые
                    filteredRequests = filteredRequests
                        .OrderBy(r => DateTime.ParseExact(r.CreatedDateFull, "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                        .ToList();
                    break;
                case 2: // По приоритету
                    var priorityOrder = new Dictionary<string, int> 
                    { 
                        { "Высокий", 0 }, 
                        { "Средний", 1 }, 
                        { "Низкий", 2 } 
                    };
                    filteredRequests = filteredRequests
                        .OrderBy(r => priorityOrder[r.Priority])
                        .ToList();
                    break;
            }

            RequestsList.ItemsSource = filteredRequests;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded) // Проверяем, что страница загружена
                ApplyFilters();
        }

        private void SortFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded) // Проверяем, что страница загружена
                ApplyFilters();
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