﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using GetDocument.Properties;
using Microsoft.DotNet.Cli.CommandLine;

namespace GetDocument.Commands
{
    internal class ContextCommandBase : ProjectCommandBase
    {
        protected CommandOption Context { get; private set; }

        public override void Configure(CommandLineApplication command)
        {
            Context = command.Option("-c|--context <DBCONTEXT>", Resources.ContextDescription);

            base.Configure(command);
        }
    }
}
