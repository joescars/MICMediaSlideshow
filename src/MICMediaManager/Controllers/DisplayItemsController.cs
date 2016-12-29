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

namespace MICMediaManager.Controllers
{
    public class DisplayItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly MyOptions _optionsAccessor;

        public DisplayItemsController(ApplicationDbContext context, IOptions<MyOptions> optionsAccessor)
        {
            _context = context;
            _optionsAccessor = optionsAccessor.Value;
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
            return await _context.DisplayItem
                .Where(d => d.IsActive == true)
                .OrderBy(d => d.OrderIndex)
                .ToListAsync();
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
                var filePath = Path.GetTempFileName();
                string fileNameNew = "";
                string retUrl = "nothing";

                if (model.MediaFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.MediaFile.CopyToAsync(stream);
                        string fileName = model.MediaFile.FileName.ToLower();
                        string fileNameExt = fileName.Substring((fileName.Length - 4), 4);
                        fileNameNew = Guid.NewGuid() + fileNameExt;
                    }

                    // process uploaded files
                    CloudStorageAccount storageAccount = new CloudStorageAccount(
                    new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                        _optionsAccessor.StorageAccountName,
                        _optionsAccessor.StorageAccountKey), true);

                    // Create a blob client.
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                    // Get a reference to a container named “mycontainer.”
                    CloudBlobContainer container = blobClient.GetContainerReference("micscreenmedia");

                    // If container doesn’t exist, create it.
                    await container.CreateIfNotExistsAsync();

                    // Get a reference to a blob named "myblob".
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileNameNew);

                    // Create or overwrite the "myblob" blob with the contents of a local file
                    using (var fileStream = System.IO.File.OpenRead(filePath))
                    {
                        await blockBlob.UploadFromStreamAsync(fileStream);
                    }
                    retUrl = blockBlob.Uri.ToString();
                }

                //create new display item
                DisplayItem d = new DisplayItem();
                d.IsActive = model.IsActive;
                d.OrderIndex = model.OrderIndex;
                d.ImageUri = retUrl;

                //save
                _context.Add(d);
                await _context.SaveChangesAsync();

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
