using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GenerationTasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace GetDocument.Commands
{
    internal class GetDocumentCommandWorker
    {
        public static int Process(GetDocumentCommandContext context)
        {
            var assemblyName = new AssemblyName(context.AssemblyName);
            var assembly = Assembly.Load(assemblyName);
            var entryPointType = assembly.EntryPoint?.DeclaringType;
            if (entryPointType == null)
            {
                Reporter.WriteError($"Assembly {context.AssemblyPath} does not contain an entry point.");
                return 2;
            }

            var builder = GetBuilder(entryPointType, context.AssemblyPath, context.AssemblyName);
            if (builder == null)
            {
                return 3;
            }

            using (var server = new TestServer(builder))
            {
                ProcessAsync(context, server).Wait();
            }

            return 0;
        }

        public static async Task ProcessAsync(GetDocumentCommandContext context, TestServer server)
        {
            if (string.IsNullOrEmpty(context.Service))
            {
                var httpClient = server.CreateClient();

                // TODO: Instead configure logging to mute the HttpsRedirectionMiddleware warning and choose an address
                // from IServerAddressesFeature's list.
                httpClient.BaseAddress = new Uri("https://localhost");

                await DownloadFileCore.DownloadAsync(
                    context.Uri,
                    context.Output,
                    httpClient,
                    new LogWrapper(),
                    CancellationToken.None,
                    timeoutSeconds: 60);
            }
            else
            {
                try
                {
                    // TODO: Pass a TextWriter into the method.
                    var services = server.Host.Services;
                    var serviceType = Type.GetType(context.Service, throwOnError: true);
                    var method = serviceType.GetMethod(context.Method, Array.Empty<Type>());
                    var service = services.GetRequiredService(serviceType);
                    method.Invoke(service, Array.Empty<object>());
                }
                catch (Exception ex)
                {
                    Reporter.WriteWarning(ex.ToString());
                }
            }
        }

        private static IWebHostBuilder GetBuilder(Type entryPointType, string assemblyPath, string assemblyName)
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
                    return new FakeWebHostBuilder(webHost);
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
                    var builder = (IWebHostBuilder)methodInfo.Invoke(obj: null, parameters: args);
                    return builder;
                }
                catch (Exception ex)
                {
                    Reporter.WriteError($"CreateWebHostBuilder method threw: {ex.ToString()}");
                    return null;
                }
            }

            // Startup
            return new WebHostBuilder().UseStartup(assemblyName);
        }
    }
}
