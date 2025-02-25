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

    /// Логика взаимодействия для StatisticsPage.xaml

    /// </summary>

    public class ExecutorStatistics

    {

        public string Name { get; set; }

        public string ActiveRequests { get; set; }

        public string CompletedRequests { get; set; }

    }



    public partial class StatisticsPage : Page

    {

        public StatisticsPage()

        {

            InitializeComponent();

            LoadStatistics();

        }



        private void LoadStatistics()

        {

            // Временные данные, пока нет БД

            TotalRequestsBlock.Text = "156";

            ActiveRequestsBlock.Text = "23";

            CompletedRequestsBlock.Text = "48";



            var executors = new[]

            {

                new ExecutorStatistics {

                    Name = "Иванов И.И.",

                    ActiveRequests = "12 в работе",

                    CompletedRequests = "28 выполнено"

                },

                new ExecutorStatistics {

                    Name = "Петров П.П.",

                    ActiveRequests = "8 в работе",

                    CompletedRequests = "15 выполнено"

                },

                new ExecutorStatistics {

                    Name = "Сидоров С.С.",

                    ActiveRequests = "3 в работе",

                    CompletedRequests = "5 выполнено"

                }

            };



            ExecutorsList.ItemsSource = executors;

        }

    }

}


