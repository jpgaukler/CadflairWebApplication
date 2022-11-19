namespace CadflairInventorAddin.Commands
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
            this.TextBoxBucketKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBoxObjectName = new System.Windows.Forms.TextBox();
            this.ButtonUpload = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.TextBoxFilename = new System.Windows.Forms.TextBox();
            this.DataGridViewParameters = new System.Windows.Forms.DataGridView();
            this.ButtonSaveAttributes = new System.Windows.Forms.Button();
            this.ComboBoxILogicForms = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ILogicUIElementColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParameterNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParameterUnitsColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UiElementSpecColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewParameters)).BeginInit();
            this.SuspendLayout();
            // 
            // TextBoxBucketKey
            // 
            this.TextBoxBucketKey.Location = new System.Drawing.Point(117, 18);
            this.TextBoxBucketKey.Name = "TextBoxBucketKey";
            this.TextBoxBucketKey.Size = new System.Drawing.Size(100, 20);
            this.TextBoxBucketKey.TabIndex = 0;
            this.TextBoxBucketKey.Text = "cadflair.testbucket";
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
            // TextBoxObjectName
            // 
            this.TextBoxObjectName.Location = new System.Drawing.Point(117, 44);
            this.TextBoxObjectName.Name = "TextBoxObjectName";
            this.TextBoxObjectName.Size = new System.Drawing.Size(100, 20);
            this.TextBoxObjectName.TabIndex = 2;
            this.TextBoxObjectName.Text = "Test";
            // 
            // ButtonUpload
            // 
            this.ButtonUpload.Location = new System.Drawing.Point(605, 15);
            this.ButtonUpload.Name = "ButtonUpload";
            this.ButtonUpload.Size = new System.Drawing.Size(75, 23);
            this.ButtonUpload.TabIndex = 4;
            this.ButtonUpload.Text = "Upload";
            this.ButtonUpload.UseVisualStyleBackColor = true;
            this.ButtonUpload.Click += new System.EventHandler(this.ButtonUpload_Click);
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
            // TextBoxFilename
            // 
            this.TextBoxFilename.Enabled = false;
            this.TextBoxFilename.Location = new System.Drawing.Point(106, 81);
            this.TextBoxFilename.Name = "TextBoxFilename";
            this.TextBoxFilename.Size = new System.Drawing.Size(425, 20);
            this.TextBoxFilename.TabIndex = 5;
            this.TextBoxFilename.Text = "Test";
            // 
            // DataGridViewParameters
            // 
            this.DataGridViewParameters.AllowUserToAddRows = false;
            this.DataGridViewParameters.AllowUserToDeleteRows = false;
            this.DataGridViewParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridViewParameters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ILogicUIElementColumn,
            this.ParameterNameColumn,
            this.ParameterUnitsColumn,
            this.UiElementSpecColumn,
            this.MinValueColumn,
            this.MaxValueColumn});
            this.DataGridViewParameters.Location = new System.Drawing.Point(12, 182);
            this.DataGridViewParameters.Name = "DataGridViewParameters";
            this.DataGridViewParameters.Size = new System.Drawing.Size(682, 404);
            this.DataGridViewParameters.TabIndex = 7;
            // 
            // ButtonSaveAttributes
            // 
            this.ButtonSaveAttributes.Location = new System.Drawing.Point(538, 121);
            this.ButtonSaveAttributes.Name = "ButtonSaveAttributes";
            this.ButtonSaveAttributes.Size = new System.Drawing.Size(142, 23);
            this.ButtonSaveAttributes.TabIndex = 8;
            this.ButtonSaveAttributes.Text = "Save Attributes";
            this.ButtonSaveAttributes.UseVisualStyleBackColor = true;
            this.ButtonSaveAttributes.Click += new System.EventHandler(this.ButtonSaveAttributes_Click);
            // 
            // ComboBoxILogicForms
            // 
            this.ComboBoxILogicForms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxILogicForms.FormattingEnabled = true;
            this.ComboBoxILogicForms.Location = new System.Drawing.Point(103, 131);
            this.ComboBoxILogicForms.Name = "ComboBoxILogicForms";
            this.ComboBoxILogicForms.Size = new System.Drawing.Size(171, 21);
            this.ComboBoxILogicForms.TabIndex = 9;
            this.ComboBoxILogicForms.SelectedIndexChanged += new System.EventHandler(this.ComboBoxILogicForms_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 131);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "iLogic Forms:";
            // 
            // ILogicUIElementColumn
            // 
            this.ILogicUIElementColumn.HeaderText = "Element";
            this.ILogicUIElementColumn.Name = "ILogicUIElementColumn";
            this.ILogicUIElementColumn.ReadOnly = true;
            this.ILogicUIElementColumn.Visible = false;
            // 
            // ParameterNameColumn
            // 
            this.ParameterNameColumn.HeaderText = "Parameter Name";
            this.ParameterNameColumn.Name = "ParameterNameColumn";
            this.ParameterNameColumn.ReadOnly = true;
            // 
            // ParameterUnitsColumn
            // 
            this.ParameterUnitsColumn.HeaderText = "Units";
            this.ParameterUnitsColumn.Name = "ParameterUnitsColumn";
            this.ParameterUnitsColumn.ReadOnly = true;
            // 
            // UiElementSpecColumn
            // 
            this.UiElementSpecColumn.HeaderText = "UI Element Spec";
            this.UiElementSpecColumn.Name = "UiElementSpecColumn";
            this.UiElementSpecColumn.ReadOnly = true;
            // 
            // MinValueColumn
            // 
            this.MinValueColumn.HeaderText = "Minimum";
            this.MinValueColumn.Name = "MinValueColumn";
            // 
            // MaxValueColumn
            // 
            this.MaxValueColumn.HeaderText = "Maximum";
            this.MaxValueColumn.Name = "MaxValueColumn";
            // 
            // UploadToCadflairDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 598);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ComboBoxILogicForms);
            this.Controls.Add(this.ButtonSaveAttributes);
            this.Controls.Add(this.DataGridViewParameters);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TextBoxFilename);
            this.Controls.Add(this.ButtonUpload);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TextBoxObjectName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBoxBucketKey);
            this.Name = "UploadToCadflairDialog";
            this.Text = "UploadToCadflairDialog";
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewParameters)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxBucketKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TextBoxObjectName;
        private System.Windows.Forms.Button ButtonUpload;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextBoxFilename;
        private System.Windows.Forms.DataGridView DataGridViewParameters;
        private System.Windows.Forms.Button ButtonSaveAttributes;
        private System.Windows.Forms.ComboBox ComboBoxILogicForms;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewTextBoxColumn ILogicUIElementColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParameterNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParameterUnitsColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn UiElementSpecColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxValueColumn;
    }
}