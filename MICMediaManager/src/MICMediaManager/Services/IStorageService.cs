using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MICMediaManager.Models;
using Microsoft.AspNetCore.Http;

namespace MICMediaManager.Services
{
    public interface IStorageService
    {
        Task<String> UploadImageAsync(IFormFile file);
    }
}
