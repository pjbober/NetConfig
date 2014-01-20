using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml.Serialization;

namespace NetworkManager.Profiles
{
    public class WiredProfileModel : AbstractProfileModel, IEquatable<WiredProfileModel>
    {

        public virtual bool IsDHCP { get; set; }

        public virtual string IP { get; set; }
        public virtual string SubnetMask { get; set; }
        public virtual string Gateway { get; set; }
        public virtual string DNS { get; set; }

        public WiredProfileModel(String name, NetInterfaceModel netInterface)
            : base(name, netInterface)
        {
            SystemProfileModel system = new SystemProfileModel(netInterface, true);

            this.IsDHCP = system.IsDHCP;
            this.IP = system.IP;
            this.SubnetMask = system.SubnetMask;
            this.Gateway = system.Gateway;
            this.DNS = system.DNS;
        }

        public WiredProfileModel()
            : base()
        {

        }

        public override bool Equals(AbstractProfileModel other)
        {
            if (other is WiredProfileModel)
                return Equals(other as WiredProfileModel);

            return false;
        }

        public bool Equals(WiredProfileModel other)
        {
            return other.IsDHCP == this.IsDHCP &&
                    other.IP == this.IP &&
                    other.SubnetMask == this.SubnetMask &&
                    other.Gateway == this.Gateway &&
                    other.DNS == this.DNS;
        }
    }
}
