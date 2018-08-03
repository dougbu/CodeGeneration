// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using GetDocument.Properties;
using Microsoft.DotNet.Cli.CommandLine;

namespace GetDocument.Commands
{
    internal class RootCommand : HelpCommandBase
    {
        public override void Configure(CommandLineApplication command)
        {
            command.FullName = Resources.FullName;
            command.VersionOption("--version", GetVersion);

            base.Configure(command);
        }

        protected override int Execute()
        {
            Reporter.WriteInformation("Hello World");
            return base.Execute();
        }

        private static string GetVersion()
            => typeof(RootCommand)
                .GetTypeInfo()
                .Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
    }
}
