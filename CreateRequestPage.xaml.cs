using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;

using ServiceWPF; // Добавляем для NotificationManager

namespace ServiceWPF
{
    public partial class CreateRequestPage : Page
    {
        public CreateRequestPage()
        {
            InitializeComponent();
            PriorityComboBox.SelectedIndex = 0;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                NotificationManager.Show("Введите заголовок заявки", NotificationType.Warning);
                TitleTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
            {
                NotificationManager.Show("Введите описание проблемы", NotificationType.Warning);
                DescriptionTextBox.Focus();
                return;
            }

            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    
                    // Начинаем транзакцию
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var currentUserLogin = "";
                            if (Application.Current.MainWindow is MainWindow mainWindow)
                            {
                                currentUserLogin = mainWindow.CurrentUserLogin;
                            }

                            // Получаем ID пользователя
                            var checkUserQuery = "SELECT UserID FROM Users WHERE Login = @Login";
                            int userId;
                            using (var command = new SqlCommand(checkUserQuery, connection, transaction))
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

                            // Создаем заявку
                            int requestId;
                            var query = @"INSERT INTO Requests (Title, [Description], CreatedDate, StatusID, PriorityID, CreatedByUserID) 
                                        VALUES (@Title, @Description, GETDATE(), @StatusID, @PriorityID, @UserID);
                                        SELECT SCOPE_IDENTITY();";

                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@Title", TitleTextBox.Text);
                                command.Parameters.AddWithValue("@Description", DescriptionTextBox.Text);
                                command.Parameters.AddWithValue("@StatusID", 1);
                                command.Parameters.AddWithValue("@PriorityID", PriorityComboBox.SelectedIndex + 1);
                                command.Parameters.AddWithValue("@UserID", userId);

                                requestId = Convert.ToInt32(command.ExecuteScalar());
                            }

                            // Добавляем запись в историю
                            var historyQuery = @"INSERT INTO RequestHistory (RequestID, StatusID, ChangedByUserID, ChangeDate, Comment)
                                               VALUES (@RequestID, 1, @UserID, GETDATE(), 'Заявка создана')";

                            using (var command = new SqlCommand(historyQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@RequestID", requestId);
                                command.Parameters.AddWithValue("@UserID", userId);
                                command.ExecuteNonQuery();
                            }

                            // Подтверждаем транзакцию
                            transaction.Commit();

                            NotificationManager.Show("Заявка успешно создана!", NotificationType.Success);
                            TitleTextBox.Clear();
                            DescriptionTextBox.Clear();
                            PriorityComboBox.SelectedIndex = 0;
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
                NotificationManager.Show($"Ошибка при создании заявки: {ex.Message}", NotificationType.Error);
            }
        }
    }
}
