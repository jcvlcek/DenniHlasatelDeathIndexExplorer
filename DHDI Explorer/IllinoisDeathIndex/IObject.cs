using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Genealogy
{
    /// <summary>
    /// Interface implemented by objects comprised of scalar properties and child objects
    /// </summary>
    [Description("Interface implemented by objects comprised of scalar properties and child objects" )]
    public interface IObject
    {
        string Name { get; }
        string Class { get; }

        // Properties
        int PropertyCount { get; }
        bool PropertyExists(string sName);
        IProperty GetProperty(int iIndex);
        string GetPropertyValue(string sName);
        void SetPropertyValue( string sName, string sValue );
        void AddProperty(string sName, string sValue);

        // Children
        int ChildCount { get; }
        bool ChildExists(string sName);
        IObject GetChild(string sName);
        IObject GetChild(int iIndex);
        void AddChild(IObject oNew);
        void RemoveChild(string sName);
        void RemoveChild(int iIndex);
    }
}
