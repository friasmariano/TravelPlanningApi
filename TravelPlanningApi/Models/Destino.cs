
namespace TravelPlanningApi.Models;

public class Destino
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int PaisId { get; set; }
    public float RatingPromedio { get; set; }
    
    public Pais? Pais { get; set; }
}