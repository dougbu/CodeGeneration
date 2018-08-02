﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using GetDocument.Insider.Properties;
using Microsoft.DotNet.Cli.CommandLine;

namespace GetDocument.Insider.Commands
{
    internal class DatabaseCommand : HelpCommandBase
    {
        public override void Configure(CommandLineApplication command)
        {
            command.Description = Resources.DatabaseDescription;

            command.Command("drop", new DatabaseDropCommand().Configure);
            command.Command("update", new DatabaseUpdateCommand().Configure);

            base.Configure(command);
        }
    }
}
