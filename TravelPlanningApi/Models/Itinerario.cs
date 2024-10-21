
namespace TravelPlanningApi.Models;

public class Itinerario
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Status { get; set; }
    
    public Usuario? Usuario { get; set; }
}