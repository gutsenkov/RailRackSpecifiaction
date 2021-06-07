using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCADLib
{
    class DeviceComparer : IComparer<Device>
    {
        public int Compare(Device x, Device y)
        {
            if (x.Name == y.Name) { return 0; }
            else { return -1; }
        }
    }
}
