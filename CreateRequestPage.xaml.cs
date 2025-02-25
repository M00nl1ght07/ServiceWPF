using System.Windows;
using System.Windows.Controls;

namespace ServiceWPF
{
    public partial class CreateRequestPage : Page
    {
        public CreateRequestPage()
        {
            InitializeComponent();
            PriorityComboBox.SelectedIndex = 0; // По умолчанию низкий приоритет
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Временно, пока нет БД
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Введите заголовок заявки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
            {
                MessageBox.Show("Введите описание проблемы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Заявка успешно создана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // Очищаем поля
            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            PriorityComboBox.SelectedIndex = 0;
        }
    }
}