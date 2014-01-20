using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace NetworkManager
{
    public static class Helpers
    {
        public const string BadIP = "0.0.0.0";
        public static IPAddress GetIP(NetworkInterface iface)
        {
            if (iface == null)
                return IPAddress.Parse(BadIP);

            IPInterfaceProperties ipProperties = iface.GetIPProperties();

            foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork &&
                    (ip.PrefixOrigin != PrefixOrigin.WellKnown && ip.SuffixOrigin != SuffixOrigin.LinkLayerAddress))
                    return ip.Address;

            return IPAddress.Parse(BadIP);
        }

        public static IPAddress GetSubnetMask(NetworkInterface iface)
        {
            IPInterfaceProperties ipProperties = iface.GetIPProperties();

            foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork &&
                    (ip.PrefixOrigin == PrefixOrigin.Manual || ip.SuffixOrigin == SuffixOrigin.Manual))
                    return ip.IPv4Mask;

            return IPAddress.Parse(BadIP);
        }

        public static IPAddress GetGatewayAddress(NetworkInterface iface)
        {
            IPInterfaceProperties ipProperties = iface.GetIPProperties();

            foreach (GatewayIPAddressInformation ip in ipProperties.GatewayAddresses)
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    return ip.Address;

            return IPAddress.Parse(BadIP);
        }

        public static IPAddress GetDNSAddress(NetworkInterface iface)
        {
            IPInterfaceProperties ipProperties = iface.GetIPProperties();

            foreach (IPAddress ip in ipProperties.DnsAddresses)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;

            return IPAddress.Parse(BadIP);
        }

        public static IPAddress GetDhcpAddress(NetworkInterface iface)
        {
            return IPAddress.Parse(BadIP);
        }

        public static string BytesToString(byte[] bytes, int size = -1)
        {
            if (size == -1)
                size = bytes.Count();

            string str = System.Text.Encoding.Default.GetString(bytes, 0, size);
            return str;
        }
    }
}
