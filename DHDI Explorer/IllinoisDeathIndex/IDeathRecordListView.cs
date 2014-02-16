using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using dbAccess;

namespace Genealogy
{
    public partial class Form1
    {
        #region Public methods

        public void ClearRecords()
        {
            lvHits.Items.Clear();
            lvHits.Columns.Clear();

            lvHits.Columns.Add("Full name", "Full name");
            lvHits.Columns.Add("Death date", "Death date");
            lvHits.Columns.Add("Filing date", "Filing date");
            lvHits.Columns.Add("First name", "First name");
            lvHits.Columns.Add("Middle name", "Middle name");
            lvHits.Columns.Add("Last name", "Last name");
            lvHits.Columns.Add("Age (Yrs)", "Age (Yrs)");
            lvHits.Columns.Add("Gender", "Gender");
            lvHits.Columns.Add("Race", "Race");
            lvHits.Columns.Add("Cert #", "Cert #");
            lvHits.Columns.Add("City", "City");
            lvHits.Columns.Add("County", "County");
            lvHits.Columns.Add("Volume", "Volume");
            lvHits.Columns.Add("Page", "Page");
        }

        public void DisplayRecords(List<IDeathRecord> lRecords, bool bClearBeforeDisplaying = true)
        {
            if (bClearBeforeDisplaying)
            {
                ClearRecords();
            }

            foreach (IDeathRecord drNext in lRecords)
            {
                string sName = drNext.LastName + ", " + drNext.FirstName;
                ListViewItem itmNext = lvHits.Items.Add( sName, "NoMatch" );
                itmNext.Tag = drNext;
                itmNext.SubItems.Add(drNext.DeathDate.ToShortDateString());
                itmNext.SubItems.Add(drNext.FilingDate.ToShortDateString());
                itmNext.SubItems.Add(drNext.FirstName);
                itmNext.SubItems.Add(drNext.MiddleName);
                itmNext.SubItems.Add(drNext.LastName);
                itmNext.SubItems.Add(drNext.AgeInYears.ToString());
                itmNext.SubItems.Add(drNext.Gender.ToString());
                itmNext.SubItems.Add(drNext.Race.ToString());
                itmNext.SubItems.Add(drNext.CertificateNumber.ToString());
                itmNext.SubItems.Add(drNext.City);
                itmNext.SubItems.Add(drNext.County);
                itmNext.SubItems.Add(drNext.Volume);
                itmNext.SubItems.Add(drNext.PageNumber.ToString());
            }
        }

        public void DisplayRecord(IDeathRecord drTarg, bool bClearBeforeDisplaying = true)
        {
            DisplayRecords(new List<IDeathRecord>(new IDeathRecord[] { drTarg }), bClearBeforeDisplaying);
        }

        #endregion
    }
}
