using Dapper;
using eCommerce.Core.Domain.Contracts;
using eCommerce.Core.Domain.Entities;
using eCommerce.Infrastructure.DbContext;

namespace eCommerce.Infrastructure.Repositories;
internal class UserRepository(DapperDbContext dbContext) : IUserRepository
{
    public async Task<ApplicationUser?> AddUser(ApplicationUser user)
    {
        user.UserId = Guid.NewGuid();

        string query = """
    INSERT INTO public."Users"
    ("UserId", "Email", "Name", "Gender", "Password")
    VALUES (@UserId, @Email, @Name, @Gender, @Password)
    """;

        await dbContext.DbConnection.ExecuteAsync(query, user);

        return user;
    }

    public async Task<ApplicationUser?> GetUserByEmailAndPassword(string? email, string? password)
    {
        string query = """
             SELECT * FROM public."Users"
             WHERE "Email" = @Email AND "Password" = @Password
            """;
        var parameters = new { Email = email, Password = password };

        ApplicationUser? user = await dbContext.DbConnection.QueryFirstOrDefaultAsync<ApplicationUser>(query, parameters);
        return user; 
    }

    public async Task<ApplicationUser?> GetUserByUserId(Guid userId)
    {
        string query = """
                SELECT * FROM public."Users"
                WHERE "UserId" = @UserId
            """;
        var parameter = new { UserId = userId };
        var user = await dbContext.DbConnection.QueryFirstOrDefaultAsync<ApplicationUser>(query, parameter);
        return user;
    }
}