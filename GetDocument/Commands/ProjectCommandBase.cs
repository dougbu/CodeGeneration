﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.DotNet.Cli.CommandLine;

namespace GetDocument.Commands
{
    internal class ProjectCommandBase : CommandBase
    {
        public override void Configure(CommandLineApplication command)
        {
            new ProjectOptions().Configure(command);

            base.Configure(command);
        }
    }
}