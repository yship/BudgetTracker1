using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : EfRepository<Users>, IUserRepository
    {
        public UserRepository(BudgetTrackerDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<Users> GetUserByEmail(string email)
        {
        //    return await _dbContext.Users.Include(u => u.Expenses).FirstOrDefaultAsync(u => u.Email == email);
        return await _dbContext.Users.Include(u => u.Expenses).FirstOrDefaultAsync(u => u.Email == email);
        
        }
    }
}
