using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashLogLib.Models
{
    public partial class Company : IEquatable<Company>
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

        public override bool Equals(object obj)
        {
            return Equals(obj as Company);
        }

        public bool Equals(Company other)
        {
            return other is object && Id == other.Id;
        }
    }
}
