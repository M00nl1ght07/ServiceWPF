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
    /// Логика взаимодействия для ActiveRequestsPage.xaml
    /// </summary>
    public class ActiveRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
        public string Status { get; set; }
    }

    public partial class ActiveRequestsPage : Page
    {
        public ActiveRequestsPage()
        {
            InitializeComponent();
            LoadActiveRequests();
            StatusFilter.SelectedIndex = 0;
        }

        private void LoadActiveRequests()
        {
            // Временные данные, пока нет БД
            var requests = new[]
            {
                new ActiveRequest {
                    Title = "Настройка сети",
                    Description = "Необходимо настроить сетевое подключение на новых компьютерах в кабинете 301.",
                    CreatedDate = "16.03.2024",
                    Status = "В работе"
                },
                new ActiveRequest {
                    Title = "Установка антивируса",
                    Description = "Требуется установить корпоративный антивирус на компьютеры отдела продаж.",
                    CreatedDate = "16.03.2024",
                    Status = "На проверке"
                },
                new ActiveRequest {
                    Title = "Замена картриджа",
                    Description = "Замена картриджа в принтере HP кабинета 405.",
                    CreatedDate = "15.03.2024",
                    Status = "Завершена"
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

        private void RequestsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RequestsList.SelectedItem != null)
            {
                NavigationService?.Navigate(new RequestDetailsPage());
                RequestsList.SelectedItem = null;
            }
        }

        private void StatusChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.Tag != null)
            {
                var newStatus = (comboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                // Временно, пока нет БД
                NotificationManager.Show($"Статус заявки изменен на: {newStatus}", NotificationType.Success);
            }
        }
    }
}
