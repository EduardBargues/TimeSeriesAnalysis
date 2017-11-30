namespace DashBoard
{
    partial class Form
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageHistory = new System.Windows.Forms.TabPage();
            this.historyCtl1 = new DashBoard.HistoryCtl();
            this.tabPageDayByDay = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPageHistory.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageHistory);
            this.tabControl1.Controls.Add(this.tabPageDayByDay);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1264, 507);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPageHistory
            // 
            this.tabPageHistory.Controls.Add(this.historyCtl1);
            this.tabPageHistory.Location = new System.Drawing.Point(4, 22);
            this.tabPageHistory.Name = "tabPageHistory";
            this.tabPageHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHistory.Size = new System.Drawing.Size(1256, 481);
            this.tabPageHistory.TabIndex = 0;
            this.tabPageHistory.Text = "History";
            this.tabPageHistory.UseVisualStyleBackColor = true;
            // 
            // historyCtl1
            // 
            this.historyCtl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyCtl1.Location = new System.Drawing.Point(3, 3);
            this.historyCtl1.Name = "historyCtl1";
            this.historyCtl1.Size = new System.Drawing.Size(1250, 475);
            this.historyCtl1.TabIndex = 0;
            // 
            // tabPageDayByDay
            // 
            this.tabPageDayByDay.Location = new System.Drawing.Point(4, 22);
            this.tabPageDayByDay.Name = "tabPageDayByDay";
            this.tabPageDayByDay.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDayByDay.Size = new System.Drawing.Size(1256, 481);
            this.tabPageDayByDay.TabIndex = 1;
            this.tabPageDayByDay.Text = "Day by day";
            this.tabPageDayByDay.UseVisualStyleBackColor = true;
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 507);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form";
            this.ShowIcon = false;
            this.Text = "DASHBOARD";
            this.Load += new System.EventHandler(this.Form_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPageHistory.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageHistory;
        private System.Windows.Forms.TabPage tabPageDayByDay;
        private HistoryCtl historyCtl1;
    }
}

