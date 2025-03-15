using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Data.SqlClient;

namespace ServiceWPF
{
    public class NotificationManager
    {
        private static Window _mainWindow;
        private static Grid _notificationGrid;

        public static void Initialize(Window window)
        {
            _mainWindow = window;
            _notificationGrid = (Grid)_mainWindow.FindName("NotificationGrid");
        }

        public static void Show(string message, NotificationType type)
        {
            if (_notificationGrid == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var notification = new Border
                {
                    Background = GetBackgroundBrush(type),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(20, 15, 20, 15),
                    Margin = new Thickness(0, 0, 0, 10),
                    MaxWidth = 400,
                    Effect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        BlurRadius = 10,
                        ShadowDepth = 1,
                        Opacity = 0.3
                    }
                };

                var messageText = new TextBlock
                {
                    Text = message,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = Brushes.White,
                    FontSize = 14
                };

                notification.Child = messageText;
                _notificationGrid.Children.Insert(0, notification);

                // Анимация появления
                notification.Opacity = 0;
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
                notification.BeginAnimation(UIElement.OpacityProperty, fadeIn);

                // Таймер для удаления уведомления
                var timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(3);
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
                    fadeOut.Completed += (s2, e2) =>
                    {
                        _notificationGrid.Children.Remove(notification);
                    };
                    notification.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                };
                timer.Start();
            });
        }

        private static SolidColorBrush GetBackgroundBrush(NotificationType type)
        {
            switch(type)
            {
                case NotificationType.Success:
                    return new SolidColorBrush(Color.FromRgb(76, 175, 80));
                case NotificationType.Warning:
                    return new SolidColorBrush(Color.FromRgb(255, 152, 0));
                case NotificationType.Error:
                    return new SolidColorBrush(Color.FromRgb(244, 67, 54));
                default:
                    return new SolidColorBrush(Color.FromRgb(33, 150, 243));
            }
        }

        public static void CreateNotification(string userLogin, string title, string message, NotificationType type)
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    var query = @"INSERT INTO Notifications (UserID, Title, Message, Type)
                                 SELECT U.UserID, @Title, @Message, @Type
                                 FROM Users U
                                 WHERE U.Login = @Login";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Login", userLogin);
                        command.Parameters.AddWithValue("@Title", title);
                        command.Parameters.AddWithValue("@Message", message);
                        command.Parameters.AddWithValue("@Type", type.ToString());
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Show($"Ошибка при создании уведомления: {ex.Message}", NotificationType.Error);
            }
        }
    }

    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }
}