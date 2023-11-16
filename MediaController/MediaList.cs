using MediaSyncAPI.Utilities;
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
            //MediaPath = 
        }

        public static string HandleListRequest()
        {
            try
            {
                if (Directory.Exists(MediaPath))
                {
                    var audioFiles = FileSystem.ListAudioFiles(MediaPath);

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
