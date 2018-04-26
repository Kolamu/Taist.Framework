using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

using Mock;
using Mock.Data;

namespace TaistControlSelecter
{
    public partial class NewSectionForm : Form
    {
        public NewSectionForm()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _continue = false;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _continue = true;
            this.Close();
        }

        private string _name;
        public string SectionName
        {
            get
            {
                return _name;
            }
        }

        private bool _continue = false;
        public bool Continue
        {
            get
            {
                return _continue;
            }
        }

        private void name_txt_TextChanged(object sender, EventArgs e)
        {
            _name = name_txt.Text;
        }
    }
}
