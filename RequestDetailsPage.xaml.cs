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
    /// Логика взаимодействия для RequestDetailsPage.xaml
    /// </summary>
    public partial class RequestDetailsPage : Page
    {
        public RequestDetailsPage()
        {
            InitializeComponent();
            LoadRequestDetails();
            LoadHistory();
            LoadComments();
        }

        private void LoadRequestDetails()
        {
            // Временно, пока нет БД
            TitleBlock.Text = "Не работает принтер";
            CreatedDateBlock.Text = "15.03.2024";
            StatusBlock.Text = "В работе";
            PriorityBlock.Text = "Высокий";
            DescriptionBlock.Text = "Принтер HP в кабинете 405 не печатает документы. При отправке на печать появляется ошибка.";
        }

        private void LoadHistory()
        {
            // Временно, пока нет БД
            var history = new[]
            {
                new { Status = "В работе", Date = "15.03.2024 15:30" },
                new { Status = "Назначен исполнитель", Date = "15.03.2024 14:45" },
                new { Status = "Новая", Date = "15.03.2024 14:30" }
            };

            HistoryList.ItemsSource = history;
        }

        private void LoadComments()
        {
            // Временно, пока нет БД
            var comments = new[]
            {
                new { 
                    Author = "Иванов Иван", 
                    Text = "Выезжаю на место для диагностики проблемы", 
                    Date = "15.03.2024 15:30" 
                },
                new { 
                    Author = "Петров Петр", 
                    Text = "Заявка принята в работу", 
                    Date = "15.03.2024 14:45" 
                }
            };

            CommentsList.ItemsSource = comments;
        }

        private void AddComment_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CommentBox.Text))
            {
                MessageBox.Show("Введите текст комментария", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Временно, пока нет БД
            MessageBox.Show("Комментарий добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            CommentBox.Clear();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
