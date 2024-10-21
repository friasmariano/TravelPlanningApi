
namespace TravelPlanningApi.Interfaces;

public interface IUserValidationService
{
    Task<bool> IsUserAdminAsync(int userId);
}