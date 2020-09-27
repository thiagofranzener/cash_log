using System;

namespace CashLogWebApi.ViewModels
{
    public partial class ExpenseEmployeeRequestViewModel
    {
        public int CompanyId { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public DateTime Date { get; set; }
        public string Picture { get; set; }
        public int TypeId { get; set; }
    }
}