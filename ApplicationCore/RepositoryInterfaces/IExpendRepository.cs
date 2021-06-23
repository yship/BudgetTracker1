using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.RepositoryInterfaces
{
    public interface IExpendRepository:IAsyncRepository<Expenditure>
    {
        Task<IEnumerable<Expenditure>> GetAllExpense(int id, int pageSize = 30, int pageIndex = 1);

    }
}
