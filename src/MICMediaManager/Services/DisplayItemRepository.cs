using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MICMediaManager.Data;
using MICMediaManager.Services;
using MICMediaManager.ViewModels;
using MICMediaManager.Models;
using Microsoft.EntityFrameworkCore;

namespace MICMediaManager.Services
{
    public class DisplayItemRepository : IDisplayItemRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DisplayItemRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAsync(DisplayItemCreateViewModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<List<DisplayItem>> GetActiveAsync()
        {
            return await _dbContext.DisplayItem
                .Where(d => d.IsActive == true)
                .OrderBy(d => d.OrderIndex)
                .ToListAsync();
        }
    }
}
