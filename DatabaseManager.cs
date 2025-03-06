using System;
using System.Data.SqlClient;
using System.Configuration;

namespace ServiceWPF
{
    public static class DatabaseManager
    {
        private static readonly string _connectionString =
            "Server=95.31.128.97,1433;" +
            "Database=ServiceDesk;" +
            "User Id=admin;" +
            "Password=winServer==;" +
            "TrustServerCertificate=True;" +
            "Encrypt=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public static void TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    NotificationManager.Show("Подключение к БД успешно установлено", NotificationType.Success);
                }
            }
            catch (Exception ex)
            {
                NotificationManager.Show($"Ошибка подключения к БД: {ex.Message}", NotificationType.Error);
            }
        }

        // Вспомогательный метод для выполнения запросов
        public static void ExecuteQuery(string query, Action<SqlCommand> parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    parameters?.Invoke(command);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Вспомогательный метод для получения данных
        public static T ExecuteScalar<T>(string query, Action<SqlCommand> parameters = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    parameters?.Invoke(command);
                    var result = command.ExecuteScalar();
                    return (T)Convert.ChangeType(result, typeof(T));
                }
            }
        }
    }
}
