using System;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

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

            using (var testHost = new TestServer(builder))
            {
                Console.WriteLine($"Hello world!!");
            }

            return 0;
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
                    return (IWebHostBuilder)methodInfo.Invoke(obj: null, parameters: args);
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
