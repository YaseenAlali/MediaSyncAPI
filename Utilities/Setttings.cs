using Newtonsoft.Json;

namespace MediaSyncAPI.Utilities
{
    public static class AppSettings
    {
        public static string MediaRoot { get; set; } = "";
        public static string DownloadedFilesDirectory { get; set; } = "";

        public static bool InitSettings()
        {
            try
            {
                string settingsPath = "./appsettings.json";
                if (File.Exists(settingsPath)) {
                    var settingsString = File.ReadAllText(settingsPath);
                    if (string.IsNullOrEmpty(settingsString))
                    {
                        Console.WriteLine("Settings content empty");
                        return false;
                    }

                    dynamic json = JsonConvert.DeserializeObject(settingsString);
                    if (json != null)
                    {
                        MediaRoot = json.MediaRoot;
                        DownloadedFilesDirectory = json.DownloadedFilesDirectory;
                        return true;
                    }
                    return false;
                }
                else
                {
                    Console.WriteLine("settings file does not exist");
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        
    }

}
