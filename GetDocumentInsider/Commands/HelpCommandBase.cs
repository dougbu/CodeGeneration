// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.DotNet.Cli.CommandLine;

namespace GetDocument.Commands
{
    internal class HelpCommandBase : CommandBase
    {
        private CommandLineApplication _command;

        public override void Configure(CommandLineApplication command)
        {
            _command = command;
            command.HelpOption("-h|--help");

            base.Configure(command);
        }
    }
}
