using THTools.ORM.Common;

namespace TH.Core.Base.Database
{
    class ConnectionProvider : IDbConnectionProvider
    {
        public string GetConnectionString()
        {
            return Config.DbConnectionString;
        }
    }
}
