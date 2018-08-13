// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if NETCOREAPP2_0
using System.Runtime.Loader;
#endif

namespace GetDocument.Commands
{
    internal class GetDocumentCommand : ProjectCommandBase
    {
        private const string WorkerType = "GetDocument.Commands.GetDocumentCommandWorker";

        protected override int Execute()
        {
            var thisAssembly = typeof(GetDocumentCommand).GetTypeInfo().Assembly;
            var thisAssemblyPath = thisAssembly.Location;
            var thisAssemblyDirectory = Path.GetDirectoryName(thisAssemblyPath);

            var packagedAssemblies = Directory
                .EnumerateFiles(thisAssemblyDirectory, "*.dll")
                .Except(new[] { thisAssemblyPath })
                .ToDictionary(path => Path.GetFileNameWithoutExtension(path));

            // Explicitly load all assemblies we need because target project may depend on some of them
            foreach (var name in packagedAssemblies.Keys)
            {
                try
                {
                    Assembly.Load(new AssemblyName(name));
                }
                catch
                {
                    // ... but ignore all failures because assembly should be loadable from our directory.
                }
            }

#if NETCOREAPP2_0
            var loadContext = AssemblyLoadContext.Default;
            loadContext.Resolving += (context, assemblyName) =>
            {
                var assemblyPath = GetFullPath(assemblyName.Name, thisAssemblyDirectory, packagedAssemblies);
                if (assemblyPath == null)
                {
                    return null;
                }

                return context.LoadFromAssemblyPath(assemblyPath);
            };
#elif NET461
            AppDomain.CurrentDomain.AssemblyResolve += (source, eventArgs) =>
            {
                var assemblyName = new AssemblyName(eventArgs.Name);
                var name = assemblyName.Name;
                var assemblyPath = GetFullPath(name, thisAssemblyDirectory, packagedAssemblies);
                if (assemblyPath == null)
                {
                    return null;
                }

                return Assembly.LoadFile(assemblyPath);
            };
#else
#error target frameworks need to be updated.
#endif

            var workerType = thisAssembly.GetType(WorkerType, throwOnError: true);
            var methodInfo = workerType.GetMethod("Process", BindingFlags.Public | BindingFlags.Static);
            try
            {
                return (int)methodInfo.Invoke(obj: null, parameters: new[] { this });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }
        }

        private static string GetFullPath(string name, string directory, IDictionary<string, string> packagedAssemblies)
        {
            if (!packagedAssemblies.TryGetValue(name, out var assemblyPath))
            {
                return null;
            }

            if (!File.Exists(assemblyPath))
            {
                throw new InvalidOperationException($"Referenced assembly '{name}' was not found in {directory}.");
            }

            return assemblyPath;
        }
    }
}
