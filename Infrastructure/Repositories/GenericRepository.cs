using Application.Exceptions;
using Domain.Repositories;
using Infrastructure.Database_Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly Context _context;
        public GenericRepository(Context context)
        {
            _context = context;            
        }
        public async Task AddAsync(T item) => await _context.Set<T>().AddAsync(item);

        public async Task<T?> GetByIdAsync(Guid id) => await _context.Set<T>().FindAsync(id);
    }
}
