// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using GetDocument.Properties;
using Microsoft.DotNet.Cli.CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GetDocument.Commands
{
    internal class InvokeCommand : HelpCommandBase
    {
        private const string InsideManName = "GetDocument.Insider";

        private CommandOption _project;
        private CommandOption _framework;
        private CommandOption _configuration;
        private CommandOption _runtime;
        private CommandOption _msbuildprojectextensionspath;
        private CommandOption _noBuild;
        private IList<string> _args;

        public override void Configure(CommandLineApplication command)
        {
            var options = new ProjectOptions();
            options.Configure(command);

            _project = options.Project;
            _framework = options.Framework;
            _configuration = options.Configuration;
            _runtime = options.Runtime;
            _msbuildprojectextensionspath = options.MSBuildProjectExtensionsPath;
            _noBuild = options.NoBuild;

            command.VersionOption("--version", ProductInfo.GetVersion);
            _args = command.RemainingArguments;

            base.Configure(command);
        }

        protected override int Execute()
        {
            var projectFile = FindProjects(
                _project.Value(),
                Resources.NoProject,
                Resources.MultipleProjects);
            Reporter.WriteVerbose(Resources.UsingProject(projectFile));

            var project = Project.FromFile(
                projectFile,
                _msbuildprojectextensionspath.Value(),
                _framework.Value(),
                _configuration.Value(),
                _runtime.Value());

            if (!_noBuild.HasValue())
            {
                project.Build();
            }

            var targetDirectory = Path.GetFullPath(Path.Combine(project.ProjectDirectory, project.OutputPath));
            if (!Directory.Exists(targetDirectory))
            {
                var message = _noBuild.HasValue() ? Resources.MustBuild : Resources.ProjectMisconfiguration;
                throw new CommandException(message);
            }

            var targetPath = Path.Combine(targetDirectory, project.TargetFileName);
            var thisPath = Path.GetFullPath(Path.GetDirectoryName(typeof(InvokeCommand).Assembly.Location));

            string executable = null;
            var cleanupExecutable = false;
            try
            {
                string toolsDirectory;
                var args = new List<string>();
                var targetFramework = new FrameworkName(project.TargetFrameworkMoniker);
                switch (targetFramework.Identifier)
                {
                    case ".NETFramework":
                        cleanupExecutable = true;
                        executable = Path.Combine(targetDirectory, InsideManName + ".exe");
                        toolsDirectory = Path.Combine(
                            thisPath,
                            project.PlatformTarget == "x86" ? "net461-x86" : "net461");

                        var executableSource = Path.Combine(toolsDirectory, InsideManName + ".exe");
                        File.Copy(executableSource, executable, overwrite: true);

                        var configurationPath = targetPath + ".config";
                        if (File.Exists(configurationPath))
                        {
                            File.Copy(configurationPath, executable + ".config", overwrite: true);
                        }
                        break;

                    case ".NETCoreApp":
                        executable = "dotnet";
                        toolsDirectory = Path.Combine(thisPath, "netcoreapp2.0");

                        if (targetFramework.Version < new Version(2, 0))
                        {
                            throw new CommandException(
                                Resources.NETCoreApp1Project(project.ProjectName, targetFramework.Version));
                        }

                        args.Add("exec");
                        args.Add("--depsFile");
                        args.Add(Path.Combine(targetDirectory, project.AssemblyName + ".deps.json"));

                        var projectAssetsFile = project.ProjectAssetsFile;
                        if (!string.IsNullOrEmpty(projectAssetsFile))
                        {
                            using (var reader = new JsonTextReader(File.OpenText(projectAssetsFile)))
                            {
                                var projectAssets = JToken.ReadFrom(reader);
                                var packageFolders = projectAssets["packageFolders"]
                                    .Children<JProperty>()
                                    .Select(p => p.Name);

                                foreach (var packageFolder in packageFolders)
                                {
                                    args.Add("--additionalProbingPath");
                                    args.Add(packageFolder.TrimEnd(Path.DirectorySeparatorChar));
                                }
                            }
                        }

                        var runtimeConfig = Path.Combine(targetDirectory, project.AssemblyName + ".runtimeconfig.json");
                        if (File.Exists(runtimeConfig))
                        {
                            args.Add("--runtimeConfig");
                            args.Add(runtimeConfig);
                        }
                        else if (!string.IsNullOrEmpty(project.RuntimeFrameworkVersion))
                        {
                            args.Add("--fx-version");
                            args.Add(project.RuntimeFrameworkVersion);
                        }

                        args.Add(Path.Combine(toolsDirectory, InsideManName + ".dll"));
                        break;

                    case ".NETStandard":
                        throw new CommandException(Resources.NETStandardProject(project.ProjectName));

                    default:
                        throw new CommandException(
                            Resources.UnsupportedFramework(project.ProjectName, targetFramework.Identifier));
                }

                args.AddRange(_args);
                args.Add("--assembly");
                args.Add(targetPath);
                args.Add("--tools-directory");
                args.Add(toolsDirectory);

                if (Reporter.IsVerbose)
                {
                    args.Add("--verbose");
                }

                if (Reporter.NoColor)
                {
                    args.Add("--no-color");
                }

                if (Reporter.PrefixOutput)
                {
                    args.Add("--prefix-output");
                }

                return Exe.Run(executable, args, project.ProjectDirectory);
            }
            finally
            {
                if (cleanupExecutable && !string.IsNullOrEmpty(executable))
                {
                    File.Delete(executable);
                    File.Delete(executable + ".config");
                }
            }
        }

        private static string FindProjects(
            string path,
            string errorWhenNoProject,
            string errorWhenMultipleProjects)
        {
            var specified = true;
            if (path == null)
            {
                specified = false;
                path = Directory.GetCurrentDirectory();
            }
            else if (!Directory.Exists(path)) // It's not a directory
            {
                return path;
            }

            var projectFiles = Directory
                .EnumerateFiles(path, "*.*proj", SearchOption.TopDirectoryOnly)
                .Where(f => !string.Equals(Path.GetExtension(f), ".xproj", StringComparison.OrdinalIgnoreCase))
                .Take(2)
                .ToList();
            if (projectFiles.Count == 0)
            {
                throw new CommandException(
                    specified
                        ? Resources.NoProjectInDirectory(path)
                        : errorWhenNoProject);
            }
            if (projectFiles.Count != 1)
            {
                throw new CommandException(
                    specified
                        ? Resources.MultipleProjectsInDirectory(path)
                        : errorWhenMultipleProjects);
            }

            return projectFiles[0];
        }
    }
}
