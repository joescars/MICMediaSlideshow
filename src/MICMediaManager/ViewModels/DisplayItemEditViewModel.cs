using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MICMediaManager.ViewModels
{
    public class DisplayItemEditViewModel
    {
        public int Id { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }
        public IFormFile MediaFile { get; set; }
        public string ImageUri_New { get; set; }
    }
}
