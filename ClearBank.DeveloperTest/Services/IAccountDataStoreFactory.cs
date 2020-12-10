using ClearBank.DeveloperTest.Data;

namespace ClearBank.DeveloperTest.Services
{
    public interface IAccountDataStoreFactory
    {
        IAccountDataStore Create();
    }
}