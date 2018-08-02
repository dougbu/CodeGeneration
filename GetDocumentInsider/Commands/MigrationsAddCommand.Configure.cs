﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using GetDocument.Insider.Properties;
using Microsoft.DotNet.Cli.CommandLine;

namespace GetDocument.Insider.Commands
{
    internal partial class MigrationsAddCommand : ContextCommandBase
    {
        private CommandArgument _name;
        private CommandOption _outputDir;
        private CommandOption _json;

        public override void Configure(CommandLineApplication command)
        {
            command.Description = Resources.MigrationsAddDescription;

            _name = command.Argument("<NAME>", Resources.MigrationNameDescription);

            _outputDir = command.Option("-o|--output-dir <PATH>", Resources.MigrationsOutputDirDescription);
            _json = Json.ConfigureOption(command);

            base.Configure(command);
        }
    }
}
