using System;

namespace Genealogy
{
    /// <summary>
    /// IObject serializer for JSON (JavaScript Object Notation) format
    /// </summary>
    class JsonSaveFormat : ISaveFormat
    {
        /// <summary>
        /// Stream an IObject to JSON
        /// </summary>
        /// <param name="stream">the stream to serialize the object to</param>
        /// <param name="o">the oject to be serialized</param>
        public void Stream( System.IO.StreamWriter stream, IObject o )
        {
            try
            {
                Newtonsoft.Json.JsonWriter jw = new Newtonsoft.Json.JsonTextWriter(stream);
                jw.Formatting = Newtonsoft.Json.Formatting.Indented;
                var js = new Newtonsoft.Json.JsonSerializer();
                js.Serialize(jw, o);
            }
            catch (Exception)
            {
                throw new NotImplementedException("Method Stream not yet implemented for objects like \"" + o.Name + "\" of class \"SimpleObject\"");
            } 
        }

        /// <summary>
        /// Stream an IProperty to JSON
        /// </summary>
        /// <param name="stream">the stream to serialize the property to</param>
        /// <param name="p">the property to be serialized</param>
        public void Stream(System.IO.StreamWriter stream, IProperty p)
        {
            try
            {
                var jsonSerializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
                jsonSerializer.Serialize(stream, p);
            }
            catch (Exception)
            {
                throw new NotImplementedException("Method Stream not yet implemented for properties like \"" + p.Name + "\" of class \"SimpleProperty\"");
            }
        }
    }
}
