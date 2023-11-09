using MediaSyncAPI.MediaController;
using System.Diagnostics;

namespace MediaSyncAPI.Utilities
{
    public class ProcessHandling
    {
        public static string RunProcess(string args, string directory = "")
        {
            try
            {
                if (string.IsNullOrEmpty(directory))
                {
                    directory = MediaList.MediaPath;
                }
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {args}",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = directory
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();

                    StreamWriter sw = process.StandardInput;
                    sw.WriteLine(args);
                    sw.Close();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    string result = output + error;

                    return result;
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public static string CreateUniqueDirectory()
        {
            try { 
                string uniqueId = Guid.NewGuid().ToString();
                Directory.CreateDirectory($@".\DownloadedFiles\{uniqueId}");
                return uniqueId;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return "";
            }
            
        }
    }
}
