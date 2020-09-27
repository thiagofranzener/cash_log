using CashLogLib;

namespace CashLogWebApi.ViewModels
{
    public partial class UserViewModel
    {
        public string User { get; set; }
        public string Cpf { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public UserRole Role { get; set; }
    }
}