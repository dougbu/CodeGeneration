﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NET461

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using GetDocument.Design;
using GetDocument.Properties;

namespace GetDocument
{
    internal class AppDomainOperationExecutor : OperationExecutorBase
    {
        private readonly object _executor;
        private readonly AppDomain _domain;
        private bool _disposed;

        public AppDomainOperationExecutor(
            string assembly,
            string projectDir,
            string dataDirectory,
            string rootNamespace,
            string language)
            : base(assembly, projectDir, dataDirectory, rootNamespace, language)
        {
            var info = new AppDomainSetup { ApplicationBase = AppBasePath };

            var configurationFile = assembly + ".config";
            if (File.Exists(configurationFile))
            {
                Reporter.WriteVerbose(Resources.UsingConfigurationFile(configurationFile));
                info.ConfigurationFile = configurationFile;
            }

            _domain = AppDomain.CreateDomain("GetDocument.Insider.DesignDomain", null, info);

            if (dataDirectory != null)
            {
                Reporter.WriteVerbose(Resources.UsingDataDir(dataDirectory));
                _domain.SetData("DataDirectory", dataDirectory);
            }

            var reportHandler = new OperationReportHandler(
                Reporter.WriteError,
                Reporter.WriteWarning,
                Reporter.WriteInformation,
                Reporter.WriteVerbose);

            _executor = _domain.CreateInstanceAndUnwrap(
                DesignAssemblyName,
                ExecutorTypeName,
                false,
                BindingFlags.Default,
                null,
                new object[]
                {
                    reportHandler,
                    new Hashtable
                    {
                        { "targetName", AssemblyFileName },
                        { "projectDir", ProjectDirectory },
                        { "rootNamespace", RootNamespace },
                        { "language", Language },
                        { "toolsVersion", ProductInfo.GetVersion() }
                    }
                },
                null,
                null);
        }

        protected override object CreateResultHandler()
            => new OperationResultHandler();

        protected override void Execute(string operationName, object resultHandler, IDictionary arguments)
            => _domain.CreateInstance(
                DesignAssemblyName,
                ExecutorTypeName + "+" + operationName,
                false,
                BindingFlags.Default,
                null,
                new[] { _executor, resultHandler, arguments },
                null,
                null);

        public override void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                AppDomain.Unload(_domain);
            }
        }
    }
}
#elif NETCOREAPP2_0
#else
#error target frameworks need to be updated.
#endif
