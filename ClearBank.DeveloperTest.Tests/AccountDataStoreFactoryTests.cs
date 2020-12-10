using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using FluentAssertions;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
    public class AccountDataStoreFactoryTests
    {
        record TestDataStoreOptions(string DataStoreType) : IDataStoreOptions;
        
        [Fact]
        public void ShouldCreateBackupAccountDataStore()
        {
            var testDataStoreOptions = new TestDataStoreOptions("Backup");
            new AccountDataStoreFactory(testDataStoreOptions).Create()
                .Should().BeOfType<BackupAccountDataStore>();
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abcde")]
        [InlineData("1234")]
        public void ShouldCreateAccountDataStore(string dataStoreType)
        {
            var testDataStoreOptions = new TestDataStoreOptions(dataStoreType);
            new AccountDataStoreFactory(testDataStoreOptions).Create()
                .Should().BeOfType<AccountDataStore>();
        }
    }
}