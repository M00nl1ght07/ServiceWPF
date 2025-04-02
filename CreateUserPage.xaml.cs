using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ServiceWPF
{
    public partial class CreateUserPage : Page
    {
        private Dictionary<string, int> _roleIds = new Dictionary<string, int>
        {
            { "Администратор", 1 },
            { "Исполнитель", 2 },
            { "Пользователь", 3 }
        };

        public CreateUserPage()
        {
            InitializeComponent();
            RoleComboBox.SelectedIndex = 2; // По умолчанию "Пользователь"
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения обязательных полей
            if (string.IsNullOrWhiteSpace(LoginBox.Text) ||
                string.IsNullOrWhiteSpace(EmailBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                PasswordBox.Password.Length == 0 ||
                ConfirmPasswordBox.Password.Length == 0)
            {
                NotificationManager.Show("Заполните все обязательные поля", NotificationType.Warning);
                return;
            }

            // Проверка совпадения паролей
            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                NotificationManager.Show("Пароли не совпадают", NotificationType.Warning);
                ConfirmPasswordBox.Focus();
                return;
            }

            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    
                    // Проверка уникальности логина
                    var checkLoginQuery = "SELECT COUNT(*) FROM Users WHERE Login = @Login";
                    using (var checkCommand = new SqlCommand(checkLoginQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Login", LoginBox.Text);
                        var count = (int)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            NotificationManager.Show("Пользователь с таким логином уже существует", NotificationType.Warning);
                            LoginBox.Focus();
                            return;
                        }
                    }

                    // Проверка уникальности email
                    var checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                    using (var checkCommand = new SqlCommand(checkEmailQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Email", EmailBox.Text);
                        var count = (int)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            NotificationManager.Show("Пользователь с таким email уже существует", NotificationType.Warning);
                            EmailBox.Focus();
                            return;
                        }
                    }

                    // Создание пользователя
                    var registerQuery = @"INSERT INTO Users (Login, PasswordHash, Email, FirstName, LastName, MiddleName, RoleID, IsActive) 
                                        VALUES (@Login, @PasswordHash, @Email, @FirstName, @LastName, @MiddleName, @RoleID, 1)";
                    
                    using (var registerCommand = new SqlCommand(registerQuery, connection))
                    {
                        registerCommand.Parameters.AddWithValue("@Login", LoginBox.Text);
                        registerCommand.Parameters.AddWithValue("@PasswordHash", HashPassword(PasswordBox.Password));
                        registerCommand.Parameters.AddWithValue("@Email", EmailBox.Text);
                        registerCommand.Parameters.AddWithValue("@FirstName", FirstNameBox.Text);
                        registerCommand.Parameters.AddWithValue("@LastName", LastNameBox.Text);
                        registerCommand.Parameters.AddWithValue("@MiddleName", 
                            string.IsNullOrWhiteSpace(MiddleNameBox.Text) ? DBNull.Value : (object)MiddleNameBox.Text);
                        
                        // Получаем ID роли из выбранного элемента
                        var selectedRole = ((ComboBoxItem)RoleComboBox.SelectedItem).Content.ToString();
                        registerCommand.Parameters.AddWithValue("@RoleID", _roleIds[selectedRole]);

                        registerCommand.ExecuteNonQuery();

                        // Показываем уведомление об успехе
                        NotificationManager.Show("Пользователь успешно создан", NotificationType.Success);
                        
                        // Возвращаемся на страницу пользователей
                        NavigationService?.GoBack();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при создании пользователя: {ex.Message}", NotificationType.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
} 