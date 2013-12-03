using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;

namespace ASK.Logic
{
    class NetInterfaceEnumerator
    {
        private string interfaceSeparator = "----------------------------------------------";

        public IList<NetInterface> GetNetInterfaces()
        {
            IList<NetInterface> interfaces = new List<NetInterface>();

            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            string output = NetshWrapper.invoke("interface ip show interface level=verbose");
            var lines = GetLines(output);

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] == interfaceSeparator)
                {
                    int start = i;
                    int end = i;
                    for (end = i; end < lines.Count; end++)
                    {
                        if (String.IsNullOrWhiteSpace(lines[end]))
                            break;
                    }

                    var sliced = lines.Skip(i + 1).Take(end - i - 1).ToList();
                    var tuples = GetTuples(sliced);

                    int id = Int32.Parse(tuples[1].Item2);
                    string name = lines[i - 1].Substring(21);
                    NetInterfaceType type = GetType(tuples[0].Item2);
                    NetworkInterface niface = networkInterfaces.FirstOrDefault(iface => iface.Name.Equals(name)) as NetworkInterface;
                    
                    interfaces.Add(new NetInterface(id, name, type, niface));
                }
            }

            return interfaces;
        }

        private IList<string> GetLines(string str)
        {
            return str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
        }

        private IList<Tuple<string, string>> GetTuples(IList<string> lines)
        {
            IList<Tuple<string, string>> tuples = new List<Tuple<string, string>>();

            foreach (string l in lines)
            {
                var s = l.Split(':');
                tuples.Add(new Tuple<string, string>(s[0].Trim(), s[1].Trim()));
            }

            return tuples;
        }

        private NetInterfaceType GetType(string type)
        {
            if (type.Contains("loopback"))
                return NetInterfaceType.Loopback;
            else if (type.Contains("ethernet"))
                return NetInterfaceType.Wired;
            else if (type.Contains("wireless"))
                return NetInterfaceType.Wireless;
            else
                return NetInterfaceType.Other;
        }
    }
}
