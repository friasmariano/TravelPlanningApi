
namespace TravelPlanningApi.Models;

public class UsuarioToken
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime CreadoEn { get; set; }
    public DateTime ExpiraEn { get; set; }
    public bool Valido { get; set; }
    
    public Usuario? Usuario { get; set; }
}