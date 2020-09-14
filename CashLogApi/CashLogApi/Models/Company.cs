using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashLogApi.Models
{
    class Company : IEquatable<Company>
    {
        
        int _id, _number;
        string _name, _cnpj, _adress, _complement, _city, _state, _cep;
        Guid _token;

        public int Id { get => _id; set => _id = value; }
        public int Number { get => _number; set => _number = value; }
        public string Name { get => _name; set => _name = value; }
        public string Cnpj { get => _cnpj; set => _cnpj = value; }
        public string Adress { get => _adress; set => _adress = value; }
        public string Complement { get => _complement; set => _complement = value; }
        public string City { get => _city; set => _city = value; }
        public string State { get => _state; set => _state = value; }
        public string Cep { get => _cep; set => _cep = value; }
        public Guid Token { get => _token; set => _token = value; }

        bool IEquatable<Company>.Equals(Company other)
        {
            return (other != null && other.Id == this.Id);
        }
    }
}
