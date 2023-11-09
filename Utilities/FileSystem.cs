namespace MediaSyncAPI.Utilities
{
    public class FileSystem
    {
        public static string GetFirstAudioFile(string path)
        {
            try
            {
                string[] audioExtensions = { "mp3", ".wav", ".flac", "opus", ".ogg", ".aac" };

                var audioFiles = Directory.GetFiles(path)
                    .Where(file => audioExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (audioFiles.Count > 0)
                {
                    return audioFiles.First();
                }
                else
                {
                    return "";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting audio file: {ex.Message}");
                return "";
            }
        }

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
    }
}
