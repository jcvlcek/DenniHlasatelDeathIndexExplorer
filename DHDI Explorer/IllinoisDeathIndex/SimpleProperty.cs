using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Genealogy
{
    class SimpleProperty : IProperty
    {
        #region Private members
        private string mName = string.Empty;
        private string mValue = string.Empty;
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
            mName = sName;
            mValue = sValue;
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
            get { return mName; }
            private set { mName = value; }
        }

        /// <summary>
        /// Value of the property
        /// </summary>
        [Description("Value of the property")]
        [Browsable(true)]
        public string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }
        #endregion
    }
}
