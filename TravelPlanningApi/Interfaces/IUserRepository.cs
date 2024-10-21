
using TravelPlanningApi.Models;

namespace TravelPlanningApi.Interfaces;

public interface IUserRepository
{
    Task<Usuario> GetUserByIdAsync(int id);    
}