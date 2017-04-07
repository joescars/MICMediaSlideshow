using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MICMediaManager.ViewModels
{
    public class DisplayItemCreateViewModel
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public IFormFile MediaFile { get; set; }
    }
}
