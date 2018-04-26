namespace TaistControlSelecter
{
    using System;
    using System.Windows.Forms;

    using Mock.Nature.Native;
    using Mock.Tools.Controls;

    public partial class CursorPositionForm : Form
    {
        public CursorPositionForm()
        {
            InitializeComponent();
        }

        private void CursorPositionForm_Move(object sender, EventArgs e)
        {
            POINT p = Mouse.Position;
            cur_point_lb.Text = string.Format("{0},{1}", p.x, p.y);
        }
    }
}
