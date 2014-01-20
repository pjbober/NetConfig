using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace NetworkManager.Profiles
{
    public class WiredProfileModel : AbstractProfileModel
    {

        public override bool IsWifi { get { return false; } }

        public virtual bool IsDHCP { get; set; }
        public virtual IPAddress IP { get; set; }
        public virtual IPAddress SubnetMask { get; set; }
        public virtual IPAddress Gateway { get; set; }
        public virtual IPAddress DNS { get; set; }

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
