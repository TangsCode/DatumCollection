using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Data.SqlServer
{
    public class SqlServerStorage : IDataStorage
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> ExecuteAsync()
        {
            throw new NotImplementedException();
        }

        public IDbConnection GetConnection()
        {
            throw new NotImplementedException();
        }
    }
}
