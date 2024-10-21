
using TravelPlanningApi.Models;
using TravelPlanningApi.Interfaces;

namespace TravelPlanningApi.Repositories;

public class UserRepository: IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario> GetUserByIdAsync(int userId)
    {
        var user = await _context.Usuarios.FindAsync(userId);
        
        if (user == null)
        {
            throw new KeyNotFoundException($"El usuario con el Id# {userId} no fue encontrado.");
        }

        return user;
    }
}