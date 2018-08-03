// <auto-generated />

using System;
using System.Reflection;
using System.Resources;
using JetBrains.Annotations;

namespace GetDocument.Properties
{
    /// <summary>
    ///		This API supports the GetDocument infrastructure and is not intended to be used
    ///   directly from your code. This API may change or be removed in future releases.
    /// </summary>
    internal static class Resources
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("GetDocument.Properties.Resources", typeof(Resources).GetTypeInfo().Assembly);

        /// <summary>
        ///     The assembly to use.
        /// </summary>
        public static string AssemblyDescription
            => GetString("AssemblyDescription");

        /// <summary>
        ///     The data directory.
        /// </summary>
        public static string DataDirDescription
            => GetString("DataDirDescription");

        /// <summary>
        ///     GetDocument Command-line Tool inside man
        /// </summary>
        public static string FullName
            => GetString("FullName");

        /// <summary>
        ///     Show JSON output.
        /// </summary>
        public static string JsonDescription
            => GetString("JsonDescription");

        /// <summary>
        ///     Missing required argument '{arg}'.
        /// </summary>
        public static string MissingArgument([CanBeNull] object arg)
            => string.Format(
                GetString("MissingArgument", nameof(arg)),
                arg);

        /// <summary>
        ///     Missing required option '--{option}'.
        /// </summary>
        public static string MissingOption([CanBeNull] object option)
            => string.Format(
                GetString("MissingOption", nameof(option)),
                option);

        /// <summary>
        ///     Don't colorize output.
        /// </summary>
        public static string NoColorDescription
            => GetString("NoColorDescription");

        /// <summary>
        ///     The file to write the result to.
        /// </summary>
        public static string OutputDescription
            => GetString("OutputDescription");

        /// <summary>
        ///     The directory to put files in. Paths are relative to the project directory.
        /// </summary>
        public static string OutputDirDescription
            => GetString("OutputDirDescription");

        /// <summary>
        ///     Prefix output with level.
        /// </summary>
        public static string PrefixDescription
            => GetString("PrefixDescription");

        /// <summary>
        ///     The project directory. Defaults to the current directory.
        /// </summary>
        public static string ProjectDirDescription
            => GetString("ProjectDirDescription");

        /// <summary>
        ///     The root namespace. Defaults to the target assembly name.
        /// </summary>
        public static string RootNamespaceDescription
            => GetString("RootNamespaceDescription");

        /// <summary>
        ///     The startup assembly to use. Defaults to the target assembly.
        /// </summary>
        public static string StartupAssemblyDescription
            => GetString("StartupAssemblyDescription");

        /// <summary>
        ///     Using application base '{appBase}'.
        /// </summary>
        public static string UsingApplicationBase([CanBeNull] object appBase)
            => string.Format(
                GetString("UsingApplicationBase", nameof(appBase)),
                appBase);

        /// <summary>
        ///     Using assembly '{assembly}'.
        /// </summary>
        public static string UsingAssembly([CanBeNull] object assembly)
            => string.Format(
                GetString("UsingAssembly", nameof(assembly)),
                assembly);

        /// <summary>
        ///     Using configuration file '{config}'.
        /// </summary>
        public static string UsingConfigurationFile([CanBeNull] object config)
            => string.Format(
                GetString("UsingConfigurationFile", nameof(config)),
                config);

        /// <summary>
        ///     Using data directory '{dataDir}'.
        /// </summary>
        public static string UsingDataDir([CanBeNull] object dataDir)
            => string.Format(
                GetString("UsingDataDir", nameof(dataDir)),
                dataDir);

        /// <summary>
        ///     Using project directory '{projectDir}'.
        /// </summary>
        public static string UsingProjectDir([CanBeNull] object projectDir)
            => string.Format(
                GetString("UsingProjectDir", nameof(projectDir)),
                projectDir);

        /// <summary>
        ///     Using root namespace '{rootNamespace}'.
        /// </summary>
        public static string UsingRootNamespace([CanBeNull] object rootNamespace)
            => string.Format(
                GetString("UsingRootNamespace", nameof(rootNamespace)),
                rootNamespace);

        /// <summary>
        ///     Using startup assembly '{startupAssembly}'.
        /// </summary>
        public static string UsingStartupAssembly([CanBeNull] object startupAssembly)
            => string.Format(
                GetString("UsingStartupAssembly", nameof(startupAssembly)),
                startupAssembly);

        /// <summary>
        ///     Show verbose output.
        /// </summary>
        public static string VerboseDescription
            => GetString("VerboseDescription");

        /// <summary>
        ///     Writing '{file}'...
        /// </summary>
        public static string WritingFile([CanBeNull] object file)
            => string.Format(
                GetString("WritingFile", nameof(file)),
                file);

        /// <summary>
        ///     Using working directory '{workingDirectory}'.
        /// </summary>
        public static string UsingWorkingDirectory([CanBeNull] object workingDirectory)
            => string.Format(
                GetString("UsingWorkingDirectory", nameof(workingDirectory)),
                workingDirectory);

        /// <summary>
        ///     Your startup project '{startupProject}' doesn't reference Microsoft.AspNetCore.TestHost. This package is required for the GetDocument Command-Line Tool to work. Ensure your startup project is correct, install the package, and try again.
        /// </summary>
        public static string DesignNotFound([CanBeNull] object startupProject)
            => string.Format(
                GetString("DesignNotFound", nameof(startupProject)),
                startupProject);

        /// <summary>
        ///     Provider name: {provider}
        /// </summary>
        public static string ProviderName([CanBeNull] object provider)
            => string.Format(
                GetString("ProviderName", nameof(provider)),
                provider);

        /// <summary>
        ///     Options: {options}
        /// </summary>
        public static string Options([CanBeNull] object options)
            => string.Format(
                GetString("Options", nameof(options)),
                options);

        /// <summary>
        ///     The language. Defaults to 'C#'.
        /// </summary>
        public static string LanguageDescription
            => GetString("LanguageDescription");

        /// <summary>
        ///     The working directory of the tool invoking this command.
        /// </summary>
        public static string WorkingDirDescription
            => GetString("WorkingDirDescription");

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);
            for (var i = 0; i < formatterNames.Length; i++)
            {
                value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
            }

            return value;
        }
    }
}

