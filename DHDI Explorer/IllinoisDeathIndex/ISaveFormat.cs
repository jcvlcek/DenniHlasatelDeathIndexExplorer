using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genealogy
{
    interface ISaveFormat
    {
        void Stream(System.IO.StreamWriter stream, SimpleObject o);
        void Stream(System.IO.StreamWriter stream, SimpleProperty p);
    }
}
