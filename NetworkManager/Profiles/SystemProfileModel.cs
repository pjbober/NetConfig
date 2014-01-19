using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkManager.Profiles
{
    public class SystemProfileModel : AbstractProfileModel
    {
        public SystemProfileModel(NetInterfaceModel netInterface)
            : base("Konfiguracja systemowa", netInterface)
        {
            
        }

        public SystemProfileModel()
            : base()
        {

        }
    }
}
