
using Microsoft.EntityFrameworkCore;
using TravelPlanningApi.Models;

namespace TravelPlanningApi;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Destino> Destinos { get; set; }
    public DbSet<DetallesItinerario> DetallesItinerarios { get; set; }
    public DbSet<Dia> Dias { get; set; }
    public DbSet<Genero> Generos { get; set; }
    public DbSet<Itinerario> Itinerarios { get; set; }
    public DbSet<Pais> Paises { get; set; }
    public DbSet<Persona> Personas  { get; set; }
    public DbSet<Recomendacion> Recomendaciones { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<UsuarioRol> UsuarioRoles { get; set; }
    public DbSet<UsuarioToken> UsuarioTokens { get; set; }
}
