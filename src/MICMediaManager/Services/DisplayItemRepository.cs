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

        public async Task<DisplayItem> GetAsync(int? id)
        {
            return await _dbContext.DisplayItem.SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<DisplayItem>> ListAsync()
        {
            return await _dbContext.DisplayItem                
                .OrderBy(d => d.OrderIndex)
                .ToListAsync();
        }

        public async Task DeleteAsync(DisplayItem d)
        {
            //remove unwanted slide
            _dbContext.DisplayItem.Remove(d);

            //re-order existing slides
            var itemsToUpdate = await _dbContext.DisplayItem
             .Where(e => e.Id != d.Id)
            .OrderBy(e => e.OrderIndex)
            .ToListAsync();

            int counter = 1;
            foreach (DisplayItem di in itemsToUpdate)
            {
                //increment additional if we are replacing old one
                di.OrderIndex = counter;
                _dbContext.Update(di);

                //increment for next round
                counter++;
            }

            //save
            await _dbContext.SaveChangesAsync();
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

             //if index changes, we need to check and change index of others
            if (displayItem.OrderIndex != model.OrderIndex)
            {
                var itemsToUpdate = await _dbContext.DisplayItem
                    .Where(d => d.Id != displayItem.Id)
                    .OrderBy(d => d.OrderIndex)
                    .ToListAsync();

                var itemsTotal = itemsToUpdate.Count();

                int counter = 1;
                foreach (DisplayItem di in itemsToUpdate)
                {
                    //increment additional if we are replacing old one
                    if (counter == model.OrderIndex)
                        counter++;
                    di.OrderIndex = counter;
                    _dbContext.Update(di);

                    //increment for next round
                    counter++;
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
