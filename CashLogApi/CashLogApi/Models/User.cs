using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashLogApi.Models
{
    class User
    {
        private string _userId, _cpf, _name, _password, _email;
        private int _companyId;
        private UserRole _role;

        public string UserId { get => _userId; set => _userId = value; }
        public string Cpf { get => _cpf; set => _cpf = value; }
        public string Name { get => _name; set => _name = value; }
        public string Password { get => _password; set => _password = value; }
        public string Email { get => _email; set => _email = value; }
        public int CompanyId { get => _companyId; set => _companyId = value; }
        internal UserRole Role { get => _role; set => _role = value; }
    }
}
