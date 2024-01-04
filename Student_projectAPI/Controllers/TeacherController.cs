using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Student_projectAPI.Models.ViewModels;
using Student_projectAPI.Models;
using System.Data.SqlClient;
using System.Data;

namespace Student_projectAPI.Controllers
{
    [Route("api/Teacher")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(IConfiguration configuration, ILogger<TeacherController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [Route("")]
        [HttpGet]
        public async Task<List<TeacherViewModel>> GetAllTeachers()
        {
            try
            {
                List<TeacherViewModel> teacherList = new List<TeacherViewModel>();
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                SqlCommand cmd = new SqlCommand("SELECT * FROM Teachers", con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TeacherViewModel teachers = new TeacherViewModel();
                    teachers.Id = (int)dt.Rows[i]["id"];
                    teachers.FirstName = dt.Rows[i]["FirstName"].ToString();
                    teachers.LastName = dt.Rows[i]["LastName"].ToString();
                    teachers.EmailAddress = dt.Rows[i]["EmailAddress"].ToString();
                    teachers.ContactNo = dt.Rows[i]["ContactNo"].ToString();

                    teacherList.Add(teachers);
                }
                _logger.LogInformation("Successfully retrieved all teachers.");
                return teacherList;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("There is a exception : " + ex);
                throw;
            }
        
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<TeacherViewModel> GetTeacherById(int id)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Teachers WHERE Id = @Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                TeacherViewModel teacher = new TeacherViewModel();

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    teacher.Id = Convert.ToInt32(row["Id"]);
                    teacher.FirstName = row["FirstName"].ToString();
                    teacher.LastName = row["LastName"].ToString();
                    teacher.EmailAddress = row["EmailAddress"].ToString();
                    teacher.ContactNo = row["ContactNo"].ToString();

                }

                _logger.LogInformation("Successfully retrieved teacher detials of id = " + id);
                return teacher;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("There is a exception : " + ex);
                throw;
            }

        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostTeacher(Teacher teacher)
        {
            try
            {

                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                if (teacher != null)
                {
                    string query = "INSERT INTO Teachers VALUES (@FirstName, @LastName, @ContactNo, @EmailAddress)";
                    con.Open();

                    SqlCommand cmd = new SqlCommand(query, con);

                    // Add parameters
                    cmd.Parameters.AddWithValue("@FirstName", teacher?.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", teacher?.LastName);
                    cmd.Parameters.AddWithValue("@EmailAddress", teacher?.EmailAddress);
                    cmd.Parameters.AddWithValue("@ContactNo", teacher?.ContactNo);

                    cmd.ExecuteNonQuery();
                    con.Close();

                    _logger.LogInformation("Successfully saved teacher details");
                    return Ok(teacher);
                }
                else
                {
                    _logger.LogInformation("No data pass");
                    return BadRequest("Invalid teacher data");
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation("There is a exception : " + ex);
                throw;
            }
        }

        [Route("{id}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateTeacher(Teacher teacher, int id)
        {
            try
            {

                // Optionally, you may want to retrieve the updated record
                teacher = await MapForDatabaseUpdateAsync(teacher, id);

                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("UPDATE Teachers SET FirstName=@FirstName, LastName=@LastName, EmailAddress=@EmailAddress, ContactNo=@ContactNo WHERE Id=@Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@FirstName", teacher?.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", teacher?.LastName);
                        cmd.Parameters.AddWithValue("@EmailAddress", teacher?.EmailAddress);
                        cmd.Parameters.AddWithValue("@ContactNo", teacher?.ContactNo);

                        cmd.ExecuteNonQuery();
                    }



                    con.Close();
                    _logger.LogInformation("Successfully Updated teacher details of id = " + id);

                    return Ok(teacher);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("There is a exception : " + ex);
                throw;
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<TeacherViewModel> DeleteTeacher(int id)
        {
            try
            {
                DataTable dt = new DataTable();

                TeacherViewModel deletedTeacher = await GetTeacherById(id);
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Delete from Teachers where Id=@Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                _logger.LogInformation("Successfully deleted teacher of id = " + id);
                return deletedTeacher;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("There is a exception : " + ex);
                throw;
            }
        }

        private async Task<Teacher> MapForDatabaseUpdateAsync(Teacher teacher, int id)
        {
            Teacher newlist = new Teacher();

            // Call GetAllStudents method
            TeacherViewModel DbList = await GetTeacherById(id);

            newlist.FirstName = teacher.FirstName ?? DbList.FirstName;
            newlist.LastName = teacher.LastName ?? DbList.LastName;
            newlist.EmailAddress = teacher.EmailAddress ?? DbList.EmailAddress;
            newlist.ContactNo = teacher.ContactNo ?? DbList.ContactNo;

            return newlist;
        }


    }
}
