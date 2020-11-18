using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Wikibot.DataAccess
{
    public class SqlDataAccess : IDataAccess
    {
        private IConfiguration _config;
        public SqlDataAccess(IConfiguration configuration)
        {
            _config = configuration;
        }
        public string GetConnectionString(string name)
        {
            return new ConfigurationBuilder()
                .AddConfiguration(_config)
                .Build().GetConnectionString(name);
        }

        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                List<T> rows = connection.Query<T>(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }

        public List<T> LoadDataComplex
            <T, U>(string storedProcedure, U parameters, string connectionStringName, Type[] typeArray, Func<object[],T> map, string splitOnString)
        {
            string connectionString = GetConnectionString(connectionStringName);
            Console.WriteLine($"ConnectionString: {connectionString}");
            
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                List<T> rows = connection.Query<T>(storedProcedure, typeArray, map, parameters, splitOn: splitOnString,
                    commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure);

            }
        }
    }
}
