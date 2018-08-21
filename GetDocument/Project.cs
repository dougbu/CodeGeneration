// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GetDocument.Properties;

namespace GetDocument
{
    internal class Project
    {
        private const string MSBuildResourceName = "GetDocument.ServiceProjectReferenceMetadata";

        private readonly string _file;
        private readonly string _framework;
        private readonly string _configuration;
        private readonly string _runtime;

        public Project(string file, string framework, string configuration, string runtime)
        {
            Debug.Assert(!string.IsNullOrEmpty(file), "file is null or empty.");

            _file = file;
            _framework = framework;
            _configuration = configuration;
            _runtime = runtime;
            ProjectName = Path.GetFileName(file);
        }

        public string ProjectName { get; }

        public string AssemblyName { get; set; }

        public string DefaultServiceProjectMethod { get; set; }

        public string DefaultServiceProjectService { get; set; }

        public string DefaultServiceProjectUri { get; set; }

        public string Language { get; set; }

        public string OutputPath { get; set; }

        public string Platform { get; set; }

        public string PlatformTarget { get; set; }

        public string ProjectAssetsFile { get; set; }

        public string ProjectDepsFilePath { get; set; }

        public string ProjectDirectory { get; set; }

        public string ProjectRuntimeConfigFilePath { get; set; }

        public string RootNamespace { get; set; }

        public string RuntimeFrameworkVersion { get; set; }

        public string TargetFileName { get; set; }

        public string TargetFramework { get; set; }

        public string TargetFrameworkMoniker { get; set; }

        public static Project FromFile(
            string file,
            string buildExtensionsDirectory,
            string framework = null,
            string configuration = null,
            string runtime = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(file), "file is null or empty.");

            if (buildExtensionsDirectory == null)
            {
                buildExtensionsDirectory = Path.Combine(Path.GetDirectoryName(file), "obj");
            }

            Directory.CreateDirectory(buildExtensionsDirectory);

            var assembly = typeof(Project).Assembly;
            var propsPath = Path.Combine(
                buildExtensionsDirectory,
                Path.GetFileName(file) + ".ServiceProjectReferenceMetadata.props");
            using (var input = assembly.GetManifestResourceStream($"{MSBuildResourceName}.props"))
            {
                using (var output = File.OpenWrite(propsPath))
                {
                    Reporter.WriteVerbose(Resources.WritingFile(propsPath));
                    input.CopyTo(output);
                }
            }

            var targetsPath = Path.ChangeExtension(propsPath, ".targets");
            using (var input = assembly.GetManifestResourceStream($"{MSBuildResourceName}.targets"))
            {
                using (var output = File.OpenWrite(targetsPath))
                {
                    // NB: Copy always in case it changes
                    Reporter.WriteVerbose(Resources.WritingFile(targetsPath));
                    input.CopyTo(output);
                }
            }

            IDictionary<string, string> metadata;
            var metadataPath = Path.GetTempFileName();
            try
            {
                var propertyArg = "/property:ServiceProjectReferenceMetadataPath=" + metadataPath;
                if (framework != null)
                {
                    propertyArg += ";TargetFramework=" + framework;
                }
                if (configuration != null)
                {
                    propertyArg += ";Configuration=" + configuration;
                }
                if (runtime != null)
                {
                    propertyArg += ";RuntimeIdentifier=" + runtime;
                }

                var args = new List<string>
                {
                    "msbuild",
                    "/target:WriteServiceProjectReferenceMetadata",
                    propertyArg,
                    "/verbosity:quiet",
                    "/nologo"
                };

                if (file != null)
                {
                    args.Add(file);
                }

                var exitCode = Exe.Run("dotnet", args);
                if (exitCode != 0)
                {
                    throw new CommandException(Resources.GetMetadataFailed);
                }

                metadata = File.ReadLines(metadataPath).Select(l => l.Split(new[] { ':' }, 2))
                    .ToDictionary(s => s[0], s => s[1].TrimStart());
            }
            finally
            {
                File.Delete(propsPath);
                File.Delete(metadataPath);
                File.Delete(targetsPath);
            }

            return new Project(file, framework, configuration, runtime)
            {
                AssemblyName = metadata[nameof(AssemblyName)],
                DefaultServiceProjectMethod = metadata[nameof(DefaultServiceProjectMethod)],
                DefaultServiceProjectService = metadata[nameof(DefaultServiceProjectService)],
                DefaultServiceProjectUri = metadata[nameof(DefaultServiceProjectUri)],
                Language = metadata[nameof(Language)],
                OutputPath = metadata[nameof(OutputPath)],
                Platform = metadata[nameof(Platform)],
                PlatformTarget = metadata[nameof(PlatformTarget)] ?? metadata[nameof(Platform)],
                ProjectAssetsFile = metadata[nameof(ProjectAssetsFile)],
                ProjectDepsFilePath = metadata[nameof(ProjectDepsFilePath)],
                ProjectDirectory = metadata[nameof(ProjectDirectory)],
                ProjectRuntimeConfigFilePath = metadata[nameof(ProjectRuntimeConfigFilePath)],
                RootNamespace = metadata[nameof(RootNamespace)],
                RuntimeFrameworkVersion = metadata[nameof(RuntimeFrameworkVersion)],
                TargetFileName = metadata[nameof(TargetFileName)],
                TargetFramework = metadata[nameof(TargetFramework)],
                TargetFrameworkMoniker = metadata[nameof(TargetFrameworkMoniker)]
            };
        }

        public void Build()
        {
            var args = new List<string>
            {
                "build"
            };

            if (_file != null)
            {
                args.Add(_file);
            }

            // TODO: Only build for the first framework when unspecified
            if (_framework != null)
            {
                args.Add("--framework");
                args.Add(_framework);
            }

            if (_configuration != null)
            {
                args.Add("--configuration");
                args.Add(_configuration);
            }

            if (_runtime != null)
            {
                args.Add("--runtime");
                args.Add(_runtime);
            }

            args.Add("/verbosity:quiet");
            args.Add("/nologo");


            var exitCode = Exe.Run("dotnet", args, interceptOutput: true);
            if (exitCode != 0)
            {
                throw new CommandException(Resources.BuildFailed);
            }
        }
    }
}
