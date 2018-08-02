// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using GetDocument.Insider.Properties;
using Microsoft.DotNet.Cli.CommandLine;

namespace GetDocument.Insider.Commands
{
    internal partial class DatabaseUpdateCommand : ContextCommandBase
    {
        private CommandArgument _migration;

        public override void Configure(CommandLineApplication command)
        {
            command.Description = Resources.DatabaseUpdateDescription;

            _migration = command.Argument("<MIGRATION>", Resources.MigrationDescription);

            base.Configure(command);
        }
    }
}
