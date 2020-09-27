using System;

namespace CashLogWebApi.ViewModels
{
    public partial class UserSignUpViewModel
    {
        public string User { get; set; }
        public string Cpf { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public Guid CompanyToken { get; set; }
    }
}