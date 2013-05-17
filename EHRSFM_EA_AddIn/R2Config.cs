using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace R4C_EHRSFM_EA_AddIn
{
    public class R2Config
    {
        private Dictionary<string, Color> sectionColors;
        private Dictionary<string, int> sectionColorsInt;
        private Dictionary<string, string> sectionTitles;

        public R2Config()
        {
            // Categories and colors
            sectionColors = new Dictionary<string, Color>();
            sectionColors["OV"] = Color.FromArgb(0x99, 0xCC, 0xFF);
            sectionColors["CP"] = Color.FromArgb(0x99, 0xFF, 0x99);
            sectionColors["CPS"] = Color.FromArgb(0xD0, 0xEB, 0xBF);
            sectionColors["POP"] = Color.FromArgb(0xFF, 0xE3, 0x89);
            sectionColors["AS"] = Color.FromArgb(0xCD, 0xAB, 0xE7);
            sectionColors["RI"] = Color.FromArgb(0xE2, 0xC4, 0xA6);
            sectionColors["TI"] = Color.FromArgb(0xFF, 0xA3, 0xA3);

            // Categories and colors
            sectionColorsInt = new Dictionary<string, int>();
            sectionColorsInt["OV"] = 0x99CCFF;
            sectionColorsInt["CP"] = 0x99FF99;
            sectionColorsInt["CPS"] = 0xD0EBBF;
            sectionColorsInt["POP"] = 0xFFE389;
            sectionColorsInt["AS"] = 0xCDABE7;
            sectionColorsInt["RI"] = 0xE2C4A6;
            sectionColorsInt["TI"] = 0xFFA3A3;

            sectionTitles = new Dictionary<string, string>();
            sectionTitles["OV"] = "Overarching";
            sectionTitles["CP"] = "Care Provision";
            sectionTitles["CPS"] = "Care Provision Support";
            sectionTitles["POP"] = "Population Health Support";
            sectionTitles["AS"] = "Administration Support";
            sectionTitles["RI"] = "Record Infrastructure";
            sectionTitles["TI"] = "Trust Infrastructure";
        }

        public Color getSectionColor(string ID, Color defaultColor)
        {
            int idx = ID.IndexOf('.');
            if (idx == -1) idx = ID.Length;
            string sectionID = ID.Substring(0, idx);
            Color color = defaultColor;
            if (sectionColors.ContainsKey(sectionID))
            {
                color = sectionColors[sectionID];
            }
            return color;
        }

        public int? getSectionColorInt(string ID)
        {
            int idx = ID.IndexOf('.');
            if (idx == -1) idx = ID.Length;
            string sectionID = ID.Substring(0, idx);
            int? color = null;
            if (sectionColorsInt.ContainsKey(sectionID))
            {
                color = sectionColorsInt[sectionID];
            }
            return color;
        }

        public string getSectionTitle(string ID)
        {
            int idx = ID.IndexOf('.');
            if (idx == -1) idx = ID.Length;
            string sectionID = ID.Substring(0, idx);
            string title = "?";
            if (sectionTitles.ContainsKey(sectionID))
            {
                title = sectionTitles[sectionID];
            }
            return title;
        }
    }
}
