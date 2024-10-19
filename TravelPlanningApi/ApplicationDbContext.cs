using Microsoft.EntityFrameworkCore;
using TravelPlanningApi.Models;

namespace TravelPlanningApi;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}
