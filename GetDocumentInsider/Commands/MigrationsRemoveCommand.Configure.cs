﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using GetDocument.Insider.Properties;
using Microsoft.DotNet.Cli.CommandLine;

namespace GetDocument.Insider.Commands
{
    internal partial class MigrationsRemoveCommand : ContextCommandBase
    {
        private CommandOption _force;
        private CommandOption _json;

        public override void Configure(CommandLineApplication command)
        {
            command.Description = Resources.MigrationsRemoveDescription;

            _force = command.Option("-f|--force", Resources.MigrationsRemoveForceDescription);
            _json = Json.ConfigureOption(command);

            base.Configure(command);
        }
    }
}
