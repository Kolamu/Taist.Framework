using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

using Mock;
using Mock.Data;

namespace TaistControlSelecter
{
    public partial class ImportForm : Form
    {
        public ImportForm()
        {
            InitializeComponent();
        }

        private string sectionName = null;

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog(this);
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                path_txt.Text = openFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            save();
            
            
            this.Close();
        }

        private void ImportForm_Load(object sender, EventArgs e)
        {
            path_txt.Text = "c:\\import.xml";
            openFileDialog1.FileName = "c:\\import.xml";
            try
            {
                ControlInfo ci = ControlInfo.getInstance();
                List<string> nameList = ci.SectionNameList;
                xmName_cmb.Items.Add("新建...");
                foreach (string name in nameList)
                {
                    xmName_cmb.Items.Add(name);
                }
                
                xmName_cmb.SelectedIndex = 1;    
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\n" + ex.StackTrace, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void save()
        {
            string path = path_txt.Text.Trim();
            try
            {
                XmlDocument doc = XmlFactory.LoadXml(path);
                DataFactory.WriteLibrary(Config.WorkingDirectory + "\\Lib\\Controls.dll", sectionName, doc.OuterXml);
                MessageBox.Show(this, "导入成功", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\n" + ex.StackTrace, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void xmName_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            sectionName = xmName_cmb.SelectedItem.ToString();
            if (string.Equals(sectionName, "新建..."))
            {
                NewSectionForm form = new NewSectionForm();
                form.ShowDialog(this);
                if (form.Continue)
                {
                    sectionName = form.SectionName;
                    xmName_cmb.Items.Add(sectionName);
                }
                else
                {
                    xmName_cmb.SelectedIndex = 1;
                }
            }
        }
    }
}
