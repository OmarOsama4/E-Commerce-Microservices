using eCommerce.Core.Domain.Entities;

namespace eCommerce.Core.Domain.Contracts;
public interface IUserRepository
{
    Task<ApplicationUser?> AddUser(ApplicationUser user);
    Task<ApplicationUser?> GetUserByEmailAndPassword(string? email, string? password);
}

