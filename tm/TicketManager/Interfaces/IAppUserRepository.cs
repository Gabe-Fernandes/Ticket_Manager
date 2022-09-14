using TicketManager.Models;

namespace TicketManager.Interfaces;

public interface IAppUserRepository
{
    Task<IEnumerable<AppUser>> GetAllAsync();
    Task<AppUser> GetByIdAsync(string id);
    AppUser GetById(string id);
    Task<string> GetPfpAsync(string id);
    bool Add(AppUser appUser);
    bool Update(AppUser appUser);
    bool Delete(AppUser appUser);
    bool Save();
}
