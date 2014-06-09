using System;

namespace dbAccess
{
    /// <summary>
    /// Interface implemented by class representing a Czech-language name
    /// </summary>
    public interface IJmeno
    {
        /// <summary>
        /// Native form of the name (with diacriticals)
        /// </summary>
        /// <remarks>The native form may be in code page, but will be made into UTF-8 ultimately</remarks>
        String Native { get; }

        /// <summary>
        /// ASCII string representing the name, using HTML entities for non-ASCII characters
        /// </summary>
        string Web { get; }

        /// <summary>
        /// ASCII string representing the name, with all diacriticals removed
        /// and all other non-ASCII characters replaced by a suitable ASCII equivalent
        /// </summary>
        string PlainText { get; }

        /// <summary>
        /// Number of instances of this name in the database
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Rank of this name among all first/last names in the database
        /// </summary>
        int Rank { get; }
    }
}
