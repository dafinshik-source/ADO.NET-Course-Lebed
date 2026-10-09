namespace DBConnect.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{GroupId}: {GroupName}";
        }
    }
}
