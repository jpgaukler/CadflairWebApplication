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
        public UploadToCadflairDialog()
        {
            InitializeComponent();
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

            //string fullFileName = Globals.InventorApplication.ActiveDocument.FullFileName;
            ConvertiLogicFormSpec.UploadModelToForge(textBoxFilename.Text, textBoxBucketKey.Text, textBoxObjectName.Text);
        }
    }
}
