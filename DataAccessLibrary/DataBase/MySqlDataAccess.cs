using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.DataBase
{
    public class MySqlDataAccess : IMySqlDataAccess
    {
        private readonly IConfiguration _config;
#if RELEASE
        public string ConnectionStringName { get; set; } = "Release";
#elif LAPTOP
        public string ConnectionStringName { get; set; } = "Laptop";
#else
        public string ConnectionStringName { get; set; } = "Default";
#endif

        public MySqlDataAccess(IConfiguration config)
        {
            _config = config;
        }


        // === Reading data

        /// <summary> Returns a list of entrys for the specified sql and parameters. </summary>
        /// <typeparam name="T"> The object to be returned. </typeparam>
        /// <typeparam name="U"> (optional) The parameters for the sql statement, of type dynamic. </typeparam>
        /// <param name="sql"> The sql to be executed. </param>
        /// <param name="parameters"> (optional) The parameters for the sql statement, of type dynamic. </param>
        /// <returns> A list of objects of type T. </returns>
        public async Task<List<T>> LoadDataList<T, U>(string sql, U parameters)
        {
            using (IDbConnection con = new MySqlConnection(GetConnectionString()))
            {
                var data = await con.QueryAsync<T>(sql, parameters);
                
                return data.ToList();
            }
        }

        /// <summary> Returns the first entry for the specified sql and parameters. </summary>
        /// <typeparam name="T"> The object to be returned. </typeparam>
        /// <typeparam name="U"> The parameters for the sql statement, of type dynamic. </typeparam>
        /// <param name="sql"> The sql to be executed. </param>
        /// <param name="parameters"> Parameters for the sql statement, of type dynamic. </param>
        /// <returns> A single object of type T. </returns>
        public async Task<T> LoadData<T, V>(string sql, V parameters)
        {
            using (IDbConnection con = new MySqlConnection(GetConnectionString()))
            {
                var data = await con.QueryAsync<T>(sql, parameters);

                return data.FirstOrDefault();
            }
        }

        /// <summary> Maps 2 objects of different types into the T object, before returning it. </summary>
        /// <typeparam name="T"> The object to be returned. </typeparam>
        /// <typeparam name="U"> The object to be inserted into the T object. </typeparam>
        /// <typeparam name="V"> (optional) Parameters for the sql statement, of type dynamic. </typeparam>
        /// <param name="sql"> The sql to be executed. </param>
        /// <param name="map"> The function for mapping U into T. </param>
        /// <param name="parameters"> (optional) Parameters for the sql statement, of type dynamic. </param>
        /// <returns> Objects of type T with containing type U. </returns>
        public async Task<List<T>> MapMultipleObjects<T, U, V>(string sql, Func<T, U, T> map, V parameters)
        {
            using (IDbConnection con = new MySqlConnection(GetConnectionString()))
            {
                var data = await con.QueryAsync<T, U, T>(sql, map, parameters);
                
                return data.ToList();
            }
        }

        /// <summary> Returns a tuple of (list of T) and (list of U). </summary>
        /// <typeparam name="T"> The first type to place in the tuple. </typeparam>
        /// <typeparam name="U"> The second type to place in the tuple. </typeparam>
        /// <typeparam name="V"> (optional) Parameters for the sql statement, of type dynamic. </typeparam>
        /// <param name="sql"> The sql to be executed ('select * from T; Select * from U;'). </param>
        /// <param name="parameters"> (optional) Parameters for the sql statment, of type dynamic. </param>
        /// <returns> A tuple containing lists of type T and type U. </returns>
        public async Task<(List<T>, List<U>)> MultipleSets<T, U, V>(string sql, V parameters)
        {
            using (IDbConnection con = new MySqlConnection(GetConnectionString()))
            {
                List<T> listT = null;
                List<U> listU = null;

                using (var lists = await con.QueryMultipleAsync(sql, parameters))
                {
                    listT = lists.Read<T>().ToList();
                    listU = lists.Read<U>().ToList();
                }

                return (listT, listU);
            }
        }


        // === Writing data

        /// <summary> Save data to the database. </summary>
        /// <typeparam name="T"> The type of the data object. </typeparam>
        /// <param name="sql"> The sql to be executed. </param>
        /// <param name="parameters"> The data parameters, of type dynamic. </param>
        public async Task SaveData<T>(string sql, T parameters)
        {
            using (IDbConnection con = new MySqlConnection(GetConnectionString()))
            {
                await con.ExecuteAsync(sql, parameters);
            }
        }


        // === Helpers

        /// <summary> Get the connection string to the database. </summary>
        public string GetConnectionString()
        {
            return _config.GetConnectionString(ConnectionStringName);
        }
    }
}
