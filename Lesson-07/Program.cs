using System;
using DBConnect.Data;
using DBConnect.Models;
using Microsoft.Data.SqlClient;

namespace DBConnect
{
    class Program
    {
        // Измените имя сервера при необходимости. Скрипт CollegeDB.sql из Lesson-08 должен быть выполнен.
        static string connectionString =
            "Data Source=comp7a2\\sqlexpress01;" +
            "Initial Catalog=CollegeDB;" +
            "Integrated Security=True;" +
            "TrustServerCertificate=True;";

        static StudentRepository student_repo = new StudentRepository(connectionString);
        static GroupRepository group_repo = new GroupRepository(connectionString);

        static void Main()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== Добро пожаловать в журнал ===");
                Console.WriteLine("1. Показать всех студентов");
                Console.WriteLine("2. Показать все группы");
                Console.WriteLine("3. Добавить студента");
                Console.WriteLine("4. Редактировать студента");
                Console.WriteLine("5. Удалить студента");
                Console.WriteLine("6. Выход");
                Console.Write("Ваш выбор: ");

                try
                {
                    switch (Console.ReadLine())
                    {
                        case "1": ShowAllStudents(); break;
                        case "2": ShowAllGroups(); break;
                        case "3": AddStudent(); break;
                        case "4": EditStudent(); break;
                        case "5": DeleteStudent(); break;
                        case "6": running = false; break;
                        default: Console.WriteLine("Неверный выбор."); break;
                    }
                }
                catch (SqlException ex) when (ex.Number == 547)
                {
                    Console.WriteLine("Ошибка: группы с таким ID не существует.");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Ошибка базы данных: {ex.Message}");
                }

                if (running)
                {
                    Console.WriteLine("\nНажмите любую клавишу...");
                    Console.ReadKey();
                }
            }
        }

        static void ShowAllStudents()
        {
            var students = student_repo.GetAll();
            if (students.Count == 0)
            {
                Console.WriteLine("Список пуст.");
                return;
            }

            foreach (var s in students)
                Console.WriteLine(s);
        }

        static void ShowAllGroups()
        {
            var groups = group_repo.GetAllGroup();
            if (groups.Count == 0)
            {
                Console.WriteLine("Список пуст.");
                return;
            }

            foreach (var g in groups)
                Console.WriteLine(g);
        }

        static void AddStudent()
        {
            var student = new Student
            {
                FirstName = ReadRequired("Имя: "),
                LastName = ReadRequired("Фамилия: "),
                Age = ReadOptionalInt("Возраст (Enter — пропустить): "),
                GroupId = ReadOptionalInt("ID группы (Enter — без группы): ")
            };
            int id = student_repo.Add(student);
            Console.WriteLine($"Студент добавлен. ID: {id}");
        }

        static void EditStudent()
        {
            int id = ReadId("ID студента для редактирования: ");
            var student = student_repo.GetById(id);
            if (student == null)
            {
                Console.WriteLine("Студент не найден.");
                return;
            }

            Console.WriteLine($"Текущие данные: {student}");
            student.FirstName = ReadRequired("Новое имя: ");
            student.LastName = ReadRequired("Новая фамилия: ");
            student.Age = ReadOptionalInt("Новый возраст (Enter — пропустить): ");
            student.GroupId = ReadOptionalInt("Новый ID группы (Enter — без группы): ");

            Console.WriteLine(student_repo.Update(student) ? "Студент обновлён." : "Студент не найден.");
        }

        static void DeleteStudent()
        {
            int id = ReadId("ID студента для удаления: ");
            try
            {
                Console.WriteLine(student_repo.Delete(id) ? "Студент удалён." : "Студент не найден.");
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                Console.WriteLine("Нельзя удалить студента: у него есть оценки.");
            }
        }

        static string ReadRequired(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? value = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(value)) return value;
                Console.WriteLine("Поле не может быть пустым.");
            }
        }

        static int? ReadOptionalInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input)) return null;
                if (int.TryParse(input, out int value) && value > 0) return value;
                Console.WriteLine("Введите положительное число или оставьте поле пустым.");
            }
        }

        static int ReadId(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int id) && id > 0) return id;
                Console.WriteLine("Введите положительное целое число.");
            }
        }
    }
}
