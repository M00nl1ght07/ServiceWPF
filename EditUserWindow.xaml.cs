using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Page
    {
        private int _userId;
        private Dictionary<string, string> _roleDisplayNames = new Dictionary<string, string>
        {
            { "Admin", "Администратор" },
            { "Executor", "Исполнитель" },
            { "User", "Пользователь" }
        };

        public EditUserWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    var query = @"SELECT U.LastName, U.FirstName, U.MiddleName, U.Login, R.Name as RoleName
                                FROM Users U
                                JOIN Roles R ON U.RoleID = R.RoleID
                                WHERE U.UserID = @UserID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", _userId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                LastNameBox.Text = reader.GetString(0);
                                FirstNameBox.Text = reader.GetString(1);
                                MiddleNameBox.Text = !reader.IsDBNull(2) ? reader.GetString(2) : "";
                                LoginBox.Text = reader.GetString(3);
                                string roleName = reader.GetString(4);

                                foreach (ComboBoxItem item in RoleComboBox.Items)
                                {
                                    if (item.Content.ToString() == roleName)
                                    {
                                        RoleComboBox.SelectedItem = item;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при загрузке данных пользователя: {ex.Message}", NotificationType.Error);
                NavigationService?.GoBack();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LastNameBox.Text) || 
                    string.IsNullOrWhiteSpace(FirstNameBox.Text) || 
                    string.IsNullOrWhiteSpace(LoginBox.Text) ||
                    RoleComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Заполните обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var roleQuery = "SELECT RoleID FROM Roles WHERE Name = @RoleName";
                            int roleId;

                            using (var command = new SqlCommand(roleQuery, connection, transaction))
                            {
                                var selectedRole = ((ComboBoxItem)RoleComboBox.SelectedItem).Content.ToString();
                                command.Parameters.AddWithValue("@RoleName", selectedRole);
                                roleId = (int)command.ExecuteScalar();
                            }

                            var updateQuery = @"UPDATE Users 
                                            SET LastName = @LastName,
                                                FirstName = @FirstName,
                                                MiddleName = @MiddleName,
                                                Login = @Login,
                                                RoleID = @RoleID
                                            WHERE UserID = @UserID";

                            using (var command = new SqlCommand(updateQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@LastName", LastNameBox.Text.Trim());
                                command.Parameters.AddWithValue("@FirstName", FirstNameBox.Text.Trim());
                                command.Parameters.AddWithValue("@MiddleName", 
                                    string.IsNullOrWhiteSpace(MiddleNameBox.Text) ? DBNull.Value : (object)MiddleNameBox.Text.Trim());
                                command.Parameters.AddWithValue("@Login", LoginBox.Text.Trim());
                                command.Parameters.AddWithValue("@RoleID", roleId);
                                command.Parameters.AddWithValue("@UserID", _userId);

                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            NotificationManager.Show("Данные пользователя успешно обновлены", NotificationType.Success);
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
                NotificationManager.Show($"Ошибка при сохранении данных: {ex.Message}", NotificationType.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
