using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashLogLib.Models
{
    public partial class ExpenseEmployeeRequest
    {
        public int CompanyId { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public string RequestUser { get; set; }
        public string ManagerUser { get; set; }
        public DateTime Date { get; set; }
        public string Picture { get; set; }
        public int TypeId { get; set; }
        public ExpenseSituation Situation { get; set; }
    }
}
