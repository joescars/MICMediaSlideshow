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

        public async Task UpdateAsync(DisplayItemEditViewModel model)
        {
            var displayItem = await _dbContext.DisplayItem.SingleOrDefaultAsync(m => m.Id == model.Id);

            //TODO: Revisit order index change

            //if index changes, we need to check and change index of others
            if (displayItem.OrderIndex != model.OrderIndex)
            {
                //Move all items forward if position changes
                var itemsToUpdateIndex = await _dbContext.DisplayItem
                    .Where(d => d.OrderIndex == model.OrderIndex)
                    .ToListAsync();

                foreach (DisplayItem di in itemsToUpdateIndex)
                {
                    di.OrderIndex = di.OrderIndex + 1;
                    _dbContext.Update(di);
                }
            }

            //Update the rest of the fields
            displayItem.OrderIndex = model.OrderIndex;
            displayItem.IsActive = model.IsActive;
            displayItem.DateModified = DateTime.Now;

            //Only update image uri if exists
            if (model.ImageUri_New != null)
                displayItem.ImageUri = model.ImageUri_New;

            _dbContext.Update(displayItem);
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
