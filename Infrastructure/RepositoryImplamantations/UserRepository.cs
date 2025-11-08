using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.RepositoryImplementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbcontext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}
