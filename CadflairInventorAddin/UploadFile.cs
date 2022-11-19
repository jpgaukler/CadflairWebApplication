using Inventor;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using File = System.IO.File;
using Path = System.IO.Path;

namespace CadflairInventorAddin
{
    internal class UploadFile
    {
        public static void UploadToCadflairButton_OnExecute(NameValueMap Context)
        {
            try
            {
                string zipFileName = CreateTemporaryZipFile(Globals.InventorApplication.ActiveDocument, true);

                UploadToCadflairDialog commandDialog = new UploadToCadflairDialog(zipFileName);
                commandDialog.ShowDialog();

                File.Delete(zipFileName);

                //Process.Start(zipFileName);
                //MessageBox.Show(zipFileName, "Success");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Create a zip folder in the temp directory for the Inventor document that is provided. 
        /// Resulting zip file will include all related child documents.
        /// Method will fail if the document has not yet been saved.
        /// <br></br>
        /// <br></br>
        /// includeDrawings - If a drawing file with the same name (.idw only) exists for any of the files given, it will be copied with the file.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="includeDrawings"></param>
        /// <returns>The full file name of the resulting zip file.</returns>
        /// <exception cref="FileNotFoundException"></exception>
        private static string CreateTemporaryZipFile(Document doc, bool includeDrawings)
        {
            if (doc.FileSaveCounter == 0) throw new FileNotFoundException();

            //save the doc
            doc.Save();

            //create the temp folder
            string tempFolderName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            DirectoryInfo dir = Directory.CreateDirectory(tempFolderName);

            //copy the doc to the temp folder
            CopyInventorFile(doc, tempFolderName, true);

            //copy all the references to the temp folder
            foreach (Document refDoc in doc.AllReferencedDocuments)
            {
                CopyInventorFile(refDoc, tempFolderName, true);
            }

            //zip up the temp folder
            string zipFileName = $"{tempFolderName}.zip";
            System.IO.Compression.ZipFile.CreateFromDirectory(tempFolderName, zipFileName);

            //delete the copied files
            dir.Delete(true);

            //return the path of the zip file
            return zipFileName;
        }

        /// <summary>
        /// Copies an Inventor model to the specified directory. 
        /// <br></br>
        /// <br></br>
        /// includeDrawing - If a drawing file with the same name exists (.idw only), it will be copied with the file.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="destinationFolderName"></param>
        /// <param name="includeDrawing"></param>
        private static void CopyInventorFile(Document doc, string destinationFolderName, bool includeDrawing)
        {
            string filePath = Path.GetDirectoryName(doc.FullFileName);
            string fileName = Path.GetFileName(doc.FullFileName);
            string idwFileName = Path.GetFileNameWithoutExtension(fileName) + ".idw";
            string fullIdwFileName = Path.Combine(filePath, idwFileName);

            //copy the doc to the temp folder
            File.Copy(doc.FullFileName, Path.Combine(destinationFolderName, fileName));

            //copy drawing if it exists
            if (includeDrawing && File.Exists(fullIdwFileName))
            {
                File.Copy(fullIdwFileName, Path.Combine(destinationFolderName, idwFileName));
            }
        }

        public static async void UploadModelToForge(string fullFileName, string bucketKey, string objectName)
        {
            try
            {
                HttpClient client = new HttpClient();
                MultipartFormDataContent content = new MultipartFormDataContent();

                //add bucket key content to the request
                StringContent bucketKeyContent = new StringContent(bucketKey);
                bucketKeyContent.Headers.Add("Content-Disposition", "form-data; name=\"bucketKey\"");
                content.Add(bucketKeyContent, "bucketKey");

                //add bucket key content to the request
                StringContent objectNameContent = new StringContent(objectName);
                objectNameContent.Headers.Add("Content-Disposition", "form-data; name=\"objectName\"");
                content.Add(objectNameContent, "objectName");

                //add file data to the form as a stream content
                //byte[] bytes = System.IO.File.ReadAllBytes(fullFileName);
                //MemoryStream stream = new MemoryStream(bytes);
                FileStream stream = System.IO.File.Open(fullFileName, FileMode.Open);

                StreamContent streamContent = new StreamContent(stream);
                streamContent.Headers.Add("Content-Type", "application/octet-stream");
                streamContent.Headers.Add("Content-Disposition", $"form-data; name=\"file\"; filename=\"{System.IO.Path.GetFileName(fullFileName)}\"");
                content.Add(streamContent, "file", System.IO.Path.GetFileName(fullFileName));

                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"http://www.cadflair.com/api/forge/oss/objects/upload"),
                    Content = content
                };

                //string reqTxt = await request.Content.ReadAsStringAsync(); //this will fail if the memory stream is closed before this line is called

                HttpResponseMessage response = await client.SendAsync(request);

                //print to txt file
                string fileName = @"C:\Users\Admin\source\repos\CadflairWebApplication\CadflairInventorAddin\bin\Debug\request.txt";
                //string fileName = @"C:\Users\jpgau\source\repos\jpgaukler\CadflairWebApplication\CadflairInventorAddin\bin\Debug\request.txt";
                StreamWriter txt = System.IO.File.CreateText(fileName);
                string responseMessage = await response.Content.ReadAsStringAsync();

                //txt.WriteLine("Request content:");
                //txt.Write(reqTxt);
                //txt.WriteLine();

                txt.WriteLine("Response content:");
                txt.Write(responseMessage);
                txt.Close();
                Process.Start(fileName);

                //clean up
                txt.Dispose();
                stream.Dispose();
                bucketKeyContent.Dispose();
                objectNameContent.Dispose();
                streamContent.Dispose();
                content.Dispose();
                response.Dispose();
                request.Dispose();
                client.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
