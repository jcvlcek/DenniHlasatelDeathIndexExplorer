using System;
using System.Collections.Generic;
using System.Linq;

namespace Genealogy
{
    /// <summary>
    /// Represents one of the components of a person's full name,
    /// e.g. first (given) name, family name (surname), etc.
    /// </summary>
    /// <typeparam name="T">the type of objects that will be stored in the name cache</typeparam>
    /// <remarks>This abstract class provides default implementation of generic methods,
    /// and caches names for faster lookup when multiple lookups are performed</remarks>
    public abstract class PersonName< T > where T : PersonName< T >
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
        protected static Dictionary<string, PersonName< T >> NameCache = new Dictionary<string, PersonName< T >>();
        private readonly string _originalName;
        protected List<string> Equivalents = null;
        protected List<String> NativeFormsList = null;
        protected List<NativeForm> AlternateFormsList = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <see cref="PersonName{T}"/> object corresponding to the argument string,
        /// and binds that object into the name cache
        /// </summary>
        /// <param name="sName"></param>
        protected PersonName(string sName)
        {
            _originalName = sName;
        }

        /// <summary>
        /// Gets a PersonName corresponding to argument string
        /// </summary>
        /// <param name="sName">The name to search for</param>
        /// <returns>A cached <see cref="PersonName{T}"/>, if one has already been created for <see cref="sName"/>,
        /// otherwise a new <see cref="PersonName{T}"/> created for the argument string</returns>
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
            get { return _originalName; }
        }

        /// <summary>
        /// Gets a list of spellings of this name in the original (native) language.
        /// Typically, native forms differ from "English" spellings only in the use of diacriticals
        /// </summary>
        public List<String> NativeForms
        {
            get
            {
                if (NativeFormsList == null)
                {
                    NativeFormsList = new List<String>();

                    foreach (NativeForm nfNext in AlternateForms)
                    {
                        string sNative = nfNext.Value;
                        if ( !( string.IsNullOrEmpty( sNative ) || NativeFormsList.Contains( nfNext.Value ) ) )
                            NativeFormsList.Add(nfNext.Value);
                    }
                }

                return new List<String>(NativeFormsList.AsEnumerable());
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
                if (Equivalents == null)
                {
                    Equivalents = new List<string>();

                    foreach (NativeForm nfNext in AlternateForms)
                    {
                        if (!Equivalents.Contains(nfNext.PlainText))
                            Equivalents.Add(nfNext.PlainText);
                    }
                }

                return new List<string>(Equivalents.AsEnumerable());
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
        public virtual int TotalCount { get; protected set; }

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
