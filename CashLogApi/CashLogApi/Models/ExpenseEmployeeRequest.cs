using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashLogApi.Models
{
    class ExpenseEmployeeRequest
    {
        private int _companyId, _typeId;
        private string _description, _requestUser, _managerUser, _picture;
        private double _cost;
        private DateTime _date;
        private ExpenseSituation _situation;

        public int CompanyId { get => _companyId; set => _companyId = value; }
        public int TypeId { get => _typeId; set => _typeId = value; }
        public string Description { get => _description; set => _description = value; }
        public string RequestUser { get => _requestUser; set => _requestUser = value; }
        public string ManagerUser { get => _managerUser; set => _managerUser = value; }
        public string Picture { get => _picture; set => _picture = value; }
        public double Cost { get => _cost; set => _cost = value; }
        public DateTime Date { get => _date; set => _date = value; }
        internal ExpenseSituation Situation { get => _situation; set => _situation = value; }
    }
}
