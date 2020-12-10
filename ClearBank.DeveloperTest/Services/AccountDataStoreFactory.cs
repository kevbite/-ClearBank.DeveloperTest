using ClearBank.DeveloperTest.Data;

namespace ClearBank.DeveloperTest.Services
{
    public class AccountDataStoreFactory : IAccountDataStoreFactory
    {
        private readonly IDataStoreOptions _options;

        public AccountDataStoreFactory(IDataStoreOptions options)
            => _options = options;

        public IAccountDataStore Create() =>
            _options.DataStoreType switch
            {
                "Backup" => new BackupAccountDataStore(),
                _ => new AccountDataStore()
            };
    }
}