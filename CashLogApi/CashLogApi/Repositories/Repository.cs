using System;

namespace CashLogApi.Repositories
{
    public abstract class Repository : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }
    }
}
