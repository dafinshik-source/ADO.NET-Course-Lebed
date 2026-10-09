using System;
using Microsoft.Data.SqlClient;

namespace CollegeApp
{
    class Program
    {
        // Измените имя сервера при необходимости. Сначала выполните college.sql.
        static string connectionString =
            "Data Source=comp7a2\\sqlexpress01;" +
            "Initial Catalog=CollegeDB;" +
            "Integrated Security=True;" +
            "TrustServerCertificate=True;";

        static void Main()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== CollegeDB ===");
                Console.WriteLine("1. Показать всех студентов");
                Console.WriteLine("2. Найти студента по имени");
                Console.WriteLine("3. Выход");
                Console.Write("Ваш выбор: ");

                try
                {
                    switch (Console.ReadLine())
                    {
                        case "1":
                            ShowAllStudents();
                            break;
                        case "2":
                            Console.Write("Введите имя студента: ");
                            FindStudentByName(Console.ReadLine() ?? string.Empty);
                            break;
                        case "3":
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Неверный выбор.");
                            break;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Ошибка базы данных: {ex.Message}");
                }

                if (running)
                {
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }

        static void ShowAllStudents()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql =
                    "SELECT s.StudentId, s.FirstName, s.LastName, s.Age, g.GroupName " +
                    "FROM dbo.Students AS s " +
                    "LEFT JOIN dbo.Groups AS g ON g.GroupId = s.GroupId " +
                    "ORDER BY s.StudentId";
                using (SqlCommand command = new SqlCommand(sql, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Студентов пока нет.");
                        return;
                    }
                    while (reader.Read())
                        PrintStudent(reader);
                }
            }
        }

        static void FindStudentByName(string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Параметризованный запрос — защита от SQL-инъекций
                string sql =
                    "SELECT s.StudentId, s.FirstName, s.LastName, s.Age, g.GroupName " +
                    "FROM dbo.Students AS s " +
                    "LEFT JOIN dbo.Groups AS g ON g.GroupId = s.GroupId " +
                    "WHERE s.FirstName = @name";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@name", name.Trim());
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Студент не найден.");
                            return;
                        }
                        while (reader.Read())
                            PrintStudent(reader);
                    }
                }
            }
        }

        static void PrintStudent(SqlDataReader reader)
        {
            int id = reader.GetInt32(0);
            string firstName = reader.GetString(1);
            string lastName = reader.GetString(2);
            string age = reader.IsDBNull(3) ? "—" : reader.GetInt32(3).ToString();
            string group = reader.IsDBNull(4) ? "без группы" : reader.GetString(4);
            Console.WriteLine($"{id}: {firstName} {lastName}, возраст: {age}, группа: {group}");
        }
    }
}
