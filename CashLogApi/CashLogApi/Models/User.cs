using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashLogLib.Models
{
    public partial class UserType
    {
        public string User { get; set; }
        public string Cpf { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public UserRole Role { get; set; }
    }
}
