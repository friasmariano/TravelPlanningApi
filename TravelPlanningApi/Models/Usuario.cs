
namespace TravelPlanningApi.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int PersonaId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ProfilePic { get; set; } = string.Empty;  
    public DateTime FechaCreacion { get; set; }
    
    public Persona? Persona { get; set; }
}