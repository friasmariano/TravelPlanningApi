
namespace TravelPlanningApi.Models;

public class DetallesItinerario
{
    public int Id { get; set; }
    public int ItinerarioId { get; set; }
    public int DestinoId { get; set; }
    public int DiaId { get; set; }
    public string Actividad { get; set; } = String.Empty;
    public string Notas { get; set; } = String.Empty;
    
    public Itinerario? Itinerario { get; set; }
    public Destino? Destino { get; set; }
    public Dia? Dia { get; set; }
}