using System.ComponentModel.DataAnnotations;

namespace Student_projectAPI.Models
{
    public class Classroom
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
