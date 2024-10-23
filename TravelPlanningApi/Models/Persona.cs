
namespace TravelPlanningApi.Models;

public class Persona
{
    public int Id { get; set; }
    public string Nombre { get; set; } = String.Empty;
    public int IdPaisOrigen { get; set; }
    public string? Direccion { get; set; } = String.Empty;
    public int GeneroId { get; set; }
    public string FechaNacimiento { get; set; } = string.Empty;
    
    public Pais? PaisOrigen { get; set; }
    public Genero? Genero { get; set; }
 }