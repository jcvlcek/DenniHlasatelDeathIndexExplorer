using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genealogy
{
    /// <summary>
    /// Interface implemeted by simple, scalar properties
    /// </summary>
    public interface IProperty
    {
        /// <summary>
        /// The (read-only) name of the property
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The (read/write) value of the property
        /// </summary>
        string Value { get; set; }
    }
}
