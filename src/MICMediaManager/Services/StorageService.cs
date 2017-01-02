using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MICMediaManager.Models;
using Microsoft.Extensions.Options;

namespace MICMediaManager.Services
{
    public class StorageService : IStorageService
    {

        private readonly MyOptions _optionsAccessor;
        private readonly CloudStorageAccount _storageAccount;

        public StorageService(IOptions<MyOptions> optionsAccessor)
        {
            _optionsAccessor = optionsAccessor.Value;
            _storageAccount = new CloudStorageAccount(
            new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                _optionsAccessor.StorageAccountName,
                _optionsAccessor.StorageAccountKey), true);
        }
        public async Task<string> UploadImageAsync(IFormFile file)
        {

            var filePath = Path.GetTempFileName();
            string fileNameNew = "";
            string retUrl = "nothing";

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                string fileName = file.FileName.ToLower();
                string fileNameExt = fileName.Substring((fileName.Length - 4), 4);
                fileNameNew = Guid.NewGuid() + fileNameExt;
            }

            // Create a blob client.
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();

            // Get a reference to a container named “mycontainer.”
            CloudBlobContainer container = blobClient.GetContainerReference("micscreenmedia");

            // If container doesn’t exist, create it.
            await container.CreateIfNotExistsAsync();

            // Get a reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileNameNew);

            // Create or overwrite the "myblob" blob with the contents of a local file
            using (var fileStream = System.IO.File.OpenRead(filePath))
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }

            // Return full Uri to store in db
            return retUrl = blockBlob.Uri.ToString();

        }
    }
}
