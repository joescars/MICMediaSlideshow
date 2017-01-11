using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MICMediaManager.Models
{
    public class MyOptions
    {
        public string StorageAccountName { get; set; }
        public string StorageAccountKey { get; set; }
        public string BlobContainerName { get; set; }
        public string TwitterAPIKey { get; set; }
        public string TwitterAPISecret { get; set; }
    }
}
