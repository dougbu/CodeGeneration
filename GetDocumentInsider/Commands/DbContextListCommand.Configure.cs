﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using GetDocument.Properties;
using Microsoft.DotNet.Cli.CommandLine;

namespace GetDocument.Commands
{
    internal partial class DbContextListCommand : ProjectCommandBase
    {
        private CommandOption _json;

        public override void Configure(CommandLineApplication command)
        {
            command.Description = Resources.DbContextListDescription;

            _json = Json.ConfigureOption(command);

            base.Configure(command);
        }
    }
}
