﻿namespace VisUncertainty
{
    partial class frmSpatialRegression
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Residuals",
            "spr_resi"}, -1);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSpatialRegression));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboFieldName = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnMoveLeft = new System.Windows.Forms.Button();
            this.btnMoveRight = new System.Windows.Forms.Button();
            this.lstFields = new System.Windows.Forms.ListBox();
            this.lstIndeVar = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboTargetLayer = new System.Windows.Forms.ComboBox();
            this.btnOpenSWM = new System.Windows.Forms.Button();
            this.txtSWM = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ofdOpenSWM = new System.Windows.Forms.OpenFileDialog();
            this.grbSave = new System.Windows.Forms.GroupBox();
            this.lstSave = new System.Windows.Forms.ListView();
            this.colTypes = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colNames = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chkSave = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtMatrixJ = new System.Windows.Forms.RadioButton();
            this.rbtMC = new System.Windows.Forms.RadioButton();
            this.rbtLU = new System.Windows.Forms.RadioButton();
            this.rbtChebyshev = new System.Windows.Forms.RadioButton();
            this.rbtEigen = new System.Windows.Forms.RadioButton();
            this.rbtMatrix = new System.Windows.Forms.RadioButton();
            this.grbModels = new System.Windows.Forms.GroupBox();
            this.rbtDurbin = new System.Windows.Forms.RadioButton();
            this.rbtCAR = new System.Windows.Forms.RadioButton();
            this.rbtSMA = new System.Windows.Forms.RadioButton();
            this.rbtError = new System.Windows.Forms.RadioButton();
            this.rbtLag = new System.Windows.Forms.RadioButton();
            this.grbSave.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grbModels.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(461, 332);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(115, 23);
            this.btnCancel.TabIndex = 35;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(310, 332);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(115, 23);
            this.btnRun.TabIndex = 34;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(167, 154);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "Independent Variables";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 154);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Fields";
            // 
            // cboFieldName
            // 
            this.cboFieldName.FormattingEnabled = true;
            this.cboFieldName.Location = new System.Drawing.Point(14, 65);
            this.cboFieldName.Name = "cboFieldName";
            this.cboFieldName.Size = new System.Drawing.Size(265, 21);
            this.cboFieldName.TabIndex = 31;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Dependent Variable:";
            // 
            // btnMoveLeft
            // 
            this.btnMoveLeft.Location = new System.Drawing.Point(136, 243);
            this.btnMoveLeft.Name = "btnMoveLeft";
            this.btnMoveLeft.Size = new System.Drawing.Size(28, 23);
            this.btnMoveLeft.TabIndex = 29;
            this.btnMoveLeft.Text = "<";
            this.btnMoveLeft.UseVisualStyleBackColor = true;
            this.btnMoveLeft.Click += new System.EventHandler(this.btnMoveLeft_Click);
            // 
            // btnMoveRight
            // 
            this.btnMoveRight.Location = new System.Drawing.Point(136, 214);
            this.btnMoveRight.Name = "btnMoveRight";
            this.btnMoveRight.Size = new System.Drawing.Size(28, 23);
            this.btnMoveRight.TabIndex = 28;
            this.btnMoveRight.Text = ">";
            this.btnMoveRight.UseVisualStyleBackColor = true;
            this.btnMoveRight.Click += new System.EventHandler(this.btnMoveRight_Click);
            // 
            // lstFields
            // 
            this.lstFields.FormattingEnabled = true;
            this.lstFields.Location = new System.Drawing.Point(17, 171);
            this.lstFields.Name = "lstFields";
            this.lstFields.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstFields.Size = new System.Drawing.Size(113, 173);
            this.lstFields.TabIndex = 27;
            this.lstFields.DoubleClick += new System.EventHandler(this.lstFields_DoubleClick);
            // 
            // lstIndeVar
            // 
            this.lstIndeVar.FormattingEnabled = true;
            this.lstIndeVar.Location = new System.Drawing.Point(170, 173);
            this.lstIndeVar.Name = "lstIndeVar";
            this.lstIndeVar.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstIndeVar.Size = new System.Drawing.Size(113, 173);
            this.lstIndeVar.TabIndex = 26;
            this.lstIndeVar.DoubleClick += new System.EventHandler(this.lstIndeVar_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Select a Target Layer:";
            // 
            // cboTargetLayer
            // 
            this.cboTargetLayer.FormattingEnabled = true;
            this.cboTargetLayer.Location = new System.Drawing.Point(13, 25);
            this.cboTargetLayer.Name = "cboTargetLayer";
            this.cboTargetLayer.Size = new System.Drawing.Size(266, 21);
            this.cboTargetLayer.TabIndex = 24;
            this.cboTargetLayer.SelectedIndexChanged += new System.EventHandler(this.cboTargetLayer_SelectedIndexChanged);
            // 
            // btnOpenSWM
            // 
            this.btnOpenSWM.Location = new System.Drawing.Point(250, 104);
            this.btnOpenSWM.Name = "btnOpenSWM";
            this.btnOpenSWM.Size = new System.Drawing.Size(29, 23);
            this.btnOpenSWM.TabIndex = 69;
            this.btnOpenSWM.Text = "...";
            this.btnOpenSWM.UseVisualStyleBackColor = true;
            this.btnOpenSWM.Click += new System.EventHandler(this.btnOpenSWM_Click);
            // 
            // txtSWM
            // 
            this.txtSWM.Location = new System.Drawing.Point(14, 106);
            this.txtSWM.Name = "txtSWM";
            this.txtSWM.Size = new System.Drawing.Size(230, 20);
            this.txtSWM.TabIndex = 68;
            this.txtSWM.Text = "Default";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 90);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 13);
            this.label5.TabIndex = 67;
            this.label5.Text = "Spatial Weights Matrix";
            // 
            // ofdOpenSWM
            // 
            this.ofdOpenSWM.Filter = "GAL files|*.gal";
            this.ofdOpenSWM.Title = "Open GAL files";
            // 
            // grbSave
            // 
            this.grbSave.Controls.Add(this.lstSave);
            this.grbSave.Controls.Add(this.chkSave);
            this.grbSave.Location = new System.Drawing.Point(307, 207);
            this.grbSave.Name = "grbSave";
            this.grbSave.Size = new System.Drawing.Size(274, 111);
            this.grbSave.TabIndex = 70;
            this.grbSave.TabStop = false;
            this.grbSave.Text = "Save";
            // 
            // lstSave
            // 
            this.lstSave.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTypes,
            this.colNames});
            this.lstSave.Enabled = false;
            this.lstSave.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstSave.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.lstSave.LabelEdit = true;
            this.lstSave.Location = new System.Drawing.Point(12, 42);
            this.lstSave.Name = "lstSave";
            this.lstSave.Size = new System.Drawing.Size(244, 55);
            this.lstSave.TabIndex = 57;
            this.lstSave.UseCompatibleStateImageBehavior = false;
            this.lstSave.View = System.Windows.Forms.View.Details;
            this.lstSave.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstSave_MouseUp);
            // 
            // colTypes
            // 
            this.colTypes.Text = "Types";
            this.colTypes.Width = 103;
            // 
            // colNames
            // 
            this.colNames.Text = "Field Name";
            this.colNames.Width = 105;
            // 
            // chkSave
            // 
            this.chkSave.AutoSize = true;
            this.chkSave.Location = new System.Drawing.Point(12, 19);
            this.chkSave.Name = "chkSave";
            this.chkSave.Size = new System.Drawing.Size(146, 17);
            this.chkSave.TabIndex = 56;
            this.chkSave.Text = "Save regression residuals";
            this.chkSave.UseVisualStyleBackColor = true;
            this.chkSave.CheckedChanged += new System.EventHandler(this.chkSave_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox1);
            this.groupBox2.Controls.Add(this.grbModels);
            this.groupBox2.Location = new System.Drawing.Point(307, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(274, 189);
            this.groupBox2.TabIndex = 71;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Configuration";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtMatrixJ);
            this.groupBox1.Controls.Add(this.rbtMC);
            this.groupBox1.Controls.Add(this.rbtLU);
            this.groupBox1.Controls.Add(this.rbtChebyshev);
            this.groupBox1.Controls.Add(this.rbtEigen);
            this.groupBox1.Controls.Add(this.rbtMatrix);
            this.groupBox1.Location = new System.Drawing.Point(8, 100);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(256, 75);
            this.groupBox1.TabIndex = 46;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Jacobian Computation";
            // 
            // rbtMatrixJ
            // 
            this.rbtMatrixJ.Location = new System.Drawing.Point(176, 20);
            this.rbtMatrixJ.Name = "rbtMatrixJ";
            this.rbtMatrixJ.Size = new System.Drawing.Size(72, 24);
            this.rbtMatrixJ.TabIndex = 44;
            this.rbtMatrixJ.Text = "Matrix_J";
            this.rbtMatrixJ.UseVisualStyleBackColor = true;
            // 
            // rbtMC
            // 
            this.rbtMC.AutoSize = true;
            this.rbtMC.Location = new System.Drawing.Point(175, 45);
            this.rbtMC.Name = "rbtMC";
            this.rbtMC.Size = new System.Drawing.Size(82, 17);
            this.rbtMC.TabIndex = 43;
            this.rbtMC.Text = "Monte Carlo";
            this.rbtMC.UseVisualStyleBackColor = true;
            // 
            // rbtLU
            // 
            this.rbtLU.Location = new System.Drawing.Point(7, 41);
            this.rbtLU.Name = "rbtLU";
            this.rbtLU.Size = new System.Drawing.Size(56, 24);
            this.rbtLU.TabIndex = 42;
            this.rbtLU.Text = "LU";
            this.rbtLU.UseVisualStyleBackColor = true;
            // 
            // rbtChebyshev
            // 
            this.rbtChebyshev.AutoSize = true;
            this.rbtChebyshev.Location = new System.Drawing.Point(81, 45);
            this.rbtChebyshev.Name = "rbtChebyshev";
            this.rbtChebyshev.Size = new System.Drawing.Size(78, 17);
            this.rbtChebyshev.TabIndex = 41;
            this.rbtChebyshev.Text = "Chebyshev";
            this.rbtChebyshev.UseVisualStyleBackColor = true;
            // 
            // rbtEigen
            // 
            this.rbtEigen.Checked = true;
            this.rbtEigen.Location = new System.Drawing.Point(7, 20);
            this.rbtEigen.Name = "rbtEigen";
            this.rbtEigen.Size = new System.Drawing.Size(56, 24);
            this.rbtEigen.TabIndex = 40;
            this.rbtEigen.TabStop = true;
            this.rbtEigen.Text = "Eigen";
            this.rbtEigen.UseVisualStyleBackColor = true;
            // 
            // rbtMatrix
            // 
            this.rbtMatrix.AutoSize = true;
            this.rbtMatrix.Location = new System.Drawing.Point(81, 24);
            this.rbtMatrix.Name = "rbtMatrix";
            this.rbtMatrix.Size = new System.Drawing.Size(53, 17);
            this.rbtMatrix.TabIndex = 39;
            this.rbtMatrix.Text = "Matrix";
            this.rbtMatrix.UseVisualStyleBackColor = true;
            // 
            // grbModels
            // 
            this.grbModels.Controls.Add(this.rbtDurbin);
            this.grbModels.Controls.Add(this.rbtCAR);
            this.grbModels.Controls.Add(this.rbtSMA);
            this.grbModels.Controls.Add(this.rbtError);
            this.grbModels.Controls.Add(this.rbtLag);
            this.grbModels.Location = new System.Drawing.Point(8, 19);
            this.grbModels.Name = "grbModels";
            this.grbModels.Size = new System.Drawing.Size(256, 75);
            this.grbModels.TabIndex = 45;
            this.grbModels.TabStop = false;
            this.grbModels.Text = "Model";
            // 
            // rbtDurbin
            // 
            this.rbtDurbin.AutoSize = true;
            this.rbtDurbin.Location = new System.Drawing.Point(174, 45);
            this.rbtDurbin.Name = "rbtDurbin";
            this.rbtDurbin.Size = new System.Drawing.Size(56, 17);
            this.rbtDurbin.TabIndex = 43;
            this.rbtDurbin.Text = "Durbin";
            this.rbtDurbin.UseVisualStyleBackColor = true;
            // 
            // rbtCAR
            // 
            this.rbtCAR.Location = new System.Drawing.Point(6, 42);
            this.rbtCAR.Name = "rbtCAR";
            this.rbtCAR.Size = new System.Drawing.Size(56, 24);
            this.rbtCAR.TabIndex = 42;
            this.rbtCAR.Text = "CAR";
            this.rbtCAR.UseVisualStyleBackColor = true;
            // 
            // rbtSMA
            // 
            this.rbtSMA.AutoSize = true;
            this.rbtSMA.Location = new System.Drawing.Point(94, 45);
            this.rbtSMA.Name = "rbtSMA";
            this.rbtSMA.Size = new System.Drawing.Size(48, 17);
            this.rbtSMA.TabIndex = 41;
            this.rbtSMA.Text = "SMA";
            this.rbtSMA.UseVisualStyleBackColor = true;
            // 
            // rbtError
            // 
            this.rbtError.Checked = true;
            this.rbtError.Location = new System.Drawing.Point(6, 18);
            this.rbtError.Name = "rbtError";
            this.rbtError.Size = new System.Drawing.Size(119, 24);
            this.rbtError.TabIndex = 40;
            this.rbtError.TabStop = true;
            this.rbtError.Text = "SAR (Spatial Error)";
            this.rbtError.UseVisualStyleBackColor = true;
            // 
            // rbtLag
            // 
            this.rbtLag.AutoSize = true;
            this.rbtLag.Location = new System.Drawing.Point(131, 22);
            this.rbtLag.Name = "rbtLag";
            this.rbtLag.Size = new System.Drawing.Size(102, 17);
            this.rbtLag.TabIndex = 39;
            this.rbtLag.Text = "AR (Spatial Lag)";
            this.rbtLag.UseVisualStyleBackColor = true;
            // 
            // frmSpatialRegression
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(607, 367);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.grbSave);
            this.Controls.Add(this.btnOpenSWM);
            this.Controls.Add(this.txtSWM);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cboFieldName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnMoveLeft);
            this.Controls.Add(this.btnMoveRight);
            this.Controls.Add(this.lstFields);
            this.Controls.Add(this.lstIndeVar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboTargetLayer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSpatialRegression";
            this.Text = "Spatial Autoregression";
            this.grbSave.ResumeLayout(false);
            this.grbSave.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grbModels.ResumeLayout(false);
            this.grbModels.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboFieldName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnMoveLeft;
        private System.Windows.Forms.Button btnMoveRight;
        private System.Windows.Forms.ListBox lstFields;
        private System.Windows.Forms.ListBox lstIndeVar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboTargetLayer;
        private System.Windows.Forms.Button btnOpenSWM;
        private System.Windows.Forms.TextBox txtSWM;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.OpenFileDialog ofdOpenSWM;
        private System.Windows.Forms.GroupBox grbSave;
        private System.Windows.Forms.ListView lstSave;
        private System.Windows.Forms.ColumnHeader colTypes;
        private System.Windows.Forms.ColumnHeader colNames;
        private System.Windows.Forms.CheckBox chkSave;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtMatrixJ;
        private System.Windows.Forms.RadioButton rbtMC;
        private System.Windows.Forms.RadioButton rbtLU;
        private System.Windows.Forms.RadioButton rbtChebyshev;
        private System.Windows.Forms.RadioButton rbtEigen;
        private System.Windows.Forms.RadioButton rbtMatrix;
        private System.Windows.Forms.GroupBox grbModels;
        private System.Windows.Forms.RadioButton rbtDurbin;
        private System.Windows.Forms.RadioButton rbtCAR;
        private System.Windows.Forms.RadioButton rbtSMA;
        private System.Windows.Forms.RadioButton rbtError;
        private System.Windows.Forms.RadioButton rbtLag;
    }
}