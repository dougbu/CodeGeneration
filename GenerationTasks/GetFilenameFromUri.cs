using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace GenerationTasks
{
    /// <summary>
    /// Converts a URI into a valid Unix or Windows filename.
    /// </summary>
    public class GetFilenameFromUri : Task
    {
        /// <summary>
        /// The URI to convert.
        /// </summary>
        [Required]
        public ITaskItem[] Uris { get; set; }

        /// <summary>
        /// If <see langword="true" />, <see cref="Uris"/> items may include relative URIs. Otherwise, the task will
        /// fail if <see cref="Uris"/> contains anything bug absolute URIs.
        /// </summary>
        /// <value>Defaults to <see langword="false" />.</value>
        public bool AllowRelativeUris { get; set; }

        /// <summary>
        /// Optional hostname to use when generating an absolute URI. Ignored unless <see cref="AllowRelativeUris"/> is
        /// <see langword="true" />.
        /// </summary>
        /// <value>Defaults to "localhost".</value>
        public string Hostname { get; set; } = "localhost";

        /// <summary>
        /// The converted filename.
        /// </summary>
        [Output]
        public ITaskItem[] Filenames{ get; set; }

        /// <inheritdoc />
        public override bool Execute()
        {
            var filenames = new List<ITaskItem>(Uris.Length);
            foreach (var item in Uris)
            {
                var uri = item.ItemSpec;
                UriBuilder builder;
                if (AllowRelativeUris)
                {
                    var absoluteUri = new Uri(new Uri($"http://{Hostname}/"), uri);
                    builder = new UriBuilder(absoluteUri);
                }
                else
                {
                    builder = new UriBuilder(uri);
                    if (!builder.Uri.IsAbsoluteUri)
                    {
                        Log.LogError($"{nameof(Uris)} item '{uri}' is not an absolute URI.");
                        return false;
                    }
                }

                if (!string.Equals(Uri.UriSchemeHttp, builder.Scheme, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(Uri.UriSchemeHttps, builder.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    Log.LogError($"{nameof(Uris)} item '{uri}' does not have scheme {Uri.UriSchemeHttp} or " +
                        $"{Uri.UriSchemeHttps}.");
                    return false;
                }

                var host = builder.Host
                  .Replace("/", string.Empty)
                  .Replace("[", string.Empty)
                  .Replace("]", string.Empty)
                  .Replace(':', '_');
                var path = builder.Path
                  .Replace("!", string.Empty)
                  .Replace("'", string.Empty)
                  .Replace("$", string.Empty)
                  .Replace("%", string.Empty)
                  .Replace("&", string.Empty)
                  .Replace("(", string.Empty)
                  .Replace(")", string.Empty)
                  .Replace("*", string.Empty)
                  .Replace("@", string.Empty)
                  .Replace("~", string.Empty)
                  .Replace('/', '_')
                  .Replace(':', '_')
                  .Replace(';', '_')
                  .Replace('+', '_')
                  .Replace('=', '_');

                var filename = host + path;
                if (char.IsLower(filename[0]))
                {
                    filename = char.ToUpper(filename[0]) + filename.Substring(startIndex: 1);
                }

                if (!filename.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    filename = Path.ChangeExtension(filename, ".json");
                }

                var newItem = new TaskItem(item);
                if (string.IsNullOrEmpty(newItem.GetMetadata("LocalFilename")))
                {
                    newItem.SetMetadata("LocalFilename", filename);
                }

                filenames.Add(newItem);
            }

            Filenames = filenames.ToArray();

            return true;
        }
    }
}
