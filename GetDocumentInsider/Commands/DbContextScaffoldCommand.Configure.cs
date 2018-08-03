﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using GetDocument.Properties;
using Microsoft.DotNet.Cli.CommandLine;

namespace GetDocument.Commands
{
    internal partial class DbContextScaffoldCommand : ProjectCommandBase
    {
        private CommandArgument _connection;
        private CommandArgument _provider;
        private CommandOption _dataAnnotations;
        private CommandOption _context;
        private CommandOption _contextDir;
        private CommandOption _force;
        private CommandOption _outputDir;
        private CommandOption _schemas;
        private CommandOption _tables;
        private CommandOption _useDatabaseNames;
        private CommandOption _json;

        public override void Configure(CommandLineApplication command)
        {
            command.Description = Resources.DbContextScaffoldDescription;

            _connection = command.Argument("<CONNECTION>", Resources.ConnectionDescription);
            _provider = command.Argument("<PROVIDER>", Resources.ProviderDescription);

            _dataAnnotations = command.Option("-d|--data-annotations", Resources.DataAnnotationsDescription);
            _context = command.Option("-c|--context <NAME>", Resources.ContextNameDescription);
            _contextDir = command.Option("--context-dir <PATH>", Resources.ContextDirDescription);
            _force = command.Option("-f|--force", Resources.DbContextScaffoldForceDescription);
            _outputDir = command.Option("-o|--output-dir <PATH>", Resources.OutputDirDescription);
            _schemas = command.Option("--schema <SCHEMA_NAME>...", Resources.SchemasDescription);
            _tables = command.Option("-t|--table <TABLE_NAME>...", Resources.TablesDescription);
            _useDatabaseNames = command.Option("--use-database-names", Resources.UseDatabaseNamesDescription);
            _json = Json.ConfigureOption(command);

            base.Configure(command);
        }
    }
}
