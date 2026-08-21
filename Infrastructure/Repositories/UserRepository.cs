using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Infrastructure.Database_Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly Context _context; 
        public UserRepository(Context context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByMobileOrUsernameAsync(string username, string mobile)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Phone == mobile || x.Username == username);
        }

        public async Task<User?> GetByMobileAsync(string mobile)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Phone == mobile);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
        }
    }
}
