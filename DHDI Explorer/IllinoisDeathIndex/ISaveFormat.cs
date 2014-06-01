using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genealogy
{
    interface ISaveFormat
    {
        void Stream(System.IO.StreamWriter stream, IObject o);
        void Stream(System.IO.StreamWriter stream, IProperty p);
    }
}
