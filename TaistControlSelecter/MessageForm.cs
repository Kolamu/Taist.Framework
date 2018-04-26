using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace TaistControlSelecter
{
    public partial class MessageForm : Form
    {
        private Font msgFont = null;
        private Size msgSize;
        private string message = string.Empty;
        public MessageForm()
        {
            InitializeComponent();
            msgFont = new Font("宋体", 40, FontStyle.Bold);
            msgSize = TextRenderer.MeasureText(message, msgFont);
            this.Width = msgSize.Width + 2;
            this.Height = msgFont.Height + 2;
            this.Left = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
            this.Top = 10;
        }

        private void SelectAreaForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            e.Graphics.DrawString(message, msgFont, Brushes.OrangeRed, 1, 1, StringFormat.GenericTypographic);
        }

        public String Message
        {
            set
            {
                message = value;
                msgFont = new Font("宋体", 40, FontStyle.Bold);
                msgSize = TextRenderer.MeasureText(message, msgFont);
                this.Width = msgSize.Width + 2;
                this.Height = msgFont.Height + 2;
                this.Left = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
                this.Top = 10;
                this.Invalidate();
            }
            get
            {
                return message;
            }
        }
        private bool drag = false;
        private Point p = new Point();
        private void MessageForm_MouseDown(object sender, MouseEventArgs e)
        {
            drag = true;
            p.X = e.X;
            p.Y = e.Y;
        }

        private void MessageForm_MouseUp(object sender, MouseEventArgs e)
        {
            drag = false;
        }

        private void MessageForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                this.Left = this.Left + e.X - p.X;
                this.Top = this.Top + e.Y - p.Y;
            }
        }

    }
}
