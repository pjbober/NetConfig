using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkManager.Profiles
{
    public class SystemProfileModel : AbstractProfileModel
    {
        public SystemProfileModel(String name, NetInterfaceModel netInterface)
            : base(name, netInterface)
        {
            
        }
    }
}
