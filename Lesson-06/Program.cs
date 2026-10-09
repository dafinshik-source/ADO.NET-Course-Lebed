using Microsoft.Data.SqlClient;
using System;

namespace AcademyApp
{
    class Program
    {
        static string conn_str = ConnectDB();

        static void Main(string[] args)
        {
            bool is_running = true;

            while (is_running)
            {
                Console.WriteLine(" 1  Просмотреть всех студентов");
                Console.WriteLine(" 2  Найти студента по имени");
                Console.WriteLine(" 3  Добавить группу");
                Console.WriteLine(" 4  Добавить студента");
                Console.WriteLine(" 9  Выход");
                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ShowAllStudents();
                            break;
                        case "2":
                            Console.WriteLine(" Введите имя студента");
                            string name = Console.ReadLine() ?? string.Empty;
                            FindStudentByName(name);
                            break;
                        case "3":
                            Console.WriteLine(" Введите название группы");
                            string group_name = Console.ReadLine() ?? string.Empty;
                            if (string.IsNullOrWhiteSpace(group_name))
                                Console.WriteLine("Название не может быть пустым.");
                            else
                            {
                                AddGroup(group_name.Trim());
                                Console.WriteLine("Группа добавлена.");
                            }
                            break;
                        case "4":
                            Console.WriteLine(" Введите имя");
                            string first_name = Console.ReadLine() ?? string.Empty;
                            Console.WriteLine(" Введите фамилию");
                            string last_name = Console.ReadLine() ?? string.Empty;
                            Console.WriteLine(" Введите возраст");
                            string age = Console.ReadLine() ?? string.Empty;
                            Console.WriteLine(" Введите ID группы (пусто — без группы)");
                            string group_id = Console.ReadLine() ?? string.Empty;
                            if (string.IsNullOrWhiteSpace(first_name) || string.IsNullOrWhiteSpace(last_name))
                                Console.WriteLine("Имя и фамилия обязательны.");
                            else
                            {
                                AddStudent(first_name.Trim(), last_name.Trim(), age, group_id);
                                Console.WriteLine("Студент добавлен.");
                            }
                            break;
                        case "9":
                            is_running = false;
                            break;
                        default:
                            Console.WriteLine("Введите номер действия!");
                            break;
                    }
                }
                catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
                {
                    Console.WriteLine("Такая группа уже существует.");
                }
                catch (SqlException ex) when (ex.Number == 547)
                {
                    Console.WriteLine("Группы с таким ID не существует.");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Ошибка базы данных: {ex.Message}");
                }

                if (is_running)
                {
                    Console.WriteLine("Нажмите любую кнопку для продолжения!");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        // Измените имя сервера при необходимости. Скрипт college.sql должен быть выполнен.
        static string ConnectDB()
        {
            string conection_string = "Data Source=COMP7A2\\SQLEXPRESS01;" +
                                      "Initial Catalog=CollegeDB;" +
                                      "Integrated Security=True;" +
                                      "TrustServerCertificate=True;";
            return conection_string;
        }

        static void AddGroup(string name_group)
        {
            using (SqlConnection connection = new SqlConnection(conn_str))
            {
                connection.Open();
                string sql_str = "INSERT INTO dbo.Groups(GroupName) VALUES(@name)";
                using (SqlCommand command = new SqlCommand(sql_str, connection))
                {
                    command.Parameters.AddWithValue("@name", name_group);
                    command.ExecuteNonQuery();
                }
            }
        }

        static void AddStudent(string first_name, string last_name, string age, string group_id)
        {
            using (SqlConnection connection = new SqlConnection(conn_str))
            {
                connection.Open();
                string sql_str = "INSERT INTO dbo.Students(FirstName, LastName, Age, GroupId) " +
                                 "VALUES(@firstName, @lastName, @age, @groupId)";
                using (SqlCommand command = new SqlCommand(sql_str, connection))
                {
                    command.Parameters.AddWithValue("@firstName", first_name);
                    command.Parameters.AddWithValue("@lastName", last_name);
                    // Пустое или нечисловое значение сохраняется как NULL
                    command.Parameters.AddWithValue("@age",
                        int.TryParse(age, out int age_value) ? age_value : DBNull.Value);
                    command.Parameters.AddWithValue("@groupId",
                        int.TryParse(group_id, out int group_value) ? group_value : DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        static void FindStudentByName(string name)
        {
            using (SqlConnection connection = new SqlConnection(conn_str))
            {
                connection.Open();
                string sql_str = "SELECT FirstName, LastName, Age " +
                                 "FROM Students " +
                                 "WHERE FirstName = @name";
                using (SqlCommand command = new SqlCommand(sql_str, connection))
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
                        {
                            string first_name = reader.GetString(0);
                            string last_name = reader.GetString(1);
                            string age = reader.IsDBNull(2) ? "—" : reader.GetInt32(2).ToString();
                            Console.WriteLine($"| {first_name} | {last_name} | {age} |");
                        }
                    }
                }
            }
        }

        static void ShowAllStudents()
        {
            using (SqlConnection connection = new SqlConnection(conn_str))
            {
                connection.Open();
                string sql_str = "SELECT s.FirstName, s.LastName, s.Age, g.GroupName " +
                                 "FROM Students AS s " +
                                 "LEFT JOIN Groups AS g ON s.GroupId = g.GroupId";
                using (SqlCommand command = new SqlCommand(sql_str, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Студентов пока нет.");
                        return;
                    }
                    while (reader.Read())
                    {
                        string first_name = reader.GetString(0);
                        string last_name = reader.GetString(1);
                        string age = reader.IsDBNull(2) ? "—" : reader.GetInt32(2).ToString();
                        string group_name = reader.IsDBNull(3) ? "без группы" : reader.GetString(3);
                        Console.WriteLine($"| {first_name} | {last_name} | {age} | {group_name} |");
                    }
                }
            }
        }
    }
}
