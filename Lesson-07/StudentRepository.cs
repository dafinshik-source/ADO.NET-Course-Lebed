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
            const string sql = "SELECT StudentId, FirstName, LastName, Age, GroupId FROM dbo.Students ORDER BY StudentId;";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
                result.Add(Map(reader));
            return result;
        }

        public Student? GetById(int studentId)
        {
            const string sql = "SELECT StudentId, FirstName, LastName, Age, GroupId FROM dbo.Students WHERE StudentId = @StudentId;";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@StudentId", studentId);
            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
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
            AddValueParameters(command, student);
            connection.Open();
            return (int)command.ExecuteScalar()!;
        }

        public bool Update(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            const string sql = @"
                UPDATE dbo.Students
                SET FirstName = @FirstName, LastName = @LastName, Age = @Age, GroupId = @GroupId
                WHERE StudentId = @StudentId;";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@StudentId", student.StudentId);
            AddValueParameters(command, student);
            connection.Open();
            return command.ExecuteNonQuery() > 0;
        }

        public bool Delete(int studentId)
        {
            const string sql = "DELETE FROM dbo.Students WHERE StudentId = @StudentId;";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@StudentId", studentId);
            connection.Open();
            return command.ExecuteNonQuery() > 0;
        }

        private static Student Map(SqlDataReader reader)
        {
            return new Student
            {
                StudentId = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Age = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                GroupId = reader.IsDBNull(4) ? null : reader.GetInt32(4)
            };
        }

        private static void AddValueParameters(SqlCommand command, Student student)
        {
            command.Parameters.AddWithValue("@FirstName", student.FirstName);
            command.Parameters.AddWithValue("@LastName", student.LastName);
            command.Parameters.AddWithValue("@Age", (object?)student.Age ?? DBNull.Value);
            command.Parameters.AddWithValue("@GroupId", (object?)student.GroupId ?? DBNull.Value);
        }
    }
}
