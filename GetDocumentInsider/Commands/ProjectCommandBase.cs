// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using GetDocument.Properties;
using Microsoft.DotNet.Cli.CommandLine;

namespace GetDocument.Commands
{
    internal abstract class ProjectCommandBase : HelpCommandBase
    {
        public CommandOption AssemblyPath { get; private set; }

        public override void Configure(CommandLineApplication command)
        {
            AssemblyPath = command.Option("-a|--assembly <PATH>", Resources.AssemblyDescription);

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
