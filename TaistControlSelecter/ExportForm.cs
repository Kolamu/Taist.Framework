using System;
using System.Xml;
using System.Collections.Generic;
using System.Windows.Forms;

using Mock.Data;

namespace TaistControlSelecter
{
    public partial class ExportForm : Form
    {
        private string sectionName = null;
        public ExportForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {   
            DialogResult dr = saveFileDialog1.ShowDialog(this);
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                path_txt.Text = saveFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            export();
            this.Close();
        }

        private void export()
        {
            
            try
            {
                ControlInfo ci = ControlInfo.getInstance();
                XmlDocument doc = ci[sectionName];
                doc.Save(path_txt.Text);
                MessageBox.Show(this, "导出成功", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\n" + ex.StackTrace, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportForm_Load(object sender, EventArgs e)
        {
            path_txt.Text = "c:\\export.xml";
            saveFileDialog1.FileName = "c:\\export.xml";

            try
            {
                ControlInfo ci = ControlInfo.getInstance();
                List<string> nameList = ci.SectionNameList;
                foreach (string name in nameList)
                {
                    xmName_cmb.Items.Add(name);
                }
                xmName_cmb.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\n" + ex.StackTrace, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void xmName_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            sectionName = xmName_cmb.SelectedItem.ToString();
        }
    }
}
