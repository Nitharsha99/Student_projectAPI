using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Student_projectAPI.Models;
using System.Data.SqlClient;
using System.Data;

namespace Student_projectAPI.Controllers
{
    [Route("api/Subject")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SubjectController> _logger;

        public SubjectController(IConfiguration configuration, ILogger<SubjectController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [Route("")]
        [HttpGet]
        public async Task<List<Subject>> GetAllSubjects()
        {
            try
            {
                List<Subject> subjectList = new List<Subject>();
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                SqlCommand cmd = new SqlCommand("SELECT * FROM Subjects", con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Subject subject = new Subject();
                    subject.Id = (int)dt.Rows[i]["id"];
                    subject.Name = dt.Rows[i]["Name"].ToString();

                    subjectList.Add(subject);
                }
                _logger.LogInformation("Successfully retrieved all subjects.");
                return subjectList;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("There is a exception : " + ex);
                throw;
            }

        }

        [Route("{id}")]
        [HttpGet]
        public async Task<Subject> GetSubjectById(int id)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Subjects WHERE Id = @Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                Subject subject = new Subject();

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    subject.Id = Convert.ToInt32(row["Id"]);
                    subject.Name = row["Name"].ToString();
                }

                _logger.LogInformation("Successfully retrieved subject detials of id = " + id);
                return subject;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("There is a exception : " + ex);
                throw;
            }

        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostSubject(Subject subject)
        {
            try
            {

                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                if (subject != null)
                {
                    string query = "INSERT INTO Subjects VALUES (@Name)";
                    con.Open();

                    SqlCommand cmd = new SqlCommand(query, con);

                    // Add parameters
                    cmd.Parameters.AddWithValue("@Name", subject?.Name);

                    cmd.ExecuteNonQuery();
                    con.Close();

                    _logger.LogInformation("Successfully saved subject details");
                    return Ok(subject);
                }
                else
                {
                    _logger.LogInformation("No data pass");
                    return BadRequest("Invalid subject data");
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
        public async Task<IActionResult> UpdateSubject(Subject subject, int id)
        {
            try
            {

                // Optionally, you may want to retrieve the updated record
                subject = await MapForDatabaseUpdateAsync(subject, id);

                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("UPDATE Subjects SET Name=@Name WHERE Id=@Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Name", subject?.Name);

                        cmd.ExecuteNonQuery();
                    }



                    con.Close();
                    _logger.LogInformation("Successfully Updated subject details of id = " + id);

                    return Ok(subject);
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
        public async Task<Subject> DeleteSubject(int id)
        {
            try
            {
                DataTable dt = new DataTable();

                Subject deletedSubject = await GetSubjectById(id);
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Delete from Subjects where Id=@Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                _logger.LogInformation("Successfully deleted subject of id = " + id);
                return deletedSubject;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("There is a exception : " + ex);
                throw;
            }
        }

        private async Task<Subject> MapForDatabaseUpdateAsync(Subject subject, int id)
        {
            Subject newlist = new Subject();

            // Call GetAllStudents method
            Subject DbList = await GetSubjectById(id);

            newlist.Name = subject.Name ?? DbList.Name;

            return newlist;
        }
    }
}
