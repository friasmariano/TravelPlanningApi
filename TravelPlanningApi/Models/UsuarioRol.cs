
namespace TravelPlanningApi.Models;

public class UsuarioRol
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int RolId { get; set; }
    
    public Usuario? Usuario { get; set; }
    public Rol? Rol { get; set; }
}