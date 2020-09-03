namespace TH.Core
{
    public class Config
    {
#if (RELEASE)
        /// <summary> DB connection. </summary>
        public const string DbConnectionString = "pooling=true;CHARSET=utf8;server=localhost;database=th_calc;uid=th;pwd=dbaccess;Allow User Variables=True;"
#else
        /// <summary> DB connection. </summary>
        public const string DbConnectionString = "pooling=true;CHARSET=utf8;server=localhost;database=th_calc;uid=root;pwd=admin;Allow User Variables=True;";
#endif


        /// <summary> The timezone used by the platform, all database times are stored in this timezone. When using it will be converted to the timezone of the user. </summary>
        public const string TH_PLATFORM_TIMEZONE_ID = "W. Europe Standard Time";
    }
}
