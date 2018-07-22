// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Task = System.Threading.Tasks.Task;
using Utilities = Microsoft.Build.Utilities;

namespace BuildTasks
{
    /// <summary>
    /// Downloads a file.
    /// </summary>
    public class DownloadFile : Utilities.Task, ICancelableTask
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        /// <summary>
        /// The URI to download.
        /// </summary>
        [Required]
        public string Uri { get; set; }

        /// <summary>
        /// Destination for the downloaded file. If the file already exists, it is not re-downloaded unless
        /// <see cref="Overwrite"/> is true.
        /// </summary>
        [Required]
        public string DestinationPath { get; set; }

        /// <summary>
        /// Should <see cref="DestinationPath"/> be overwritten. When <c>true</c>, the file is downloaded and its hash
        /// compared to the existing file. If those hashes do not match (or <see cref="DestinationPath"/> does not
        /// exist), <see cref="DestinationPath"/> is overwritten.
        /// </summary>
        public bool Overwrite { get; set; }

        /// <summary>
        /// The maximum amount of time in seconds to allow for downloading the file. Defaults to 2 minutes.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 60 * 2;

        /// <inheritdoc/>
        public void Cancel() => _cts.Cancel();

        /// <inheritdoc/>
        public override bool Execute() => ExecuteAsync().Result;

        public async Task<bool> ExecuteAsync()
        {
            if (string.IsNullOrEmpty(Uri))
            {
                Log.LogError("Uri parameter must not be null or empty.");
                return false;
            }

            if (string.IsNullOrEmpty(Uri))
            {
                Log.LogError("DestinationPath parameter must not be null or empty.");
                return false;
            }

            var builder = new UriBuilder(Uri);
            if (!string.Equals(System.Uri.UriSchemeHttp, builder.Scheme, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(System.Uri.UriSchemeHttps, builder.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                Log.LogError($"{nameof(Uri)} parameter does not have scheme {System.Uri.UriSchemeHttp} or " +
                    $"{System.Uri.UriSchemeHttps}.");
                return false;
            }

            await DownloadFileAsync(Uri, DestinationPath, Overwrite,  _cts.Token, TimeoutSeconds, Log);

            return !Log.HasLoggedErrors;
        }

        private static async Task DownloadFileAsync(
            string uri,
            string destinationPath,
            bool overwrite,
            CancellationToken cancellationToken,
            int timeoutSeconds,
            TaskLoggingHelper log)
        {
            var destinationExists = File.Exists(destinationPath);
            if (destinationExists && !overwrite)
            {
                log.LogMessage($"Not downloading '{uri}' to overwrite existing file '{destinationPath}'.");
                return;
            }

            log.LogMessage($"Downloading '{uri}' to '{destinationPath}'.");

            using (var httpClient = new HttpClient
            {
                // Timeout if no response starts in 1 minute
                Timeout = TimeSpan.FromMinutes(1),
            })
            {
                var reachedCopy = false;
                try
                {
                    using (var response = await httpClient.GetAsync(uri, cancellationToken))
                    {
                        response.EnsureSuccessStatusCode();
                        cancellationToken.ThrowIfCancellationRequested();

                        using (var responseStreamTask = response.Content.ReadAsStreamAsync())
                        {
                            var finished = await Task.WhenAny(
                                responseStreamTask,
                                Task.Delay(TimeSpan.FromSeconds(timeoutSeconds)));

                            if (!ReferenceEquals(responseStreamTask, finished))
                            {
                                throw new TimeoutException($"Download failed to complete in {timeoutSeconds} seconds.");
                            }

                            var responseStream = await responseStreamTask;
                            if (destinationExists)
                            {
                                // Check hashes before using the downloaded information.
                                var downloadHash = GetHash(responseStream);
                                responseStream.Position = 0L;

                                byte[] destinationHash;
                                using (var destinationStream = File.OpenRead(destinationPath))
                                {
                                    destinationHash = GetHash(destinationStream);
                                }

                                var sameHashes = downloadHash.LongLength == destinationHash.LongLength;
                                for (var i = 0L; sameHashes && i < downloadHash.LongLength; i++)
                                {
                                    sameHashes = downloadHash[i] == destinationHash[i];
                                }

                                if (sameHashes)
                                {
                                    log.LogMessage($"Not overwriting existing and matching file '{destinationPath}'.");
                                    return;
                                }
                            }
                            else
                            {
                                // May need to create directory to hold the file.
                                var destinationDirectory = Path.GetDirectoryName(destinationPath);
                                if (!(string.IsNullOrEmpty(destinationDirectory) || Directory.Exists(destinationDirectory)))
                                {
                                    Directory.CreateDirectory(destinationDirectory);
                                }
                            }

                            // Create or overwrite the destination file.
                            reachedCopy = true;
                            using (var outStream = File.Create(destinationPath))
                            {
                                responseStream.CopyTo(outStream);
                            }
                        }
                    }
                }
                catch (HttpRequestException ex) when (destinationExists)
                {
                    if (ex.InnerException is SocketException socketException)
                    {
                        log.LogWarning($"Unable to download {uri}, socket error code '{socketException.SocketErrorCode}'.");
                    }
                    else
                    {
                        log.LogWarning($"Unable to download {uri}: {ex.Message}");
                    }

                }
                catch (Exception ex)
                {
                    log.LogError($"Downloading '{uri}' failed.");
                    log.LogErrorFromException(ex, showStackTrace: true);
                    if (reachedCopy)
                    {
                        File.Delete(destinationPath);
                    }
                }
            }
        }

        private static byte[] GetHash(Stream stream)
        {
            using (var algorithm = MD5.Create())
            {
                return algorithm.ComputeHash(stream);
            }
        }
    }
}
