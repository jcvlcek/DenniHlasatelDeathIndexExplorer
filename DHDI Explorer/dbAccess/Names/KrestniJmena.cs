using System;

namespace dbAccess
{
    /// <summary>
    /// Represents a Czech first (given, Christian) name in the SQL Server database
    /// </summary>
    public partial class KrestniJmena : IJmeno
    {
        #region IJmeno interface implementation

        /// <summary>
        /// Native form of the first / given / Christian name (with diacriticals), as stored in the SQL Server database
        /// </summary>
        /// <remarks>The native form may be in code page, but will be made into UTF-8 ultimately</remarks>
        public String Native { get { return CodePage; } }

        public int Count { get { return MaleCount + FemaleCount; } }

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
