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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(string userRole = "user") // временно, пока нет БД
        {
            InitializeComponent();
            ConfigureForRole(userRole);
            
            // Загружаем начальную страницу
            MainFrame.Navigate(new MyRequestsPage());
        }

        private void ConfigureForRole(string role)
        {
            // Здесь будет настройка интерфейса под конкретную роль
            switch (role.ToLower())
            {
                case "admin":
                    ConfigureForAdmin();
                    break;
                case "executor":
                    ConfigureForExecutor();
                    break;
                default:
                    ConfigureForUser();
                    break;
            }
        }

        private void ConfigureForUser()
        {
            // Базовая конфигурация для пользователя
            MenuPanel.Children.Clear();
            
            var myRequestsButton = new RadioButton 
            { 
                Content = "Мои заявки",
                Style = FindResource("MenuButtonStyle") as Style,
                IsChecked = true
            };
            myRequestsButton.Checked += MenuButton_Checked;
            MenuPanel.Children.Add(myRequestsButton);

            var createRequestButton = new RadioButton 
            { 
                Content = "Создать заявку",
                Style = FindResource("MenuButtonStyle") as Style
            };
            createRequestButton.Checked += MenuButton_Checked;
            MenuPanel.Children.Add(createRequestButton);
        }

        private void ConfigureForExecutor()
        {
            // Конфигурация для исполнителя
            MenuPanel.Children.Clear();
            
            MenuPanel.Children.Add(new RadioButton 
            { 
                Content = "Доступные заявки",
                Style = FindResource("MenuButtonStyle") as Style,
                IsChecked = true
            });
            MenuPanel.Children.Add(new RadioButton 
            { 
                Content = "Мои заявки в работе",
                Style = FindResource("MenuButtonStyle") as Style
            });

            // Добавляем обработчик событий для новых кнопок
            foreach (RadioButton button in MenuPanel.Children)
            {
                button.Checked += MenuButton_Checked;
            }
        }

        private void ConfigureForAdmin()
        {
            // Конфигурация для администратора
            MenuPanel.Children.Clear();
            
            MenuPanel.Children.Add(new RadioButton 
            { 
                Content = "Все заявки",
                Style = FindResource("MenuButtonStyle") as Style,
                IsChecked = true
            });
            MenuPanel.Children.Add(new RadioButton 
            { 
                Content = "Пользователи",
                Style = FindResource("MenuButtonStyle") as Style
            });
            MenuPanel.Children.Add(new RadioButton 
            { 
                Content = "Статистика",
                Style = FindResource("MenuButtonStyle") as Style
            });

            // Добавляем обработчик событий для новых кнопок
            foreach (RadioButton button in MenuPanel.Children)
            {
                button.Checked += MenuButton_Checked;
            }
        }

        private void MenuButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton button)
            {
                CurrentPageTitle.Text = button.Content.ToString();
                
                switch (button.Content.ToString())
                {
                    case "Мои заявки":
                        MainFrame.Navigate(new MyRequestsPage());
                        break;
                    case "Создать заявку":
                        MainFrame.Navigate(new CreateRequestPage());
                        break;
                    case "Доступные заявки":
                        MainFrame.Navigate(new AvailableRequestsPage());
                        break;
                    case "Мои заявки в работе":
                        MainFrame.Navigate(new ActiveRequestsPage());
                        break;
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
