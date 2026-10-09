using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using DBConnect.Models;

namespace DBConnect.Data
{
    public class GroupRepository
    {
        private readonly string _connectionString;

        public GroupRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public List<Group> GetAll()
        {
            var groups = new List<Group>();
            const string sql = "SELECT GroupId, GroupName FROM dbo.Groups ORDER BY GroupId;";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                groups.Add(new Group
                {
                    GroupId = reader.GetInt32(0),
                    GroupName = reader.GetString(1)
                });
            }
            return groups;
        }

        public Group? GetById(int groupId)
        {
            const string sql = "SELECT GroupId, GroupName FROM dbo.Groups WHERE GroupId = @GroupId;";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@GroupId", groupId);
            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? new Group { GroupId = reader.GetInt32(0), GroupName = reader.GetString(1) } : null;
        }

        public int Add(Group group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));
            const string sql = "INSERT INTO dbo.Groups (GroupName) VALUES (@GroupName); SELECT CAST(SCOPE_IDENTITY() AS int);";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@GroupName", group.GroupName);
            connection.Open();
            return (int)command.ExecuteScalar()!;
        }

        public bool Update(Group group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));
            const string sql = "UPDATE dbo.Groups SET GroupName = @GroupName WHERE GroupId = @GroupId;";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@GroupId", group.GroupId);
            command.Parameters.AddWithValue("@GroupName", group.GroupName);
            connection.Open();
            return command.ExecuteNonQuery() > 0;
        }

        public bool Delete(int groupId)
        {
            const string sql = "DELETE FROM dbo.Groups WHERE GroupId = @GroupId;";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@GroupId", groupId);
            connection.Open();
            return command.ExecuteNonQuery() > 0;
        }
    }
}
