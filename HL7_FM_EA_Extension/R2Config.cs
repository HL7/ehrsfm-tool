﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using HL7_FM_EA_Extension.R2ModelV2.Base;

namespace HL7_FM_EA_Extension
{
    public class R2Config
    {
        private Dictionary<string, Color> sectionColors;
        private Dictionary<string, int> sectionColorsInt;
        private Dictionary<string, string> sectionTitles;

        private static R2Config _config;
        public static R2Config config
        {
            get
            {
                if (_config == null)
                {
                    _config = new R2Config();
                }
                return _config;
            }
        }

        private R2Config()
        {
            // EHR Categories and colors
            sectionColors = new Dictionary<string, Color>();
            sectionColors["OV"] = Color.FromArgb(0x99, 0xCC, 0xFF);
            sectionColors["CP"] = Color.FromArgb(0x99, 0xFF, 0x99);
            sectionColors["CPS"] = Color.FromArgb(0xD0, 0xEB, 0xBF);
            sectionColors["POP"] = Color.FromArgb(0xFF, 0xE3, 0x89);
            sectionColors["AS"] = Color.FromArgb(0xCD, 0xAB, 0xE7);
            sectionColors["RI"] = Color.FromArgb(0xE2, 0xC4, 0xA6);
            sectionColors["TI"] = Color.FromArgb(0xFF, 0xA3, 0xA3);

            sectionColorsInt = new Dictionary<string, int>();
            sectionColorsInt["OV"] = 0xFFCC99;
            sectionColorsInt["CP"] = 0x99FF99;
            sectionColorsInt["CPS"] = 0xBFEBD0;
            sectionColorsInt["POP"] = 0x89E3FF;
            sectionColorsInt["AS"] = 0xE7ABCD;
            sectionColorsInt["RI"] = 0xA6C4E2;
            sectionColorsInt["TI"] = 0xA3A3FF;

            sectionTitles = new Dictionary<string, string>();
            sectionTitles["OV"] = "Overarching";
            sectionTitles["CP"] = "Care Provision";
            sectionTitles["CPS"] = "Care Provision Support";
            sectionTitles["POP"] = "Population Health Support";
            sectionTitles["AS"] = "Administration Support";
            sectionTitles["RI"] = "Record Infrastructure";
            sectionTitles["TI"] = "Trust Infrastructure";

            // PHR Categories and colors
            sectionColors["PH"] = Color.FromArgb(0x99, 0xCC, 0xFF); // 153,204,255
            sectionColors["S"] = Color.FromArgb(0xCC, 0xFF, 0xCC); // 204,255,204
            sectionColors["IN"] = Color.FromArgb(0xFF, 0xFF, 0x99); // 255,255,153

            sectionColorsInt["PH"] = 0xFFCC99;
            sectionColorsInt["S"] = 0xCCFFCC;
            sectionColorsInt["IN"] = 0x99FFFF;

            sectionTitles["PH"] = "Personal Health";
            sectionTitles["S"] = "Supportive";
            sectionTitles["IN"] = "Information Infrastructure";
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

        // N.B. element.Update needed after this call!
        public void updateStyle(EA.Element element)
        {
            // For Criteria: apply border based on Optionality
            EA.TaggedValue tv = (EA.TaggedValue)element.TaggedValues.GetByName(R2Const.TV_OPTIONALITY);
            if (tv != null)
            {
                string optionality = tv.Value;
                if ("SHALL".Equals(optionality))
                {
                    element.SetAppearance(1/*Base*/, 3/*Border width*/, 3);
                }
                else
                {
                    element.SetAppearance(1/*Base*/, 3/*Border width*/, 1);
                }
            }
            // Strip whitespaces from names
            element.Name = element.Name.Trim();
            // Update bgcolor based on Section
            string ID;
            switch (element.Stereotype)
            {
                case R2Const.ST_SECTION:
                    ID = element.Alias;
                    break;
                default:
                    ID = element.Name;
                    break;
            }
            int? sectionColor = getSectionColorInt(ID);
            if (sectionColor != null)
            {
                element.SetAppearance(1/*Base*/, 0/*BGCOLOR*/, (int)sectionColor);
            }
            // else: Warning; unknown Section
        }
    }
}
