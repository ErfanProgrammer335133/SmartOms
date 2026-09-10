using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByMobileAsync(string mobile);
        Task<User?> GetByMobileOrUsernameAsync(string username , string mobile);
        Task<User?> GetByRefreshToken(string refreshToken);
        IQueryable<User> GetUsersByCondition(Expression<Func<User , bool>> predicate);

    }
}
