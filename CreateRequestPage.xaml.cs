using System;

using System.Windows;

using System.Windows.Controls;

using ServiceWPF; // Добавляем для NotificationManager



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

                NotificationManager.Show("Заполните все поля", NotificationType.Warning);

                return;

            }



            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))

            {

                NotificationManager.Show("Заполните все поля", NotificationType.Warning);

                return;

            }



            NotificationManager.Show("Заявка создана!", NotificationType.Success);



            // Очищаем поля

            TitleTextBox.Clear();

            DescriptionTextBox.Clear();

            PriorityComboBox.SelectedIndex = 0;

        }

    }

}
