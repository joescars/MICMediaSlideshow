using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MICMediaManager.Data;
using MICMediaManager.Models;
using MICMediaManager.ViewModels;
using Microsoft.Extensions.Options;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MICMediaManager.Services;

namespace MICMediaManager.Controllers
{
    public class DisplayItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly MyOptions _optionsAccessor;
        private readonly IDisplayItemRepository _displayItemRepository;
        private readonly IStorageService _storageService;

        public DisplayItemsController(ApplicationDbContext context, 
            IOptions<MyOptions> optionsAccessor,
            IDisplayItemRepository displayItemRepository,
            IStorageService storageService)
        {
            _context = context;
            _optionsAccessor = optionsAccessor.Value;
            _displayItemRepository = displayItemRepository;
            _storageService = storageService;
        }

        // GET: DisplayItems
        public async Task<IActionResult> Index()
        {
            return View(await _context.DisplayItem.ToListAsync());
        }

        // GET: DisplayItems/GetActive
        // Public API to push out active slides for Media Player
        public async Task<List<DisplayItem>> GetActive()
        {
            return await _displayItemRepository.GetActiveAsync();
        }

        // GET: DisplayItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var displayItem = await _context.DisplayItem.SingleOrDefaultAsync(m => m.Id == id);
            if (displayItem == null)
            {
                return NotFound();
            }

            return View(displayItem);
        }

        // GET: DisplayItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DisplayItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DisplayItemCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                //upload file to blob storage
                string retUrl = "";

                if (model.MediaFile.Length > 0)
                {
                    retUrl = await _storageService.UploadImageAsync(model.MediaFile);
                }

                //create new display item
                DisplayItem d = new DisplayItem();
                d.IsActive = model.IsActive;
                d.OrderIndex = model.OrderIndex;
                d.ImageUri = retUrl;

                //save
                await _displayItemRepository.CreateAsync(d);

                //redirect back to index
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: DisplayItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var displayItem = await _context.DisplayItem.SingleOrDefaultAsync(m => m.Id == id);
            if (displayItem == null)
            {
                return NotFound();
            }
            return View(displayItem);
        }

        // POST: DisplayItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateCreated,DateModified,ImageUri,IsActive,OrderIndex")] DisplayItem displayItem)
        {
            if (id != displayItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(displayItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisplayItemExists(displayItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(displayItem);
        }

        // GET: DisplayItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var displayItem = await _context.DisplayItem.SingleOrDefaultAsync(m => m.Id == id);
            if (displayItem == null)
            {
                return NotFound();
            }

            return View(displayItem);
        }

        // POST: DisplayItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var displayItem = await _context.DisplayItem.SingleOrDefaultAsync(m => m.Id == id);
            _context.DisplayItem.Remove(displayItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool DisplayItemExists(int id)
        {
            return _context.DisplayItem.Any(e => e.Id == id);
        }
    }
}
