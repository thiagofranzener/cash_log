using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashLogLib.Models
{
    public partial class ExpenseAdministratorApprovation
    {
        public int CompanyId { get; set; }
        public int ExpenseId { get; set; }
        public ExpenseSituation Situation { get; set; }
        public double ApprovedCost { get; set; }
        public string Motive { get; set; }
    }
}
