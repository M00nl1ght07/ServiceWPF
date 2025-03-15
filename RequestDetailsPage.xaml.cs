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
    /// Логика взаимодействия для RequestDetailsPage.xaml
    /// </summary>
    public partial class RequestDetailsPage : Page
    {
        private int _requestId;

        public RequestDetailsPage(int requestId)
        {
            InitializeComponent();
            _requestId = requestId;
            LoadRequestDetails();
        }

        private void LoadRequestDetails()
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    var query = @"SELECT R.Title, R.Description, 
                                FORMAT(R.CreatedDate, 'dd.MM.yyyy HH:mm') as CreatedDate,
                                S.Name as Status, P.Name as Priority,
                                CONCAT(U.LastName, ' ', U.FirstName, ' ', ISNULL(U.MiddleName, '')) as CreatedBy,
                                CONCAT(E.LastName, ' ', E.FirstName, ' ', ISNULL(E.MiddleName, '')) as Executor,
                                FORMAT(R.CompletionDate, 'dd.MM.yyyy HH:mm') as CompletionDate,
                                FORMAT(R.LastModifiedDate, 'dd.MM.yyyy HH:mm') as LastModifiedDate
                                FROM Requests R
                                JOIN RequestStatuses S ON R.StatusID = S.StatusID
                                JOIN RequestPriorities P ON R.PriorityID = P.PriorityID
                                JOIN Users U ON R.CreatedByUserID = U.UserID
                                LEFT JOIN Users E ON R.ExecutorID = E.UserID
                                WHERE R.RequestID = @RequestID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RequestID", _requestId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TitleTextBlock.Text = reader.GetString(0);
                                DescriptionTextBlock.Text = reader.GetString(1);
                                CreatedDateTextBlock.Text = reader.GetString(2);
                                StatusTextBlock.Text = reader.GetString(3);
                                PriorityTextBlock.Text = reader.GetString(4);
                                CreatedByTextBlock.Text = reader.GetString(5);
                                ExecutorTextBlock.Text = reader.IsDBNull(6) ? "Не назначен" : reader.GetString(6);
                                CompletionDateTextBlock.Text = reader.IsDBNull(7) ? "Не завершена" : reader.GetString(7);
                                LastModifiedDateTextBlock.Text = reader.GetString(8);
                            }
                        }
                    }

                    // Загружаем историю заявки
                    LoadRequestHistory();

                    // После загрузки основных деталей добавим загрузку комментариев
                    LoadComments();
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при загрузке деталей заявки: {ex.Message}", NotificationType.Error);
            }
        }

        private void LoadRequestHistory()
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    var query = @"SELECT FORMAT(H.ChangeDate, 'dd.MM.yyyy HH:mm') as ChangeDate,
                                CONCAT(U.LastName, ' ', 
                                      LEFT(U.FirstName, 1), '.', 
                                      CASE 
                                        WHEN U.MiddleName IS NOT NULL 
                                        THEN CONCAT(LEFT(U.MiddleName, 1), '.')
                                        ELSE ''
                                      END) as ChangedBy,
                                H.Comment
                                FROM RequestHistory H
                                JOIN Users U ON H.ChangedByUserID = U.UserID
                                WHERE H.RequestID = @RequestID
                                ORDER BY H.ChangeDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RequestID", _requestId);
                        using (var reader = command.ExecuteReader())
                        {
                            var history = new List<dynamic>();
                            while (reader.Read())
                            {
                                history.Add(new
                                {
                                    ChangeDate = reader.GetString(0),
                                    ChangedBy = reader.GetString(1),
                                    Comment = reader.IsDBNull(2) ? "" : reader.GetString(2)
                                });
                            }
                            HistoryList.ItemsSource = history;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при загрузке истории заявки: {ex.Message}", NotificationType.Error);
            }
        }

        private void LoadComments()
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    var query = @"SELECT 
                                FORMAT(C.CreatedDate, 'dd.MM.yyyy HH:mm') as CommentDate,
                                C.Text,
                                CONCAT(U.LastName, ' ', LEFT(U.FirstName, 1), '.', 
                                    CASE 
                                        WHEN U.MiddleName IS NOT NULL 
                                        THEN CONCAT(LEFT(U.MiddleName, 1), '.')
                                        ELSE ''
                                    END) as Author
                                FROM RequestComments C
                                JOIN Users U ON C.UserID = U.UserID
                                WHERE C.RequestID = @RequestID
                                ORDER BY C.CreatedDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RequestID", _requestId);
                        using (var reader = command.ExecuteReader())
                        {
                            var comments = new List<dynamic>();
                            while (reader.Read())
                            {
                                comments.Add(new
                                {
                                    Date = reader.GetString(0),
                                    Text = reader.GetString(1),
                                    Author = reader.GetString(2)
                                });
                            }
                            CommentsList.ItemsSource = comments;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при загрузке комментариев: {ex.Message}", NotificationType.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void AddComment_Click(object sender, RoutedEventArgs e)
        {
            var commentText = CommentBox.Text.Trim();
            if (string.IsNullOrEmpty(commentText))
            {
                NotificationManager.Show("Введите текст комментария", NotificationType.Warning);
                return;
            }

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

                            int userId;
                            using (var command = new SqlCommand("SELECT UserID FROM Users WHERE Login = @Login", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@Login", currentUserLogin);
                                var result = command.ExecuteScalar();
                                if (result == null)
                                {
                                    NotificationManager.Show("Ошибка: пользователь не найден", NotificationType.Error);
                                    return;
                                }
                                userId = (int)result;
                            }

                            // Добавляем комментарий
                            var query = @"INSERT INTO RequestComments (RequestID, UserID, Text, CreatedDate)
                                         VALUES (@RequestID, @UserID, @Text, GETDATE())";

                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@RequestID", _requestId);
                                command.Parameters.AddWithValue("@UserID", userId);
                                command.Parameters.AddWithValue("@Text", commentText);
                                command.ExecuteNonQuery();
                            }

                            // Получаем автора заявки для уведомления
                            string requestAuthorLogin;
                            using (var command = new SqlCommand(
                                @"SELECT U.Login 
                                  FROM Requests R 
                                  JOIN Users U ON R.CreatedByUserID = U.UserID 
                                  WHERE R.RequestID = @RequestID", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@RequestID", _requestId);
                                requestAuthorLogin = (string)command.ExecuteScalar();
                            }

                            // Получаем исполнителя заявки для уведомления
                            string executorLogin;
                            using (var command = new SqlCommand(
                                @"SELECT U.Login 
                                  FROM Requests R 
                                  JOIN Users U ON R.ExecutorID = U.UserID 
                                  WHERE R.RequestID = @RequestID", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@RequestID", _requestId);
                                executorLogin = command.ExecuteScalar() as string;
                            }

                            // Создаем уведомления
                            if (requestAuthorLogin != currentUserLogin) // Уведомление автору
                            {
                                NotificationManager.CreateNotification(
                                    requestAuthorLogin,
                                    "Новый комментарий к заявке",
                                    $"К вашей заявке #{_requestId} добавлен новый комментарий",
                                    NotificationType.Info
                                );
                            }

                            if (executorLogin != null && executorLogin != currentUserLogin) // Уведомление исполнителю
                            {
                                NotificationManager.CreateNotification(
                                    executorLogin,
                                    "Новый комментарий к заявке",
                                    $"К заявке #{_requestId} добавлен новый комментарий",
                                    NotificationType.Info
                                );
                            }

                            transaction.Commit();
                            CommentBox.Clear();
                            LoadComments();
                            NotificationManager.Show("Комментарий добавлен", NotificationType.Success);
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
                NotificationManager.Show($"Ошибка при добавлении комментария: {ex.Message}", NotificationType.Error);
            }
        }
    }
}
