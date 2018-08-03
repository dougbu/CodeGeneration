﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.DotNet.Cli.CommandLine;
using GetDocument.Commands;
using GetDocument.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using EFCommand = GetDocument.Commands.RootCommand;

namespace GetDocument
{
    internal class RootCommand : CommandBase
    {
        private CommandLineApplication _command;
        private CommandOption _project;
        private CommandOption _startupProject;
        private CommandOption _framework;
        private CommandOption _configuration;
        private CommandOption _runtime;
        private CommandOption _msbuildprojectextensionspath;
        private CommandOption _noBuild;
        private CommandOption _help;
        private IList<string> _args;

        public override void Configure(CommandLineApplication command)
        {
            command.FullName = Resources.DotnetEfFullName;

            var options = new ProjectOptions();
            options.Configure(command);

            _project = options.Project;
            _startupProject = options.StartupProject;
            _framework = options.Framework;
            _configuration = options.Configuration;
            _runtime = options.Runtime;
            _msbuildprojectextensionspath = options.MSBuildProjectExtensionsPath;
            _noBuild = options.NoBuild;

            command.VersionOption("--version", GetVersion);
            _help = command.Option("-h|--help", description: null);

            _args = command.RemainingArguments;

            base.Configure(command);

            _command = command;
        }

        protected override int Execute()
        {
            var commands = _args.TakeWhile(a => a[0] != '-').ToList();
            if (_help.HasValue() || ShouldHelp(commands))
            {
                return ShowHelp(_help.HasValue(), commands);
            }

            var projectFile = FindProjects(
                _project.Value(),
                Resources.NoProject,
                Resources.MultipleProjects);
            Reporter.WriteVerbose(Resources.UsingProject(projectFile));

            var starupProjectFile = FindProjects(
                _startupProject.Value(),
                Resources.NoStartupProject,
                Resources.MultipleStartupProjects);
            Reporter.WriteVerbose(Resources.UsingStartupProject(starupProjectFile));

            var project = Project.FromFile(projectFile, _msbuildprojectextensionspath.Value());
            var startupProject = Project.FromFile(
                starupProjectFile,
                _msbuildprojectextensionspath.Value(),
                _framework.Value(),
                _configuration.Value(),
                _runtime.Value());

            if (!_noBuild.HasValue())
            {
                startupProject.Build();
            }

            string executable;
            var args = new List<string>();

            var toolsPath = Path.Combine(
                Path.GetDirectoryName(typeof(Program).GetTypeInfo().Assembly.Location),
                "tools");

            var targetDir = Path.GetFullPath(Path.Combine(startupProject.ProjectDir, startupProject.OutputPath));
            var targetPath = Path.Combine(targetDir, project.TargetFileName);
            var startupTargetPath = Path.Combine(targetDir, startupProject.TargetFileName);
            var depsFile = Path.Combine(
                targetDir,
                startupProject.AssemblyName + ".deps.json");
            var runtimeConfig = Path.Combine(
                targetDir,
                startupProject.AssemblyName + ".runtimeconfig.json");
            var projectAssetsFile = startupProject.ProjectAssetsFile;

            var targetFramework = new FrameworkName(startupProject.TargetFrameworkMoniker);
            if (targetFramework.Identifier == ".NETFramework")
            {
                executable = Path.Combine(
                    toolsPath,
                    "net461",
                    startupProject.PlatformTarget == "x86"
                        ? "win-x86"
                        : "any",
                    "ef.exe");
            }
            else if (targetFramework.Identifier == ".NETCoreApp")
            {
                if (targetFramework.Version < new Version(2, 0))
                {
                    throw new CommandException(
                        Resources.NETCoreApp1StartupProject(startupProject.ProjectName, targetFramework.Version));
                }

                executable = "dotnet";
                args.Add("exec");
                args.Add("--depsfile");
                args.Add(depsFile);

                if (!string.IsNullOrEmpty(projectAssetsFile))
                {
                    using (var reader = new JsonTextReader(File.OpenText(projectAssetsFile)))
                    {
                        var projectAssets = JObject.ReadFrom(reader);
                        var packageFolders = projectAssets["packageFolders"].Children<JProperty>().Select(p => p.Name);

                        foreach (var packageFolder in packageFolders)
                        {
                            args.Add("--additionalprobingpath");
                            args.Add(packageFolder.TrimEnd(Path.DirectorySeparatorChar));
                        }
                    }
                }

                if (File.Exists(runtimeConfig))
                {
                    args.Add("--runtimeconfig");
                    args.Add(runtimeConfig);
                }
                else if (startupProject.RuntimeFrameworkVersion.Length != 0)
                {
                    args.Add("--fx-version");
                    args.Add(startupProject.RuntimeFrameworkVersion);
                }

                args.Add(Path.Combine(toolsPath, "netcoreapp2.0", "any", "ef.dll"));
            }
            else if (targetFramework.Identifier == ".NETStandard")
            {
                throw new CommandException(Resources.NETStandardStartupProject(startupProject.ProjectName));
            }
            else
            {
                throw new CommandException(
                    Resources.UnsupportedFramework(startupProject.ProjectName, targetFramework.Identifier));
            }

            args.AddRange(_args);
            args.Add("--assembly");
            args.Add(targetPath);
            args.Add("--startup-assembly");
            args.Add(startupTargetPath);
            args.Add("--project-dir");
            args.Add(project.ProjectDir);
            args.Add("--language");
            args.Add(project.Language);
            args.Add("--working-dir");
            args.Add(Directory.GetCurrentDirectory());

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

            if (project.RootNamespace.Length != 0)
            {
                args.Add("--root-namespace");
                args.Add(project.RootNamespace);
            }

            return Exe.Run(executable, args, startupProject.ProjectDir);
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

            var projectFiles = Directory.EnumerateFiles(path, "*.*proj", SearchOption.TopDirectoryOnly)
                    .Where(f => !string.Equals(Path.GetExtension(f), ".xproj", StringComparison.OrdinalIgnoreCase))
                    .Take(2).ToList();
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

        private static string GetVersion()
            => typeof(RootCommand).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;

        private static bool ShouldHelp(IReadOnlyList<string> commands)
            => commands.Count == 0
                || (commands.Count == 1
                    && (commands[0] == "database"
                        || commands[0] == "dbcontext"
                        || commands[0] == "migrations"));

        private int ShowHelp(bool help, IEnumerable<string> commands)
        {
            var app = new CommandLineApplication
            {
                Name = _command.Name
            };

            new EFCommand().Configure(app);

            app.FullName = _command.FullName;

            var args = new List<string>(commands);
            if (help)
            {
                args.Add("--help");
            }

            return app.Execute(args.ToArray());
        }
    }
}
