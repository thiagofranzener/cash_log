using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashLogLib.Models
{
    public partial class ExpenseCategory : IEquatable<ExpenseCategory>
    {
        public int CompanyId { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ExpenseCategory);
        }

        public bool Equals(ExpenseCategory other)
        {
            return other is object && CompanyId == other.CompanyId && CategoryId == other.CategoryId;
        }
    }
}
