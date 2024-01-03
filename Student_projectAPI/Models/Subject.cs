using System.ComponentModel.DataAnnotations;

namespace Student_projectAPI.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
