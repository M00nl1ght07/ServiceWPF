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
    /// Логика взаимодействия для AvailableRequestsPage.xaml
    /// </summary>
    public partial class AvailableRequestsPage : Page
    {
        public AvailableRequestsPage()
        {
            InitializeComponent();
            LoadAvailableRequests();
            PriorityFilter.SelectedIndex = 0;
        }

        private void LoadAvailableRequests()
        {
            // Временные данные, пока нет БД
            var requests = new[]
            {
                new {
                    Title = "Настройка сети",
                    Description = "Необходимо настроить сетевое подключение на новых компьютерах в кабинете 301.",
                    CreatedDate = "16.03.2024",
                    Priority = "Высокий"
                },
                new {
                    Title = "Установка антивируса",
                    Description = "Требуется установить корпоративный антивирус на компьютеры отдела продаж.",
                    CreatedDate = "16.03.2024",
                    Priority = "Средний"
                },
                new {
                    Title = "Обновление Windows",
                    Description = "Обновить Windows на всех компьютерах бухгалтерии.",
                    CreatedDate = "15.03.2024",
                    Priority = "Низкий"
                }
            };

            RequestsList.ItemsSource = requests;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Здесь будет логика поиска
        }

        private void PriorityFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Здесь будет логика фильтрации по приоритету
        }

        private void TakeRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                // Временно, пока нет БД
                MessageBox.Show("Заявка взята в работу!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadAvailableRequests(); // Перезагружаем список
            }
        }
    }
}
