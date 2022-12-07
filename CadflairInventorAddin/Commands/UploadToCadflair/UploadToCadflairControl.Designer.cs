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
            this.ButtonUpload = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.TextBoxDisplayName = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.CheckBoxIsConfigurable = new System.Windows.Forms.CheckBox();
            this.CheckBoxIsPublic = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBoxProductFamilyId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TextBoxUserId = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewParameters)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 227);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "iLogic Forms:";
            // 
            // ComboBoxILogicForms
            // 
            this.ComboBoxILogicForms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxILogicForms.FormattingEnabled = true;
            this.ComboBoxILogicForms.Location = new System.Drawing.Point(101, 224);
            this.ComboBoxILogicForms.Name = "ComboBoxILogicForms";
            this.ComboBoxILogicForms.Size = new System.Drawing.Size(171, 21);
            this.ComboBoxILogicForms.TabIndex = 19;
            this.ComboBoxILogicForms.SelectedIndexChanged += new System.EventHandler(this.ComboBoxILogicForms_SelectedIndexChanged);
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
            this.DataGridViewParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataGridViewParameters.Location = new System.Drawing.Point(10, 10);
            this.DataGridViewParameters.Name = "DataGridViewParameters";
            this.DataGridViewParameters.RowHeadersVisible = false;
            this.DataGridViewParameters.RowHeadersWidth = 82;
            this.DataGridViewParameters.Size = new System.Drawing.Size(733, 479);
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
            // ButtonUpload
            // 
            this.ButtonUpload.Dock = System.Windows.Forms.DockStyle.Right;
            this.ButtonUpload.Location = new System.Drawing.Point(668, 10);
            this.ButtonUpload.Name = "ButtonUpload";
            this.ButtonUpload.Size = new System.Drawing.Size(75, 23);
            this.ButtonUpload.TabIndex = 15;
            this.ButtonUpload.Text = "Upload";
            this.ButtonUpload.UseVisualStyleBackColor = true;
            this.ButtonUpload.Click += new System.EventHandler(this.ButtonUpload_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Display Name:";
            // 
            // TextBoxDisplayName
            // 
            this.TextBoxDisplayName.Location = new System.Drawing.Point(101, 31);
            this.TextBoxDisplayName.Name = "TextBoxDisplayName";
            this.TextBoxDisplayName.Size = new System.Drawing.Size(100, 20);
            this.TextBoxDisplayName.TabIndex = 11;
            this.TextBoxDisplayName.Text = "My Product Test";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ButtonUpload);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 769);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(753, 43);
            this.panel1.TabIndex = 21;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.DataGridViewParameters);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 270);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(10);
            this.panel2.Size = new System.Drawing.Size(753, 499);
            this.panel2.TabIndex = 22;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.TextBoxUserId);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.TextBoxProductFamilyId);
            this.panel3.Controls.Add(this.CheckBoxIsConfigurable);
            this.panel3.Controls.Add(this.CheckBoxIsPublic);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.TextBoxDisplayName);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.ComboBoxILogicForms);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(753, 270);
            this.panel3.TabIndex = 23;
            // 
            // CheckBoxIsConfigurable
            // 
            this.CheckBoxIsConfigurable.AutoSize = true;
            this.CheckBoxIsConfigurable.Location = new System.Drawing.Point(351, 34);
            this.CheckBoxIsConfigurable.Name = "CheckBoxIsConfigurable";
            this.CheckBoxIsConfigurable.Size = new System.Drawing.Size(96, 17);
            this.CheckBoxIsConfigurable.TabIndex = 22;
            this.CheckBoxIsConfigurable.Text = "Is Configurable";
            this.CheckBoxIsConfigurable.UseVisualStyleBackColor = true;
            // 
            // CheckBoxIsPublic
            // 
            this.CheckBoxIsPublic.AutoSize = true;
            this.CheckBoxIsPublic.Location = new System.Drawing.Point(264, 34);
            this.CheckBoxIsPublic.Name = "CheckBoxIsPublic";
            this.CheckBoxIsPublic.Size = new System.Drawing.Size(66, 17);
            this.CheckBoxIsPublic.TabIndex = 21;
            this.CheckBoxIsPublic.Text = "Is Public";
            this.CheckBoxIsPublic.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Product Family Id:";
            // 
            // TextBoxProductFamilyId
            // 
            this.TextBoxProductFamilyId.Location = new System.Drawing.Point(117, 93);
            this.TextBoxProductFamilyId.Name = "TextBoxProductFamilyId";
            this.TextBoxProductFamilyId.Size = new System.Drawing.Size(100, 20);
            this.TextBoxProductFamilyId.TabIndex = 23;
            this.TextBoxProductFamilyId.Text = "1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "User Id:";
            // 
            // TextBoxUserId
            // 
            this.TextBoxUserId.Location = new System.Drawing.Point(117, 67);
            this.TextBoxUserId.Name = "TextBoxUserId";
            this.TextBoxUserId.Size = new System.Drawing.Size(100, 20);
            this.TextBoxUserId.TabIndex = 25;
            this.TextBoxUserId.Text = "1";
            // 
            // UploadToCadflairControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Name = "UploadToCadflairControl";
            this.Size = new System.Drawing.Size(753, 812);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewParameters)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Button ButtonUpload;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBoxDisplayName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox CheckBoxIsConfigurable;
        private System.Windows.Forms.CheckBox CheckBoxIsPublic;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TextBoxProductFamilyId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextBoxUserId;
    }
}
