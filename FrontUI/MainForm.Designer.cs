namespace FrontUI
{
    partial class MainForm
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
            this.cbPorts = new System.Windows.Forms.ComboBox();
            this.bRefreshPortNames = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tbIncoming = new System.Windows.Forms.TextBox();
            this.tbOutcoming = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bDraw = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbPorts
            // 
            this.cbPorts.FormattingEnabled = true;
            this.cbPorts.Location = new System.Drawing.Point(105, 43);
            this.cbPorts.Name = "cbPorts";
            this.cbPorts.Size = new System.Drawing.Size(121, 21);
            this.cbPorts.TabIndex = 0;
            this.cbPorts.SelectedValueChanged += new System.EventHandler(this.cbPorts_SelectedValueChanged);
            // 
            // bRefreshPortNames
            // 
            this.bRefreshPortNames.Location = new System.Drawing.Point(77, 42);
            this.bRefreshPortNames.Name = "bRefreshPortNames";
            this.bRefreshPortNames.Size = new System.Drawing.Size(22, 23);
            this.bRefreshPortNames.TabIndex = 1;
            this.bRefreshPortNames.UseVisualStyleBackColor = true;
            this.bRefreshPortNames.Click += new System.EventHandler(this.RefreshPortNames);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 550);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1047, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel1.Text = "Ready";
            // 
            // tbIncoming
            // 
            this.tbIncoming.Location = new System.Drawing.Point(283, 44);
            this.tbIncoming.Multiline = true;
            this.tbIncoming.Name = "tbIncoming";
            this.tbIncoming.Size = new System.Drawing.Size(288, 338);
            this.tbIncoming.TabIndex = 3;
            // 
            // tbOutcoming
            // 
            this.tbOutcoming.Location = new System.Drawing.Point(631, 42);
            this.tbOutcoming.Multiline = true;
            this.tbOutcoming.Name = "tbOutcoming";
            this.tbOutcoming.Size = new System.Drawing.Size(288, 338);
            this.tbOutcoming.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(280, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Incoming";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(628, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Outcoming";
            // 
            // bDraw
            // 
            this.bDraw.Location = new System.Drawing.Point(527, 426);
            this.bDraw.Name = "bDraw";
            this.bDraw.Size = new System.Drawing.Size(159, 50);
            this.bDraw.TabIndex = 7;
            this.bDraw.Text = "DRAW";
            this.bDraw.UseVisualStyleBackColor = true;
            this.bDraw.Click += new System.EventHandler(this.bDraw_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1047, 572);
            this.Controls.Add(this.bDraw);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbOutcoming);
            this.Controls.Add(this.tbIncoming);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.bRefreshPortNames);
            this.Controls.Add(this.cbPorts);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbPorts;
        private System.Windows.Forms.Button bRefreshPortNames;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.TextBox tbIncoming;
        private System.Windows.Forms.TextBox tbOutcoming;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bDraw;
    }
}

