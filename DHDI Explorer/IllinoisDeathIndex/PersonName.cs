using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dbAccess;

namespace Genealogy
{
    /// <summary>
    /// <para></para>Represents one of the components of a person's full name,
    /// e.g. first (given) name, family name (surname), etc.
    /// </summary>
    /// <typeparam name="T">the type of objects that will be stored in the name cache</typeparam>
    /// <remarks>This abstract class provides default implementation of generic methods,
    /// and caches names for faster lookup when multiple lookups are performed</remarks>
    public abstract class PersonName< T >
    {
        #region Public members
        /// <summary>
        /// Provides a representation of a name in the three following forms:
        /// <list type="bullet">
        /// <item>Native (code page)</item>
        /// <item>Web (HTML)</item>
        /// <item>Plain text (7-bit ASCII equivalent)</item>
        /// </list>
        /// </summary>
        public class NativeForm : Tuple<String, string, string>
        {
            /// <summary>
            /// Creates a new instance of the <see cref="NativeForm"/> class
            /// </summary>
            /// <param name="sNative">The name in its native spelling (with diacriticals)</param>
            /// <param name="sWeb">The HTML version of the name</param>
            /// <param name="sPlainText">The name with diacriticals removed</param>
            public NativeForm(String sNative, string sWeb, string sPlainText) : base( sNative, sWeb, sPlainText )
            {
            }

            /// <summary>
            /// The original (native code page) form of the name string
            /// </summary>
            public String Value { get { return Item1; } }
            
            /// <summary>
            /// A web- (HTML)-compatible form of the name string
            /// </summary>
            public string Web { get { return Item2; } }

            /// <summary>
            /// A plain-text (7-bit ASCII) rendering of the name string
            /// </summary>
            public string PlainText { get { return Item3; } }
        }
        #endregion

        #region Private members
        protected static Dictionary<string, PersonName< T >> mCache = new Dictionary<string, PersonName< T >>();
        private string mOriginal;
        protected List<string> mEquivalents = null;
        protected List<String> mNativeForms = null;
        protected List<NativeForm> mAlternateForms = null;
        private int mCount = 0;
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <see cref="PersonName"/> object corresponding to the argument string,
        /// and binds that object into the name cache
        /// </summary>
        /// <param name="dcTarg">The data context to search for forms equivalent to this name</param>
        /// <param name="sName"></param>
        protected PersonName(string sName)
        {
            mOriginal = sName;
        }

        /// <summary>
        /// Gets a <see cref="PersonName"/> corresponding to argument string
        /// </summary>
        /// <param name="sName">The name to search for</param>
        /// <returns>A cached <see cref="PersonName"/>, if one has already been created for <see cref="sName"/>,
        /// otherwise a new <see cref="PersonName"/> created for the argument string</returns>
        public static PersonName< T > Get( string sName )
        {
            throw new NotImplementedException("Get method not implemented in the base class PersonName< T>");
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The text of the given name
        /// </summary>
        public string Value
        {
            get { return mOriginal; }
        }

        /// <summary>
        /// Gets a list of spellings of this name in the original (native) language.
        /// Typically, native forms differ from "English" spellings only in the use of diacriticals
        /// </summary>
        public List<String> NativeForms
        {
            get
            {
                if (mNativeForms == null)
                {
                    mNativeForms = new List<String>();

                    foreach (NativeForm nfNext in AlternateForms)
                    {
                        string sNative = nfNext.Value;
                        if ( !( string.IsNullOrEmpty( sNative ) || mNativeForms.Contains( nfNext.Value ) ) )
                            mNativeForms.Add(nfNext.Value);
                    }
                }

                return new List<String>(mNativeForms.AsEnumerable<String>());
            }
        }

        /// <summary>
        /// Gets a list of alternate, plain-text forms of the name
        /// The first item in the returned list is always the original name
        /// </summary>
        public List<string> EquivalentForms
        {
            get 
            {
                if (mEquivalents == null)
                {
                    mEquivalents = new List<string>();

                    foreach (NativeForm nfNext in AlternateForms)
                    {
                        if (!mEquivalents.Contains(nfNext.PlainText))
                            mEquivalents.Add(nfNext.PlainText);
                    }
                }

                return new List<string>(mEquivalents.AsEnumerable<string>());
            }
        }

        /// <summary>
        /// Gets a direct transliteration of the given name, if one exists
        /// Returns the given name in its native language, if a direct transliteration exists,
        /// otherwise the empty string is returned
        /// </summary>
        public string EquivalentNativeForm
        {
            get { return AlternateForms[ 0 ].Value; }
        }

        /// <summary>
        /// Total number of individuals in the census with this name
        /// </summary>
        virtual public int TotalCount
        {
            get { return mCount; }
            protected set { mCount = value; }
        }

        #endregion

        #region Private properties

        /// <summary>
        /// A comprehensive list of all available alternate forms of this name.
        /// This includes forms that match either a plain text or a "native" spelling of the name.
        /// The returned list will always have at least one entry, and the first entry in the list
        /// will always have a <see cref="NativeForm.PlainText"/> property equal to <see cref="Value"/>
        /// </summary>
        public abstract List<NativeForm> AlternateForms
        {
            get;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Generate a listing of all names matching a plain-text name string
        /// </summary>
        /// <param name="sPlainTextName">the plain-text name string to match</param>
        /// <returns>a list of names matching <paramref name="sPlainTextName"/>.  This list may be empty if no matching names are found.</returns>
        /// <remarks>Multiple matches are possible, due to varying use of diacritical marks in names.</remarks>
        public static List<PersonName<T>> MatchToPlainTextName(string sPlainTextName)
        {
            throw new NotImplementedException("MatchToPlainText not implemented in base class PersonName< T >");
        }

        #endregion

        #region Private methods
        #endregion
    }
}
