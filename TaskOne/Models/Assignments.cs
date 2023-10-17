using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskOne.Models
{
    public class Assignments
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Assignment Name")]
        public string AssignmentTitle { get; set; }

        [NotMapped]
        [FileValidation(new string[] { ".jpg", ".jpeg", ".png", ".gif",".pdf" }, 5 * 1024 * 1024, ErrorMessage = "Invalid file.")]
        [Required]
        public List<IFormFile>? Files { get; set; }
    }
}
