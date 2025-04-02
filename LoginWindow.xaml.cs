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
using System.Security.Cryptography;

namespace ServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            
            // Инициализируем NotificationManager
            NotificationManager.Initialize(this);
            
            // Добавляем обработчики событий для подсказок паролей
            InitializePasswordHints();
            
            // Убираем кнопку перехода к регистрации
            SwitchToRegisterButton.Visibility = Visibility.Collapsed;
        }

        private void InitializePasswordHints()
        {
            // Для поля пароля при входе
            PasswordBox.PasswordChanged += (s, e) =>
            {
                PasswordHint.Visibility = string.IsNullOrEmpty(PasswordBox.Password) ? 
                    Visibility.Visible : Visibility.Collapsed;
            };
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка логина
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                NotificationManager.Show("Введите логин", NotificationType.Warning);
                UsernameTextBox.Focus();
                return;
            }

            // Проверка пароля
            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                NotificationManager.Show("Введите пароль", NotificationType.Warning);
                PasswordBox.Focus();
                return;
            }

            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    var query = @"SELECT U.UserID, U.PasswordHash, R.Name as Role, 
                                CONCAT(U.LastName, ' ', U.FirstName, ' ', ISNULL(U.MiddleName, '')) as FullName
                                FROM Users U
                                JOIN Roles R ON U.RoleID = R.RoleID
                                WHERE U.Login = @Login AND U.IsActive = 1";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Login", UsernameTextBox.Text.Trim());
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var storedHash = reader.GetString(1);
                                var userRole = reader.GetString(2).ToLower();
                                var fullName = reader.GetString(3);
                                
                                // Проверяем пароль
                                var inputHash = HashPassword(PasswordBox.Password);
                                
                                if (storedHash == inputHash)
                                {
                                    // Успешный вход
                                    NotificationManager.Show("Вход выполнен успешно", NotificationType.Success);
                                    
                                    var mainWindow = new MainWindow(userRole, UsernameTextBox.Text.Trim());
                                    mainWindow.UserNameText.Text = fullName;
                                    mainWindow.Show();
                                    
                                    Application.Current.MainWindow = mainWindow;
                                    
                                    this.Close();
                                }
                                else
                                {
                                    NotificationManager.Show("Неверный логин или пароль", NotificationType.Error);
                                }
                            }
                            else
                            {
                                NotificationManager.Show("Неверный логин или пароль", NotificationType.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при входе: {ex.Message}", NotificationType.Error);
            }
        }

        private void SwitchToRegister_Click(object sender, RoutedEventArgs e)
        {
            // Этот метод оставляем пустым, так как регистрация отключена
        }

        private void SwitchToLogin_Click(object sender, RoutedEventArgs e)
        {
            // Этот метод тоже можно удалить, но оставим для совместимости
        }
    }
}
