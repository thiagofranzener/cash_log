using System;
using LazyDevUtils.Validators;

namespace CashLogWebApi.ViewModels
{
    public partial class UserAuthenticationResult
    {
        public Guid Token { get; set; }
        public CompanyViewModel Company { get; set; }
        public UserViewModel User { get; set; }
    }
}