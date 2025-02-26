using System;
using System.Windows;
using System.Windows.Controls;
using ServiceWPF; // Добавляем для NotificationManager

namespace ServiceWPF
{
    public partial class MyRequestsPage : Page
    {
        public MyRequestsPage()
        {
            InitializeComponent();
            LoadRequests();
        }

        private void LoadRequests()
        {
            // Временные данные, пока нет БД
            var requests = new[]
            {
                new {
                    Title = "Не работает принтер",
                    Description = "Принтер HP в кабинете 405 не печатает документы. При отправке на печать появляется ошибка.",
                    CreatedDate = "15.03.2024",
                    Status = "В работе",
                    Priority = "Высокий"
                },
                new {
                    Title = "Замена картриджа",
                    Description = "Требуется замена картриджа в принтере Samsung кабинета 301.",
                    CreatedDate = "14.03.2024",
                    Status = "Новая",
                    Priority = "Средний"
                },
                new {
                    Title = "Настройка почты",
                    Description = "Необходимо настроить корпоративную почту на новом компьютере.",
                    CreatedDate = "13.03.2024",
                    Status = "Новая",
                    Priority = "Низкий"
                },
                new {
                    Title = "Установка ПО",
                    Description = "Требуется установить пакет Microsoft Office на компьютер в кабинете 506.",
                    CreatedDate = "12.03.2024",
                    Status = "В работе",
                    Priority = "Средний"
                },
                new {
                    Title = "Не работает интернет",
                    Description = "Отсутствует подключение к интернету на всех компьютерах 4 этажа.",
                    CreatedDate = "11.03.2024",
                    Status = "Завершена",
                    Priority = "Высокий"
                },
                new {
                    Title = "Замена картриджа",
                    Description = "Требуется замена картриджа в принтере Samsung кабинета 301.",
                    CreatedDate = "14.03.2024",
                    Status = "Новая",
                    Priority = "Средний"
                },
                new {
                    Title = "Настройка почты",
                    Description = "Необходимо настроить корпоративную почту на новом компьютере.",
                    CreatedDate = "13.03.2024",
                    Status = "Новая",
                    Priority = "Низкий"
                },
                new {
                    Title = "Установка ПО",
                    Description = "Требуется установить пакет Microsoft Office на компьютер в кабинете 506.",
                    CreatedDate = "12.03.2024",
                    Status = "В работе",
                    Priority = "Средний"
                },
                new {
                    Title = "Не работает интернет",
                    Description = "Отсутствует подключение к интернету на всех компьютерах 4 этажа.",
                    CreatedDate = "11.03.2024",
                    Status = "Завершена",
                    Priority = "Высокий"
                }
            };

        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Здесь будет логика поиска
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Здесь будет логика фильтрации по статусу
        }

        private void SortFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Здесь будет логика сортировки
        }

        private void RequestsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}