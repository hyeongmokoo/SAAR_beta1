﻿namespace VisUncertainty
{
    partial class frmFieldCalculator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFieldCalculator));
            this.label1 = new System.Windows.Forms.Label();
            this.lstFields = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTargetField = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNExpression = new System.Windows.Forms.TextBox();
            this.btnAddTarget = new System.Windows.Forms.Button();
            this.btnAddExpression = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdArcGIS = new System.Windows.Forms.RadioButton();
            this.radR = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.cboLayer = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fields:";
            // 
            // lstFields
            // 
            this.lstFields.FormattingEnabled = true;
            this.lstFields.Location = new System.Drawing.Point(11, 61);
            this.lstFields.Name = "lstFields";
            this.lstFields.Size = new System.Drawing.Size(115, 134);
            this.lstFields.TabIndex = 1;
            this.lstFields.DoubleClick += new System.EventHandler(this.lstFields_DoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(165, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Target Field:";
            // 
            // txtTargetField
            // 
            this.txtTargetField.Location = new System.Drawing.Point(168, 61);
            this.txtTargetField.Name = "txtTargetField";
            this.txtTargetField.Size = new System.Drawing.Size(113, 20);
            this.txtTargetField.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(287, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "=";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(165, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Numeric Expression:";
            // 
            // txtNExpression
            // 
            this.txtNExpression.AcceptsReturn = true;
            this.txtNExpression.Location = new System.Drawing.Point(168, 100);
            this.txtNExpression.Multiline = true;
            this.txtNExpression.Name = "txtNExpression";
            this.txtNExpression.Size = new System.Drawing.Size(205, 95);
            this.txtNExpression.TabIndex = 6;
            // 
            // btnAddTarget
            // 
            this.btnAddTarget.Location = new System.Drawing.Point(133, 61);
            this.btnAddTarget.Name = "btnAddTarget";
            this.btnAddTarget.Size = new System.Drawing.Size(26, 23);
            this.btnAddTarget.TabIndex = 7;
            this.btnAddTarget.Text = ">";
            this.btnAddTarget.UseVisualStyleBackColor = true;
            this.btnAddTarget.Click += new System.EventHandler(this.btnAddTarget_Click);
            // 
            // btnAddExpression
            // 
            this.btnAddExpression.Location = new System.Drawing.Point(132, 117);
            this.btnAddExpression.Name = "btnAddExpression";
            this.btnAddExpression.Size = new System.Drawing.Size(26, 23);
            this.btnAddExpression.TabIndex = 8;
            this.btnAddExpression.Text = ">";
            this.btnAddExpression.UseVisualStyleBackColor = true;
            this.btnAddExpression.Click += new System.EventHandler(this.btnAddExpression_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdArcGIS);
            this.groupBox1.Controls.Add(this.radR);
            this.groupBox1.Location = new System.Drawing.Point(13, 202);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(113, 38);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parser";
            // 
            // rdArcGIS
            // 
            this.rdArcGIS.AutoSize = true;
            this.rdArcGIS.Location = new System.Drawing.Point(47, 15);
            this.rdArcGIS.Name = "rdArcGIS";
            this.rdArcGIS.Size = new System.Drawing.Size(59, 17);
            this.rdArcGIS.TabIndex = 1;
            this.rdArcGIS.TabStop = true;
            this.rdArcGIS.Text = "ArcGIS";
            this.rdArcGIS.UseVisualStyleBackColor = true;
            // 
            // radR
            // 
            this.radR.AutoSize = true;
            this.radR.Checked = true;
            this.radR.Location = new System.Drawing.Point(7, 15);
            this.radR.Name = "radR";
            this.radR.Size = new System.Drawing.Size(33, 17);
            this.radR.TabIndex = 0;
            this.radR.TabStop = true;
            this.radR.Text = "R";
            this.radR.UseVisualStyleBackColor = true;
            this.radR.CheckedChanged += new System.EventHandler(this.radR_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(219, 216);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(300, 216);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "Close";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Layer:";
            // 
            // cboLayer
            // 
            this.cboLayer.FormattingEnabled = true;
            this.cboLayer.Location = new System.Drawing.Point(54, 7);
            this.cboLayer.Name = "cboLayer";
            this.cboLayer.Size = new System.Drawing.Size(227, 21);
            this.cboLayer.TabIndex = 14;
            this.cboLayer.SelectedIndexChanged += new System.EventHandler(this.cboLayer_SelectedIndexChanged);
            // 
            // frmFieldCalculator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(388, 258);
            this.Controls.Add(this.cboLayer);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnAddExpression);
            this.Controls.Add(this.btnAddTarget);
            this.Controls.Add(this.txtNExpression);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTargetField);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstFields);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFieldCalculator";
            this.Text = "Field Calculator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstFields;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtTargetField;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNExpression;
        private System.Windows.Forms.Button btnAddTarget;
        private System.Windows.Forms.Button btnAddExpression;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radR;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.ComboBox cboLayer;
        private System.Windows.Forms.RadioButton rdArcGIS;
    }
}