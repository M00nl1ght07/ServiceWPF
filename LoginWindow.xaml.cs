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
            
            // Инициализируем начальное состояние
            LoginTab.IsChecked = true;
            LoginContent.Visibility = Visibility.Visible;
            RegisterContent.Visibility = Visibility.Collapsed;
            
            // Добавляем обработчики событий для подсказок паролей
            InitializePasswordHints();
        }

        private void InitializePasswordHints()
        {
            // Подсказки для входа
            PasswordBox.GotFocus += (s, e) => PasswordHint.Visibility = Visibility.Collapsed;
            PasswordBox.LostFocus += (s, e) => 
            {
                if (string.IsNullOrEmpty(PasswordBox.Password))
                    PasswordHint.Visibility = Visibility.Visible;
            };

            // Подсказки для регистрации
            RegisterPasswordBox.GotFocus += (s, e) => RegisterPasswordHint.Visibility = Visibility.Collapsed;
            RegisterPasswordBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrEmpty(RegisterPasswordBox.Password))
                    RegisterPasswordHint.Visibility = Visibility.Visible;
            };

            ConfirmPasswordBox.GotFocus += (s, e) => ConfirmPasswordHint.Visibility = Visibility.Collapsed;
            ConfirmPasswordBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrEmpty(ConfirmPasswordBox.Password))
                    ConfirmPasswordHint.Visibility = Visibility.Visible;
            };
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
            // Временно, пока нет БД
            if (UsernameTextBox.Text == "master")
            {
                new MainWindow("executor").Show();
                this.Close();
            }
            else
            {
                new MainWindow("user").Show();
                this.Close();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Здесь будет логика регистрации
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
