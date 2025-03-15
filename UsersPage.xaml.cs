using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        private List<UserInfo> _allUsers;

        public UsersPage()
        {
            InitializeComponent();
            LoadUsers();
            // Подписываемся на событие загрузки страницы
            this.Loaded += UsersPage_Loaded;
        }

        private void UsersPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUsers(); // Перезагружаем список при каждой загрузке страницы
        }

        private void LoadUsers()
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    var query = @"SELECT 
                                U.UserID,
                                U.Login,
                                U.LastName,
                                U.FirstName,
                                U.MiddleName,
                                U.Email,
                                R.Name as RoleName,
                                ISNULL((SELECT AVG(Rating * 1.0)
                                    FROM Reviews 
                                    WHERE ExecutorID = U.UserID), 0) as Rating
                            FROM Users U
                            JOIN Roles R ON U.RoleID = R.RoleID
                            WHERE U.IsActive = 1
                            ORDER BY U.LastName, U.FirstName";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        _allUsers = new List<UserInfo>();
                        while (reader.Read())
                        {
                            var userInfo = new UserInfo
                            {
                                UserID = reader.GetInt32(0),
                                Login = reader.GetString(1),
                                FullName = $"{reader.GetString(2)} {reader.GetString(3)} {(reader.IsDBNull(4) ? "" : reader.GetString(4))}",
                                Email = reader.GetString(5),
                                Role = reader.GetString(6)
                            };

                            // Если это мастер - добавляем информацию о рейтинге
                            if (userInfo.Role == "Исполнитель")
                            {
                                userInfo.RatingInfo = $"Рейтинг: {reader.GetDouble(7):F1}";
                                userInfo.IsExecutor = true;
                            }

                            _allUsers.Add(userInfo);
                        }
                        ApplySearch();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при загрузке пользователей: {ex.Message}", NotificationType.Error);
            }
        }

        private void ApplySearch()
        {
            var searchText = SearchBox.Text.Trim().ToLower();
            var filteredUsers = string.IsNullOrEmpty(searchText) 
                ? _allUsers 
                : _allUsers.Where(u => 
                    u.FullName.ToLower().Contains(searchText) || 
                    u.Email.ToLower().Contains(searchText) ||
                    u.Role.ToLower().Contains(searchText)).ToList();

            UsersList.ItemsSource = filteredUsers;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySearch();
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is UserInfo user)
            {
                NavigationService?.Navigate(new EditUserWindow(user.UserID));
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is UserInfo user)
            {
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить пользователя {user.FullName}?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var connection = DatabaseManager.GetConnection())
                        {
                            connection.Open();
                            // Вместо физического удаления, помечаем пользователя как неактивного
                            var query = "UPDATE Users SET IsActive = 0 WHERE UserID = @UserID";
                            using (var command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@UserID", user.UserID);
                                command.ExecuteNonQuery();
                                NotificationManager.Show("Пользователь успешно удален", NotificationType.Success);
                                LoadUsers(); // Перезагружаем список
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        NotificationManager.Show($"Ошибка при удалении пользователя: {ex.Message}", NotificationType.Error);
                    }
                }
            }
        }
    }

    public class UserInfo
    {
        public int UserID { get; set; }
        public string Login { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string RatingInfo { get; set; }
        public bool IsExecutor { get; set; }
    }
}

