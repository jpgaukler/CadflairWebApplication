using CadflairDataAccess.Models;
using Inventor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadflairInventorAddin
{
    public partial class UploadToCadflairDialog : Form
    {
        private Document _doc;
        private ILogicUiElement[] _iLogicForms;

        public UploadToCadflairDialog(Document doc)
        {
            InitializeComponent();

            _doc = doc;
            _iLogicForms = UploadToCadflair.GetILogicFormElements(doc);

            foreach(ILogicUiElement form in _iLogicForms)
            {
                comboBoxILogicForms.Items.Add(form.Name);
            }

            comboBoxILogicForms.SelectedIndex = 0;
        }

        private void comboBoxILogicForms_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridViewParameters.Rows.Clear();
            ILogicUiElement form = _iLogicForms.FirstOrDefault(i => i.Name == comboBoxILogicForms.SelectedItem.ToString());
            PopulateParametersGrid(form);
        }

        private void PopulateParametersGrid(ILogicUiElement element)
        {
            foreach(ILogicUiElement item in element.Items)
            {
                if(item.ParameterName != null)
                {
                    dataGridViewParameters.Rows.Add(item.ParameterName, item.ParameterUnits, "", "");
                }

                if (item.Items != null)
                {
                    PopulateParametersGrid(item);
                }
            }
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBoxBucketKey.Text))
            {
                MessageBox.Show("Please enter a value for BucketKey", "BuckeyKey Not Provided", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxObjectName.Text))
            {
                MessageBox.Show("Please enter a value for ObjectName", "ObjectName Not Provided", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string zipFileName = UploadToCadflair.CreateTemporaryZipFile(_doc, true);
            UploadToCadflair.UploadModelToForge(zipFileName, textBoxBucketKey.Text, $"{textBoxObjectName.Text}.zip");

            System.IO.File.Delete(zipFileName);

            //Process.Start(zipFileName);
            //MessageBox.Show(zipFileName, "Success");

        }

    }
}
