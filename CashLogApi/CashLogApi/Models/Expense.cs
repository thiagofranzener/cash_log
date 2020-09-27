using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashLogLib.Models
{
    public partial class Expense
    {
        public int CompanyId { get; set; }
        public int ExpenseId { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public string RequestUser { get; set; }
        public string ManagerUser { get; set; }
        public DateTime Date { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Picture { get; set; }
        public ExpenseSituation Situation { get; set; }
        public double? ApprovedCost { get; set; }
        public string Motive { get; set; }
        public ExpenseCategory Category { get; set; }
    }
}
