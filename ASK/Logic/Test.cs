using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASK.Logic
{
    class Test
    {
        static int Main(string[] args)
        {
            NetInterfaceEnumerator nie = new NetInterfaceEnumerator();

            var interfaces = nie.GetNetInterfaces();

            return 0;
        }
    }
}
