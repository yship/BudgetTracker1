using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class Expenditure
    {
        public int Id { set; get; }
        public int UserId { set; get; }
        [ForeignKey(nameof(UserId))]
        public double Amount { get; set; }
        public String Description { get; set; }
        public DateTime ExpDate { get; set; }
        public String Remarks { get; set; }
    }
}
