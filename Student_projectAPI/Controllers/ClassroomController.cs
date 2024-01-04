using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Student_projectAPI.Models.ViewModels;
using Student_projectAPI.Models;
using System.Data.SqlClient;
using System.Data;

namespace Student_projectAPI.Controllers
{
    [Route("api/Classroom")]
    [ApiController]
    public class ClassroomController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ClassroomController> _logger;

        public ClassroomController(IConfiguration configuration, ILogger<ClassroomController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [Route("")]
        [HttpGet]
        public async Task<List<Classroom>> GetAllClassrooms()
        {
            try
            {
                List<Classroom> classroomList = new List<Classroom>();
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                SqlCommand cmd = new SqlCommand("SELECT * FROM Classrooms", con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Classroom rooms = new Classroom();
                    rooms.Id = (int)dt.Rows[i]["id"];
                    rooms.Name = dt.Rows[i]["Name"].ToString();

                    classroomList.Add(rooms);
                }
                _logger.LogInformation("Successfully retrieved all classrooms.");
                return classroomList;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("There is a exception : " + ex);
                throw;
            }

        }

        [Route("{id}")]
        [HttpGet]
        public async Task<Classroom> GetClassroomById(int id)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Classrooms WHERE Id = @Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                Classroom classroom = new Classroom();

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    classroom.Id = Convert.ToInt32(row["Id"]);
                    classroom.Name = row["Name"].ToString();
                }

                _logger.LogInformation("Successfully retrieved classroom detials of id = " + id);
                return classroom;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("There is a exception : " + ex);
                throw;
            }

        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostClassroom(Classroom classroom)
        {
            try
            {

                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                if (classroom != null)
                {
                    string query = "INSERT INTO Classrooms VALUES (@Name)";
                    con.Open();

                    SqlCommand cmd = new SqlCommand(query, con);

                    // Add parameters
                    cmd.Parameters.AddWithValue("@Name", classroom?.Name);

                    cmd.ExecuteNonQuery();
                    con.Close();

                    _logger.LogInformation("Successfully saved classroom details");
                    return Ok(classroom);
                }
                else
                {
                    _logger.LogInformation("No data pass");
                    return BadRequest("Invalid classroom data");
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
        public async Task<IActionResult> UpdateClassroom(Classroom classroom, int id)
        {
            try
            {

                // Optionally, you may want to retrieve the updated record
                classroom = await MapForDatabaseUpdateAsync(classroom, id);

                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("UPDATE Classrooms SET Name=@Name WHERE Id=@Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Name", classroom?.Name);

                        cmd.ExecuteNonQuery();
                    }



                    con.Close();
                    _logger.LogInformation("Successfully Updated classrooms details of id = " + id);

                    return Ok(classroom);
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
        public async Task<Classroom> DeleteClassroom(int id)
        {
            try
            {
                DataTable dt = new DataTable();

                Classroom deletedClassroom = await GetClassroomById(id);
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Delete from Classrooms where Id=@Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                _logger.LogInformation("Successfully deleted classroom of id = " + id);
                return deletedClassroom;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("There is a exception : " + ex);
                throw;
            }
        }

        private async Task<Classroom> MapForDatabaseUpdateAsync(Classroom classroom, int id)
        {
            Classroom newlist = new Classroom();

            // Call GetAllStudents method
            Classroom DbList = await GetClassroomById(id);

            newlist.Name = classroom.Name ?? DbList.Name;

            return newlist;
        }
    }
}
