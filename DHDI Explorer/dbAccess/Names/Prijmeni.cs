using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dbAccess
{
    public partial class Prijmeni : IJmeno
    {
        #region IJmeno interface

        public String Native { get { return this.Windows; } }

        #endregion
    }
}
