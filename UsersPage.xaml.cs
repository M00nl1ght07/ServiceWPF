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



    /// Логика взаимодействия для UsersPage.xaml



    /// </summary>



    public class User



    {



        public string FullName { get; set; }



        public string Email { get; set; }



        public string Role { get; set; }



    }







    public partial class UsersPage : Page



    {



        public UsersPage()



        {



            InitializeComponent();



            LoadUsers();



        }







        private void LoadUsers()



        {



            // Временные данные, пока нет БД



            var users = new[]



            {



                new User {



                    FullName = "Иванов Иван Иванович",



                    Email = "ivanov@mail.ru",



                    Role = "Исполнитель"



                },



                new User {



                    FullName = "Петров Петр Петрович",



                    Email = "petrov@mail.ru",



                    Role = "Исполнитель"



                },



                new User {



                    FullName = "Сидоров Сидор Сидорович",



                    Email = "sidorov@mail.ru",



                    Role = "Пользователь"



                },



                new User {



                    FullName = "Администратор",



                    Email = "admin@mail.ru",



                    Role = "Администратор"



                }



            };







            UsersList.ItemsSource = users;



        }







        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)



        {



            // Здесь будет логика поиска



        }







        private void AddUser_Click(object sender, RoutedEventArgs e)



        {



            // Временно, пока нет окна добавления пользователя



            NotificationManager.Show("Здесь будет открываться окно добавления пользователя", NotificationType.Info);



        }







        private void EditUser_Click(object sender, RoutedEventArgs e)



        {



            if (sender is Button button && button.Tag != null)



            {



                var user = button.Tag as User;



                // Временно, пока нет окна редактирования



                NotificationManager.Show($"Здесь будет открываться окно редактирования пользователя: {user.FullName}", NotificationType.Info);



            }



        }







        private void DeleteUser_Click(object sender, RoutedEventArgs e)



        {



            if (sender is Button button && button.Tag != null)



            {



                var user = button.Tag as User;



                var result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя: {user.FullName}?",



                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);







                if (result == MessageBoxResult.Yes)



                {



                    NotificationManager.Show("Пользователь удален", NotificationType.Success);



                    LoadUsers();



                }



            }



        }



    }



}

