using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genealogy
{
    interface ISaveFormat
    {
        void SaveElement<T>(T element);
    }
}
