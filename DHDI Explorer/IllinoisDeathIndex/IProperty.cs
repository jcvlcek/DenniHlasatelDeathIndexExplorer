using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genealogy
{
    public interface IProperty
    {
        string Name { get; }
        string Value { get; set; }
    }
}
