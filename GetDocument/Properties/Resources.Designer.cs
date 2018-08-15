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
        ///     Build failed.
        /// </summary>
        public static string BuildFailed
            => GetString("BuildFailed");

        /// <summary>
        ///     The configuration to use.
        /// </summary>
        public static string ConfigurationDescription
            => GetString("ConfigurationDescription");

        /// <summary>
        ///     dotnet getdocument
        /// </summary>
        public static string CommandFullName
            => GetString("CommandFullName");

        /// <summary>
        ///     The target framework.
        /// </summary>
        public static string FrameworkDescription
            => GetString("FrameworkDescription");

        /// <summary>
        ///     Unable to retrieve project metadata. Ensure it is a MSBuild-based .NET Core project. If you are using custom BaseIntermediateOutputPath or MSBuildProjectExtensionsPath values, use the --msbuildprojectextensionspath option.
        /// </summary>
        public static string GetMetadataFailed
            => GetString("GetMetadataFailed");

        /// <summary>
        ///     More than one project was found in the current working directory. Use the --project option.
        /// </summary>
        public static string MultipleProjects
            => GetString("MultipleProjects");

        /// <summary>
        ///     More than one project was found in directory '{projectDirectory}'. Specify one using its file name.
        /// </summary>
        public static string MultipleProjectsInDirectory([CanBeNull] object projectDirectory)
            => string.Format(
                GetString("MultipleProjectsInDirectory", nameof(projectDirectory)),
                projectDirectory);

        /// <summary>
        ///     Project '{Project}' targets framework '.NETCoreApp' version '{targetFrameworkVersion}'. This version of the GetDocument Command-line Tool only supports version 2.0 or higher.
        /// </summary>
        public static string NETCoreApp1Project([CanBeNull] object Project, [CanBeNull] object targetFrameworkVersion)
            => string.Format(
                GetString("NETCoreApp1Project", nameof(Project), nameof(targetFrameworkVersion)),
                Project, targetFrameworkVersion);

        /// <summary>
        ///     Project '{Project}' targets framework '.NETStandard'. There is no runtime associated with this framework, and projects targeting it cannot be executed directly. To use the GetDocument Command-line Tool with this project, add an executable project targeting .NET Core or .NET Framework that references this project and specify it using the --project option; or, update this project to target .NET Core and / or .NET Framework.
        /// </summary>
        public static string NETStandardProject([CanBeNull] object Project)
            => string.Format(
                GetString("NETStandardProject", nameof(Project)),
                Project);

        /// <summary>
        ///     Do not colorize output.
        /// </summary>
        public static string NoColorDescription
            => GetString("NoColorDescription");

        /// <summary>
        ///     No project was found. Change the current working directory or use the --project option.
        /// </summary>
        public static string NoProject
            => GetString("NoProject");

        /// <summary>
        ///     No project was found in directory '{projectDirectory}'.
        /// </summary>
        public static string NoProjectInDirectory([CanBeNull] object projectDirectory)
            => string.Format(
                GetString("NoProjectInDirectory", nameof(projectDirectory)),
                projectDirectory);

        /// <summary>
        ///     Prefix output with level.
        /// </summary>
        public static string PrefixDescription
            => GetString("PrefixDescription");

        /// <summary>
        ///     The project to use.
        /// </summary>
        public static string ProjectDescription
            => GetString("ProjectDescription");

        /// <summary>
        ///     The MSBuild project extensions path. Defaults to "obj".
        /// </summary>
        public static string ProjectExtensionsDescription
            => GetString("ProjectExtensionsDescription");

        /// <summary>
        ///     The runtime to use.
        /// </summary>
        public static string RuntimeDescription
            => GetString("RuntimeDescription");

        /// <summary>
        ///     Project '{Project}' targets framework '{targetFramework}'. The GetDocument Command-line Tool does not support this framework.
        /// </summary>
        public static string UnsupportedFramework([CanBeNull] object Project, [CanBeNull] object targetFramework)
            => string.Format(
                GetString("UnsupportedFramework", nameof(Project), nameof(targetFramework)),
                Project, targetFramework);

        /// <summary>
        ///     Using project '{project}'.
        /// </summary>
        public static string UsingProject([CanBeNull] object project)
            => string.Format(
                GetString("UsingProject", nameof(project)),
                project);

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
        ///     Do not build the project. Only use this when the build is up-to-date.
        /// </summary>
        public static string NoBuildDescription
            => GetString("NoBuildDescription");

        /// <summary>
        ///     Project output not found and --no-build was specified. Project must be up-to-date when using the --no-build option.
        /// </summary>
        public static string MustBuild
            => GetString("MustBuild");

        /// <summary>
        ///     Project output was not found after a succesful build. Confirm `OutputPath` property of the project is correct.
        /// </summary>
        public static string ProjectMisconfiguration
            => GetString("ProjectMisconfiguration");

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

