using Microsoft.EntityFrameworkCore;
using TaskOne.Models;

namespace TaskOne.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Assignments> Assignments { get; set; }
        public DbSet<FilePath> FilePaths { get; set; }
    }
}
