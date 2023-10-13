namespace TaskOne.Models
{
    public class FilePath
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int AssignmentId { get; set; }
        public Assignments Assignment { get; set; }
    }
}
