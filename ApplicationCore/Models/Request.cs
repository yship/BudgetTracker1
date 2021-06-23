using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class UserRegisterRequestModel
    {
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessage =
            "Password Should have minimum 8 with at least one upper, lower, number and special character")]
        public string Password { get; set; }

        [StringLength(50)] public string FirstName { get; set; }

        [StringLength(50)] public string LastName { get; set; }

   
    }
    public class LoginRequestModel
    {
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    public class IncomeRequestModel
    {
        public int Id { set; get; }
        public int UserId { set; get; }
        public double Amount { get; set; }
        public String Description { get; set; }
        public DateTime? IncomeDate { get; set; }
        public String Remarks { get; set; }


    }

    public class ExpenditureRequestModel
    {
        public int Id { set; get; }
        public int UserId { set; get; }
        public double Amount { get; set; }
        public String Description { get; set; }
        public DateTime? ExpDate { get; set; }
        public String Remarks { get; set; }

    }



}
