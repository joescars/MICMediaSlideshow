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
        Task<List<DisplayItem>> ListAsync();
        Task<DisplayItem> GetAsync(int? id);
        Task CreateAsync(DisplayItem model);
        Task UpdateAsync(DisplayItemEditViewModel model);
        Task DeleteAsync(DisplayItem model);
        Task<List<DisplayItem>> GetActiveAsync();
        
    }
}
