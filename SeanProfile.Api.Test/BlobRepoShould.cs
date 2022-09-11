using AutoFixture;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using SeanProfile.Api.DataLayer;
using SeanProfile.Api.Model;
using System;
using Xunit;

namespace SeanProfile.Api.Test
{
    public class BlobRepoShould
    {
        private readonly IConfigurationRoot _configuration;
        private readonly Mock<IOptions<AppSettingsModel>> _appsettings;

        public BlobRepoShould()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json", optional: false)
                .Build();

            _appsettings = new Mock<IOptions<AppSettingsModel>>();

            _appsettings.Setup(x => x.Value).Returns(new AppSettingsModel
            {
                SqlConnection = _configuration.GetValue<string>("AppSettings:SqlConnection"),
                BlobConnString = _configuration.GetValue<string>("AppSettings:BlobConnString"),
                BlobContainer = _configuration.GetValue<string>("AppSettings:BlobContainer"),
                Token = _configuration.GetValue<string>("AppSettings:Token")
            });
        }

        [Fact]
        public void BuildCorrectConnString()
        {
            var sut = new BlobStorageRepository(_appsettings.Object);

            var client = sut.ConnectionStringAsync();

            Assert.Equal("test", client.Name);
            Assert.Equal(new Uri("https://schoolagreementtest.blob.core.windows.net/test"), client.Uri);
            Assert.Equal("schoolagreementtest", client.AccountName);
        }


    }
}
