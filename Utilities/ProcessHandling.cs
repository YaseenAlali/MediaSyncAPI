using MediaSyncAPI.MediaController;
using System.Diagnostics;

namespace MediaSyncAPI.Utilities
{
    public class ProcessHandling
    {
        /// <summary>
        /// Executes a command-line process with the specified arguments in the given directory.
        /// </summary>
        /// <param name="args">The command-line arguments to be passed to the process.</param>
        /// <param name="directory">The working directory for the process. Defaults to the MediaPath from MediaList if not provided.</param>
        /// <returns>
        /// A string containing the combined standard output and standard error of the executed process,
        /// or an error message in case of an exception.
        /// </returns>
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
    }
}
