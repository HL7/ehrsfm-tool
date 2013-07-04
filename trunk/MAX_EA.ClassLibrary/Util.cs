using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MAX_EA
{
    public abstract class Util
    {
        public string showFileDialog(string title, string filter, string fileName, bool open)
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
            dialog.FileName = fileName;

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
