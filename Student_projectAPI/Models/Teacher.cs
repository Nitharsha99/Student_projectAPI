using System.ComponentModel.DataAnnotations;
namespace Student_projectAPI.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNo { get; set; }
        public string EmailAddress { get; set; }
    }
}
