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

                    var currentUserLogin = "";
                    if (Application.Current.MainWindow is MainWindow mainWindow)
                    {
                        currentUserLogin = mainWindow.CurrentUserLogin;
                    }
                    else
                    {
                        NotificationManager.Show("Ошибка: не удалось определить пользователя", NotificationType.Error);
                        return;
                    }

                    // Получаем ID пользователя
                    var checkUserQuery = "SELECT UserID FROM Users WHERE Login = @Login";
                    int userId;
                    using (var command = new SqlCommand(checkUserQuery, connection))
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
                    var query = @"INSERT INTO Requests (Title, [Description], CreatedDate, StatusID, PriorityID, CreatedByUserID) 
                                 VALUES (@Title, @Description, GETDATE(), @StatusID, @PriorityID, @UserID)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Title", TitleTextBox.Text);
                        command.Parameters.AddWithValue("@Description", DescriptionTextBox.Text);
                        command.Parameters.AddWithValue("@StatusID", 1);
                        command.Parameters.AddWithValue("@PriorityID", PriorityComboBox.SelectedIndex + 1);
                        command.Parameters.AddWithValue("@UserID", userId);

                        command.ExecuteNonQuery();
                        NotificationManager.Show("Заявка успешно создана!", NotificationType.Success);
                        TitleTextBox.Clear();
                        DescriptionTextBox.Clear();
                        PriorityComboBox.SelectedIndex = 0;
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
