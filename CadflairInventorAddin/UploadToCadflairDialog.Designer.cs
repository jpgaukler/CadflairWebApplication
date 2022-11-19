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
            this.dataGridViewParameters = new System.Windows.Forms.DataGridView();
            this.ParameterNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParameterUnitsColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonSaveAttributes = new System.Windows.Forms.Button();
            this.comboBoxILogicForms = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewParameters)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxBucketKey
            // 
            this.textBoxBucketKey.Location = new System.Drawing.Point(117, 18);
            this.textBoxBucketKey.Name = "textBoxBucketKey";
            this.textBoxBucketKey.Size = new System.Drawing.Size(100, 20);
            this.textBoxBucketKey.TabIndex = 0;
            this.textBoxBucketKey.Text = "cadflair.testbucket";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Bucket Key:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Object Name:";
            // 
            // textBoxObjectName
            // 
            this.textBoxObjectName.Location = new System.Drawing.Point(117, 44);
            this.textBoxObjectName.Name = "textBoxObjectName";
            this.textBoxObjectName.Size = new System.Drawing.Size(100, 20);
            this.textBoxObjectName.TabIndex = 2;
            this.textBoxObjectName.Text = "Test";
            // 
            // buttonUpload
            // 
            this.buttonUpload.Location = new System.Drawing.Point(561, 21);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(75, 23);
            this.buttonUpload.TabIndex = 4;
            this.buttonUpload.Text = "Upload";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.buttonUpload_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "File path:";
            // 
            // textBoxFilename
            // 
            this.textBoxFilename.Enabled = false;
            this.textBoxFilename.Location = new System.Drawing.Point(106, 81);
            this.textBoxFilename.Name = "textBoxFilename";
            this.textBoxFilename.Size = new System.Drawing.Size(425, 20);
            this.textBoxFilename.TabIndex = 5;
            this.textBoxFilename.Text = "Test";
            // 
            // dataGridViewParameters
            // 
            this.dataGridViewParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewParameters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ParameterNameColumn,
            this.ParameterUnitsColumn,
            this.MaxValueColumn,
            this.MinValueColumn});
            this.dataGridViewParameters.Location = new System.Drawing.Point(54, 267);
            this.dataGridViewParameters.Name = "dataGridViewParameters";
            this.dataGridViewParameters.Size = new System.Drawing.Size(593, 150);
            this.dataGridViewParameters.TabIndex = 7;
            // 
            // ParameterNameColumn
            // 
            this.ParameterNameColumn.HeaderText = "Parameter Name";
            this.ParameterNameColumn.Name = "ParameterNameColumn";
            // 
            // ParameterUnitsColumn
            // 
            this.ParameterUnitsColumn.HeaderText = "Units";
            this.ParameterUnitsColumn.Name = "ParameterUnitsColumn";
            // 
            // MaxValueColumn
            // 
            this.MaxValueColumn.HeaderText = "Maximum";
            this.MaxValueColumn.Name = "MaxValueColumn";
            // 
            // MinValueColumn
            // 
            this.MinValueColumn.HeaderText = "Minimum";
            this.MinValueColumn.Name = "MinValueColumn";
            // 
            // buttonSaveAttributes
            // 
            this.buttonSaveAttributes.Location = new System.Drawing.Point(494, 210);
            this.buttonSaveAttributes.Name = "buttonSaveAttributes";
            this.buttonSaveAttributes.Size = new System.Drawing.Size(142, 23);
            this.buttonSaveAttributes.TabIndex = 8;
            this.buttonSaveAttributes.Text = "Save Attributes";
            this.buttonSaveAttributes.UseVisualStyleBackColor = true;
            // 
            // comboBoxILogicForms
            // 
            this.comboBoxILogicForms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxILogicForms.FormattingEnabled = true;
            this.comboBoxILogicForms.Location = new System.Drawing.Point(117, 153);
            this.comboBoxILogicForms.Name = "comboBoxILogicForms";
            this.comboBoxILogicForms.Size = new System.Drawing.Size(171, 21);
            this.comboBoxILogicForms.TabIndex = 9;
            this.comboBoxILogicForms.SelectedIndexChanged += new System.EventHandler(this.comboBoxILogicForms_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "iLogic Forms:";
            // 
            // UploadToCadflairDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(981, 500);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBoxILogicForms);
            this.Controls.Add(this.buttonSaveAttributes);
            this.Controls.Add(this.dataGridViewParameters);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxFilename);
            this.Controls.Add(this.buttonUpload);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxObjectName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxBucketKey);
            this.Name = "UploadToCadflairDialog";
            this.Text = "UploadToCadflairDialog";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewParameters)).EndInit();
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
        private System.Windows.Forms.DataGridView dataGridViewParameters;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParameterNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParameterUnitsColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinValueColumn;
        private System.Windows.Forms.Button buttonSaveAttributes;
        private System.Windows.Forms.ComboBox comboBoxILogicForms;
        private System.Windows.Forms.Label label4;
    }
}