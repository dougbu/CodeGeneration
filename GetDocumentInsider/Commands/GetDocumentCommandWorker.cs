using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GetDocument.Commands
{
    internal class GetDocumentCommandWorker
    {
        public static int Process(ProjectCommandBase projectCommand)
        {
            var assemblyPath = projectCommand.AssemblyPath.Value();
            var assemblyFilename = Path.GetFileNameWithoutExtension(assemblyPath);
            var assemblyDirectory = Path.GetFullPath(Path.GetDirectoryName(assemblyPath));
            var assemblyName = new AssemblyName(assemblyFilename);
            var assembly = Assembly.Load(assemblyName);
            var entryPointType = assembly.EntryPoint?.DeclaringType;
            if (entryPointType == null)
            {
                Reporter.WriteError($"Assembly {assemblyPath} does not contain an entry point.");
                return 2;
            }

            var builder = GetBuilder(entryPointType, assemblyPath, assemblyFilename);
            if (builder == null)
            {
                return 3;
            }

            using (var testHost = new TestServer(builder))
            {
                Console.WriteLine($"Hello world!!");
            }

            return 0;
        }

        private static IWebHostBuilder GetBuilder(Type entryPointType, string assemblyPath, string assemblyFilename)
        {
            var args = new[] { Array.Empty<string>() };
            var methodInfo = entryPointType.GetMethod("BuildWebHost");
            if (methodInfo != null)
            {
                // BuildWebHost (old style has highest priority)
                var parameters = methodInfo.GetParameters();
                if (!methodInfo.IsStatic ||
                    parameters.Length != 1 ||
                    typeof(string[]) != parameters[0].ParameterType ||
                    typeof(IWebHost) != methodInfo.ReturnType)
                {
                    Reporter.WriteError("BuildWebHost method found in {assemblyPath} does not have expected signature.");
                    return null;
                }

                try
                {
                    var webHost = (IWebHost)methodInfo.Invoke(obj: null, parameters: args);
                    return new FakeBuilder(webHost);
                }
                catch (Exception ex)
                {
                    Reporter.WriteError($"BuildWebHost method threw: {ex.ToString()}");
                    return null;
                }
            }

            if ((methodInfo = entryPointType.GetMethod("CreateWebHostBuilder")) != null)
            {
                // CreateWebHostBuilder
                var parameters = methodInfo.GetParameters();
                if (!methodInfo.IsStatic ||
                    parameters.Length != 1 ||
                    typeof(string[]) != parameters[0].ParameterType ||
                    typeof(IWebHostBuilder) != methodInfo.ReturnType)
                {
                    Reporter.WriteError("CreateWebHostBuilder method found in {assemblyPath} does not have expected signature.");
                    return null;
                }

                try
                {
                    return (IWebHostBuilder)methodInfo.Invoke(obj: null, parameters: args);
                }
                catch (Exception ex)
                {
                    Reporter.WriteError($"CreateWebHostBuilder method threw: {ex.ToString()}");
                    return null;
                }
            }

            // Startup
            return new WebHostBuilder().UseStartup(assemblyFilename);
        }

        private class FakeBuilder : IWebHostBuilder
        {
            private readonly IWebHost _webHost;

            public FakeBuilder(IWebHost webHost)
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
}
