using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Genealogy
{
    /// <summary>
    /// Base class implementation of an "alias" (wrapper or decorator) to an existing <see cref="IObject"/> instance
    /// </summary>
    [Description("Base class implementation of an \"alias\" to an existing IObject instance")]
    [DefaultProperty("Name")]
    public abstract class AliasObject : IObject
    {
        #region Private members
        private IObject mTarget = null;
        #endregion

        #region Constructors

        /// <summary>
        /// Create an alias wrapper around a specified target object
        /// </summary>
        /// <param name="target">The target object to alias (wrap)</param>
        public AliasObject(IObject target)
        {
            mTarget = target;
        }

        #endregion

        #region Public properties

        public string Name { get { return mTarget.Name; } }

        public string Class { get { return mTarget.Class; } }

        public int PropertyCount { get { return mTarget.PropertyCount; } }

        public int ChildCount { get { return mTarget.ChildCount; } }

        public IEnumerable<IObject> Children { get { return mTarget.Children; } }

        #endregion

        #region Public methods

        public bool PropertyExists(string sName)
        {
            return mTarget.PropertyExists(sName);
        }

        public void AddProperty(string sName, string sValue)
        {
            mTarget.AddProperty(sName, sValue);
        }

        public string GetPropertyValue(string sName)
        {
            return mTarget.GetPropertyValue(sName);
        }

        public void SetPropertyValue(string sName, string sValue)
        {
            mTarget.SetPropertyValue(sName, sValue);
        }

        public IProperty GetProperty(int i)
        {
            return mTarget.GetProperty(i);
        }

        public bool ChildExists(string sName)
        {
            return mTarget.ChildExists(sName);
        }

        public void AddChild(IObject oNew)
        {
            mTarget.AddChild(oNew);
        }

        public IObject GetChild(string sName)
        {
            return mTarget.GetChild(sName);
        }

        public IObject GetChild(int iIndex)
        {
            return mTarget.GetChild(iIndex);
        }

        public void RemoveChild(string sName)
        {
            mTarget.RemoveChild(sName);
        }

        public void RemoveChild(int iIndex)
        {
            mTarget.RemoveChild(iIndex);
        }
        #endregion

    }
}
