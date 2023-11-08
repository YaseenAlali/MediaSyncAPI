using System.Net.Sockets;
using System.Net;

namespace MediaSyncAPI.Utilities
{
    public class Networking
    {
        public static string GenerateUrl(string port = "5000")
        {
            string localIP = GetLocalIpAddress();
            if (string.IsNullOrEmpty(localIP) )
            {
                return "";
            }
            return $"http://{localIP}:{port}";
        }

        public static string GetLocalIpAddress()
        {
            try { 
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    return endPoint.Address.ToString();
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return "";
            }
        }

    }
}
