using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASK.Model
{
    class NetInterface
    {
        public string InterfaceName { get; set; }
        public List<Profile> Profiles { get; set; }

        public NetInterface(string name)
        {
            Profiles = new List<Profile>();
            this.InterfaceName = name;
        }

        public void AddProfile(Profile profile)
        {
            Profiles.Add(profile);
        }
    }
}
