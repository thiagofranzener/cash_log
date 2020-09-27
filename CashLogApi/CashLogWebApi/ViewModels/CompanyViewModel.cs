using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LazyDevUtils.Validators;

namespace CashLogWebApi.ViewModels
{
    public partial class CompanyViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cnpj { get; set; }
        public string Address { get; set; }
        public int Number { get; set; }
        public string Complement { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Cep { get; set; }
        public Guid Token { get; set; }
    }
}