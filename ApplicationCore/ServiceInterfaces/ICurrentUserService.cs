using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;


namespace ApplicationCore.ServiceInterfaces
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        bool IsAuthenticated { get; }
        string Email { get; }
        string FullName { get; }

        bool isAdmin { get; }

        IEnumerable<Expenditure> Expenses { get; }

        IEnumerable<Income> Incomes { get; }

    }
}

