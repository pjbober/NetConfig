using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using NativeWifi;

namespace NetworkManager
{

    public enum NetInterfaceType
    {
        Wired,
        Wireless,
        Unknown,
        Other
    }

    public class NetInterface : IDisposable
    {

        public string Name { get { return networkAdapter.NetConnectionID; } }

        public string Description { get { return networkAdapter.Description; } }

        public NetInterfaceType Type { get { return type; } }

        public IList<Profile> Profiles = new List<Profile>();

        public Profile ActiveProfile = null;

        public string MACAddress { get { return networkAdapter.MACAddress; } }

        public event EventHandler NameChanged;

        public event EventHandler InterfaceUp;
        public event EventHandler InterfaceDown;

        public event EventHandler Connected;
        public event EventHandler Disconnected;

        public event EventHandler IPSettingsChanged;
        public event EventHandler WifiSettingsChanged;
        //public event EventHandler MACAddressSettingsChanged;

        public event EventHandler ActiveProfileChanged;

        NetInterfaceType type;

        private NetworkInterface networkInterface;

        WlanClient.WlanInterface wlanInterface;

        private NetworkAdapter networkAdapter;
        private NetworkAdapterConfiguration adapterConfiguration;

        private int netshId = -1;

        private ManagementEventWatcher AdapterWatcher;


        public NetInterface(NetworkAdapter netAdptr, bool startWatchers = true)
        {
            CreateFromNetworkAdapter(netAdptr);
            if (startWatchers)
                StartEventWatchers();
        }

        public NetInterface(NetworkAdapter netAdptr, NetworkInterface netIface = null, bool startWatchers = true)
        {
            CreateFromNetworkAdapter(netAdptr, netIface);
            if (startWatchers)
                StartEventWatchers();
        }

        public bool SetNetworkInterface(NetworkInterface niface)
        {
            if (this.networkAdapter.NetConnectionID == niface.Name)
            {
                this.networkInterface = niface;
                return true;
            }
            else
                return false;
        }

        public bool SetWlanInterface(WlanClient.WlanInterface wlanIface)
        {
            this.wlanInterface = wlanIface;
            return true;
        }

        private void StartEventWatchers()
        {
            StartAdapterEventWatcher();
        }


        private void StartAdapterEventWatcher()
        {
            string WmiQuery = "Select * From __InstanceModificationEvent Within 1 " +
                                "Where TargetInstance ISA 'Win32_NetworkAdapter' AND " +
                                "TargetInstance.InterfaceIndex=" + networkAdapter.InterfaceIndex;

            AdapterWatcher = new ManagementEventWatcher(new EventQuery(WmiQuery));
            AdapterWatcher.EventArrived += new EventArrivedEventHandler(this.WmiAdapterEventHandler);
            AdapterWatcher.Start();
        }


        private void CreateFromNetworkAdapter(NetworkAdapter netAdapter)
        {
            this.networkAdapter = netAdapter;
            this.networkInterface = null;
        }

        private void CreateFromNetworkAdapter(NetworkAdapter netAdapter, NetworkInterface netIface = null)
        {
            this.networkAdapter = netAdapter;
            this.networkInterface = null;

            if (netIface != null)
                this.networkInterface = netIface;
            else
            {
                IDictionary<string, NetworkInterface> ifaceDict = NetworkInterface.GetAllNetworkInterfaces().ToDictionary(i => i.Name);

                NetworkInterface value;
                if (ifaceDict.TryGetValue(networkAdapter.NetConnectionID, out value))
                    this.networkInterface = value;

            }

            if (this.networkInterface == null)
                this.type = NetInterfaceType.Unknown;
            else
            {
                switch (this.networkInterface.NetworkInterfaceType)
                {
                    case NetworkInterfaceType.Wireless80211:
                        this.type = NetInterfaceType.Wireless;
                        break;
                    case NetworkInterfaceType.Ethernet:
                        this.type = NetInterfaceType.Wired;
                        break;
                    default:
                        this.type = NetInterfaceType.Other;
                        break;
                }
            }

            this.netshId = (int) networkAdapter.InterfaceIndex;
            ReloadAdapterConfiguration();
        }

        private bool ReloadAdapterConfiguration()
        {
            this.adapterConfiguration = AdapterConfigurationEnumerator.GetAdapterConfiguration(this.netshId);
            return this.adapterConfiguration != null;
        }

        private bool ReloadAdapterConfiguration(NetworkAdapterConfiguration config)
        {
            this.adapterConfiguration = config;
            return this.adapterConfiguration != null;
        }


        public bool ActivateProfile(Profile p)
        {
            try {
                if (p.IsDHCP)
                    EnableDhcp();
                else
                {
                    IPAddress ip = IPAddress.Parse(p.IpAddress);
                    IPAddress netmask = IPAddress.Parse(p.SubnetMask);
                    IPAddress gateway = IPAddress.Parse(p.Gateway);
                    IPAddress dns = IPAddress.Parse(p.DNS);

                    IPAddressCollection dnss = new IPAddressCollection();
                    dnss.Add(dns);

                    SetAddress(ip, netmask, gateway);
                    SetStaticDNSes(dnss);
                }
            }
            catch (Exception e)
            {
                return false;
            }

            ActiveProfile = p;

            return true;
        }




        public bool IsConnected
        {
            get
            {
                return networkAdapter.NetEnabled;
            }
        }


        public bool IsEnabled
        {
            get
            {
                if (networkAdapter.IsNetConnectionStatusNull)
                    return false;

                int status = networkAdapter.NetConnectionStatus;

                return status == 2 || status == 7;
            }
        }


        public bool Enable()
        {
            return networkAdapter.Enable() == 0;
        }

        public bool Disable()
        {
            return networkAdapter.Disable() == 0;
        }


        public bool IsDhcpEnabled()
        {
            return networkInterface != null && networkInterface.GetIPProperties().GetIPv4Properties().IsDhcpEnabled;
        }


        public void EnableDhcp()
        {
            Netsh.invoke("netsh interface ip set address " + netshId + " dhcp");
        }

        public void SetAddress(IPAddress address)
        {
            string cmd = "netsh interface ip set address " + netshId + " static ";
            cmd += address.ToString();
            Netsh.invoke(cmd);
        }

        public void SetAddress(IPAddress address, IPAddress netmask)
        {
            string cmd = "netsh interface ip set address " + netshId + " static ";
            cmd += address.ToString() + " " + netmask.ToString();
            Netsh.invoke(cmd);
        }

        public void SetAddress(IPAddress address, IPAddress netmask, IPAddress gateway)
        {
            string cmd = "netsh interface ip set address " + netshId + " static ";
            cmd += address.ToString() + " " + netmask.ToString() + " " + gateway.ToString();
            Netsh.invoke(cmd);
        }

        public void SetNetmask(IPAddress netmask)
        {
            if (GetIP() == null)
                return;

            string cmd = "netsh interface ip set address " + netshId + " static ";
            cmd += GetIP().ToString() + " " + netmask.ToString();
            Netsh.invoke(cmd);
        }

        public void SetGateway(IPAddress netmask)
        {
            if (GetIP() == null || GetNetmask() == null)
                return;

            string cmd = "netsh interface ip set address " + netshId + " static ";
            cmd += GetIP().ToString() + " " + netmask.ToString();
            Netsh.invoke(cmd);
        }

        // http://superuser.com/questions/204046/how-can-i-set-my-dns-settings-using-the-command-promp
        public void SetStaticDNSes(IPAddressCollection dnses)
        {
            string cmd = "netsh interface ip set dns " + netshId;
            foreach (IPAddress dns in dnses)
                cmd += " " + dns.ToString();

            Netsh.invoke(cmd);
        }

        public void SetDynamicDNSes()
        {
            string cmd = "netsh interface ip set dns " + netshId + " dhcp";
            Netsh.invoke(cmd);
        }

        public IPAddress GetIP()
        {
            return Helpers.GetIP(this.networkInterface);
        }

        public IPAddress GetNetmask()
        {
            return Helpers.GetSubnetMask(this.networkInterface);
        }

        public IPAddress GetGateway()
        {
            return Helpers.GetGatewayAddress(this.networkInterface);
        }


        public IPAddressCollection GetDNSes()
        {
            return networkInterface.GetIPProperties().DnsAddresses;
        }


        private void WmiAdapterEventHandler(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            ManagementBaseObject previousInstance = (ManagementBaseObject)e.NewEvent["PreviousInstance"];

            NetworkAdapter targetAdapter = new NetworkAdapter(targetInstance);
            NetworkAdapter previousAdapter = new NetworkAdapter(previousInstance);

            NetInterface oldInterface = new NetInterface(previousAdapter, null, false);
            CreateFromNetworkAdapter(targetAdapter);

            if (oldInterface.IsConnected != this.IsConnected)
                if (this.IsConnected)
                    OnConnected(EventArgs.Empty);
                else
                    OnDisconnected(EventArgs.Empty);

            if (oldInterface.IsEnabled != this.IsEnabled)
                if (this.IsEnabled)
                    OnInterfaceUp(EventArgs.Empty);
                else
                    OnInterfaceDown(EventArgs.Empty);

            Console.WriteLine("WmiAdapterEventHandler");
        }


        // Events

        protected virtual void OnNameChanged(EventArgs e)
        {
            EventHandler handler = NameChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnInterfaceUp(EventArgs e)
        {
            EventHandler handler = InterfaceUp;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnInterfaceDown(EventArgs e)
        {
            EventHandler handler = InterfaceDown;
            if (handler != null)
            {
                handler(this, e);
            }
        }


        protected virtual void OnConnected(EventArgs e)
        {
            EventHandler handler = Connected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnDisconnected(EventArgs e)
        {
            EventHandler handler = Disconnected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnIPSettingsChanged(EventArgs e)
        {
            EventHandler handler = IPSettingsChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnWifiSettingsChanged(EventArgs e)
        {
            EventHandler handler = WifiSettingsChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }


        public void Dispose()
        {
            Console.WriteLine("Dispose start " + netshId);

            AdapterWatcher.Stop();
            AdapterWatcher.Dispose();

            Console.WriteLine("Dispose end " + netshId);
        }

    }
}
