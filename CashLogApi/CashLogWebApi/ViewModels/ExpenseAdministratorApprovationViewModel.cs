using CashLogLib;

namespace CashLogWebApi.ViewModels
{
    public partial class ExpenseAdministratorApprovationViewModel
    {
        public int CompanyId { get; set; }
        public int ExpenseId { get; set; }
        public ExpenseSituation Situation { get; set; }
        public double ApprovedCost { get; set; }
        public string Motive { get; set; }
    }
}