// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace GetDocument.Commands
{
    internal class GetDocumentCommand : ProjectCommandBase
    {
        protected override int Execute()
        {
            using (var testServer = new TestServer(new WebHostBuilder()))
            {
                Reporter.WriteInformation("Hello World");
            }

            return base.Execute();
        }
    }
}
