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
    public static class NetworkInterfaceComparer
    {


        public enum UpdateType
        {
            Name,
            IP,
            Netmask,
            Gateway,
            DHCP,
            DNS,
            Other,
            None
        }





        public static UpdateType GetUpdateType(NetworkInterface oldInterface, NetworkInterface newInterface)
        {
            if (oldInterface.Id != newInterface.Id)
                return UpdateType.Name;

            IPInterfaceProperties oldIPProps = oldInterface.GetIPProperties();
            IPInterfaceProperties newIPProps = newInterface.GetIPProperties();

            if (HasIPChanged(oldIPProps, newIPProps))
                return UpdateType.IP;

            if (HasNetmaskChanged(oldIPProps, newIPProps))
                return UpdateType.Netmask;

            if (HasGatewayChanged(oldIPProps, newIPProps))
                return UpdateType.Gateway;

            if (HasDHCPChanged(oldIPProps, newIPProps))
                return UpdateType.DHCP;

            if (HasDNSChanged(oldIPProps, newIPProps))
                return UpdateType.DNS;

            if (!oldIPProps.Equals(newIPProps))
                return UpdateType.Other;

            return UpdateType.None;
        }





        private static bool HasIPChanged(IPInterfaceProperties oldProperties, IPInterfaceProperties newProperties)
        {
            UnicastIPAddressInformation oldIP = null, newIP = null;

            if (oldProperties.UnicastAddresses.Count != newProperties.UnicastAddresses.Count)
                return true;

            foreach (UnicastIPAddressInformation ip in oldProperties.UnicastAddresses)
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    oldIP = ip;
                    break;
                }

            foreach (UnicastIPAddressInformation ip in newProperties.UnicastAddresses)
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    newIP = ip;
                    break;
                }

            return oldIP != null && newIP != null && !oldIP.Address.Equals(newIP.Address);
        }





        private static bool HasNetmaskChanged(IPInterfaceProperties oldProperties, IPInterfaceProperties newProperties)
        {
            UnicastIPAddressInformation oldIP = null, newIP = null;

            if (oldProperties.UnicastAddresses.Count != newProperties.UnicastAddresses.Count)
                return true;

            foreach (UnicastIPAddressInformation ip in oldProperties.UnicastAddresses)
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    oldIP = ip;
                    break;
                }

            foreach (UnicastIPAddressInformation ip in newProperties.UnicastAddresses)
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    newIP = ip;
                    break;
                }

            return oldIP != null && newIP != null && !oldIP.IPv4Mask.Equals(newIP.IPv4Mask);
        }





        private static bool HasGatewayChanged(IPInterfaceProperties oldProperties, IPInterfaceProperties newProperties)
        {
            GatewayIPAddressInformation oldGW = null, newGW = null;

            if (oldProperties.GatewayAddresses.Count != newProperties.GatewayAddresses.Count)
                return true;

            foreach (GatewayIPAddressInformation gw in oldProperties.GatewayAddresses)
                if (gw.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    oldGW = gw;
                    break;
                }

            foreach (GatewayIPAddressInformation gw in newProperties.GatewayAddresses)
                if (gw.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    newGW = gw;
                    break;
                }

            return oldGW != null && newGW != null && !oldGW.Address.Equals(newGW.Address);
        }





        private static bool HasDHCPChanged(IPInterfaceProperties oldProperties, IPInterfaceProperties newProperties)
        {
            if (oldProperties.GetIPv4Properties().IsDhcpEnabled != newProperties.GetIPv4Properties().IsDhcpEnabled)
                return true;

            if (oldProperties.DhcpServerAddresses.Count != newProperties.DhcpServerAddresses.Count)
                return true;

            for (int i = 0; i < oldProperties.DhcpServerAddresses.Count; i++)
            {
                IPAddress oldDHCP = oldProperties.DhcpServerAddresses[i];
                IPAddress newDHCP = newProperties.DhcpServerAddresses[i];

                if (!oldDHCP.Equals(newDHCP))
                    return true;
            }

            return false;
        }





        private static bool HasDNSChanged(IPInterfaceProperties oldProperties, IPInterfaceProperties newProperties)
        {
            if (oldProperties.DnsAddresses.Count != newProperties.DnsAddresses.Count)
                return true;

            for (int i = 0; i < oldProperties.DnsAddresses.Count; i++)
            {
                IPAddress oldDNS = oldProperties.DnsAddresses[i];
                IPAddress newDNS = newProperties.DnsAddresses[i];

                if (!oldDNS.Equals(newDNS))
                    return true;
            }

            return false;
        }
    }
}
