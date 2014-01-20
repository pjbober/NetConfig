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
using System.Xml;
using NetworkManager.Profiles;
using System.Threading;


namespace NetworkManager
{
    public delegate void ProfileAdded(AbstractProfileModel newProfile);

    public delegate void InterfaceUp();
    public delegate void InterfaceDown();

    public delegate void Connected();
    public delegate void Disconnected();

    public delegate void IPSettingsChanged();
    public delegate void WifiSettingsChanged();

    public delegate void NameChanged();
    public delegate void ActiveProfileChanged(AbstractProfileModel profile);
    public delegate void ProfileActivationFailed(AbstractProfileModel profile);

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

        /*
        [XmlElement(typeof(SystemProfileModel))]
        [XmlElement(typeof(WiredProfileModel))]
        [XmlElement(typeof(WifiProfileModel))]
        */
        public IList<AbstractProfileModel> Profiles = new List<AbstractProfileModel>();

        public AbstractProfileModel ActiveProfile = null;

        private SystemProfileModel SystemProfile;

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

        WlanClient.WlanInterface wifiInterface;

        private NetworkAdapter networkAdapter;
        private NetworkAdapter originalNetworkAdapter;
        private NetworkAdapterConfiguration adapterConfiguration;

        private int netshId = -1;

        private ManagementEventWatcher AdapterWatcher;

        private Thread statusCheckThread;
        private const int POLL_TIME = 3000;
        private bool alive = true;

        private NetInterfaceManager networkManager = null;

        public void SetNetInterfaceManager(NetInterfaceManager netManager)
        {
            this.networkManager = netManager;
        }

        private void CheckStatus()
        {
            this.networkManager.RefreshNetworkInterfaces();

            if (ActiveProfile != null && SystemProfile != null && !ActiveProfile.Equals(SystemProfile))
            {
                SystemProfile.ToggleState();
            }
        }

        private void CheckStatusThread()
        {
            int short_time = 500;
            int i = 0;

            while (alive)
            {
                i++;

                if (i * short_time == POLL_TIME)
                {
                    CheckStatus();
                    i = 0;
                }

                Thread.Sleep(short_time);
            }
        }


        public NetInterfaceModel(NetworkAdapter netAdptr, NetworkInterface netIface = null, bool createFullObject = true)
        {
            this.originalNetworkAdapter = netAdptr;

            CreateFromNetworkAdapter(netAdptr, netIface);

            statusCheckThread = new Thread(CheckStatusThread);

            if (createFullObject)
            {
                LoadProfiles();
                StartEventWatchers();
            }
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
            this.wifiInterface = wlanIface;
            return true;
        }


        private void LoadProfiles()
        {
            AddSystemProfile();

            DeserializeProfiles();

            InitActiveProfile();
        }

        private void StartEventWatchers()
        {
            StartAdapterEventWatcher();
            statusCheckThread.Start();
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



        // http://blog.coretech.dk/jgs/serializing-system-net-ipaddress-to-xml/
        private void SerializeProfiles()
        {
            string profilesXML = @"profiles/" + this.Name + ".xml";

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<AbstractProfileModel>));
                StreamWriter file = new StreamWriter(profilesXML);

                IList<AbstractProfileModel> serializableProfiles = this.Profiles.Where(p => !(p is SystemProfileModel)).ToList();

                serializer.Serialize(file, serializableProfiles);
                file.Close();
            }
            catch (Exception e)
            {
                return;
            }
        }

        private void DeserializeProfiles()
        {
            string profilesDir = @"profiles";
            string profilesXML = profilesDir + "/" + this.Name + ".xml";

            if (!Directory.Exists(profilesDir))
                Directory.CreateDirectory(profilesDir);

            if (File.Exists(profilesXML))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<AbstractProfileModel>));

                    StreamReader file = new StreamReader(profilesXML);
                    foreach (AbstractProfileModel profile in serializer.Deserialize(file) as List<AbstractProfileModel>)
                    {
                        profile.NetInterface = this;
                        this.Profiles.Add(profile);
                    }

                    file.Close();
                }
                catch (Exception)
                {
                    this.Profiles.Clear();
                }
            }
        }

        private void InitActiveProfile()
        {
            foreach (AbstractProfileModel profile in Profiles)
            {
                if (!(profile is SystemProfileModel) && profile.Equals(SystemProfile))
                {
                    this.ActiveProfile = profile;
                    this.ActiveProfile.ProfileState = AbstractProfileModel.StateEnum.ON;
                    return;
                }
            }

            this.ActiveProfile = SystemProfile;
            this.ActiveProfile.ProfileState = AbstractProfileModel.StateEnum.ON;
        }

        public bool ActivateProfile(AbstractProfileModel p)
        {
            // rozpoczęcie aktywacji nowego profilu oznacza potrzebę zmiany stanu na deaktywację starego
            // i zmianę stanu na aktywację nowego

            AbstractProfileModel oldProfile = ActiveProfile;

            if (oldProfile != null)
            {
                oldProfile.ProfileState = AbstractProfileModel.StateEnum.DEACTIVATING;
            }

            p.ProfileState = AbstractProfileModel.StateEnum.ACTIVATING;

            if (!(p is SystemProfileModel))
            {
                try
                {
                    if (p is WiredProfileModel)
                    {
                        WiredProfileModel wired = p as WiredProfileModel;

                        if (wired.IsDHCP)
                            EnableDhcp();
                        else
                        {
                            SetAddress(IPAddress.Parse(wired.IP), IPAddress.Parse(wired.SubnetMask), IPAddress.Parse(wired.Gateway));
                            SetStaticDNS(IPAddress.Parse(wired.DNS));
                        }
                    }

                    if (p is WifiProfileModel)
                    {
                        ActivateWifiProfile(p as WifiProfileModel); // TODO: teraz niezależnie od wyniku...
                    }
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            ActiveProfile = p;

            // być może niepotrzebne, ale bezpieczniejsze - każdy profil inny od aktywowanego
            // powinien być w stanie wyłączonym
            foreach (AbstractProfileModel pro in Profiles)
            {
                pro.ProfileState = AbstractProfileModel.StateEnum.OFF;
            }

            ActiveProfile.ProfileState = AbstractProfileModel.StateEnum.ON;

            if (ActiveProfileChanged != null) ActiveProfileChanged(ActiveProfile);

            return true;
        }


        public bool ActivateWifiProfile(WifiProfileModel p)
        {



            return true;
        }


        public IList<string> ListWifiNetworks()
        {
            IList<string> networks = new List<string>();

            if (this.type != NetInterfaceType.Wireless || this.wifiInterface == null)
                return networks;

            foreach (var w in wifiInterface.GetAvailableNetworkList(Wlan.WlanGetAvailableNetworkFlags.IncludeAllAdhocProfiles)) {
                networks.Add(Helpers.BytesToString(w.dot11Ssid.SSID));
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


        #region IPv4 settings

        public bool IsDhcpEnabled()
        {
            return networkInterface != null && networkInterface.GetIPProperties().GetIPv4Properties().IsDhcpEnabled;
        }

        public void EnableDhcp()
        {
            Netsh.invoke("interface ip set address " + netshId + " dhcp");
            SetDynamicDNSes();
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
        public void SetStaticDNS(IPAddress dns)
        {
            string cmd = "interface ip set dns " + netshId + " static ";
            cmd += dns.ToString();

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


        public IPAddress GetDNS()
        {
            return Helpers.GetDNSAddress(this.networkInterface);
        }

        #endregion




        #region Wifi settings

        public string GetSSID()
        {
            if (wifiInterface != null)
            {
                Wlan.WlanAssociationAttributes attr = wifiInterface.CurrentConnection.wlanAssociationAttributes;
                return (Helpers.BytesToString(attr.dot11Ssid.SSID, (int)attr.dot11Ssid.SSIDLength));
            }
            else
                return "";
        }

        public string GetBSSID()
        {
            if (wifiInterface != null)
            {
                Wlan.WlanAssociationAttributes attr = wifiInterface.CurrentConnection.wlanAssociationAttributes;
                return attr.Dot11Bssid.ToString();
            }
            else
                return "";
        }

        public Profiles.WifiProfileModel.SecurityEnum GetSecurityType()
        {
            if (wifiInterface != null)
            {
                Wlan.WlanSecurityAttributes attr = wifiInterface.CurrentConnection.wlanSecurityAttributes;
                Wlan.Dot11AuthAlgorithm auth = attr.dot11AuthAlgorithm;

                // https://github.com/rainmeter/rainmeter/blob/master/Plugins/PluginWifiStatus/WifiStatus.cpp

                switch (auth)
                {
                    case Wlan.Dot11AuthAlgorithm.IEEE80211_Open:
                        return NetworkManager.Profiles.WifiProfileModel.SecurityEnum.OPEN;
                    case Wlan.Dot11AuthAlgorithm.IEEE80211_SharedKey:
                        return NetworkManager.Profiles.WifiProfileModel.SecurityEnum.SHARED;
                    case Wlan.Dot11AuthAlgorithm.WPA:
                        return NetworkManager.Profiles.WifiProfileModel.SecurityEnum.WPA;
                    case Wlan.Dot11AuthAlgorithm.WPA_PSK:
                        return NetworkManager.Profiles.WifiProfileModel.SecurityEnum.WPAPSK;
                    case Wlan.Dot11AuthAlgorithm.RSNA:
                        return NetworkManager.Profiles.WifiProfileModel.SecurityEnum.WPA2;
                    case Wlan.Dot11AuthAlgorithm.RSNA_PSK:
                        return NetworkManager.Profiles.WifiProfileModel.SecurityEnum.WPA2PSK;
                    default:
                        return NetworkManager.Profiles.WifiProfileModel.SecurityEnum.Other;
                }
            }
            else
                return NetworkManager.Profiles.WifiProfileModel.SecurityEnum.Other;
        }

        
        public Profiles.WifiProfileModel.EncryptionEnum GetEncryptionType()
        {
            if (wifiInterface != null)
            {
                Wlan.WlanSecurityAttributes attr = wifiInterface.CurrentConnection.wlanSecurityAttributes;
                Wlan.Dot11CipherAlgorithm encryption = attr.dot11CipherAlgorithm;

                // https://github.com/rainmeter/rainmeter/blob/master/Plugins/PluginWifiStatus/WifiStatus.cpp

                switch (encryption)
                {
                    case Wlan.Dot11CipherAlgorithm.None:
                        return NetworkManager.Profiles.WifiProfileModel.EncryptionEnum.None;

                    case Wlan.Dot11CipherAlgorithm.WEP:
                    case Wlan.Dot11CipherAlgorithm.WEP40:
                    case Wlan.Dot11CipherAlgorithm.WEP104:
                        return NetworkManager.Profiles.WifiProfileModel.EncryptionEnum.WEP;

                    case Wlan.Dot11CipherAlgorithm.TKIP:
                        return NetworkManager.Profiles.WifiProfileModel.EncryptionEnum.TKIP;

                    case Wlan.Dot11CipherAlgorithm.CCMP:
                        return NetworkManager.Profiles.WifiProfileModel.EncryptionEnum.AES;

                    // sa jeszcze 2 typy szyfrowan: RSN i WPA, ale w windowsach nikt o nich nigdzie nie wspomina...
                    default:
                        return NetworkManager.Profiles.WifiProfileModel.EncryptionEnum.Other;
                }
            }
            else
                return NetworkManager.Profiles.WifiProfileModel.EncryptionEnum.Other;
        }


        public Profiles.WifiProfileModel.AuthEnum GetAuthMethod()
        {
            // http://www.iana.org/assignments/eap-numbers/eap-numbers.xhtml

            try
            {
                string profile = wifiInterface.GetProfileXml(wifiInterface.CurrentConnection.profileName);
                XmlDocument profileXml = new XmlDocument();

                profileXml.LoadXml(profile);

                XmlNode node = profileXml.SelectSingleNode("/*/*/*/*/*/*/*/*[local-name()='Type']");

                int eap_type = int.Parse(node.InnerText);

                if (eap_type == 13)
                    return NetworkManager.Profiles.WifiProfileModel.AuthEnum.Card;
                else if (eap_type == 25)
                    return NetworkManager.Profiles.WifiProfileModel.AuthEnum.PEAP;
                else
                    return NetworkManager.Profiles.WifiProfileModel.AuthEnum.None;
            }
            catch (Exception)
            {
            }

            return NetworkManager.Profiles.WifiProfileModel.AuthEnum.None;
        }


        public bool IsUsingOneX()
        {
            try
            {
                string profile = wifiInterface.GetProfileXml(wifiInterface.CurrentConnection.profileName);
                XmlDocument profileXml = new XmlDocument();

                profileXml.LoadXml(profile);

                XmlNode node = profileXml.SelectSingleNode("/*/*/*/*/*[local-name()='useOneX']");

                return bool.Parse(node.InnerText);

            }
            catch (Exception)
            {
            }

            return false;
        }


        public string GetKey()
        {
            try
            {
                string profile = wifiInterface.GetProfileXml(wifiInterface.CurrentConnection.profileName);
                XmlDocument profileXml = new XmlDocument();

                profileXml.LoadXml(profile);

                XmlNode node = profileXml.SelectSingleNode("/*/*/*/*/*[local-name()='keyMaterial']");

                return node.InnerText;

            }
            catch (Exception)
            {
            }

            return "";
        }


        public string GetCertHash()
        {
            try
            {
                string profile = wifiInterface.GetProfileXml(wifiInterface.CurrentConnection.profileName);
                XmlDocument profileXml = new XmlDocument();

                profileXml.LoadXml(profile);

                XmlNode node = profileXml.SelectSingleNode("/*/*/*/*/*/*/*/*/*/*/*[local-name()='TrustedRootCA']");

                string hash = node.InnerText.Replace(" ", "");

                return hash.ToUpper();

            }
            catch (Exception)
            {
            }

            return "";
        }

        public Certificate GetCert()
        {
            string hash = GetCertHash();

            if (String.IsNullOrWhiteSpace(hash))
                return null;

            foreach (Certificate cert in CertificateCollection.Certificates)
            {
                if (cert.TrustedRootCA == hash)
                    return cert;
            }

            return null;

        }
        #endregion




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

            alive = false;
            statusCheckThread.Join();

            Console.WriteLine("Dispose end " + netshId);


        }

        public void AddSystemProfile()
        {
            foreach (AbstractProfileModel profile in this.Profiles)
                if (profile is SystemProfileModel)
                    return;

            this.SystemProfile = new SystemProfileModel(this);
            AddProfile(this.SystemProfile);
        }

        public void AddNewProfile(string name = "Nowy profil")
        {
            if (this.Type == NetInterfaceType.Wired)
                AddProfile(new WiredProfileModel(name, this));

            else if (this.Type == NetInterfaceType.Wireless)
                AddProfile(new WifiProfileModel(name, this));
        }

        public void AddProfile(AbstractProfileModel profile)
        {
            Profiles.Add(profile);
            if (ProfileAddedEvent != null)
            {
                ProfileAddedEvent(profile);
            }
        }
    }
}
