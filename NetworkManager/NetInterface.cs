using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Management;
using System.Net.Sockets;
using System.Net;
using NativeWifi;
using System.Xml.Serialization;
using System.IO;

namespace NetworkManager
{

    public delegate void ProfileAdded(ProfileModel newProfile);

    public delegate void InterfaceUp();
    public delegate void InterfaceDown();

    public delegate void Connected();
    public delegate void Disconnected();

    public delegate void IPSettingsChanged();
    public delegate void WifiSettingsChanged();

    public delegate void NameChanged();
    public delegate void ActiveProfileChanged(ProfileModel profile);
    public delegate void ProfileActivationFailed(ProfileModel profile);

    public enum NetInterfaceType
    {
        Wired,
        Wireless,
        Unknown,
        Other
    }

    public class NetInterfaceModel : IDisposable
    {

        public string Name { get { return networkAdapter.NetConnectionID; } }

        public string Description { get { return networkAdapter.Description; } }

        public NetInterfaceType Type { get { return type; } }

        public IList<ProfileModel> Profiles = new List<ProfileModel>();

        public ProfileModel ActiveProfile = null;

        public string MACAddress { get { return networkAdapter.MACAddress; } }

        public event NameChanged NameChanged;

        public event InterfaceUp InterfaceUp;
        public event InterfaceDown InterfaceDown;

        public event Connected Connected;
        public event Disconnected Disconnected;

        public event IPSettingsChanged IPSettingsChanged;
        public event WifiSettingsChanged WifiSettingsChanged;
        //public event EventHandler MACAddressSettingsChanged;

        public event ProfileAdded ProfileAddedEvent;
        public event ActiveProfileChanged ActiveProfileChanged;

        NetInterfaceType type;

        private NetworkInterface networkInterface;

        WlanClient.WlanInterface wlanInterface;

        private NetworkAdapter networkAdapter;
        private NetworkAdapter originalNetworkAdapter;
        private NetworkAdapterConfiguration adapterConfiguration;

        private int netshId = -1;

        private ManagementEventWatcher AdapterWatcher;


        public NetInterfaceModel(NetworkAdapter netAdptr, bool startWatchers = true)
        {
            this.originalNetworkAdapter = netAdptr;

            CreateFromNetworkAdapter(netAdptr);

            DeserializeProfiles();

            if (startWatchers)
                StartEventWatchers();
        }

        public NetInterfaceModel(NetworkAdapter netAdptr, NetworkInterface netIface = null, bool startWatchers = true)
        {
            this.originalNetworkAdapter = netAdptr;

            CreateFromNetworkAdapter(netAdptr, netIface);

            DeserializeProfiles();

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

            this.netshId = (int)networkAdapter.InterfaceIndex;
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



        private void SerializeProfiles()
        {
            string profilesXML = @"profiles/" + this.Name + ".xml";

            XmlSerializer serializer = new XmlSerializer(typeof(List<ProfileModel>));

            StreamWriter file = new StreamWriter(profilesXML);
            serializer.Serialize(file, Profiles);
            file.Close();
        }

        private void DeserializeProfiles()
        {
            string profilesDir = @"profiles";
            string profilesXML = profilesDir + "/" + this.Name + ".xml";

            if (!Directory.Exists(profilesDir))
                Directory.CreateDirectory(profilesDir);

            if (File.Exists(profilesXML))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<ProfileModel>));
                bool profileActivated = false;

                StreamReader file = new StreamReader(profilesXML);
                this.Profiles = serializer.Deserialize(file) as List<ProfileModel>;
                file.Close();

                foreach (ProfileModel profile in this.Profiles)
                {
                    profile.NetInterface = this;
                    if (!profileActivated && profile.ProfileState == ProfileModel.StateEnum.ON)
                    {
                        profile.ToggleState();
                        profileActivated = true;
                    }
                }
            }
        }

        public bool ActivateProfile(ProfileModel p)
        {
            // rozpoczęcie aktywacji nowego profilu oznacza potrzebę zmiany stanu na deaktywację starego
            // i zmianę stanu na aktywację nowego

            ProfileModel oldProfile = ActiveProfile;

            if (oldProfile != null)
            {
                oldProfile.ProfileState = ProfileModel.StateEnum.DEACTIVATING;
            }

            p.ProfileState = ProfileModel.StateEnum.ACTIVATING;
            

            try
            {
                if (p.IsDHCP)
                    EnableDhcp();
                else
                {
                    IPAddress ip = IPAddress.Parse(p.IpAddress);
                    IPAddress netmask = IPAddress.Parse(p.SubnetMask);
                    IPAddress gateway = IPAddress.Parse(p.Gateway);
                    IPAddress dns = IPAddress.Parse(p.DNS);

                    IList<IPAddress> dnss = new List<IPAddress>();
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

            if (this.type == NetInterfaceType.Wireless)
            {
                ActivateWifiSettings(p); // TODO: teraz niezależnie od wyniku...
            }

            // być może niepotrzebne, ale bezpieczniejsze - każdy profil inny od aktywowanego
            // powinien być w stanie wyłączonym
            foreach (var pro in Profiles)
            {
                pro.ProfileState = ProfileModel.StateEnum.OFF;
            }
            
            ActiveProfile.ProfileState = ProfileModel.StateEnum.ON;

            if (ActiveProfileChanged != null) ActiveProfileChanged(ActiveProfile);

            return true;
        }


        public bool ActivateWifiSettings(ProfileModel p)
        {



            return true;
        }


        public IList<string> ListWifiNetworks()
        {
            IList<string> networks = new List<string>();

            if (this.type != NetInterfaceType.Wireless || this.wlanInterface == null)
                return networks;

            foreach (var w in wlanInterface.GetAvailableNetworkList(Wlan.WlanGetAvailableNetworkFlags.IncludeAllAdhocProfiles)) {
                byte[] ssid = w.dot11Ssid.SSID;
                string ssid_string = System.Text.Encoding.Default.GetString(ssid, 0, (int) w.dot11Ssid.SSIDLength);
                networks.Add(ssid_string);
                networks.Add(ssid_string.Trim());
            }

            return networks;
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
            return originalNetworkAdapter.Enable() == 0;
        }

        public bool Disable()
        {
            return originalNetworkAdapter.Disable() == 0;
        }


        public bool IsDhcpEnabled()
        {
            return networkInterface != null && networkInterface.GetIPProperties().GetIPv4Properties().IsDhcpEnabled;
        }


        public void EnableDhcp()
        {
            Netsh.invoke("interface ip set address " + netshId + " dhcp");
        }

        public void SetAddress(IPAddress address)
        {
            string cmd = "interface ip set address " + netshId + " static ";
            cmd += address.ToString();
            Netsh.invoke(cmd);
        }

        public void SetAddress(IPAddress address, IPAddress netmask)
        {
            string cmd = "interface ip set address " + netshId + " static ";
            cmd += address.ToString() + " " + netmask.ToString();
            Netsh.invoke(cmd);
        }

        public void SetAddress(IPAddress address, IPAddress netmask, IPAddress gateway)
        {
            string cmd = "interface ip set address " + netshId + " static ";
            cmd += address.ToString() + " " + netmask.ToString() + " " + gateway.ToString();
            Netsh.invoke(cmd);
        }

        public void SetNetmask(IPAddress netmask)
        {
            if (GetIP() == null)
                return;

            string cmd = "interface ip set address " + netshId + " static ";
            cmd += GetIP().ToString() + " " + netmask.ToString();
            Netsh.invoke(cmd);
        }

        public void SetGateway(IPAddress netmask)
        {
            if (GetIP() == null || GetNetmask() == null)
                return;

            string cmd = "interface ip set address " + netshId + " static ";
            cmd += GetIP().ToString() + " " + netmask.ToString();
            Netsh.invoke(cmd);
        }

        // http://superuser.com/questions/204046/how-can-i-set-my-dns-settings-using-the-command-promp
        public void SetStaticDNSes(IList<IPAddress> dnses)
        {
            string cmd = "interface ip set dns " + netshId;
            foreach (IPAddress dns in dnses)
                cmd += " " + dns.ToString();

            Netsh.invoke(cmd);
        }

        public void SetDynamicDNSes()
        {
            string cmd = "interface ip set dns " + netshId + " dhcp";
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

            NetInterfaceModel oldInterface = new NetInterfaceModel(previousAdapter, null, false);
            //CreateFromNetworkAdapter(targetAdapter);
            this.networkAdapter = targetAdapter;

            if (oldInterface.IsConnected != this.IsConnected)
            {
                if (this.IsConnected)
                {
                    if (Connected != null) Connected();
                }
                else
                {
                    if (Disconnected != null) Disconnected();
                }
            }

            if (oldInterface.IsEnabled != this.IsEnabled)
            {
                if (this.IsEnabled)
                {
                    if (InterfaceUp != null) InterfaceUp();
                }
                else
                {
                    if (InterfaceDown != null) InterfaceDown();
                }
            }

            Console.WriteLine("WmiAdapterEventHandler");
        }

        public void Dispose()
        {
            Console.WriteLine("Dispose start " + netshId);

            AdapterWatcher.Stop();
            AdapterWatcher.Dispose();

            Console.WriteLine("Serializing profiles... ");

            SerializeProfiles();

            Console.WriteLine("Dispose end " + netshId);


        }

        public void AddNewProfile()
        {
            AddProfile(new ProfileModel("Nowy profil", this));
        }

        public void AddProfile(ProfileModel profile)
        {
            Profiles.Add(profile);
            if (ProfileAddedEvent != null)
            {
                ProfileAddedEvent(profile);
            }
        }
    }
}
