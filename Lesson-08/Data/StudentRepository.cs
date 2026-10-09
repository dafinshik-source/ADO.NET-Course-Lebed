using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using DBConnect.Models;

namespace DBConnect.Data
{
    public class StudentRepository
    {
        private readonly string _connectionString;

        public StudentRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public List<Student> GetAll()
        {
            var result = new List<Student>();
            const string sql = @"
                SELECT s.StudentId, s.FirstName, s.LastName, s.Age, s.GroupId, g.GroupName
                FROM dbo.Students s
                LEFT JOIN dbo.Groups g ON g.GroupId = s.GroupId
                ORDER BY s.StudentId;";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Student
                {
                    StudentId = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Age = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    GroupId = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    GroupName = reader.IsDBNull(5) ? null : reader.GetString(5)
                });
            }
            return result;
        }

        // Добавляет студента и возвращает его новый StudentId
        public int Add(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            const string sql = @"
                INSERT INTO dbo.Students (FirstName, LastName, Age, GroupId)
                VALUES (@FirstName, @LastName, @Age, @GroupId);
                SELECT CAST(SCOPE_IDENTITY() AS int);";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@FirstName", student.FirstName);
            command.Parameters.AddWithValue("@LastName", student.LastName);
            command.Parameters.AddWithValue("@Age", (object?)student.Age ?? DBNull.Value);
            command.Parameters.AddWithValue("@GroupId", (object?)student.GroupId ?? DBNull.Value);
            connection.Open();
            return (int)command.ExecuteScalar()!;
        }
    }
}
