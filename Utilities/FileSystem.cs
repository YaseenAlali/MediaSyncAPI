namespace MediaSyncAPI.Utilities
{
    public class FileSystem
    {
        /// <summary>
        /// Retrieves the path of the first audio file in the specified directory.
        /// </summary>
        /// <param name="path">The path of the directory to search for audio files.</param>
        /// <returns>
        /// The full path of the first audio file found, or an empty string if no audio files are present.
        /// </returns>
        /// <remarks>
        /// Supported audio file extensions: mp3, wav, flac, opus, ogg, aac.
        /// </remarks>
        public static string GetFirstAudioFile(string path)
        {
            try
            {
                string[] audioExtensions = { "mp3", ".wav", ".flac", "opus", ".ogg", ".aac" };

                var audioFiles = Directory.GetFiles(path)
                    .Where(file => audioExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                return audioFiles.Count > 0 ? audioFiles.First() : "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting audio file: {ex.Message}");
                return "";
            }
        }


        /// <summary>
        /// Gets the total count of items (files and subdirectories) in the specified directory and its subdirectories.
        /// </summary>
        /// <param name="path">The path of the directory to get the item count for.</param>
        /// <returns>
        /// The total count of items in the directory and its subdirectories, or -1 if the directory doesn't exist.
        /// </returns>
        public static int GetDirectoryItemsCount(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    string[] allFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                    string[] allDirectories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);

                    int itemCount = allFiles.Length + allDirectories.Length;

                    return itemCount;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting directory items count: {ex.Message}");
                return -1;
            }
        }


        /// <summary>
        /// Deletes all files and subdirectories within the specified directory.
        /// </summary>
        /// <param name="path">The path of the directory to delete files and subdirectories from.</param>
        /// <returns>
        /// True if the operation succeeds; otherwise, false.
        /// </returns>
        public static bool DeleteFilesAndSubdirectories(string path)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }

                foreach (DirectoryInfo subdirectory in directoryInfo.GetDirectories())
                {
                    subdirectory.Delete(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting files and subdirectories: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Lists all audio files within the specified directory and its subdirectories.
        /// </summary>
        /// <param name="path">The path of the directory to list audio files from.</param>
        /// <returns>
        /// An array of strings representing the paths of the discovered audio files.
        /// </returns>
        /// <remarks>
        /// Supported audio file extensions: mp3, wav, flac, opus, ogg, aac.
        /// </remarks>
        public static string[] ListAudioFiles(string path)
        {
            try
            {
                string[] mediaExtensions = { "mp3", ".wav", ".flac", "opus", ".ogg", ".aac" };

                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(file => mediaExtensions.Contains(Path.GetExtension(file).ToLower()))
                    .Select(file => Path.Combine(path, file.Replace(path, "")))
                    .Select(path => path.Replace(Path.DirectorySeparatorChar, '/'))
                    .ToArray();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new string[0];
            }
        }


        /// <summary>
        /// Creates a unique directory within the 'DownloadedFiles' directory and returns its unique identifier.
        /// </summary>
        /// <returns>
        /// A unique identifier for the newly created directory, or an empty string if creation fails.
        /// </returns>
        public static string CreateUniqueDirectory()
        {
            try
            {
                string uniqueId = Guid.NewGuid().ToString();
                Directory.CreateDirectory($@".\DownloadedFiles\{uniqueId}");
                return uniqueId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "";
            }
        }


        /// <summary>
        /// Creates the 'DownloadedFiles' directory if it does not already exist.
        /// </summary>
        /// <returns>
        /// True if the directory already exists or is successfully created; otherwise, false.
        /// </returns>
        public static bool CreateDownloadedFilesDirectory()
        {
            try
            {
                Directory.CreateDirectory("DownloadedFiles");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        /// <summary>
        /// Checks if the 'DownloadedFiles' directory exists and creates it if not.
        /// </summary>
        /// <returns>
        /// True if the directory already exists or is successfully created; otherwise, false.
        /// </returns>
        public static bool CheckDownloadedFilesDirectoryAndCreateIfNotExisting()
        {
            try
            {
                if (Directory.Exists("DownloadedFiles"))
                {
                    return true;
                }
                else
                {
                    bool created = CreateDownloadedFilesDirectory();
                    return created;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
