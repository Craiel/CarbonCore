namespace CarbonCore.Utils.Network
{
    using System.Net;
    using System.Net.Sockets;

    public static class NetworkUtils
    {
        public static string GetLocalHostName()
        {
            return Dns.GetHostName();
        }

        public static IPAddress GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(GetLocalHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }

            return null;
        }
    }
}
