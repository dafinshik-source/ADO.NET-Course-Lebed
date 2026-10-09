namespace DBConnect.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int? Age { get; set; }
        public int? GroupId { get; set; }
        public string? GroupName { get; set; }   // заполняется при чтении (JOIN с Groups)

        public override string ToString()
        {
            string age = Age.HasValue ? $"{Age} лет" : "возраст не указан";
            string group = string.IsNullOrEmpty(GroupName) ? "без группы" : $"группа {GroupName}";
            return $"{StudentId}: {FirstName} {LastName}, {age}, {group}";
        }
    }
}
