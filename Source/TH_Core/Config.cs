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


    }
}
