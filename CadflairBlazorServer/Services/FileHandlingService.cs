using Microsoft.AspNetCore.Components.Forms;

namespace CadflairBlazorServer.Services
{
    public class FileHandlingService
    {
        private readonly ILogger<FileHandlingService> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IJSRuntime _js;

        public const long MAX_UPLOAD_SIZE = 20 * 1024 * 1024;

        public FileHandlingService(IWebHostEnvironment environment, IJSRuntime js, ILogger<FileHandlingService> logger)
        {
            _logger = logger;
            _environment = environment;
            _js = js;
        }

        /// <summary>
        /// Get a random filename for uploading a file to the server. Temporary files should be deleted when processing is complete.
        /// </summary>
        /// <returns></returns>
        private string GetTempFilenameForUpload()
        {
            string uploadsFolder = Path.Combine(_environment.ContentRootPath, _environment.EnvironmentName, "Unsafe Uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string filename = Path.GetRandomFileName();
            string fullPath = Path.Combine(uploadsFolder, filename);

            return fullPath;
        }

        /// <summary>
        /// Upload a file to the server and save it to a temporary folder with a random file name. Optionally include a method to
        /// report progress for updating a UI. All uploads should be considered unsafe. It is recommended to delete the temporary file when it is no longer needed.
        /// <br/><br/>
        /// See documentation here: <see href="https://learn.microsoft.com/en-us/aspnet/core/blazor/file-uploads">ASP.NET Core Blazor file uploads</see>
        /// </summary>
        /// <param name="file"></param>
        /// <param name="progress"></param>
        /// <returns>The full path of the temporary file.</returns>
        public async Task<string> UploadBrowserFileToTempFolder(IBrowserFile file, IProgress<int>? progress = null)
        {
            if (file.Size > MAX_UPLOAD_SIZE)
                throw new Exception($"File exceeds maximum file size ({MAX_UPLOAD_SIZE / 1000000} MB)!");

            string path = GetTempFilenameForUpload();
            using Stream readStream = file.OpenReadStream(MAX_UPLOAD_SIZE);
            using FileStream writeStream = new(path, FileMode.Create);

            if (progress == null)
            {
                await readStream.CopyToAsync(writeStream);
            }
            else
            {
                int bytesRead = 0;
                int totalRead = 0;
                byte[] buffer = new byte[1024 * 10];

                while ((bytesRead = await readStream.ReadAsync(buffer)) != 0)
                {
                    totalRead += bytesRead;
                    await writeStream.WriteAsync(buffer, 0, bytesRead);
                    progress.Report(100 * totalRead / Convert.ToInt32(file.Size));
                }
            }

            return path;
        }

        /// <summary>
        /// Upload a file to the server and save it to a temporary folder with a random file name. All uploads should be considered unsafe.
        /// It is recommended to delete the temporary file when it is no longer needed.
        /// <br/><br/>
        /// See documentation here: <see href="https://learn.microsoft.com/en-us/aspnet/core/blazor/file-uploads">ASP.NET Core Blazor file uploads</see>
        /// </summary>
        /// <param name="file"></param>
        /// <returns>The full path of the temporary file.</returns>
        public async Task<string> UploadFormFileToTempFolder(IFormFile file)
        {
            if (file.Length > MAX_UPLOAD_SIZE)
                throw new Exception($"File exceeds maximum file size ({MAX_UPLOAD_SIZE / 1000000} MB)!");

            string path = GetTempFilenameForUpload();

            using (FileStream stream = File.Create(path))
                await file.CopyToAsync(stream);

            return path;
        }

        /// <summary>
        /// Get the content of a file on the server in the form of a byte array.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task<byte[]> GetFileBytes(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File could not be found on the server!", path);

            using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
            byte[] bytes = new byte[fs.Length];
            await fs.ReadAsync(bytes);
            return bytes;
        }

        /// <summary>
        /// Download a file from a byte array to the client machine. This should only be used for small files (less than 250 MB).
        /// <br/><br/>
        /// See documentation here: <see href="https://learn.microsoft.com/en-us/aspnet/core/blazor/file-downloads">ASP.NET Core Blazor file downloads</see>
        /// </summary>
        /// <param name="filename">Name of the downloaded file on the client's machine upon completion.</param>
        /// <param name="bytes">File data to be downloaded.</param>
        /// <returns></returns>
        public async Task DownloadFileFromBytes(string filename, byte[] bytes)
        {
            using MemoryStream stream = new(bytes);
            using DotNetStreamReference streamRef = new(stream);
            await _js.InvokeVoidAsync("downloadFileFromStream", filename, streamRef);
        }

    }
}
