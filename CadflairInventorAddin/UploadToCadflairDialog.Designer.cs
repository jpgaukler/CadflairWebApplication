namespace CadflairInventorAddin
{
    partial class UploadToCadflairDialog
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
            this.textBoxBucketKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxObjectName = new System.Windows.Forms.TextBox();
            this.buttonUpload = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxFilename = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxBucketKey
            // 
            this.textBoxBucketKey.Location = new System.Drawing.Point(140, 54);
            this.textBoxBucketKey.Name = "textBoxBucketKey";
            this.textBoxBucketKey.Size = new System.Drawing.Size(100, 20);
            this.textBoxBucketKey.TabIndex = 0;
            this.textBoxBucketKey.Text = "cadflair.testbucket";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Bucket Key:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Object Name:";
            // 
            // textBoxObjectName
            // 
            this.textBoxObjectName.Location = new System.Drawing.Point(140, 80);
            this.textBoxObjectName.Name = "textBoxObjectName";
            this.textBoxObjectName.Size = new System.Drawing.Size(100, 20);
            this.textBoxObjectName.TabIndex = 2;
            this.textBoxObjectName.Text = "Test";
            // 
            // buttonUpload
            // 
            this.buttonUpload.Location = new System.Drawing.Point(165, 196);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(75, 23);
            this.buttonUpload.TabIndex = 4;
            this.buttonUpload.Text = "button1";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.buttonUpload_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(62, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "File path:";
            // 
            // textBoxFilename
            // 
            this.textBoxFilename.Enabled = false;
            this.textBoxFilename.Location = new System.Drawing.Point(140, 140);
            this.textBoxFilename.Name = "textBoxFilename";
            this.textBoxFilename.Size = new System.Drawing.Size(425, 20);
            this.textBoxFilename.TabIndex = 5;
            this.textBoxFilename.Text = "Test";
            // 
            // UploadToCadflairDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 278);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxFilename);
            this.Controls.Add(this.buttonUpload);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxObjectName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxBucketKey);
            this.Name = "UploadToCadflairDialog";
            this.Text = "UploadToCadflairDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxBucketKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxObjectName;
        private System.Windows.Forms.Button buttonUpload;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxFilename;
    }
}