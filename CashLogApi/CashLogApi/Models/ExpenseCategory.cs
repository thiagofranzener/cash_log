using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashLogApi.Models
{
    class ExpenseCategory : IEquatable<ExpenseCategory>
    {

        private int _companyId, _categoryId;
        private string _description;

        public int CompanyId { get => _companyId; set => _companyId = value; }
        public int CategoryId { get => _categoryId; set => _categoryId = value; }
        public string Description { get => _description; set => _description = value; }

        public bool Equals(ExpenseCategory other)
        {
            throw new NotImplementedException();
        }
    }
}
