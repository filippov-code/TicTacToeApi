using Microsoft.EntityFrameworkCore;
using TicTacToeRESTApi.Models;

namespace TicTacToeRESTApi.DataBase
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
