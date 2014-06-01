using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genealogy
{
    class JsonSaveFormat : ISaveFormat
    {
        public void Stream( System.IO.StreamWriter stream, IObject o )
        {
            try
            {
                Newtonsoft.Json.JsonWriter jw = new Newtonsoft.Json.JsonTextWriter(stream);
                jw.Formatting = Newtonsoft.Json.Formatting.Indented;
                Newtonsoft.Json.JsonSerializer js = new Newtonsoft.Json.JsonSerializer();
                js.Serialize(jw, o);
            }
            catch (Exception)
            {
                throw new NotImplementedException("Method Stream not yet implemented for objects like \"" + o.Name + "\" of class \"SimpleObject\"");
            } 
        }

        public void Stream(System.IO.StreamWriter stream, IProperty p)
        {
            try
            {
                Newtonsoft.Json.JsonSerializer jsonSerializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
                jsonSerializer.Serialize(stream, p);
            }
            catch (Exception)
            {
                throw new NotImplementedException("Method Stream not yet implemented for properties like \"" + p.Name + "\" of class \"SimpleProperty\"");
            }
        }
    }
}
