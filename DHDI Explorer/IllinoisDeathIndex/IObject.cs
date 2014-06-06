using System.Collections.Generic;
using System.ComponentModel;

namespace Genealogy
{
    /// <summary>
    /// Interface implemented by objects comprised of scalar properties and child objects
    /// </summary>
    [Description("Interface implemented by objects comprised of scalar properties and child objects" )]
    public interface IObject
    {
        /// <summary>
        /// A string identifier for the object, unique amongst the child objects of a common parent
        /// </summary>
        /// <remarks>Name is one of the two intrinsic properties of an object.  The other intrinsic property is its <see cref="Class"/></remarks>
        string Name { get; }

        /// <summary>
        /// A type identifier for the object
        /// </summary>
        /// <remarks>Class is one of the two intrinsic properties of an object.  The other intrinsic property is its <see cref="Name"/></remarks>
        string Class { get; }

        // Properties

        /// <summary>
        /// The number of extrinsic properties of the object
        /// </summary>
        /// <remarks>PropertyCount is always zero or greater</remarks>
        int PropertyCount { get; }

        /// <summary>
        /// Flags the existence of a property with the specified name
        /// </summary>
        /// <param name="sName">the name of the property to test for existence</param>
        /// <returns>true if the object contains an extrinsic property with the name <paramref name="sName"/>, otherwise false</returns>
        bool PropertyExists(string sName);

        /// <summary>
        /// Gets the i-th (zero-referenced) extrinsic property of the object
        /// </summary>
        /// <param name="iIndex"></param>
        /// <returns></returns>
        IProperty GetProperty(int iIndex);

        /// <summary>
        /// Gets the value of the property with the name <paramref name="sName"/>
        /// </summary>
        /// <param name="sName">the name of the property to query</param>
        /// <returns>the value of the <paramref name="sName"/> property, if one exists, otherwise the empty string</returns>
        string GetPropertyValue(string sName);

        /// <summary>
        /// Sets to <paramref name="sValue"/> the value of the property with the name <paramref name="sName"/>
        /// </summary>
        /// <param name="sName">the name of the property to set</param>
        /// <param name="sValue">the new value of the property</param>
        void SetPropertyValue( string sName, string sValue );

        /// <summary>
        /// Add a property to the object, with the name <paramref name="sName"/> and value <paramref name="sValue"/>
        /// </summary>
        /// <param name="sName">the name of the new property</param>
        /// <param name="sValue">the initial value of the new property</param>
        void AddProperty(string sName, string sValue);

        // Children

        IEnumerable<IObject> Children { get;  }

        /// <summary>
        /// The number of child objects contained within this object
        /// </summary>
        /// <remarks>ChildCount is always zero or greater</remarks>
        int ChildCount { get; }

        /// <summary>
        /// Flags the existence of a child object with the specified name
        /// </summary>
        /// <param name="sName">the name of the child object to test for existence</param>
        /// <returns>true if the object contains a child object with the name <paramref name="sName"/>, otherwise false</returns>
        bool ChildExists(string sName);

        /// <summary>
        /// Gets the child object with the name <paramref name="sName"/>
        /// </summary>
        /// <param name="sName">the name of the desired child object</param>
        /// <returns>the child object with the specified name</returns>
        IObject GetChild(string sName);

        /// <summary>
        /// Gets the i-th (zero-referenced) child object of the object
        /// </summary>
        /// <param name="iIndex"></param>
        /// <returns></returns>
        IObject GetChild(int iIndex);

        /// <summary>
        /// Add an object as a child to the object
        /// </summary>
        /// <param name="oNew">the child object to add</param>
        void AddChild(IObject oNew);

        /// <summary>
        /// Remove the child object with the specified name
        /// </summary>
        /// <param name="sName">the name of the child object to remove</param>
        void RemoveChild(string sName);

        /// <summary>
        /// Remove the i-th (zero-referenced) child object
        /// </summary>
        /// <param name="iIndex">the zero-referenced index of the child object to remove</param>
        void RemoveChild(int iIndex);
    }
}
