using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASK.Model.NetsList;
using System.Diagnostics;

namespace ASK.Logic
{
    class WifiInterfacesParser
    {
        public List<Profile> parse()
        {
            string netshOutput = invokeNetsh();

            List<Profile> profiles = new List<Profile>();
            List<string> outputLines = netshOutput.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Where(p => p.Contains(":") && !p.StartsWith("There") && !p.StartsWith("Hosted")).ToList();
            List<Tuple<string, string>> oneProfile = new List<Tuple<string, string>>();

            foreach (string line in outputLines)
            {
                List<string> newLine = line.Split(new char[] { ':' }, 2).Select(p => p.Trim()).ToList();
                oneProfile.Add(new Tuple<string, string>(newLine[0], newLine[1]));
                if (newLine[0] == "Profile")
                {
                    profiles.Add(makeProfile(oneProfile.ToDictionary(x => x.Item1, x => x.Item2)    ));
                    oneProfile.Clear();
                }
            }

            return profiles;
        }

        private Profile makeProfile(Dictionary<string, string> profileOptions)
        {
            Profile profile = new Profile(profileOptions["Name"]);
            profile.PhysicalAddress = profileOptions["Physical address"];
            profile.GUID = profileOptions["GUID"];
            profile.Authentication = profileOptions["Authentication"];
            profile.BSSID = profileOptions["BSSID"];
            profile.Channel = profileOptions["Channel"];
            profile.Cipher = profileOptions["Cipher"];
            profile.ConnectionMode = profileOptions["Connection mode"];
            profile.Description = profileOptions["Description"];
            profile.NetworkType = profileOptions["Network type"];
            profile.ProfileName = profileOptions["Profile"];
            profile.RadioType = profileOptions["Radio type"];
            profile.ReceiveRate = profileOptions["Receive rate (Mbps)"];
            profile.Signal = profileOptions["Signal"];
            profile.SSID = profileOptions["SSID"];
            profile.State = profileOptions["State"];
            profile.TransmitRate = profileOptions["Transmit rate (Mbps)"];
            return profile;
        }

        private string invokeNetsh()
        {
            Process proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = "netsh";
            proc.StartInfo.Arguments = "wlan show interfaces";
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            return proc.StandardOutput.ReadToEnd();
        }
    }
}
