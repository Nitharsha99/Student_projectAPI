using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Student_projectAPI.Models;
using Student_projectAPI.Models.ViewModels;
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
        public async Task<List<StudentViewModel>> GetAllStudents()
        {
            List<StudentViewModel> studentList = new List<StudentViewModel>();
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand("SELECT * FROM Students inner join Classrooms on Classrooms.Id = Students.ClassroomId", con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                StudentViewModel students = new StudentViewModel();
                students.Id = (int)dt.Rows[i]["id"];
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
            return studentList ;
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<StudentViewModel> GetStudentById(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Students INNER JOIN Classrooms ON Classrooms.Id = Students.ClassroomId WHERE Students.Id = @Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            StudentViewModel student = new StudentViewModel();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                student.Id = Convert.ToInt32(row["Id"]);
                student.FirstName = row["FirstName"].ToString();
                student.LastName = row["LastName"].ToString();
                student.ContactPerson = row["ContactPerson"].ToString();
                student.EmailAddress = row["EmailAddress"].ToString();
                student.ContactNo = row["ContactNo"].ToString();
                student.DOB = row["DOB"] is DBNull ? null : (DateTime?)row["DOB"];
                student.Age = row["Age"] is DBNull ? null : (int?)row["Age"];

                Classroom classroom = new Classroom
                {
                    Id = Convert.ToInt32(row["ClassroomId"]),
                    Name = row["Name"].ToString()
                };

                student.Classroom = classroom;
            }

            return student;
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
                    //string query = "Insert into Students values ('" + student?.FirstName + "','" + student?.LastName + "','" + student?.ContactPerson + "','" + student?.EmailAddress + "','" + student?.ContactNo + "','" + student?.DOB + "'," + student?.Age + "," + student?.Classroom + ")";
                    string query = "INSERT INTO Students VALUES (@FirstName, @LastName, @ContactPerson, @EmailAddress, @ContactNo, @DOB, @Age, @ClassroomId)";
                    con.Open();

                    SqlCommand cmd = new SqlCommand(query, con);

                    // Add parameters
                    cmd.Parameters.AddWithValue("@FirstName", student?.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", student?.LastName);
                    cmd.Parameters.AddWithValue("@ContactPerson", student?.ContactPerson);
                    cmd.Parameters.AddWithValue("@EmailAddress", student?.EmailAddress);
                    cmd.Parameters.AddWithValue("@ContactNo", student?.ContactNo);
                    cmd.Parameters.AddWithValue("@DOB", student?.DOB);
                    cmd.Parameters.AddWithValue("@Age", student?.Age);
                    cmd.Parameters.AddWithValue("@ClassroomId", student?.ClassroomId);

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

        [Route("{id}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateProduct(Student student, int id)
        {
            try
            {

                // Optionally, you may want to retrieve the updated record
                student = await MapForDatabaseUpdateAsync(student, id);

                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("UPDATE Students SET FirstName=@FirstName, LastName=@LastName, ContactPerson=@ContactPerson, EmailAddress=@EmailAddress, ContactNo=@ContactNo, DOB=@DOB, Age=@Age, ClassroomId=@ClassroomId WHERE Id=@Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@FirstName", student?.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", student?.LastName);
                        cmd.Parameters.AddWithValue("@ContactPerson", student?.ContactPerson);
                        cmd.Parameters.AddWithValue("@EmailAddress", student?.EmailAddress);
                        cmd.Parameters.AddWithValue("@ContactNo", student?.ContactNo);
                        cmd.Parameters.AddWithValue("@DOB", student?.DOB);
                        cmd.Parameters.AddWithValue("@Age", student?.Age);
                        cmd.Parameters.AddWithValue("@ClassroomId", student?.ClassroomId);

                        cmd.ExecuteNonQuery();
                    }



                    con.Close();

                    return Ok(student);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<StudentViewModel> DeleteStudent(int id)
        {
            DataTable dt = new DataTable();

            StudentViewModel deletedStudent = await GetStudentById(id);
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("Delete from Students where Id=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            return deletedStudent;
        }

        private async Task<Student> MapForDatabaseUpdateAsync(Student student, int id)
        {
            Student newlist = new Student();

            StudentController ControllerInstance = new StudentController(_configuration);

            // Call GetAllStudents method
            StudentViewModel DbList = await ControllerInstance.GetStudentById(id);

            newlist.FirstName = student.FirstName ?? DbList.FirstName;
            newlist.LastName = student.LastName ?? DbList.LastName;
            newlist.EmailAddress = student.EmailAddress ?? DbList.EmailAddress;
            newlist.Age = student.Age ?? DbList.Age;
            newlist.ContactNo = student.ContactNo ?? DbList.ContactNo;
            newlist.ContactPerson = student.ContactPerson ?? DbList.ContactPerson;
            newlist.DOB = student.DOB ?? DbList.DOB;
            newlist.ClassroomId = student.ClassroomId ?? DbList.Classroom.Id;

            return newlist;
        }
    }
}
