using System.Collections.Generic;
using System.ComponentModel;

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

        /// <summary>
        /// The underlying object being wrapped
        /// </summary>
        private readonly IObject _mTarget;

        #endregion

        #region Constructors

        /// <summary>
        /// Create an alias wrapper around a specified target object
        /// </summary>
        /// <param name="target">The target object to alias (wrap)</param>
        protected AliasObject(IObject target)
        {
            _mTarget = target;
        }

        #endregion

        #region Public properties

        public string Name { get { return _mTarget.Name; } }

        public string Class { get { return _mTarget.Class; } }

        public int PropertyCount { get { return _mTarget.PropertyCount; } }

        public int ChildCount { get { return _mTarget.ChildCount; } }

        public IEnumerable<IObject> Children { get { return _mTarget.Children; } }

        #endregion

        #region Public methods

        public bool PropertyExists(string sName)
        {
            return _mTarget.PropertyExists(sName);
        }

        public void AddProperty(string sName, string sValue)
        {
            _mTarget.AddProperty(sName, sValue);
        }

        public string GetPropertyValue(string sName)
        {
            return _mTarget.GetPropertyValue(sName);
        }

        public void SetPropertyValue(string sName, string sValue)
        {
            _mTarget.SetPropertyValue(sName, sValue);
        }

        public IProperty GetProperty(int i)
        {
            return _mTarget.GetProperty(i);
        }

        public bool ChildExists(string sName)
        {
            return _mTarget.ChildExists(sName);
        }

        public void AddChild(IObject oNew)
        {
            _mTarget.AddChild(oNew);
        }

        public IObject GetChild(string sName)
        {
            return _mTarget.GetChild(sName);
        }

        public IObject GetChild(int iIndex)
        {
            return _mTarget.GetChild(iIndex);
        }

        public void RemoveChild(string sName)
        {
            _mTarget.RemoveChild(sName);
        }

        public void RemoveChild(int iIndex)
        {
            _mTarget.RemoveChild(iIndex);
        }
        #endregion

    }
}
