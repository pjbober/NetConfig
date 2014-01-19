using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace NetworkManager.Profiles
{
    public class WiredProfileModel : AbstractProfileModel
    {
        public bool IsDHCP { get; set; }
        public IPAddress IP { get; set; }
        public IPAddress SubnetMask { get; set; }
        public IPAddress Gateway { get; set; }
        public IPAddress DNS { get; set; }

        public WiredProfileModel(String name, NetInterfaceModel netInterface)
            : base(name, netInterface)
        {
            
        }

        public WiredProfileModel()
            : base()
        {

        }
    }
}
