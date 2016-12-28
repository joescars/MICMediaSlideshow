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

namespace MICMediaManager.Controllers
{
    public class UploadFilesController : Controller
    {
        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            string fileName = "";

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    fileName = formFile.FileName;
                }
            }

            // process uploaded files
            CloudStorageAccount storageAccount = new CloudStorageAccount(
            new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                "micmiamisa01",
                "ryMUIWFYy8ZgtC0U+84rsAYcjO22SaZcR2gTMxrcQsnJVTriKuxl+6QDG+LE3uhSZ7xhU9CUfOz007FDQm7dWw=="), true);

            // Create a blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to a container named “mycontainer.”
            CloudBlobContainer container = blobClient.GetContainerReference("micscreenmedia");

            // If container doesn’t exist, create it.
            await container.CreateIfNotExistsAsync();

            // Get a reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            // Create or overwrite the "myblob" blob with the contents of a local file
            // named “myfile”.
            using (var fileStream = System.IO.File.OpenRead(filePath))
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }
            
            return Ok(new { count = files.Count, size, filePath, blockBlob.Uri});
        }
    }
}