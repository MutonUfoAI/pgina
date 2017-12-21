using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace pGina.Configuration
{
    public partial class ImportExportReportWindow : Form
    {
        internal TextBox LogTextBox { get { return logTextArea; } }

        public ImportExportReportWindow()
        {
            InitializeComponent();
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
