using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashLogLib.Repositories
{
    public abstract partial class Repository : IDisposable
    {
        public abstract void Dispose();
    }
}
