﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace GetDocument.Insider.Commands
{
    // ReSharper disable once ArrangeTypeModifiers
    partial class DatabaseUpdateCommand
    {
        protected override int Execute()
        {
            CreateExecutor().UpdateDatabase(_migration.Value, Context.Value());

            return base.Execute();
        }
    }
}
