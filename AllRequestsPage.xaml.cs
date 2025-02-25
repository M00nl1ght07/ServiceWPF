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
using ServiceWPF; // Добавляем для NotificationManager

namespace ServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для AllRequestsPage.xaml
    /// </summary>
    public class AdminRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
        public string Status { get; set; }
        public string Executor { get; set; }
    }

    public partial class AllRequestsPage : Page
    {
        public AllRequestsPage()
        {
            InitializeComponent();
            LoadAllRequests();
            StatusFilter.SelectedIndex = 0;
            ExecutorFilter.SelectedIndex = 0;
        }

        private void LoadAllRequests()
        {
            // Временные данные, пока нет БД
            var requests = new[]
            {
                new AdminRequest {
                    Title = "Настройка сети",
                    Description = "Необходимо настроить сетевое подключение на новых компьютерах в кабинете 301.",
                    CreatedDate = "16.03.2024",
                    Status = "В работе",
                    Executor = "Иванов И.И."
                },
                new AdminRequest {
                    Title = "Установка антивируса",
                    Description = "Требуется установить корпоративный антивирус на компьютеры отдела продаж.",
                    CreatedDate = "16.03.2024",
                    Status = "Новая",
                    Executor = "Не назначен"
                },
                new AdminRequest {
                    Title = "Замена картриджа",
                    Description = "Замена картриджа в принтере HP кабинета 405.",
                    CreatedDate = "15.03.2024",
                    Status = "Завершена",
                    Executor = "Петров П.П."
                }
            };

            RequestsList.ItemsSource = requests;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Здесь будет логика поиска
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Здесь будет логика фильтрации по статусу
        }

        private void ExecutorFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Здесь будет логика фильтрации по исполнителю
        }

        private void RequestsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RequestsList.SelectedItem != null)
            {
                NavigationService?.Navigate(new RequestDetailsPage());
                RequestsList.SelectedItem = null;
            }
        }

        private void AssignExecutor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                var request = button.Tag as AdminRequest;
                NotificationManager.Show($"Исполнитель назначен на заявку: {request.Title}", NotificationType.Success);
            }
        }

        private void CancelRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                var request = button.Tag as AdminRequest;
                var result = MessageBox.Show($"Вы уверены, что хотите отменить заявку: {request.Title}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Временно, пока нет БД
                    NotificationManager.Show("Заявка отменена", NotificationType.Success);
                    LoadAllRequests(); // Перезагружаем список
                }
            }
        }
    }
}
