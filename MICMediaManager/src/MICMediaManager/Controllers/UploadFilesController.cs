using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MICMediaManager.Models;
using Microsoft.Extensions.Options;

namespace MICMediaManager.Controllers
{
    // EXAMPLE FROM: https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads
    // Modified to use secrets and single file

    public class UploadFilesController : Controller
    {
        private readonly MyOptions _optionsAccessor;

        public UploadFilesController(IOptions<MyOptions> optionsAccessor)
        {
            _optionsAccessor = optionsAccessor.Value;
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(IFormFile postedFile)
        {
         
            // Removed multiple files, set to single file
            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            string fileNameNew = "";
            string retUrl = "nothing";

            if (postedFile.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await postedFile.CopyToAsync(stream);
                    string fileName = postedFile.FileName.ToLower();
                    string fileNameExt = fileName.Substring((fileName.Length - 4), 4);
                    fileNameNew = Guid.NewGuid() + fileNameExt;
                }

                // process uploaded files
                CloudStorageAccount storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                    _optionsAccessor.StorageAccountName,
                    _optionsAccessor.StorageAccountKey), true);

                // Create a blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Get a reference to a container named “mycontainer.”
                CloudBlobContainer container = blobClient.GetContainerReference("micscreenmedia");

                // If container doesn’t exist, create it.
                await container.CreateIfNotExistsAsync();

                // Get a reference to a blob named "myblob".
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileNameNew);

                // Create or overwrite the "myblob" blob with the contents of a local file
                // named “myfile”.
                using (var fileStream = System.IO.File.OpenRead(filePath))
                {
                    await blockBlob.UploadFromStreamAsync(fileStream);
                }
                retUrl = blockBlob.Uri.ToString();
            }            
            
            return Ok(new {filePath, retUrl});
        }
    }
}