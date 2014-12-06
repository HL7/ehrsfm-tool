using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HL7_FM_EA_Extension
{
    class FileUtil
    {
        public static string showFileDialog(string title, string filter, string fileName, bool open)
        {
            FileDialog dialog;
            if (open)
            {
                dialog = new OpenFileDialog();
            }
            else
            {
                dialog = new SaveFileDialog();
            }
            dialog.Filter = filter;
            dialog.Title = title;
            if (!string.IsNullOrEmpty(fileName))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(fileName);
                dialog.FileName = Path.GetFileName(fileName);
            }
            else
            {
                dialog.InitialDirectory = null;
                dialog.FileName = null;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            else
            {
                return String.Empty;
            }
        }
    }
}
