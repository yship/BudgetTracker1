using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Models
{
    public class UserRegisterResponseModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime? Joinedon { get; set; }
    }

    public class UserLoginResponseModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
       
        public List<Expenditure> Expenses { get; set; }

        public List<Income> Incomes { get; set; }
    }

    public class ExpenditureResponseModel {
        public int Id { set; get; }
        public int UserId { set; get; }
        public double Amount { get; set; }
        public String Description { get; set; }
        public DateTime? ExpDate { get; set; }
        public String Remarks { get; set; }

    }

    public class IncomeResponseModel {
        public int Id { set; get; }
        public int UserId { set; get; }
        public double Amount { get; set; }
        public String Description { get; set; }
        public DateTime? IncomeDate { get; set; }
        public String Remarks { get; set; }


    }
}
