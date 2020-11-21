using System;
using System.Collections.Generic;

namespace Wikibot.DataAccess
{
    public interface IDataAccess
    {
        string GetConnectionString(string name);
        List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName);
        List<T> LoadDataComplex<T, V, U>(string storedProcedure, U p, string connectionStringName, Type[] types, Func<T, V, T> mapPageToWikiJobRequest, string splitOnString);
        void SaveData<T>(string storedProcedure, T parameters, string connectionStringName);

    }
}