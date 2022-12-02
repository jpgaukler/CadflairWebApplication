namespace CadflairInventorAddin.Commands
{
    partial class UploadToCadflairControl
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
            this.label4 = new System.Windows.Forms.Label();
            this.ComboBoxILogicForms = new System.Windows.Forms.ComboBox();
            this.DataGridViewParameters = new System.Windows.Forms.DataGridView();
            this.ILogicUIElementColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParameterNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParameterUnitsColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UiElementSpecColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.TextBoxFilename = new System.Windows.Forms.TextBox();
            this.ButtonUpload = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBoxObjectName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TextBoxBucketKey = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewParameters)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(41, 139);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "iLogic Forms:";
            // 
            // ComboBoxILogicForms
            // 
            this.ComboBoxILogicForms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxILogicForms.FormattingEnabled = true;
            this.ComboBoxILogicForms.Location = new System.Drawing.Point(116, 139);
            this.ComboBoxILogicForms.Name = "ComboBoxILogicForms";
            this.ComboBoxILogicForms.Size = new System.Drawing.Size(171, 21);
            this.ComboBoxILogicForms.TabIndex = 19;
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
            this.DataGridViewParameters.Location = new System.Drawing.Point(44, 203);
            this.DataGridViewParameters.Name = "DataGridViewParameters";
            this.DataGridViewParameters.RowHeadersWidth = 82;
            this.DataGridViewParameters.Size = new System.Drawing.Size(682, 404);
            this.DataGridViewParameters.TabIndex = 18;
            // 
            // ILogicUIElementColumn
            // 
            this.ILogicUIElementColumn.HeaderText = "Element";
            this.ILogicUIElementColumn.MinimumWidth = 10;
            this.ILogicUIElementColumn.Name = "ILogicUIElementColumn";
            this.ILogicUIElementColumn.ReadOnly = true;
            this.ILogicUIElementColumn.Visible = false;
            this.ILogicUIElementColumn.Width = 200;
            // 
            // ParameterNameColumn
            // 
            this.ParameterNameColumn.HeaderText = "Parameter Name";
            this.ParameterNameColumn.MinimumWidth = 10;
            this.ParameterNameColumn.Name = "ParameterNameColumn";
            this.ParameterNameColumn.ReadOnly = true;
            this.ParameterNameColumn.Width = 200;
            // 
            // ParameterUnitsColumn
            // 
            this.ParameterUnitsColumn.HeaderText = "Units";
            this.ParameterUnitsColumn.MinimumWidth = 10;
            this.ParameterUnitsColumn.Name = "ParameterUnitsColumn";
            this.ParameterUnitsColumn.ReadOnly = true;
            this.ParameterUnitsColumn.Width = 200;
            // 
            // UiElementSpecColumn
            // 
            this.UiElementSpecColumn.HeaderText = "UI Element Spec";
            this.UiElementSpecColumn.MinimumWidth = 10;
            this.UiElementSpecColumn.Name = "UiElementSpecColumn";
            this.UiElementSpecColumn.ReadOnly = true;
            this.UiElementSpecColumn.Width = 200;
            // 
            // MinValueColumn
            // 
            this.MinValueColumn.HeaderText = "Minimum";
            this.MinValueColumn.MinimumWidth = 10;
            this.MinValueColumn.Name = "MinValueColumn";
            this.MinValueColumn.Width = 200;
            // 
            // MaxValueColumn
            // 
            this.MaxValueColumn.HeaderText = "Maximum";
            this.MaxValueColumn.MinimumWidth = 10;
            this.MaxValueColumn.Name = "MaxValueColumn";
            this.MaxValueColumn.Width = 200;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "File path:";
            // 
            // TextBoxFilename
            // 
            this.TextBoxFilename.Enabled = false;
            this.TextBoxFilename.Location = new System.Drawing.Point(119, 89);
            this.TextBoxFilename.Name = "TextBoxFilename";
            this.TextBoxFilename.Size = new System.Drawing.Size(425, 20);
            this.TextBoxFilename.TabIndex = 16;
            this.TextBoxFilename.Text = "Test";
            // 
            // ButtonUpload
            // 
            this.ButtonUpload.Location = new System.Drawing.Point(618, 23);
            this.ButtonUpload.Name = "ButtonUpload";
            this.ButtonUpload.Size = new System.Drawing.Size(75, 23);
            this.ButtonUpload.TabIndex = 15;
            this.ButtonUpload.Text = "Upload";
            this.ButtonUpload.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Object Name:";
            // 
            // TextBoxObjectName
            // 
            this.TextBoxObjectName.Location = new System.Drawing.Point(130, 52);
            this.TextBoxObjectName.Name = "TextBoxObjectName";
            this.TextBoxObjectName.Size = new System.Drawing.Size(100, 20);
            this.TextBoxObjectName.TabIndex = 13;
            this.TextBoxObjectName.Text = "Test";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Bucket Key:";
            // 
            // TextBoxBucketKey
            // 
            this.TextBoxBucketKey.Location = new System.Drawing.Point(130, 26);
            this.TextBoxBucketKey.Name = "TextBoxBucketKey";
            this.TextBoxBucketKey.Size = new System.Drawing.Size(100, 20);
            this.TextBoxBucketKey.TabIndex = 11;
            this.TextBoxBucketKey.Text = "cadflair.testbucket";
            // 
            // UploadToCadflairControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ComboBoxILogicForms);
            this.Controls.Add(this.DataGridViewParameters);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TextBoxFilename);
            this.Controls.Add(this.ButtonUpload);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TextBoxObjectName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBoxBucketKey);
            this.Name = "UploadToCadflairControl";
            this.Size = new System.Drawing.Size(753, 663);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewParameters)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox ComboBoxILogicForms;
        private System.Windows.Forms.DataGridView DataGridViewParameters;
        private System.Windows.Forms.DataGridViewTextBoxColumn ILogicUIElementColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParameterNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParameterUnitsColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn UiElementSpecColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxValueColumn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextBoxFilename;
        private System.Windows.Forms.Button ButtonUpload;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TextBoxObjectName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBoxBucketKey;
    }
}
