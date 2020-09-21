using System;
using System.Collections.Generic;

namespace Wikibot.DataAccess
{
    public interface IDataAccess
    {
        string GetConnectionString(string name);
        List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName);
        List<T> LoadDataComplex<T, U>(string storedProcedure, U parameters, string connectionStringName, Type[] typeArray, Func<object[], T> map, string splitOnString);
        void SaveData<T>(string storedProcedure, T parameters, string connectionStringName);
    }
}