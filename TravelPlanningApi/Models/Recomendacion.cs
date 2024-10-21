
namespace TravelPlanningApi.Models;

public class Recomendacion
{
    public int Id { get; set; }
    public int DestinoId { get; set; }
    public string Contenido { get; set; } = String.Empty;
    
    public Destino? Destino { get; set; }
}