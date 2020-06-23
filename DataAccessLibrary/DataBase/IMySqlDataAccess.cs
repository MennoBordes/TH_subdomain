using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.DataBase
{
    public interface IMySqlDataAccess
    {
        string ConnectionStringName { get; set; }

        Task<List<T>> LoadDataList<T, U>(string sql, U parameters);
        Task<T> LoadData<T, U>(string sql, U parameters);
        Task SaveData<T>(string sql, T parameters);
        string GetConnectionString();
        Task<List<T>> MapMultipleObjects<T, U, V>(string sql, Func<T, U, T> map, V parameters);
        Task<(List<T>, List<U>)> MultipleSets<T, U, V>(string sql, V parameters);
    }
}