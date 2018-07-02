using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.CommandLineUtils;

namespace BuildTasks
{
    public class GetOpenAPIDocument : ToolTask
    {
        private string _baseAddress;
        private bool _killProcess;
        private bool _toolIsRunning;
        private AutoResetEvent _waitForNext;
        private bool _success;

        public GetOpenAPIDocument()
        {
            // Tool does not use stderr for anything. Treat everything that appears there as an error.
            LogStandardErrorAsError = true;
        }

        /// <summary>
        /// Path to the assembly to consider.
        /// </summary>
        [Required]
        public string AssemblyPath { get; set; }

        [Required]
        public string OpenAPIDocumentPath { get; set; }

        [Required]
        public string OutputPath { get; set; }

        /// <summary>
        /// The framework moniker for <see cref="AssemblyPath"/>.
        /// </summary>
        [Required]
        public string Framework { get; set; }

        /// <inheritdoc />
        protected override MessageImportance StandardErrorLoggingImportance => MessageImportance.High;

        /// <inheritdoc />
        protected override MessageImportance StandardOutputLoggingImportance => MessageImportance.High;

        /// <inheritdoc />
        protected override string ToolName
        {
            get
            {
                if (Framework.StartsWith("net4", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(".exe", Path.GetExtension(AssemblyPath), StringComparison.OrdinalIgnoreCase))
                {
                    return AssemblyPath;
                }

                var exeExtension = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : string.Empty;
                return "dotnet" + exeExtension;
            }
        }

        /// <inheritdoc />
        protected override bool ValidateParameters()
        {
            if (string.IsNullOrEmpty(AssemblyPath) || !File.Exists(AssemblyPath))
            {
                Log.LogError($"Assembly '{AssemblyPath}' not specified or does not exist.");
                return false;
            }

            if (string.IsNullOrEmpty(Framework))
            {
                Log.LogError("Framework moniker must be specified.");
                return false;
            }

            return base.ValidateParameters();
        }

        /// <inheritdoc />
        protected override string GenerateFullPathToTool()
        {
            if (Framework.StartsWith("net4", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(".exe", Path.GetExtension(AssemblyPath), StringComparison.OrdinalIgnoreCase))
            {
                // Execute the given command directly.
                return Path.GetFullPath(AssemblyPath);
            }

            // Otherwise, run `dotnet` but prefer its full path.
            // If muxer does not find dotnet, fall back to system PATH and hope for the best.
            return DotNetMuxer.MuxerPath ?? ToolExe;
        }

        /// <inheritdoc />
        protected override string GenerateCommandLineCommands()
        {
            var fullCommand = string.Empty;
            if (!Framework.StartsWith("net4", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(".exe", Path.GetExtension(AssemblyPath), StringComparison.OrdinalIgnoreCase))
            {
                // Using `dotnet` and therefore need to specify the assembly path.
                fullCommand = $@" ""{Path.GetFullPath(AssemblyPath)}""";
            }

            return fullCommand;
        }

        /// <inheritdoc />
        protected override int ExecuteTool(string pathToTool, string responseFileCommands, string commandLineCommands)
        {
            using (var process = new Process
                {
                    EnableRaisingEvents = true,
                    StartInfo = GetProcessStartInfo(pathToTool, commandLineCommands, responseFileSwitch: null),
                })
            {
                int exitCode;
                try
                {
                    process.ErrorDataReceived += Process_ErrorDataReceived;
                    process.Exited += Process_Exited;
                    process.OutputDataReceived += Process_OutputDataReceived;

                    process.Start();

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        // Close the stdin stream
                        process.StandardInput.Dispose();
                    }

                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();

                    _toolIsRunning = true;
                    using (_waitForNext = new AutoResetEvent(initialState: false))
                    {
                        while (_toolIsRunning)
                        {
                            // Handle state changes as they're signaled.
                            _waitForNext.WaitOne();

                            if (_killProcess)
                            {
                                process.Kill();
                            }
                        }
                    }

                    // cleanup
                    process.WaitForExit();
                }
                finally
                {
                    exitCode = process.ExitCode;
                }

                if (!_success && exitCode == 0)
                {
                    exitCode = 1;
                }

                return exitCode;
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            _toolIsRunning = false;
            _waitForNext.Set();
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _killProcess = true;
            Log.LogError(message: e.Data);
            _waitForNext.Set();
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            var data = e.Data;
            if (data.StartsWith("Now listening on:", StringComparison.OrdinalIgnoreCase))
            {
                var index = data.LastIndexOf(' ');
                _baseAddress = data.Substring(startIndex: index + 1);
                _waitForNext.Set();
                return;
            }

            if (data.StartsWith("Application started.", StringComparison.OrdinalIgnoreCase))
            {
                var uri = _baseAddress == null ?
                    new Uri(OpenAPIDocumentPath) :
                    new Uri(new Uri(_baseAddress), OpenAPIDocumentPath);
                using (var client = new HttpClient(new RedirectHandler()))
                {
                    var response = client.GetAsync(uri).Result;
                    if (!response.IsSuccessStatusCode && _baseAddress == null)
                    {
                        uri = new Uri(new Uri("http://localhost:5000"), OpenAPIDocumentPath);
                        response = client.GetAsync(uri).Result;
                    }

                    response.EnsureSuccessStatusCode();

                    using (var responseStream = response.Content.ReadAsStreamAsync().Result)
                    {
                        using (var outputStream = new FileStream(OutputPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                        {
                            responseStream.CopyTo(outputStream);
                        }
                    }
                }

                _killProcess = true;
                _success = true;
                _waitForNext.Set();
            }
        }
    }
}
