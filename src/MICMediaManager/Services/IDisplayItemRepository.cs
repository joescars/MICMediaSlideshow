using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MICMediaManager.Models;
using MICMediaManager.ViewModels;

namespace MICMediaManager.Services
{
    public interface IDisplayItemRepository
    {
        Task CreateAsync(DisplayItem model);
        Task<List<DisplayItem>> GetActiveAsync();
    }
}
