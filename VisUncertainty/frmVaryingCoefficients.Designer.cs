﻿namespace VisUncertainty
{
    partial class frmVaryingCoefficients
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmVaryingCoefficients));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lstIndeVar = new System.Windows.Forms.CheckedListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboFieldName = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnMoveLeft = new System.Windows.Forms.Button();
            this.btnMoveRight = new System.Windows.Forms.Button();
            this.lstFields = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboTargetLayer = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lblDirection = new System.Windows.Forms.Label();
            this.chkCoeEVs = new System.Windows.Forms.CheckBox();
            this.cboDirection = new System.Windows.Forms.ComboBox();
            this.nudEValue = new System.Windows.Forms.NumericUpDown();
            this.colNames = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTypes = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstSave = new System.Windows.Forms.ListView();
            this.btnOpenSWM = new System.Windows.Forms.Button();
            this.txtSWM = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cboNormalization = new System.Windows.Forms.ComboBox();
            this.lblNorm = new System.Windows.Forms.Label();
            this.cboFamily = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.grbSave = new System.Windows.Forms.GroupBox();
            this.grbEV = new System.Windows.Forms.GroupBox();
            this.ofdOpenSWM = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cboAlternative = new System.Windows.Forms.ComboBox();
            this.lblAlternative = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudEValue)).BeginInit();
            this.grbSave.SuspendLayout();
            this.grbEV.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(137, 394);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(144, 34);
            this.textBox1.TabIndex = 133;
            this.textBox1.Text = "Checked variables deonote Spatially Varying Coefficeints";
            // 
            // lstIndeVar
            // 
            this.lstIndeVar.CheckOnClick = true;
            this.lstIndeVar.FormattingEnabled = true;
            this.lstIndeVar.Location = new System.Drawing.Point(167, 204);
            this.lstIndeVar.Name = "lstIndeVar";
            this.lstIndeVar.Size = new System.Drawing.Size(113, 184);
            this.lstIndeVar.TabIndex = 132;
            this.lstIndeVar.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstIndeVar_ItemCheck);
            this.lstIndeVar.SelectedIndexChanged += new System.EventHandler(this.lstIndeVar_SelectedIndexChanged);
            this.lstIndeVar.SelectedValueChanged += new System.EventHandler(this.lstIndeVar_SelectedValueChanged);
            this.lstIndeVar.DoubleClick += new System.EventHandler(this.lstIndeVar_DoubleClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(449, 400);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(115, 23);
            this.btnCancel.TabIndex = 121;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(299, 400);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(115, 23);
            this.btnRun.TabIndex = 120;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(168, 185);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 119;
            this.label4.Text = "Independent Variables";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 185);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 118;
            this.label3.Text = "Fields";
            // 
            // cboFieldName
            // 
            this.cboFieldName.FormattingEnabled = true;
            this.cboFieldName.Location = new System.Drawing.Point(15, 114);
            this.cboFieldName.Name = "cboFieldName";
            this.cboFieldName.Size = new System.Drawing.Size(265, 21);
            this.cboFieldName.TabIndex = 117;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 116;
            this.label2.Text = "Dependent Variable:";
            // 
            // btnMoveLeft
            // 
            this.btnMoveLeft.Location = new System.Drawing.Point(134, 294);
            this.btnMoveLeft.Name = "btnMoveLeft";
            this.btnMoveLeft.Size = new System.Drawing.Size(28, 23);
            this.btnMoveLeft.TabIndex = 115;
            this.btnMoveLeft.Text = "<";
            this.btnMoveLeft.UseVisualStyleBackColor = true;
            this.btnMoveLeft.Click += new System.EventHandler(this.btnMoveLeft_Click);
            // 
            // btnMoveRight
            // 
            this.btnMoveRight.Location = new System.Drawing.Point(134, 265);
            this.btnMoveRight.Name = "btnMoveRight";
            this.btnMoveRight.Size = new System.Drawing.Size(28, 23);
            this.btnMoveRight.TabIndex = 114;
            this.btnMoveRight.Text = ">";
            this.btnMoveRight.UseVisualStyleBackColor = true;
            this.btnMoveRight.Click += new System.EventHandler(this.btnMoveRight_Click);
            // 
            // lstFields
            // 
            this.lstFields.FormattingEnabled = true;
            this.lstFields.Location = new System.Drawing.Point(15, 204);
            this.lstFields.Name = "lstFields";
            this.lstFields.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstFields.Size = new System.Drawing.Size(113, 186);
            this.lstFields.TabIndex = 113;
            this.lstFields.DoubleClick += new System.EventHandler(this.lstFields_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 112;
            this.label1.Text = "Select a Target Layer:";
            // 
            // cboTargetLayer
            // 
            this.cboTargetLayer.FormattingEnabled = true;
            this.cboTargetLayer.Location = new System.Drawing.Point(15, 28);
            this.cboTargetLayer.Name = "cboTargetLayer";
            this.cboTargetLayer.Size = new System.Drawing.Size(266, 21);
            this.cboTargetLayer.TabIndex = 111;
            this.cboTargetLayer.SelectedIndexChanged += new System.EventHandler(this.cboTargetLayer_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 67);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(158, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Eigenvalues over the principal >";
            // 
            // lblDirection
            // 
            this.lblDirection.AutoSize = true;
            this.lblDirection.Location = new System.Drawing.Point(74, 92);
            this.lblDirection.Name = "lblDirection";
            this.lblDirection.Size = new System.Drawing.Size(52, 13);
            this.lblDirection.TabIndex = 4;
            this.lblDirection.Text = "Direction:";
            // 
            // chkCoeEVs
            // 
            this.chkCoeEVs.AutoSize = true;
            this.chkCoeEVs.Location = new System.Drawing.Point(15, 22);
            this.chkCoeEVs.Name = "chkCoeEVs";
            this.chkCoeEVs.Size = new System.Drawing.Size(229, 17);
            this.chkCoeEVs.TabIndex = 123;
            this.chkCoeEVs.Text = "Show coefficients of selected eigenvectors";
            this.chkCoeEVs.UseVisualStyleBackColor = true;
            // 
            // cboDirection
            // 
            this.cboDirection.FormattingEnabled = true;
            this.cboDirection.Items.AddRange(new object[] {
            "Positive Only",
            "Negative Only",
            "Both"});
            this.cboDirection.Location = new System.Drawing.Point(132, 89);
            this.cboDirection.Name = "cboDirection";
            this.cboDirection.Size = new System.Drawing.Size(125, 21);
            this.cboDirection.TabIndex = 3;
            this.cboDirection.Text = "Positive Only";
            // 
            // nudEValue
            // 
            this.nudEValue.DecimalPlaces = 2;
            this.nudEValue.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.nudEValue.Location = new System.Drawing.Point(178, 64);
            this.nudEValue.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudEValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nudEValue.Name = "nudEValue";
            this.nudEValue.Size = new System.Drawing.Size(79, 20);
            this.nudEValue.TabIndex = 2;
            this.nudEValue.Value = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            // 
            // colNames
            // 
            this.colNames.Text = "Field Name";
            this.colNames.Width = 105;
            // 
            // colTypes
            // 
            this.colTypes.Text = "Varying Coefficents";
            this.colTypes.Width = 114;
            // 
            // lstSave
            // 
            this.lstSave.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTypes,
            this.colNames});
            this.lstSave.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstSave.LabelEdit = true;
            this.lstSave.Location = new System.Drawing.Point(12, 22);
            this.lstSave.Name = "lstSave";
            this.lstSave.Size = new System.Drawing.Size(244, 123);
            this.lstSave.TabIndex = 57;
            this.lstSave.UseCompatibleStateImageBehavior = false;
            this.lstSave.View = System.Windows.Forms.View.Details;
            this.lstSave.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstSave_MouseUp);
            // 
            // btnOpenSWM
            // 
            this.btnOpenSWM.Location = new System.Drawing.Point(218, 34);
            this.btnOpenSWM.Name = "btnOpenSWM";
            this.btnOpenSWM.Size = new System.Drawing.Size(39, 23);
            this.btnOpenSWM.TabIndex = 131;
            this.btnOpenSWM.Text = "...";
            this.btnOpenSWM.UseVisualStyleBackColor = true;
            this.btnOpenSWM.Click += new System.EventHandler(this.btnOpenSWM_Click);
            // 
            // txtSWM
            // 
            this.txtSWM.Location = new System.Drawing.Point(14, 36);
            this.txtSWM.Name = "txtSWM";
            this.txtSWM.Size = new System.Drawing.Size(199, 20);
            this.txtSWM.TabIndex = 130;
            this.txtSWM.Text = "Default";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(110, 13);
            this.label6.TabIndex = 129;
            this.label6.Text = "Spatial Weight Matrix:";
            // 
            // cboNormalization
            // 
            this.cboNormalization.Enabled = false;
            this.cboNormalization.FormattingEnabled = true;
            this.cboNormalization.Location = new System.Drawing.Point(15, 156);
            this.cboNormalization.Name = "cboNormalization";
            this.cboNormalization.Size = new System.Drawing.Size(265, 21);
            this.cboNormalization.TabIndex = 128;
            this.cboNormalization.SelectedIndexChanged += new System.EventHandler(this.cboNormalization_SelectedIndexChanged);
            // 
            // lblNorm
            // 
            this.lblNorm.AutoSize = true;
            this.lblNorm.Enabled = false;
            this.lblNorm.Location = new System.Drawing.Point(14, 140);
            this.lblNorm.Name = "lblNorm";
            this.lblNorm.Size = new System.Drawing.Size(73, 13);
            this.lblNorm.TabIndex = 127;
            this.lblNorm.Text = "Normalization:";
            // 
            // cboFamily
            // 
            this.cboFamily.FormattingEnabled = true;
            this.cboFamily.Items.AddRange(new object[] {
            "Linear (Gaussian)",
            "Poisson",
            "Negative Binomial",
            "Binomial",
            "Logistic"});
            this.cboFamily.Location = new System.Drawing.Point(15, 71);
            this.cboFamily.Name = "cboFamily";
            this.cboFamily.Size = new System.Drawing.Size(265, 21);
            this.cboFamily.TabIndex = 126;
            this.cboFamily.Text = "Linear (Gaussian)";
            this.cboFamily.SelectedIndexChanged += new System.EventHandler(this.cboFamily_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 125;
            this.label5.Text = "Family:";
            // 
            // grbSave
            // 
            this.grbSave.Controls.Add(this.lstSave);
            this.grbSave.Location = new System.Drawing.Point(300, 237);
            this.grbSave.Name = "grbSave";
            this.grbSave.Size = new System.Drawing.Size(264, 151);
            this.grbSave.TabIndex = 124;
            this.grbSave.TabStop = false;
            this.grbSave.Text = "Result";
            // 
            // grbEV
            // 
            this.grbEV.Controls.Add(this.label7);
            this.grbEV.Controls.Add(this.lblDirection);
            this.grbEV.Controls.Add(this.cboDirection);
            this.grbEV.Controls.Add(this.nudEValue);
            this.grbEV.Controls.Add(this.btnOpenSWM);
            this.grbEV.Controls.Add(this.label6);
            this.grbEV.Controls.Add(this.txtSWM);
            this.grbEV.Location = new System.Drawing.Point(299, 12);
            this.grbEV.Name = "grbEV";
            this.grbEV.Size = new System.Drawing.Size(265, 123);
            this.grbEV.TabIndex = 122;
            this.grbEV.TabStop = false;
            this.grbEV.Text = "Candidate Eigenvector Selection";
            // 
            // ofdOpenSWM
            // 
            this.ofdOpenSWM.Filter = "GAL files|*.gal|GWT files|*.gwt";
            this.ofdOpenSWM.Title = "Open GAL files";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cboAlternative);
            this.groupBox1.Controls.Add(this.lblAlternative);
            this.groupBox1.Controls.Add(this.chkCoeEVs);
            this.groupBox1.Location = new System.Drawing.Point(300, 141);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(264, 90);
            this.groupBox1.TabIndex = 134;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 43);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(145, 13);
            this.label8.TabIndex = 96;
            this.label8.Text = "Moran Coefficient Calculation";
            // 
            // cboAlternative
            // 
            this.cboAlternative.FormattingEnabled = true;
            this.cboAlternative.Items.AddRange(new object[] {
            "Greater",
            "Less",
            "Two Sided"});
            this.cboAlternative.Location = new System.Drawing.Point(166, 60);
            this.cboAlternative.Name = "cboAlternative";
            this.cboAlternative.Size = new System.Drawing.Size(90, 21);
            this.cboAlternative.TabIndex = 95;
            this.cboAlternative.Text = "Greater";
            // 
            // lblAlternative
            // 
            this.lblAlternative.AutoSize = true;
            this.lblAlternative.Location = new System.Drawing.Point(47, 64);
            this.lblAlternative.Name = "lblAlternative";
            this.lblAlternative.Size = new System.Drawing.Size(115, 13);
            this.lblAlternative.TabIndex = 94;
            this.lblAlternative.Text = "Alternative Hypothesis:";
            // 
            // frmVaryingCoefficients
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(579, 435);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.lstIndeVar);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cboFieldName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnMoveLeft);
            this.Controls.Add(this.btnMoveRight);
            this.Controls.Add(this.lstFields);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboTargetLayer);
            this.Controls.Add(this.cboNormalization);
            this.Controls.Add(this.lblNorm);
            this.Controls.Add(this.cboFamily);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.grbSave);
            this.Controls.Add(this.grbEV);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmVaryingCoefficients";
            this.Text = "Spatially Varying Coefficients";
            ((System.ComponentModel.ISupportInitialize)(this.nudEValue)).EndInit();
            this.grbSave.ResumeLayout(false);
            this.grbEV.ResumeLayout(false);
            this.grbEV.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckedListBox lstIndeVar;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboFieldName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnMoveLeft;
        private System.Windows.Forms.Button btnMoveRight;
        private System.Windows.Forms.ListBox lstFields;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboTargetLayer;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblDirection;
        private System.Windows.Forms.CheckBox chkCoeEVs;
        private System.Windows.Forms.ComboBox cboDirection;
        private System.Windows.Forms.NumericUpDown nudEValue;
        private System.Windows.Forms.ColumnHeader colNames;
        private System.Windows.Forms.ColumnHeader colTypes;
        private System.Windows.Forms.ListView lstSave;
        private System.Windows.Forms.Button btnOpenSWM;
        private System.Windows.Forms.TextBox txtSWM;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboNormalization;
        private System.Windows.Forms.Label lblNorm;
        private System.Windows.Forms.ComboBox cboFamily;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox grbSave;
        private System.Windows.Forms.GroupBox grbEV;
        private System.Windows.Forms.OpenFileDialog ofdOpenSWM;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cboAlternative;
        private System.Windows.Forms.Label lblAlternative;
    }
}