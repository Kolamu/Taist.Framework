namespace TaistControlSelecter
{
    partial class WebWindowInfomationForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.Submit = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AddVersion_Btn = new System.Windows.Forms.Label();
            this.ElementName_textBox = new System.Windows.Forms.TextBox();
            this.WindowName_textBox = new System.Windows.Forms.TextBox();
            this.Version_textBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.Cancel_Btn = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.Manual_Btn = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dataGridView_info = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_info)).BeginInit();
            this.SuspendLayout();
            // 
            // Submit
            // 
            this.Submit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Submit.Location = new System.Drawing.Point(265, 11);
            this.Submit.Name = "Submit";
            this.Submit.Size = new System.Drawing.Size(45, 25);
            this.Submit.TabIndex = 3;
            this.Submit.Text = "保存";
            this.Submit.UseVisualStyleBackColor = true;
            this.Submit.Click += new System.EventHandler(this.Submit_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.AddVersion_Btn);
            this.panel1.Controls.Add(this.ElementName_textBox);
            this.panel1.Controls.Add(this.WindowName_textBox);
            this.panel1.Controls.Add(this.Version_textBox);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Location = new System.Drawing.Point(6, 53);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(364, 100);
            this.panel1.TabIndex = 4;
            // 
            // AddVersion_Btn
            // 
            this.AddVersion_Btn.AutoSize = true;
            this.AddVersion_Btn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AddVersion_Btn.ForeColor = System.Drawing.Color.Blue;
            this.AddVersion_Btn.Location = new System.Drawing.Point(293, 12);
            this.AddVersion_Btn.Name = "AddVersion_Btn";
            this.AddVersion_Btn.Size = new System.Drawing.Size(38, 17);
            this.AddVersion_Btn.TabIndex = 11;
            this.AddVersion_Btn.Text = "NEW";
            this.AddVersion_Btn.Click += new System.EventHandler(this.AddVersion_Btn_Click);
            this.AddVersion_Btn.MouseEnter += new System.EventHandler(this.AddVersion_Btn_MouseEnter);
            // 
            // ElementName_textBox
            // 
            this.ElementName_textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ElementName_textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ElementName_textBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ElementName_textBox.Location = new System.Drawing.Point(140, 70);
            this.ElementName_textBox.Name = "ElementName_textBox";
            this.ElementName_textBox.Size = new System.Drawing.Size(217, 16);
            this.ElementName_textBox.TabIndex = 10;
            this.ElementName_textBox.Enter += new System.EventHandler(this.WindowAndElementNameText_MouseEnter);
            this.ElementName_textBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Name_textBox_MouseDoubleClick);
            // 
            // WindowName_textBox
            // 
            this.WindowName_textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.WindowName_textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.WindowName_textBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.WindowName_textBox.Location = new System.Drawing.Point(140, 42);
            this.WindowName_textBox.Name = "WindowName_textBox";
            this.WindowName_textBox.Size = new System.Drawing.Size(217, 16);
            this.WindowName_textBox.TabIndex = 9;
            this.WindowName_textBox.Enter += new System.EventHandler(this.WindowAndElementNameText_MouseEnter);
            this.WindowName_textBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Name_textBox_MouseDoubleClick);
            // 
            // Version_textBox
            // 
            this.Version_textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.Version_textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Version_textBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Version_textBox.Location = new System.Drawing.Point(140, 13);
            this.Version_textBox.Name = "Version_textBox";
            this.Version_textBox.ReadOnly = true;
            this.Version_textBox.Size = new System.Drawing.Size(147, 16);
            this.Version_textBox.TabIndex = 8;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(24, 70);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(97, 17);
            this.label9.TabIndex = 2;
            this.label9.Text = "ElementName:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(24, 41);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(98, 17);
            this.label8.TabIndex = 1;
            this.label8.Text = "WindowName:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(24, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 17);
            this.label7.TabIndex = 0;
            this.label7.Text = "Version:";
            // 
            // Cancel_Btn
            // 
            this.Cancel_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Cancel_Btn.Location = new System.Drawing.Point(316, 11);
            this.Cancel_Btn.Name = "Cancel_Btn";
            this.Cancel_Btn.Size = new System.Drawing.Size(45, 25);
            this.Cancel_Btn.TabIndex = 5;
            this.Cancel_Btn.Text = "取消";
            this.Cancel_Btn.UseVisualStyleBackColor = true;
            this.Cancel_Btn.Click += new System.EventHandler(this.Cancel_Btn_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.Manual_Btn);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.Cancel_Btn);
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.Submit);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(377, 480);
            this.panel2.TabIndex = 6;
            // 
            // Manual_Btn
            // 
            this.Manual_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Manual_Btn.Location = new System.Drawing.Point(214, 11);
            this.Manual_Btn.Name = "Manual_Btn";
            this.Manual_Btn.Size = new System.Drawing.Size(45, 25);
            this.Manual_Btn.TabIndex = 6;
            this.Manual_Btn.Text = "编辑";
            this.Manual_Btn.UseVisualStyleBackColor = true;
            this.Manual_Btn.Click += new System.EventHandler(this.Manual_Btn_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dataGridView_info);
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(6, 159);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(364, 316);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "录制信息";
            // 
            // dataGridView_info
            // 
            this.dataGridView_info.AllowUserToAddRows = false;
            this.dataGridView_info.AllowUserToDeleteRows = false;
            this.dataGridView_info.AllowUserToResizeColumns = false;
            this.dataGridView_info.AllowUserToResizeRows = false;
            this.dataGridView_info.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.dataGridView_info.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_info.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridView_info.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_info.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView_info.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.name,
            this.value});
            this.dataGridView_info.Location = new System.Drawing.Point(6, 34);
            this.dataGridView_info.Name = "dataGridView_info";
            this.dataGridView_info.RowHeadersVisible = false;
            this.dataGridView_info.RowTemplate.Height = 23;
            this.dataGridView_info.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView_info.Size = new System.Drawing.Size(351, 263);
            this.dataGridView_info.TabIndex = 0;
            // 
            // Select
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.NullValue = false;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this.Select.DefaultCellStyle = dataGridViewCellStyle2;
            this.Select.HeaderText = "";
            this.Select.Name = "Select";
            this.Select.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Select.Width = 30;
            // 
            // name
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this.name.DefaultCellStyle = dataGridViewCellStyle3;
            this.name.HeaderText = "名称";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // value
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            this.value.DefaultCellStyle = dataGridViewCellStyle4;
            this.value.HeaderText = "值";
            this.value.Name = "value";
            this.value.Width = 220;
            // 
            // WebWindowInfomationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(379, 482);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "WebWindowInfomationForm";
            this.Opacity = 0.9D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "WindowInfomationForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.WebWindowInfomationForm_Load);
            this.VisibleChanged += new System.EventHandler(this.WindowInfomationForm_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_info)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Submit;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox ElementName_textBox;
        private System.Windows.Forms.TextBox WindowName_textBox;
        private System.Windows.Forms.TextBox Version_textBox;
        private System.Windows.Forms.Label AddVersion_Btn;
        private System.Windows.Forms.Button Cancel_Btn;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button Manual_Btn;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView dataGridView_info;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SelectChk;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn value;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
    }
}