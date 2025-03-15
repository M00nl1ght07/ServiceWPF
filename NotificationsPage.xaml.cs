using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using ServiceWPF;

namespace ServiceWPF
{
    public partial class NotificationsPage : Page
    {
        public class NotificationItem
        {
            public int NotificationID { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public string CreatedDate { get; set; }
            public bool IsRead { get; set; }
            public string Type { get; set; }
        }

        public NotificationsPage()
        {
            InitializeComponent();
            LoadNotifications();
        }

        private void LoadNotifications()
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
                    var query = @"SELECT N.NotificationID, N.Title, N.Message, 
                                FORMAT(N.CreatedDate, 'dd.MM.yyyy HH:mm') as CreatedDate,
                                N.IsRead, N.Type
                                FROM Notifications N
                                JOIN Users U ON N.UserID = U.UserID
                                WHERE U.Login = @Login
                                ORDER BY N.CreatedDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Login", currentUserLogin);
                        using (var reader = command.ExecuteReader())
                        {
                            var notifications = new List<NotificationItem>();
                            while (reader.Read())
                            {
                                notifications.Add(new NotificationItem
                                {
                                    NotificationID = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Message = reader.GetString(2),
                                    CreatedDate = reader.GetString(3),
                                    IsRead = reader.GetBoolean(4),
                                    Type = reader.GetString(5)
                                });
                            }
                            NotificationsList.ItemsSource = notifications;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при загрузке уведомлений: {ex.Message}", NotificationType.Error);
            }
        }

        private void MarkAllAsRead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentUserLogin = "";
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    currentUserLogin = mainWindow.CurrentUserLogin;

                    using (var connection = DatabaseManager.GetConnection())
                    {
                        connection.Open();
                        var query = @"UPDATE N
                                    SET N.IsRead = 1
                                    FROM Notifications N
                                    JOIN Users U ON N.UserID = U.UserID
                                    WHERE U.Login = @Login";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Login", currentUserLogin);
                            command.ExecuteNonQuery();
                        }
                    }

                    NotificationManager.Show("Все уведомления отмечены как прочитанные", NotificationType.Success);
                    LoadNotifications();

                    // Обновляем счетчик в главном окне
                    mainWindow.UpdateUnreadNotificationsCount();
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при обновлении уведомлений: {ex.Message}", NotificationType.Error);
            }
        }
    }
}