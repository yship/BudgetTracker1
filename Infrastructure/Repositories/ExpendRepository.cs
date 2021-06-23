using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using ApplicationCore.Entities;
using ApplicationCore.RepositoryInterfaces;
using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ExpendRepository : EfRepository<Expenditure>, IExpendRepository
    {
        public ExpendRepository(BudgetTrackerDbContext dbContext) : base(dbContext) { 
        }

        // list user's expenses by expenditure ID
        public override async Task<Expenditure> GetByIdAsync(int id) {
           var expense = await _dbContext.Expenditures.Where(u => u.Id == id).FirstAsync();
            return expense;

        }

        // return all expenses from userId
        public async Task<IEnumerable<Expenditure>> GetAllExpense(int userId, int pageSize = 30, int pageIndex = 1) {
            var expenses = await _dbContext.Expenditures.Where(u => u.UserId == userId).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return expenses;
        
        }


    }
}
