﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using GetDocument.Insider.Properties;
using Microsoft.DotNet.Cli.CommandLine;

namespace GetDocument.Insider.Commands
{
    internal partial class DatabaseDropCommand : ContextCommandBase
    {
        private CommandOption _force;
        private CommandOption _dryRun;

        public override void Configure(CommandLineApplication command)
        {
            command.Description = Resources.DatabaseDropDescription;

            _force = command.Option("-f|--force", Resources.DatabaseDropForceDescription);
            _dryRun = command.Option("--dry-run", Resources.DatabaseDropDryRunDescription);

            base.Configure(command);
        }
    }
}
