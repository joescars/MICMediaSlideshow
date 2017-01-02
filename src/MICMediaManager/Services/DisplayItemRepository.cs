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

        public async Task CreateAsync(DisplayItem d)
        {
            //get the top index and set the new itemIndex +1
            int curIndex = await _dbContext.DisplayItem
                .Where(di => di.IsActive == true)
                .MaxAsync(di => di.OrderIndex);

            d.OrderIndex = curIndex + 1;

            _dbContext.Add(d);
            await _dbContext.SaveChangesAsync();
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
