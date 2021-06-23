using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class Income
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public double Amount { get; set; }
        public String Description { get; set; }
        public DateTime IncomeDate { get; set; }
        public String Remarks { get; set; }
    }
}
