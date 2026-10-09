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

        public List<Group> GetAllGroup()
        {
            var groups = new List<Group>();
            const string sql = "SELECT GroupId, GroupName FROM dbo.Groups ORDER BY GroupId;";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
                groups.Add(new Group { GroupId = reader.GetInt32(0), GroupName = reader.GetString(1) });
            return groups;
        }
    }
}
