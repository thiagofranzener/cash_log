using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashLogApi.Models
{
    class Expense
    {

        int _companyId, _expenseId;
        string _description, _requestUser, _manageUser, _picture, _motive;
        double _cost, _approvedCost;
        DateTime _date, _approvedDate;
        ExpenseSituation _situation;
        ExpenseCategory _category;

        public int CompanyId { get => _companyId; set => _companyId = value; }
        public int ExpenseId { get => _expenseId; set => _expenseId = value; }
        public string Description { get => _description; set => _description = value; }
        public string RequestUser { get => _requestUser; set => _requestUser = value; }
        public string ManageUser { get => _manageUser; set => _manageUser = value; }
        public string Picture { get => _picture; set => _picture = value; }
        public string Motive { get => _motive; set => _motive = value; }
        public double Cost { get => _cost; set => _cost = value; }
        public double ApprovedCost { get => _approvedCost; set => _approvedCost = value; }
        public DateTime Date { get => _date; set => _date = value; }
        public DateTime ApprovedDate { get => _approvedDate; set => _approvedDate = value; }
        internal ExpenseSituation Situation { get => _situation; set => _situation = value; }
        internal ExpenseCategory Category { get => _category; set => _category = value; }
    }
}
