using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashLogApi.Models
{
    class ExpenseAdmistratorApprovation
    {
        int _companyId, _expenseId;
        ExpenseSituation _situation;
        double _approvedCost;
        string _motive;

        public int CompanyId { get => _companyId; set => _companyId = value; }
        public int ExpenseId { get => _expenseId; set => _expenseId = value; }
        public double ApprovedCost { get => _approvedCost; set => _approvedCost = value; }
        public string Motive { get => _motive; set => _motive = value; }
        internal ExpenseSituation Situation { get => _situation; set => _situation = value; }
    }
}
