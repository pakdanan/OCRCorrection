namespace WinOCRCorrection
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
            this.components = new System.ComponentModel.Container();
            this.txOCR = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.btCorrectIt = new System.Windows.Forms.Button();
            this.timerGoogle = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.btOpenOCR = new System.Windows.Forms.Button();
            this.saveCorr = new System.Windows.Forms.SaveFileDialog();
            this.rtbCorr = new System.Windows.Forms.RichTextBox();
            this.rtbOCR = new System.Windows.Forms.RichTextBox();
            this.btSave = new System.Windows.Forms.Button();
            this.cbMethod = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btCopy = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txOCR
            // 
            this.txOCR.Location = new System.Drawing.Point(58, 7);
            this.txOCR.Name = "txOCR";
            this.txOCR.Size = new System.Drawing.Size(260, 20);
            this.txOCR.TabIndex = 1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 420);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(877, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(4, 17);
            // 
            // btCorrectIt
            // 
            this.btCorrectIt.Image = global::WinOCRCorrection.Properties.Resources.process_16xMD;
            this.btCorrectIt.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btCorrectIt.Location = new System.Drawing.Point(613, 32);
            this.btCorrectIt.Name = "btCorrectIt";
            this.btCorrectIt.Size = new System.Drawing.Size(80, 23);
            this.btCorrectIt.TabIndex = 6;
            this.btCorrectIt.Text = "Correct";
            this.btCorrectIt.UseVisualStyleBackColor = true;
            this.btCorrectIt.Click += new System.EventHandler(this.btCorrectIt_Click);
            // 
            // timerGoogle
            // 
            this.timerGoogle.Interval = 4000;
            this.timerGoogle.Tick += new System.EventHandler(this.timerGoogle_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "File OCR";
            // 
            // openFile
            // 
            this.openFile.Filter = "Text files|*.txt|All files|*.*";
            // 
            // btOpenOCR
            // 
            this.btOpenOCR.Image = global::WinOCRCorrection.Properties.Resources.FindinFiles_16x;
            this.btOpenOCR.Location = new System.Drawing.Point(319, 6);
            this.btOpenOCR.Name = "btOpenOCR";
            this.btOpenOCR.Size = new System.Drawing.Size(28, 23);
            this.btOpenOCR.TabIndex = 15;
            this.btOpenOCR.UseVisualStyleBackColor = true;
            this.btOpenOCR.Click += new System.EventHandler(this.btOpenOCR_Click);
            // 
            // saveCorr
            // 
            this.saveCorr.Filter = "Text files|*.txt|All files|*.*";
            this.saveCorr.FileOk += new System.ComponentModel.CancelEventHandler(this.saveCorr_FileOk);
            // 
            // rtbCorr
            // 
            this.rtbCorr.Location = new System.Drawing.Point(438, 60);
            this.rtbCorr.Name = "rtbCorr";
            this.rtbCorr.Size = new System.Drawing.Size(427, 357);
            this.rtbCorr.TabIndex = 17;
            this.rtbCorr.Text = "";
            // 
            // rtbOCR
            // 
            this.rtbOCR.Location = new System.Drawing.Point(9, 60);
            this.rtbOCR.Name = "rtbOCR";
            this.rtbOCR.Size = new System.Drawing.Size(427, 357);
            this.rtbOCR.TabIndex = 18;
            this.rtbOCR.Text = "";
            // 
            // btSave
            // 
            this.btSave.Image = global::WinOCRCorrection.Properties.Resources.save_16xMD;
            this.btSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btSave.Location = new System.Drawing.Point(699, 32);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(80, 23);
            this.btSave.TabIndex = 19;
            this.btSave.Text = "Save";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // cbMethod
            // 
            this.cbMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMethod.FormattingEnabled = true;
            this.cbMethod.Location = new System.Drawing.Point(355, 7);
            this.cbMethod.Name = "cbMethod";
            this.cbMethod.Size = new System.Drawing.Size(510, 21);
            this.cbMethod.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "OCR";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(435, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Correction";
            // 
            // btCopy
            // 
            this.btCopy.Image = global::WinOCRCorrection.Properties.Resources.CopyToClipboard_16x;
            this.btCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btCopy.Location = new System.Drawing.Point(785, 32);
            this.btCopy.Name = "btCopy";
            this.btCopy.Size = new System.Drawing.Size(80, 23);
            this.btCopy.TabIndex = 23;
            this.btCopy.Text = "Copy";
            this.btCopy.UseVisualStyleBackColor = true;
            this.btCopy.Click += new System.EventHandler(this.btCopy_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 442);
            this.Controls.Add(this.btCopy);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbMethod);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.rtbOCR);
            this.Controls.Add(this.rtbCorr);
            this.Controls.Add(this.btOpenOCR);
            this.Controls.Add(this.btCorrectIt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.txOCR);
            this.Name = "Form1";
            this.Text = "OCR Error Correction";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txOCR;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button btCorrectIt;
        private System.Windows.Forms.Timer timerGoogle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.Button btOpenOCR;
        private System.Windows.Forms.SaveFileDialog saveCorr;
        private System.Windows.Forms.RichTextBox rtbCorr;
        private System.Windows.Forms.RichTextBox rtbOCR;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.ComboBox cbMethod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btCopy;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    }
}

