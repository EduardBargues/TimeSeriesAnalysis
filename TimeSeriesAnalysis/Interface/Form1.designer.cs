namespace Interface
{
    partial class Form1
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
            this.labelFirstDate = new System.Windows.Forms.Label();
            this.firstDatePicker = new System.Windows.Forms.DateTimePicker();
            this.lastDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.gridTimeSeries = new System.Windows.Forms.DataGridView();
            this.plotPanel = new System.Windows.Forms.Panel();
            this.normalizeCheckBox = new System.Windows.Forms.CheckBox();
            this.gridSolverInfo = new System.Windows.Forms.DataGridView();
            this.gridCorrelations = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.workingPathText = new System.Windows.Forms.TextBox();
            this.selectWorkingPathButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridTimeSeries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridSolverInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCorrelations)).BeginInit();
            this.SuspendLayout();
            // 
            // labelFirstDate
            // 
            this.labelFirstDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFirstDate.AutoSize = true;
            this.labelFirstDate.Location = new System.Drawing.Point(490, 12);
            this.labelFirstDate.Name = "labelFirstDate";
            this.labelFirstDate.Size = new System.Drawing.Size(53, 13);
            this.labelFirstDate.TabIndex = 0;
            this.labelFirstDate.Text = "First date:";
            // 
            // firstDatePicker
            // 
            this.firstDatePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.firstDatePicker.Location = new System.Drawing.Point(549, 12);
            this.firstDatePicker.Name = "firstDatePicker";
            this.firstDatePicker.Size = new System.Drawing.Size(200, 20);
            this.firstDatePicker.TabIndex = 2;
            this.firstDatePicker.ValueChanged += new System.EventHandler(this.firstDatePicker_ValueChanged);
            // 
            // lastDatePicker
            // 
            this.lastDatePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lastDatePicker.Location = new System.Drawing.Point(815, 12);
            this.lastDatePicker.Name = "lastDatePicker";
            this.lastDatePicker.Size = new System.Drawing.Size(200, 20);
            this.lastDatePicker.TabIndex = 3;
            this.lastDatePicker.ValueChanged += new System.EventHandler(this.lastDatePicker_ValueChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(755, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Last date:";
            // 
            // gridTimeSeries
            // 
            this.gridTimeSeries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gridTimeSeries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridTimeSeries.Location = new System.Drawing.Point(12, 46);
            this.gridTimeSeries.Name = "gridTimeSeries";
            this.gridTimeSeries.Size = new System.Drawing.Size(240, 388);
            this.gridTimeSeries.TabIndex = 5;
            this.gridTimeSeries.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridTimeSeries_CellValueChanged);
            // 
            // plotPanel
            // 
            this.plotPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plotPanel.Location = new System.Drawing.Point(506, 46);
            this.plotPanel.Name = "plotPanel";
            this.plotPanel.Size = new System.Drawing.Size(587, 388);
            this.plotPanel.TabIndex = 7;
            // 
            // normalizeCheckBox
            // 
            this.normalizeCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.normalizeCheckBox.AutoSize = true;
            this.normalizeCheckBox.Location = new System.Drawing.Point(1021, 12);
            this.normalizeCheckBox.Name = "normalizeCheckBox";
            this.normalizeCheckBox.Size = new System.Drawing.Size(72, 17);
            this.normalizeCheckBox.TabIndex = 8;
            this.normalizeCheckBox.Text = "Normalize";
            this.normalizeCheckBox.UseVisualStyleBackColor = true;
            this.normalizeCheckBox.CheckedChanged += new System.EventHandler(this.normalizeCheckBox_CheckedChanged);
            // 
            // gridSolverInfo
            // 
            this.gridSolverInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSolverInfo.Location = new System.Drawing.Point(260, 46);
            this.gridSolverInfo.Name = "gridSolverInfo";
            this.gridSolverInfo.Size = new System.Drawing.Size(240, 165);
            this.gridSolverInfo.TabIndex = 6;
            // 
            // gridCorrelations
            // 
            this.gridCorrelations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gridCorrelations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCorrelations.Location = new System.Drawing.Point(259, 217);
            this.gridCorrelations.Name = "gridCorrelations";
            this.gridCorrelations.Size = new System.Drawing.Size(240, 217);
            this.gridCorrelations.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Working path:";
            // 
            // workingPathText
            // 
            this.workingPathText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.workingPathText.Location = new System.Drawing.Point(93, 13);
            this.workingPathText.Name = "workingPathText";
            this.workingPathText.Size = new System.Drawing.Size(345, 20);
            this.workingPathText.TabIndex = 12;
            this.workingPathText.TextChanged += new System.EventHandler(this.workingPathText_TextChanged);
            // 
            // selectWorkingPathButton
            // 
            this.selectWorkingPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectWorkingPathButton.Location = new System.Drawing.Point(444, 12);
            this.selectWorkingPathButton.Name = "selectWorkingPathButton";
            this.selectWorkingPathButton.Size = new System.Drawing.Size(40, 23);
            this.selectWorkingPathButton.TabIndex = 13;
            this.selectWorkingPathButton.Text = "...";
            this.selectWorkingPathButton.UseVisualStyleBackColor = true;
            this.selectWorkingPathButton.Click += new System.EventHandler(this.selectWorkingPathButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1105, 446);
            this.Controls.Add(this.selectWorkingPathButton);
            this.Controls.Add(this.workingPathText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gridCorrelations);
            this.Controls.Add(this.normalizeCheckBox);
            this.Controls.Add(this.plotPanel);
            this.Controls.Add(this.gridSolverInfo);
            this.Controls.Add(this.gridTimeSeries);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lastDatePicker);
            this.Controls.Add(this.firstDatePicker);
            this.Controls.Add(this.labelFirstDate);
            this.Name = "Form1";
            this.ShowIcon = false;
            ((System.ComponentModel.ISupportInitialize)(this.gridTimeSeries)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridSolverInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCorrelations)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelFirstDate;
        private System.Windows.Forms.DateTimePicker firstDatePicker;
        private System.Windows.Forms.DateTimePicker lastDatePicker;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView gridTimeSeries;
        private System.Windows.Forms.Panel plotPanel;
        private System.Windows.Forms.CheckBox normalizeCheckBox;
        private System.Windows.Forms.DataGridView gridSolverInfo;
        private System.Windows.Forms.DataGridView gridCorrelations;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox workingPathText;
        private System.Windows.Forms.Button selectWorkingPathButton;
    }
}

