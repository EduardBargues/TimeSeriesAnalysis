namespace DashBoard
{
    partial class HistoryCtl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.labelFrom = new System.Windows.Forms.Label();
            this.labelTo = new System.Windows.Forms.Label();
            this.dateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.seriesIndicatorCtl = new DashBoard.SeriesIndicatorCtl();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownDxPeriod = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownAdxSmoothingPeriod = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDxPeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAdxSmoothingPeriod)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.Location = new System.Drawing.Point(112, 7);
            this.dateTimePickerFrom.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.Size = new System.Drawing.Size(527, 38);
            this.dateTimePickerFrom.TabIndex = 0;
            // 
            // labelFrom
            // 
            this.labelFrom.AutoSize = true;
            this.labelFrom.Location = new System.Drawing.Point(8, 7);
            this.labelFrom.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.labelFrom.Name = "labelFrom";
            this.labelFrom.Size = new System.Drawing.Size(88, 32);
            this.labelFrom.TabIndex = 1;
            this.labelFrom.Text = "From:";
            // 
            // labelTo
            // 
            this.labelTo.AutoSize = true;
            this.labelTo.Location = new System.Drawing.Point(661, 7);
            this.labelTo.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.labelTo.Name = "labelTo";
            this.labelTo.Size = new System.Drawing.Size(56, 32);
            this.labelTo.TabIndex = 4;
            this.labelTo.Text = "To:";
            // 
            // dateTimePickerTo
            // 
            this.dateTimePickerTo.Location = new System.Drawing.Point(739, 7);
            this.dateTimePickerTo.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.dateTimePickerTo.Name = "dateTimePickerTo";
            this.dateTimePickerTo.Size = new System.Drawing.Size(527, 38);
            this.dateTimePickerTo.TabIndex = 3;
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRefresh.Location = new System.Drawing.Point(1282, 7);
            this.buttonRefresh.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(580, 82);
            this.buttonRefresh.TabIndex = 5;
            this.buttonRefresh.Text = "REFRESH";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            // 
            // seriesIndicatorCtl
            // 
            this.seriesIndicatorCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.seriesIndicatorCtl.Location = new System.Drawing.Point(0, 99);
            this.seriesIndicatorCtl.Name = "seriesIndicatorCtl";
            this.seriesIndicatorCtl.Size = new System.Drawing.Size(1870, 920);
            this.seriesIndicatorCtl.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 57);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 32);
            this.label1.TabIndex = 8;
            this.label1.Text = "DX Period:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(302, 57);
            this.label2.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(315, 32);
            this.label2.TabIndex = 10;
            this.label2.Text = "ADX Smoothing Period:";
            // 
            // numericUpDownDxPeriod
            // 
            this.numericUpDownDxPeriod.Location = new System.Drawing.Point(171, 55);
            this.numericUpDownDxPeriod.Name = "numericUpDownDxPeriod";
            this.numericUpDownDxPeriod.Size = new System.Drawing.Size(120, 38);
            this.numericUpDownDxPeriod.TabIndex = 11;
            // 
            // numericUpDownAdxSmoothingPeriod
            // 
            this.numericUpDownAdxSmoothingPeriod.Location = new System.Drawing.Point(628, 55);
            this.numericUpDownAdxSmoothingPeriod.Name = "numericUpDownAdxSmoothingPeriod";
            this.numericUpDownAdxSmoothingPeriod.Size = new System.Drawing.Size(120, 38);
            this.numericUpDownAdxSmoothingPeriod.TabIndex = 12;
            // 
            // HistoryCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.numericUpDownAdxSmoothingPeriod);
            this.Controls.Add(this.numericUpDownDxPeriod);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.seriesIndicatorCtl);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.labelTo);
            this.Controls.Add(this.dateTimePickerTo);
            this.Controls.Add(this.labelFrom);
            this.Controls.Add(this.dateTimePickerFrom);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "HistoryCtl";
            this.Size = new System.Drawing.Size(1870, 1019);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDxPeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAdxSmoothingPeriod)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePickerFrom;
        private System.Windows.Forms.Label labelFrom;
        private System.Windows.Forms.Label labelTo;
        private System.Windows.Forms.DateTimePicker dateTimePickerTo;
        private System.Windows.Forms.Button buttonRefresh;
        private SeriesIndicatorCtl seriesIndicatorCtl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownDxPeriod;
        private System.Windows.Forms.NumericUpDown numericUpDownAdxSmoothingPeriod;
    }
}
