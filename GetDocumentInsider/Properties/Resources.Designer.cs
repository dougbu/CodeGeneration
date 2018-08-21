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
        ///     Show JSON output.
        /// </summary>
        public static string JsonDescription
            => GetString("JsonDescription");

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
        ///     Prefix output with level.
        /// </summary>
        public static string PrefixDescription
            => GetString("PrefixDescription");

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
        ///     Location from which inside man was copied (in the .NET Framework case) or loaded.
        /// </summary>
        public static string ToolsDirectoryDescription
            => GetString("ToolsDirectoryDescription");

        /// <summary>
        ///     The URI to download the document from.
        /// </summary>
        public static string UriDescription
            => GetString("UriDescription");

        /// <summary>
        ///     The name of the method to invoke on the --service instance.
        /// </summary>
        public static string MethodDescription
            => GetString("MethodDescription");

        /// <summary>
        ///     The qualified name of the service to retrieve from dependency injection.
        /// </summary>
        public static string ServiceDescription
            => GetString("ServiceDescription");

        /// <summary>
        ///     Missing required option '--{option1}' or '--{option2}'.
        /// </summary>
        public static string MissingOptions([CanBeNull] object option1, [CanBeNull] object option2)
            => string.Format(
                GetString("MissingOptions", nameof(option1), nameof(option2)),
                option1, option2);

        /// <summary>
        ///     Option '--{extraOption}' conflicts with '--{mainOption}'.
        /// </summary>
        public static string ExtraOption([CanBeNull] object extraOption, [CanBeNull] object mainOption)
            => string.Format(
                GetString("ExtraOption", nameof(extraOption), nameof(mainOption)),
                extraOption, mainOption);

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

