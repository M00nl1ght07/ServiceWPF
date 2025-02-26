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
            
            // Инициализируем начальное состояние
            LoginTab.IsChecked = true;
            LoginContent.Visibility = Visibility.Visible;
            RegisterContent.Visibility = Visibility.Collapsed;
            
            // Добавляем обработчики событий для подсказок паролей
            InitializePasswordHints();
        }

        private void InitializePasswordHints()
        {
            // Для поля пароля при входе
            PasswordBox.PasswordChanged += (s, e) =>
            {
                PasswordHint.Visibility = string.IsNullOrEmpty(PasswordBox.Password) ? 
                    Visibility.Visible : Visibility.Collapsed;
            };

            // Для поля пароля при регистрации
            RegisterPasswordBox.PasswordChanged += (s, e) =>
            {
                RegisterPasswordHint.Visibility = string.IsNullOrEmpty(RegisterPasswordBox.Password) ? 
                    Visibility.Visible : Visibility.Collapsed;
            };

            // Для поля подтверждения пароля
            ConfirmPasswordBox.PasswordChanged += (s, e) =>
            {
                ConfirmPasswordHint.Visibility = string.IsNullOrEmpty(ConfirmPasswordBox.Password) ? 
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

        private void LoginTab_Checked(object sender, RoutedEventArgs e)
        {
            if (LoginContent != null && RegisterContent != null)
            {
                LoginContent.Visibility = Visibility.Visible;
                RegisterContent.Visibility = Visibility.Collapsed;
            }
        }

        private void RegisterTab_Checked(object sender, RoutedEventArgs e)
        {
            if (LoginContent != null && RegisterContent != null)
            {
                LoginContent.Visibility = Visibility.Collapsed;
                RegisterContent.Visibility = Visibility.Visible;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    var query = @"SELECT U.RoleID, U.IsActive, U.FirstName, U.LastName 
                                 FROM Users U 
                                 WHERE U.Login = @Login AND U.PasswordHash = @PasswordHash";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Login", UsernameTextBox.Text.Trim());
                        command.Parameters.AddWithValue("@PasswordHash", HashPassword(PasswordBox.Password));

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var roleId = reader.GetInt32(0);
                                var isActive = reader.GetBoolean(1);
                                
                                if (!isActive)
                                {
                                    NotificationManager.Show("Ваша учетная запись отключена", NotificationType.Error);
                                    return;
                                }

                                string userRole;
                                switch (roleId)
                                {
                                    case 1:
                                        userRole = "admin";
                                        break;
                                    case 2:
                                        userRole = "executor";
                                        break;
                                    default:
                                        userRole = "user";
                                        break;
                                }

                                var firstName = reader.GetString(2);
                                var lastName = reader.GetString(3);
                                var fullName = $"{firstName} {lastName}";

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
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при входе: {ex.Message}", NotificationType.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка логина
            if (string.IsNullOrWhiteSpace(RegisterUsernameTextBox.Text))
            {
                NotificationManager.Show("Введите логин", NotificationType.Warning);
                RegisterUsernameTextBox.Focus();
                return;
            }

            if (RegisterUsernameTextBox.Text.Length < 3)
            {
                NotificationManager.Show("Логин должен содержать минимум 3 символа", NotificationType.Warning);
                RegisterUsernameTextBox.Focus();
                return;
            }

            // Проверка email
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                NotificationManager.Show("Введите email", NotificationType.Warning);
                EmailTextBox.Focus();
                return;
            }

            if (!EmailTextBox.Text.Contains("@") || !EmailTextBox.Text.Contains("."))
            {
                NotificationManager.Show("Введите корректный email", NotificationType.Warning);
                EmailTextBox.Focus();
                return;
            }

            // Проверка имени
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                NotificationManager.Show("Введите имя", NotificationType.Warning);
                FirstNameTextBox.Focus();
                return;
            }

            // Проверка фамилии
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                NotificationManager.Show("Введите фамилию", NotificationType.Warning);
                LastNameTextBox.Focus();
                return;
            }

            // Проверка пароля
            if (string.IsNullOrWhiteSpace(RegisterPasswordBox.Password))
            {
                NotificationManager.Show("Введите пароль", NotificationType.Warning);
                RegisterPasswordBox.Focus();
                return;
            }

            if (RegisterPasswordBox.Password.Length < 6)
            {
                NotificationManager.Show("Пароль должен содержать минимум 6 символов", NotificationType.Warning);
                RegisterPasswordBox.Focus();
                return;
            }

            // Проверка подтверждения пароля
            if (string.IsNullOrWhiteSpace(ConfirmPasswordBox.Password))
            {
                NotificationManager.Show("Подтвердите пароль", NotificationType.Warning);
                ConfirmPasswordBox.Focus();
                return;
            }

            if (RegisterPasswordBox.Password != ConfirmPasswordBox.Password)
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
                    
                    // Проверка существования пользователя
                    var checkQuery = "SELECT COUNT(*) FROM Users WHERE Login = @Login";
                    using (var checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Login", RegisterUsernameTextBox.Text);
                        var count = (int)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            NotificationManager.Show("Пользователь с таким логином уже существует", NotificationType.Warning);
                            RegisterUsernameTextBox.Focus();
                            return;
                        }
                    }

                    // Проверка существования email
                    checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                    using (var checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Email", EmailTextBox.Text);
                        var count = (int)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            NotificationManager.Show("Пользователь с таким email уже существует", NotificationType.Warning);
                            EmailTextBox.Focus();
                            return;
                        }
                    }

                    // Регистрация пользователя
                    var registerQuery = @"INSERT INTO Users (Login, PasswordHash, Email, FirstName, LastName, MiddleName, RoleID, IsActive) 
                                        VALUES (@Login, @PasswordHash, @Email, @FirstName, @LastName, @MiddleName, 3, 1)";
                    
                    using (var registerCommand = new SqlCommand(registerQuery, connection))
                    {
                        registerCommand.Parameters.AddWithValue("@Login", RegisterUsernameTextBox.Text);
                        registerCommand.Parameters.AddWithValue("@PasswordHash", HashPassword(RegisterPasswordBox.Password));
                        registerCommand.Parameters.AddWithValue("@Email", EmailTextBox.Text);
                        registerCommand.Parameters.AddWithValue("@FirstName", FirstNameTextBox.Text);
                        registerCommand.Parameters.AddWithValue("@LastName", LastNameTextBox.Text);
                        registerCommand.Parameters.AddWithValue("@MiddleName", 
                            string.IsNullOrWhiteSpace(MiddleNameTextBox.Text) ? DBNull.Value : (object)MiddleNameTextBox.Text);

                        registerCommand.ExecuteNonQuery();

                        // Очищаем поля
                        RegisterUsernameTextBox.Clear();
                        EmailTextBox.Clear();
                        FirstNameTextBox.Clear();
                        LastNameTextBox.Clear();
                        MiddleNameTextBox.Clear();
                        RegisterPasswordBox.Clear();
                        ConfirmPasswordBox.Clear();

                        // Показываем уведомление об успехе
                        NotificationManager.Show("Регистрация успешно завершена! Теперь вы можете войти в систему", NotificationType.Success);
                        
                        // Переключаемся на вкладку входа
                        LoginTab.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка при регистрации: {ex.Message}", NotificationType.Error);
            }
        }

        private void SwitchToRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterTab.IsChecked = true;
        }

        private void SwitchToLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginTab.IsChecked = true;
        }
    }
}
