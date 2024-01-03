using System.ComponentModel.DataAnnotations;

namespace Student_projectAPI.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ContactPerson { get; set; }
        public string? EmailAddress { get; set; }
        public string? ContactNo { get; set; }
        public DateTime? DOB { get; set; }
        public int? Age { get; set; }
        public Classroom Classroom { get; set; }

    }
}
