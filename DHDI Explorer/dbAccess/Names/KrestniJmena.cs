using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dbAccess
{
    public partial class KrestniJmena : IJmeno
    {
        #region IJmeno interface implementation

        public String Native { get { return this.CodePage; } }

        public int Count { get { return this.MaleCount + this.FemaleCount; } }

        /// <summary>
        /// Rank of this given name in a list of all given names
        /// </summary>
        /// <remarks>Should use an algorithm that counts all names (both male and female)</remarks>
        public int Rank { 
            get 
            {
                if (MaleCount > FemaleCount)
                    return MaleIndex;
                else if (MaleCount == FemaleCount)
                    return MaleIndex > FemaleIndex ? MaleIndex : FemaleIndex;
                else
                    return FemaleIndex; 
            } 
        }
        #endregion
    }
}
