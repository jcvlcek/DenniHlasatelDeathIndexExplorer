namespace Genealogy
{
    /// <summary>
    /// Interface implemented by classes that serialize objects implementing IObject
    /// </summary>
    /// <remarks>This interface suggests the Visitor pattern for serializing IObjects</remarks>
    interface ISaveFormat
    {
        /// <summary>
        /// Serialize an IObject to a stream
        /// </summary>
        /// <param name="stream">the stream to serialize the object to</param>
        /// <param name="o">the object to be serialized</param>
        void Stream(System.IO.StreamWriter stream, IObject o);

        /// <summary>
        /// Serialize a property of an IObject to a stream
        /// </summary>
        /// <param name="stream">the stream to serialize the property to</param>
        /// <param name="p">the property to be serialized</param>
        void Stream(System.IO.StreamWriter stream, IProperty p);
    }
}
