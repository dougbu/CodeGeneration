using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace BuildTasks
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
                var builder = new UriBuilder(uri);
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
