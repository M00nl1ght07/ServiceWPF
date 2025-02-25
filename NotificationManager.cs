using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

using System;

using System.Windows;

using System.Windows.Controls;

using System.Windows.Media;

using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace ServiceWPF

{

    public class NotificationManager

    {

        private static Grid notificationGrid;

        private static readonly Border notificationPanel = new Border

        {

            Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)),

            CornerRadius = new CornerRadius(8),

            Padding = new Thickness(30, 20, 30, 20),

            MinWidth = 400,

            MaxWidth = 600,

            HorizontalAlignment = HorizontalAlignment.Center,

            VerticalAlignment = VerticalAlignment.Top,

            Effect = new DropShadowEffect

            {

                BlurRadius = 15,

                ShadowDepth = 2,

                Opacity = 0.2

            },

            Visibility = Visibility.Collapsed

        };



        private static readonly TextBlock notificationText = new TextBlock

        {

            Foreground = Brushes.White,

            FontSize = 16,

            TextWrapping = TextWrapping.Wrap,

            TextAlignment = TextAlignment.Center,

            LineHeight = 24

        };



        public static void Initialize(Window window)

        {

            if (window is MainWindow mainWindow)

            {

                // Проверяем, не добавлена ли уже панель уведомлений

                if (notificationPanel.Parent != null)

                {

                    var parent = notificationPanel.Parent as Panel;

                    parent?.Children.Remove(notificationPanel);

                }



                notificationPanel.Child = notificationText;

                notificationGrid = mainWindow.NotificationGrid;

                notificationGrid.Children.Add(notificationPanel);

                // Устанавливаем Z-Index для уведомлений

                Panel.SetZIndex(notificationPanel, 10000);

            }

        }



        public static void Show(string message, NotificationType type = NotificationType.Info)

        {

            Application.Current.Dispatcher.Invoke(() =>

            {

                notificationText.Text = message;



                // Устанавливаем цвет в зависимости от типа уведомления

                Color color;
                switch (type)
                {
                    case NotificationType.Success:
                        color = Color.FromRgb(76, 175, 80);
                        break;
                    case NotificationType.Warning:
                        color = Color.FromRgb(255, 152, 0);
                        break;
                    case NotificationType.Error:
                        color = Color.FromRgb(244, 67, 54);
                        break;
                    default:
                        color = Color.FromRgb(33, 150, 243);
                        break;
                }



                notificationPanel.Background = new SolidColorBrush(color);

                notificationPanel.Visibility = Visibility.Visible;

                notificationPanel.Opacity = 0;



                // Анимация появления

                var fadeIn = new DoubleAnimation

                {

                    From = 0,

                    To = 1,

                    Duration = TimeSpan.FromSeconds(0.3),

                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }

                };



                notificationPanel.BeginAnimation(UIElement.OpacityProperty, fadeIn);



                // Увеличиваем задержку до 5 секунд

                var timer = new System.Windows.Threading.DispatcherTimer

                {

                    Interval = TimeSpan.FromSeconds(5)

                };



                timer.Tick += (s, e) =>

                {

                    // Увеличиваем время анимации исчезновения

                    var fadeOut = new DoubleAnimation

                    {

                        From = 1,

                        To = 0,

                        Duration = TimeSpan.FromSeconds(0.5), // Увеличили время анимации

                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }

                    };



                    fadeOut.Completed += (sender, args) =>

                    {

                        notificationPanel.Visibility = Visibility.Collapsed;

                    };



                    notificationPanel.BeginAnimation(UIElement.OpacityProperty, fadeOut);

                    timer.Stop();

                };



                timer.Start();

            });

        }

    }



    public enum NotificationType

    {

        Info,

        Success,

        Warning,

        Error

    }

}