using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class Users 
    {
        
        public int Id { get; set; }
        public String FullName { get; set; }
        public String Email { get; set; }
        public String Password { get; set; }
        public DateTime Joinedon { get; set; }

        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public List<Expenditure> Expenses { get; set; }

        public List<Income> Incomes { get; set; }
    }
}
