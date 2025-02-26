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

            StatusFilter.SelectedIndex = 0;

            ExecutorFilter.SelectedIndex = 0;

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



                    // Загружаем список исполнителей для фильтра

                    LoadExecutors();

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

                    var query = @"SELECT DISTINCT 

                                U.UserID,

                                CONCAT(U.LastName, ' ', LEFT(U.FirstName, 1), '.', 

                                    CASE WHEN U.MiddleName IS NOT NULL 

                                    THEN CONCAT(LEFT(U.MiddleName, 1), '.') 

                                    ELSE '' END) as ExecutorName

                                FROM Users U

                                JOIN UserRoles UR ON U.UserID = UR.UserID

                                WHERE UR.RoleID = 2 -- ID роли исполнителя

                                ORDER BY ExecutorName";



                    using (var command = new SqlCommand(query, connection))

                    {

                        using (var reader = command.ExecuteReader())

                        {

                            ExecutorFilter.Items.Clear();

                            ExecutorFilter.Items.Add(new ComboBoxItem { Content = "Все исполнители" });

                            

                            while (reader.Read())

                            {

                                ExecutorFilter.Items.Add(new ComboBoxItem 

                                { 

                                    Content = reader.GetString(1),

                                    Tag = reader.GetInt32(0)

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

                filteredRequests = filteredRequests.Where(r => r.Executor == selectedExecutor).ToList();

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

                NotificationManager.Show($"Исполнитель назначен на заявку: {request.Title}", NotificationType.Success);

            }

        }



        private void CancelRequest_Click(object sender, RoutedEventArgs e)

        {

            if (sender is Button button && button.Tag != null)

            {

                var request = button.Tag as AdminRequest;

                var result = MessageBox.Show($"Вы уверены, что хотите отменить заявку: {request.Title}?",

                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);



                if (result == MessageBoxResult.Yes)

                {

                    // Временно, пока нет БД

                    NotificationManager.Show("Заявка отменена", NotificationType.Success);

                    LoadAllRequests(); // Перезагружаем список

                }

            }

        }

    }

}


