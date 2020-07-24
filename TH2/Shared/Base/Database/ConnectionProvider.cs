using System;
using System.Collections.Generic;
using System.Text;
using THTools.ORM.Common;

namespace TH2.Shared.Base.Database
{
    class ConnectionProvider : IDbConnectionProvider
    {
        public string GetConnectionString()
        {
            return Config.DbConnectionString;
        }
    }
}
