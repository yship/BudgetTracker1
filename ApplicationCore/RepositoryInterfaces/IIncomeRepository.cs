using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.RepositoryInterfaces
{
    public interface IIncomeRepository : IAsyncRepository<Income>
    {
        Task<IEnumerable<Income>> GetAllIncomes(int id, int pageSize = 30, int pageIndex = 1);

    }
}
