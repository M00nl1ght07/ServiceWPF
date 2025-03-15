using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data.SqlClient;
using System.Windows;

namespace ServiceWPF
{
    public partial class ReviewsPage : Page
    {
        private int? _executorId;
        public bool IsAdmin { get; private set; }

        public class ReviewItem
        {
            public string Author { get; set; }
            public string CreatedDate { get; set; }
            public int Rating { get; set; }
            public string Comment { get; set; }
            public string RequestInfo { get; set; }
            public string ExecutorName { get; set; }
            public List<SolidColorBrush> Stars { get; set; }

            public ReviewItem()
            {
                Stars = new List<SolidColorBrush>();
            }
        }

        public ReviewsPage(int? executorId = null)
        {
            InitializeComponent();
            _executorId = executorId;
            
            // Определяем, является ли текущий пользователь администратором
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    var query = @"SELECT R.Name 
                                FROM Users U 
                                JOIN Roles R ON U.RoleID = R.RoleID 
                                WHERE U.Login = @Login";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Login", mainWindow.CurrentUserLogin);
                        var role = command.ExecuteScalar() as string;
                        IsAdmin = role == "Администратор";
                    }
                }
            }

            DataContext = this;
            LoadReviews();
        }

        private void LoadReviews()
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();

                    // Загружаем статистику
                    var statsQuery = @"SELECT 
                                    ISNULL(AVG(CAST(Rating AS FLOAT)), 0) as AverageRating,
                                    COUNT(*) as TotalReviews
                                    FROM Reviews
                                    WHERE (@ExecutorID IS NULL OR ExecutorID = @ExecutorID)";

                    using (var command = new SqlCommand(statsQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ExecutorID", _executorId ?? (object)DBNull.Value);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                AverageRatingText.Text = reader.GetDouble(0).ToString("F1");
                                TotalReviewsText.Text = reader.GetInt32(1).ToString();
                            }
                        }
                    }

                    // Загружаем отзывы
                    var reviewsQuery = @"SELECT 
                                    CONCAT(U.LastName, ' ', LEFT(U.FirstName, 1), '.') as Author,
                                    FORMAT(R.CreatedDate, 'dd.MM.yyyy HH:mm') as CreatedDate,
                                    R.Rating,
                                    R.Comment,
                                    CONCAT('Заявка №', R.RequestID, ': ', Req.Title) as RequestInfo,
                                    CONCAT(E.LastName, ' ', E.FirstName) as ExecutorName
                                    FROM Reviews R
                                    JOIN Users U ON R.UserID = U.UserID
                                    JOIN Users E ON R.ExecutorID = E.UserID
                                    JOIN Requests Req ON R.RequestID = Req.RequestID
                                    WHERE (@ExecutorID IS NULL OR R.ExecutorID = @ExecutorID)
                                    ORDER BY R.CreatedDate DESC";

                    using (var command = new SqlCommand(reviewsQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ExecutorID", _executorId ?? (object)DBNull.Value);
                        using (var reader = command.ExecuteReader())
                        {
                            var reviews = new List<ReviewItem>();
                            while (reader.Read())
                            {
                                var review = new ReviewItem
                                {
                                    Author = reader.GetString(0),
                                    CreatedDate = reader.GetString(1),
                                    Rating = reader.GetInt32(2),
                                    Comment = !reader.IsDBNull(3) ? reader.GetString(3) : "",
                                    RequestInfo = reader.GetString(4),
                                    ExecutorName = reader.GetString(5)
                                };

                                // Добавляем звезды
                                var goldStar = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC107"));
                                var grayStar = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDBDBD"));

                                for (int i = 0; i < 5; i++)
                                {
                                    review.Stars.Add(i < review.Rating ? goldStar : grayStar);
                                }

                                reviews.Add(review);
                            }
                            ReviewsList.ItemsSource = reviews;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при загрузке отзывов: {ex.Message}", NotificationType.Error);
            }
        }
    }
}