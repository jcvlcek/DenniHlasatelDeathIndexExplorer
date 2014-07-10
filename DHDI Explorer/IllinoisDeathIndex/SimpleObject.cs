using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Genealogy
{
    /// <summary>
    /// Basic object class, containing both properties and child objects
    /// </summary>
    [Description( "Basic object class, containing both properties and child objects" )]
    [DefaultProperty("Name")]
    class SimpleObject : IObject
    {
        #region Private members
        private string _name;
        private readonly Dictionary<string, IProperty> _propertiesList = new Dictionary<string, IProperty>();
        private readonly Dictionary<string, IObject> _childrenList = new Dictionary<string, IObject>();
        private readonly List<string> _propertiesNameList = new List<string>();
        private readonly List<string> _childNamesList = new List<string>();
        private const string ObjectTag = "object";
        private const string ClassTag = "Class";
        #endregion

        #region Constructors

        /// <summary>
        /// Initialize an object with the specified name and class
        /// </summary>
        /// <param name="sName">The name to assign to the object</param>
        /// <param name="sClass">The class of the object.  This argument may be null or empty;
        /// the object class becomes <see cref="string.Empty"/> in either case
        /// </param>
        /// <exception cref="ArgumentNullException">If <see cref="sName"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <see cref="sName"/> is empty</exception>
        private void Initialize(string sName, string sClass)
        {
            if (sName == null)
                throw new ArgumentNullException("Null string cannot be used as an object name");
            if (sName.Length < 1)
                throw new ArgumentOutOfRangeException("Empty string cannot be used as an object name");
            _name = sName;
            if (!string.IsNullOrEmpty(sClass))
                AddProperty(ClassTag, sClass);
        }

        /// <summary>
        /// Create a new object with the specified name,
        /// and the default class value
        /// </summary>
        /// <param name="sName">The name of the new object</param>
        /// <exception cref="ArgumentNullException">If <see cref="sName"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <see cref="sName"/> is empty</exception>
        public SimpleObject(string sName)
        {
            Initialize(sName, string.Empty); 
        }

        /// <summary>
        /// Create a new object with the specified name and class values
        /// </summary>
        /// <param name="sName">The name of the new object</param>
        /// <param name="sClass">The class value of the new object</param>
        /// <exception cref="ArgumentNullException">If <see cref="sName"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <see cref="sName"/> is empty</exception>
        public SimpleObject(string sName, string sClass)
        {
            Initialize(sName, sClass);
        }

        /// <summary>
        /// Create a new object by copying an existing object
        /// </summary>
        /// <param name="oSource">The object to be copied</param>
        public SimpleObject(IObject oSource)
        {
            Initialize(oSource.Name, oSource.Class);
            CopyFrom(oSource);
        }

        /// <summary>
        /// Create a new object by copying an existing object,
        /// where the copy has a different name than the original
        /// </summary>
        /// <param name="sName">The name assigned to the copy</param>
        /// <param name="oSource">The object to be copied</param>
        public SimpleObject(string sName, IObject oSource)
        {
            Initialize(sName, oSource.Class);
            CopyFrom(oSource);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The name of the object
        /// </summary>
        [Description( "Name of the object" )]
        [Browsable(true)]
        [DefaultValue("ObjectName")]
        public string Name
        { 
            get { return _name; }
            protected set { _name = value; }
        }

        /// <summary>
        /// Name of the Class that this object implements
        /// </summary>
        [Description("Name of the class that this object implements")]
        [Browsable(true)]
        [DefaultValue(ObjectTag)]
        public string Class
        {
            get
            {
                if (PropertyExists(ClassTag))
                    return GetPropertyValue(ClassTag);
                else
                    return ObjectTag;
            }
        }

        /// <summary>
        /// Number of properties defined on this objects
        /// </summary>
        [Description("Number of properties defined on this object")]
        public int PropertyCount
        {
            get { return _propertiesList.Count; }
        }

        /// <summary>
        /// Number of child objects contained by this object
        /// </summary>
        [Description("Number of child objects contained by this object")]
        public int ChildCount 
        {
            get { return _childrenList.Count; }
        }

        /// <summary>
        /// Child (nested) objects within this object
        /// </summary>
        public IEnumerable<IObject> Children
        {
            get { return _childrenList.Values; }
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Add a new property, with the specified name and value, to the object
        /// </summary>
        /// <param name="sName">The name of the property to add</param>
        /// <param name="sValue">The initial value of the new property</param>
        /// <exception cref="ArgumentNullException">If <see cref="sName"/> is null </exception>
        /// <exception cref="ArgumentOutOfRangeException">If <see cref="sName"/> is empty</exception>
        /// <exception cref="ArgumentException">If a property with name <see cref="sName"/> already exists in the object</exception>
        public void AddProperty( string sName, string sValue )
        {
            if ( PropertyExists( sName ) )
                throw new ArgumentException( "Property \"" + sName + "\" already exists in object \"" + Name + "\"" );
            _propertiesList.Add( sName, new SimpleProperty( sName, sValue ) );
            _propertiesNameList.Add(sName);
        }

        /// <summary>
        /// Indicates whether the object contains a property with the specified name
        /// </summary>
        /// <param name="sName">The name to search for</param>
        /// <returns><code>false</code> if sName is null or empty
        /// <code>true</code> if the object contains a property with the name <see cref="sName"/>
        /// <code>false</code>otherwise</returns>
        public bool PropertyExists(string sName)
        {
            if (string.IsNullOrEmpty(sName))
                return false;
            return _propertiesList.ContainsKey(sName);
        }

        /// <summary>
        /// Gets the value of the property with the specified name
        /// </summary>
        /// <param name="sName">The name of the property</param>
        /// <returns>The value of the property named <see cref="sName"/>, if that property exists,
        /// <see cref="string.Empty"/> if no property exists with that name</returns>
        public string GetPropertyValue(string sName)
        {
            if (PropertyExists(sName))
                return _propertiesList[sName].Value;
            else
                return string.Empty;
        }

        /// <summary>
        /// Gets the property at (zero-referenced) index value <see cref="iIndex"/>
        /// </summary>
        /// <param name="iIndex">The zero-referenced index of the desired property</param>
        /// <returns>The property at the desired index location</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <see cref="iIndex"/> is less than zero,
        /// or if <see cref="iIndex"/> is >= <see cref="PropertyCount"/></exception>
        public IProperty GetProperty(int iIndex)
        {
            return _propertiesList[_propertiesNameList[iIndex]];
        }

        /// <summary>
        /// Change the value of the specified property
        /// </summary>
        /// <param name="sName">The name of the property whose value to change</param>
        /// <param name="sValue">The new value of the property</param>
        /// <exception cref="ArgumentException">If no property exists with name <see cref="sName"/></exception>
        public void SetPropertyValue(string sName, string sValue)
        {
            _propertiesList[sName].Value = sValue;
        }

        // Child object support

        /// <summary>
        /// Flags whether the specified child object exists
        /// </summary>
        /// <param name="sName">The name of the child object</param>
        /// <returns>True if a child object with the specified name exists in this object,
        /// False otherwise</returns>
        public bool ChildExists(string sName)
        {
            return _childrenList.ContainsKey(sName);
        }

        /// <summary>
        /// Gets the child object with the specified name
        /// </summary>
        /// <param name="sName">The name of the desired child object</param>
        /// <returns>The child object with the specified name</returns>
        /// <exception cref="KeyNotFoundException">If no child exists with the name <see cref="sName"/></exception>
        /// <exception cref="ArgumentNullException">If <see cref="sName"/> is null</exception>
        public IObject GetChild(string sName)
        {
            return _childrenList[sName];
        }

        /// <summary>
        /// Gets the child object with the (zero-referenced) index <see cref="iIndex"/>
        /// </summary>
        /// <param name="iIndex">The index of the child object to retrieve</param>
        /// <returns>The child object at the specified index value</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <see cref="iIndex"/> is less than zero,
        /// or >= <see cref="ChildCount"/></exception>
        public IObject GetChild(int iIndex)
        {
            return GetChild(_childNamesList[iIndex]);
        }

        /// <summary>
        /// Add the specified object as a child to this object
        /// The SimpleObject class adds a copy of the specified object,
        /// rather than the object itself
        /// </summary>
        /// <param name="oNew">The child object to add</param>
        /// <exception cref="ArgumentException">If a child object with the same name as <see cref="oNew"/> already exists</exception>
        public void AddChild(IObject oNew)
        {
            if (_childrenList.ContainsKey(oNew.Name))
                throw new ArgumentException("Cannot add child object: A child object with name \"" + oNew.Name + "\" already exists in object \"" + Name + "\"");
            _childrenList.Add(oNew.Name, oNew);
            _childNamesList.Add(oNew.Name);
        }

        /// <summary>
        /// Remove the child object with the specified name
        /// </summary>
        /// <param name="sName">The name of the child object to remove</param>
        /// <exception cref="ArgumentException">If no child named <see cref="sName"/> exists</exception>
        public void RemoveChild(string sName)
        {
            _childrenList.Remove(sName);
            _childNamesList.Remove(sName);
        }

        /// <summary>
        /// Remove the child object at (zero-referenced) index location <see cref="iIndex"/>
        /// </summary>
        /// <param name="iIndex">The (zero-referenced) index location of the child object to remove</param>
        /// <exception cref="ArgumentOutOfRangeException">If <see cref="iIndex"/> is less than zero or >= <see cref="ChildCount"/></exception>
        public void RemoveChild(int iIndex)
        {
            RemoveChild( _childNamesList[ iIndex ] );
        }

        /// <summary>
        /// Serializes the object in the specified format
        /// </summary>
        /// <param name="ostream">the output stream to serialize the object to</param>
        /// <param name="format">the format to save the object in</param>
        /// <remarks>Enables the visitor pattern for saving objects in differing formats</remarks>
        public void SaveAs( System.IO.StreamWriter ostream, ISaveFormat format )
        {
            format.Stream(ostream, this);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Copy another object's properties and children
        /// into this object
        /// </summary>
        /// <param name="oSource">The object to be copied</param>
        private void CopyFrom(IObject oSource)
        {
            for (int i = 0; i < oSource.PropertyCount; ++i)
            {
                IProperty pNext = oSource.GetProperty(i);
                if (PropertyExists(pNext.Name))
                    SetPropertyValue(pNext.Name, pNext.Value);
                else
                    AddProperty(pNext.Name, pNext.Value);
            }

            for (int i = 0; i < oSource.ChildCount; ++i)
            {
                AddChild(new SimpleObject(oSource.GetChild(i)));
            }
        }

        #endregion

    }
}
