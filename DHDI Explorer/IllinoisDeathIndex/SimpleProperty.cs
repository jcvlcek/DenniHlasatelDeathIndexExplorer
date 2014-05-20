using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Genealogy
{
    /// <summary>
    /// Single, scalar property of an object
    /// </summary>
    class SimpleProperty : IProperty
    {
        #region Private members
        #endregion

        #region Constructors
        /// <summary>
        /// Create a <see cref="SimpleProperty"/> with the specified name and value
        /// </summary>
        /// <param name="sName">The name of the new property</param>
        /// <param name="sValue">The initial value of the new property.  This argument may be null or empty;
        /// the property value is <see cref="string.Empty"/> in either case</param>
        /// <exception cref="ArgumentNullException">If <see cref="sName"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <see cref="sName"/> is empty</exception>
        public SimpleProperty(string sName, string sValue)
        {
            if ( sName == null )
                throw new ArgumentNullException( "Null string cannot be used as a property name" );
            if ( sName.Length < 1 )
                throw new ArgumentOutOfRangeException( "Empty string cannot be used as a property name" );
            if ( sValue == null )
                sValue = string.Empty;
            Name = sName;
            Value = sValue;
        }
        #endregion

        #region Public properties

        /// <summary>
        /// Name of the property
        /// </summary>
        [Description("Name of the property")]
        [Browsable(true)]
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Value of the property
        /// </summary>
        [Description("Value of the property")]
        [Browsable(true)]
        public string Value
        {
            get;
            set;
        }
        #endregion
    }
}
