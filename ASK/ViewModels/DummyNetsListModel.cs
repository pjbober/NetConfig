using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASK.ViewModels
{
    class DummyNetsListModel
    {
        private List<string> dummyList;

        public DummyNetsListModel()
        {
            dummyList = new List<string>();
            dummyList.Add("przewodowe");
            dummyList.Add("bezprzewodowe");
            dummyList.Add("virtual box");
        }

        public List<string> NetInterfacesCollection
        {
            get { return dummyList; }
        }
    }
}
