using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GetDocument.Commands
{
    public class FakeWebHostBuilder : IWebHostBuilder
    {
        private readonly IWebHost _webHost;

        public FakeWebHostBuilder(IWebHost webHost)
        {
            _webHost = webHost;
        }

        public IWebHost Build()
        {
            return _webHost;
        }

        public IWebHostBuilder ConfigureAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            throw new NotImplementedException();
        }

        public IWebHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            throw new NotImplementedException();
        }

        public IWebHostBuilder ConfigureServices(Action<WebHostBuilderContext, IServiceCollection> configureServices)
        {
            throw new NotImplementedException();
        }

        public string GetSetting(string key)
        {
            throw new NotImplementedException();
        }

        public IWebHostBuilder UseSetting(string key, string value)
        {
            throw new NotImplementedException();
        }
    }
}
