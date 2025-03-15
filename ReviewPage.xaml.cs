using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Windows.Input;
using System.Windows.Media;

namespace ServiceWPF
{
    public partial class ReviewPage : Page
    {
        private int _requestId;
        private int _executorId;
        private int _selectedRating = 0;
        private int _hoveredRating = 0;

        public ReviewPage(int requestId)
        {
            InitializeComponent();
            _requestId = requestId;
            LoadRequestInfo();
        }

        private void LoadRequestInfo()
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    var query = @"SELECT R.RequestID, R.Title, 
                                U.UserID as ExecutorID,
                                CONCAT(U.LastName, ' ', U.FirstName, ' ', ISNULL(U.MiddleName, '')) as ExecutorName
                                FROM Requests R
                                JOIN Users U ON R.ExecutorID = U.UserID
                                WHERE R.RequestID = @RequestID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RequestID", _requestId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                RequestInfoText.Text = $"Заявка №{reader.GetInt32(0)}: {reader.GetString(1)}";
                                _executorId = reader.GetInt32(2);
                                ExecutorInfoText.Text = $"Исполнитель: {reader.GetString(3)}";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при загрузке информации: {ex.Message}", NotificationType.Error);
            }
        }

        private void SubmitReview_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRating == 0)
            {
                NotificationManager.Show("Пожалуйста, поставьте оценку", NotificationType.Warning);
                return;
            }

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
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int userId;
                            using (var command = new SqlCommand(
                                "SELECT UserID FROM Users WHERE Login = @Login", 
                                connection, transaction))
                            {
                                command.Parameters.AddWithValue("@Login", currentUserLogin);
                                userId = (int)command.ExecuteScalar();
                            }

                            var query = @"INSERT INTO Reviews 
                                        (RequestID, ExecutorID, UserID, Rating, Comment)
                                        VALUES 
                                        (@RequestID, @ExecutorID, @UserID, @Rating, @Comment)";

                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@RequestID", _requestId);
                                command.Parameters.AddWithValue("@ExecutorID", _executorId);
                                command.Parameters.AddWithValue("@UserID", userId);
                                command.Parameters.AddWithValue("@Rating", _selectedRating);
                                command.Parameters.AddWithValue("@Comment", 
                                    string.IsNullOrWhiteSpace(CommentBox.Text) ? DBNull.Value : (object)CommentBox.Text.Trim());

                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            NotificationManager.Show("Спасибо за ваш отзыв!", NotificationType.Success);
                            NavigationService?.GoBack();
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
                NotificationManager.Show($"Ошибка при отправке отзыва: {ex.Message}", NotificationType.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void Star_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton && int.TryParse(clickedButton.Tag.ToString(), out int rating))
            {
                _selectedRating = rating;
                UpdateStars();
            }
        }

        private void Star_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button hoveredButton)
            {
                int rating = int.Parse(hoveredButton.Tag.ToString());
                UpdateStars(rating);
            }
        }

        private void Star_MouseLeave(object sender, MouseEventArgs e)
        {
            UpdateStars();
        }

        private void UpdateStars()
        {
            foreach (Button star in RatingPanel.Children)
            {
                if (int.TryParse(star.Tag.ToString(), out int buttonRating))
                {
                    var starText = (star.Template.FindName("starText", star) as TextBlock);
                    if (starText != null)
                    {
                        starText.Foreground = buttonRating <= _selectedRating ? 
                            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC107")) : 
                            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDBDBD"));
                    }
                }
            }
        }

        private void UpdateStars(int rating)
        {
            for (int i = 0; i < RatingPanel.Children.Count; i++)
            {
                if (RatingPanel.Children[i] is Button star)
                {
                    star.Tag = i < rating;
                }
            }
        }
    }
}