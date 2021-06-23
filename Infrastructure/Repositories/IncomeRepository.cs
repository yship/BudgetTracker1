using ApplicationCore.Entities;
using ApplicationCore.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class IncomeRepository : EfRepository<Income>, IIncomeRepository
    {
        public IncomeRepository(BudgetTrackerDbContext dbContext) : base(dbContext)
        {

        }

       public async Task<IEnumerable<Income>> GetAllIncomes(int id, int pageSize = 30, int pageIndex = 1) {
            var incomes = await _dbContext.Incomes.Where(u => u.UserId == id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return incomes;
        }
    }
}
