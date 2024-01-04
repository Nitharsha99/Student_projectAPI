using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Student_projectAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace Student_projectAPI.Controllers
{
    [Route("api/Student")]
    [ApiController]
    public class StudentController : ControllerBase
    {

        private readonly IConfiguration _configuration;

       public StudentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            List<Student> studentList = new List<Student>();
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand("SELECT * FROM Students inner join Classrooms on Classrooms.Id = Students.ClassroomId", con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Student students = new Student();
                students.FirstName = dt.Rows[i]["FirstName"].ToString();
                students.LastName = dt.Rows[i]["LastName"].ToString();
                students.ContactPerson = dt.Rows[i]["ContactPerson"].ToString();
                students.EmailAddress = dt.Rows[i]["EmailAddress"].ToString();
                students.ContactNo = dt.Rows[i]["ContactNo"].ToString();
                students.DOB = (DateTime?)dt.Rows[i]["DOB"];
                students.Age = (int?)dt.Rows[i]["Age"];
                Classroom classroom = new Classroom();

                classroom.Id = Convert.ToInt32(dt.Rows[i]["ClassroomId"]);
                classroom.Name = dt.Rows[i]["Name"].ToString();

                students.Classroom = classroom;
                studentList.Add(students);
            }
            return Ok(studentList);
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostStudent(Student student)
        {
            try
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                if(student != null)
                {
                    SqlCommand cmd = new SqlCommand("Insert into Students values ('" + student.FirstName + "','" + student.LastName + "','" + student.ContactPerson + "','" + student.EmailAddress + "','" + student.ContactNo + "','" + student.DOB + "'," + student.Age + ")", con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    return Ok(student);
                }
                else {
                    return BadRequest("Invalid student data");
                        }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
