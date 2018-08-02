﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Text;
using GetDocument.Insider.Properties;

namespace GetDocument.Insider.Commands
{
    // ReSharper disable once ArrangeTypeModifiers
    partial class MigrationsScriptCommand
    {
        protected override int Execute()
        {
            var sql = CreateExecutor().ScriptMigration(
                _from.Value,
                _to.Value,
                _idempotent.HasValue(),
                Context.Value());

            if (!_output.HasValue())
            {
                Reporter.WriteData(sql);
            }
            else
            {
                var output = _output.Value();
                if (WorkingDir.HasValue())
                {
                    output = Path.Combine(WorkingDir.Value(), output);
                }

                var directory = Path.GetDirectoryName(output);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                Reporter.WriteVerbose(Resources.WritingFile(_output.Value()));
                File.WriteAllText(output, sql, Encoding.UTF8);
            }

            return base.Execute();
        }
    }
}
