namespace TaistControlSelecter
{
    partial class CursorPositionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cur_point_lb = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cur_point_lb
            // 
            this.cur_point_lb.AutoSize = true;
            this.cur_point_lb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cur_point_lb.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cur_point_lb.Location = new System.Drawing.Point(5, 5);
            this.cur_point_lb.Name = "cur_point_lb";
            this.cur_point_lb.Size = new System.Drawing.Size(0, 15);
            this.cur_point_lb.TabIndex = 2;
            // 
            // CursorPositionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(52, 27);
            this.Controls.Add(this.cur_point_lb);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CursorPositionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "CursorPositionForm";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.SystemColors.ControlDarkDark;
            this.Move += new System.EventHandler(this.CursorPositionForm_Move);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label cur_point_lb;
    }
}