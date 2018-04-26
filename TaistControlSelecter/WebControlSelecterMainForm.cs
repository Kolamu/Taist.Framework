using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TaistControlSelecter
{
    public partial class WebControlSelecterMainForm : Form
    {
        public WebControlSelecterMainForm()
        {
            InitializeComponent();
            this.Left = Screen.PrimaryScreen.Bounds.Width - this.Width - 5;
            this.Top = 5;
        }

        private void Btn_Ok_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
