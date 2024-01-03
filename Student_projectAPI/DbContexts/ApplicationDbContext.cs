using Microsoft.EntityFrameworkCore;
using Student_projectAPI.Models;

namespace Student_projectAPI.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<Classroom> Classrooms { get; set; }

        public DbSet<Subject> Subjects { get; set; }


    }
}
