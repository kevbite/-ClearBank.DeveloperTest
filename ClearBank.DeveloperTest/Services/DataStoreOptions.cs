using System.Configuration;

namespace ClearBank.DeveloperTest.Services
{
    public class DataStoreOptions : IDataStoreOptions
    {
        public string DataStoreType =>
            ConfigurationManager.AppSettings["DataStoreType"];
    }
}