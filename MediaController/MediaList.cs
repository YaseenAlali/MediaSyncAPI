using Newtonsoft.Json;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaSyncAPI.MediaController
{
    public class MediaList
    {
        public static string MediaPath = "";


        public static void APIEntry()
        {
            MediaPath = "D:\\stuff\\MusicNew";
        }

        public static string APIList()
        {
            try
            {
                string[] mediaExtensions =
                {
                    "mp3", ".wav", ".flac", "opus", ".ogg", ".aac"
                };


                if (Directory.Exists(MediaPath))
                {
                    var audioFiles = Directory.GetFiles(MediaPath, "*.*", SearchOption.AllDirectories)
                    .Where(file => mediaExtensions.Contains(Path.GetExtension(file).ToLower()))
                    .Select(file => Path.Combine(MediaPath, file.Replace(MediaPath, "")))
                    .Select(path => path.Replace(Path.DirectorySeparatorChar, '/')) 
                    .ToArray();

                    var result = new { AudioFiles = audioFiles };

                    var jsonSettings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        StringEscapeHandling = StringEscapeHandling.Default
                    };

                    string jsonResult = JsonConvert.SerializeObject(result, jsonSettings);

                    return jsonResult;
                }
                else
                {
                    Console.WriteLine("Path doesn't exst");
                    return "";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
