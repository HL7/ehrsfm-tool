using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MAX_EA
{
    public partial class ProgressWindow : Form
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }

        public void setup(int max)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = max;
            progressBar1.Step = 1;
            progress.Text = string.Format("{0} / {1}", progressBar1.Value, progressBar1.Maximum);
            Refresh();
        }

        public void step()
        {
            progressBar1.PerformStep();
            progress.Text = string.Format("{0} / {1}", progressBar1.Value, progressBar1.Maximum);
            Refresh();
        }
    }
}
