// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using GetDocument.Properties;
using Microsoft.DotNet.Cli.CommandLine;

namespace GetDocument.Commands
{
    internal abstract class ProjectCommandBase : HelpCommandBase
    {
        public CommandOption AssemblyPath { get; private set; }

        public CommandOption DataDir { get; private set; }

        public CommandOption Language { get; private set; }

        public CommandOption ProjectDir { get; private set; }

        public CommandOption RootNamespace { get; private set; }

        public CommandOption WorkingDir { get; private set; }

        public override void Configure(CommandLineApplication command)
        {
            AssemblyPath = command.Option("-a|--assembly <PATH>", Resources.AssemblyDescription);
            DataDir = command.Option("--data-dir <PATH>", Resources.DataDirDescription);
            Language = command.Option("--language <LANGUAGE>", Resources.LanguageDescription);
            ProjectDir = command.Option("--project-dir <PATH>", Resources.ProjectDirDescription);
            RootNamespace = command.Option("--root-namespace <NAMESPACE>", Resources.RootNamespaceDescription);
            WorkingDir = command.Option("--working-dir <PATH>", Resources.WorkingDirDescription);

            base.Configure(command);
        }

        protected override void Validate()
        {
            base.Validate();

            if (!AssemblyPath.HasValue())
            {
                throw new CommandException(Resources.MissingOption(AssemblyPath.LongName));
            }
        }
    }
}
