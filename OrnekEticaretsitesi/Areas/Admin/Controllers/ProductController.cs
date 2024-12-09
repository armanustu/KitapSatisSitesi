using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrnekEticaretsitesi.Areas.Admin.Models;
using OrnekEticaretsitesi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace OrnekEticaretsitesi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles=Diger.Role_Admin)]//Kişi sayfaya kullanıcı rolüyle griş yapmışsa access denied mesajı gönderir.Eğer hiç bir  giriş yapmadan sayfayı açmayı çalıştırırsa login sayfasına yönlendirir
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _he;
        public ProductController(ApplicationDbContext context, IWebHostEnvironment he)
        {
            _context = context;
            _he = he;
        }

        // GET: Admin/Product
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Product/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            var files = HttpContext.Request.Form.Files;

            if (files.Count > 0)
            {
                var filename=Guid.NewGuid().ToString();
                var uploads = Path.Combine(_he.WebRootPath, @"images\product");
                var ext = Path.GetExtension(files[0].FileName);
                if (product.Image != null)
                {
                    var imagepath = Path.Combine(_he.WebRootPath, product.Image.TrimStart('\\'));
                    if (System.IO.File.Exists(imagepath))
                    {
                        System.IO.File.Delete(imagepath);
                    }
                }
                using (var fileStreams=new FileStream(Path.Combine(uploads, filename + ext), FileMode.Create))
                {
                    files[0].CopyTo(fileStreams);
                }
                product.Image = @"\images\product\" + filename + ext;
            }



                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            //ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
            //return View(product);
        }

        // GET: Admin/Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.CategoryID = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);

            return View(product);
        }

        // POST: Admin/Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( Product product)
        {
            var files = HttpContext.Request.Form.Files;

            if (files.Count > 0)
            {
                var filename = Guid.NewGuid().ToString();
                var uploads = Path.Combine(_he.WebRootPath, @"images\product");
                var ext = Path.GetExtension(files[0].FileName);
                if (product.Image != null)
                {
                    var imagepath = Path.Combine(_he.WebRootPath, product.Image.TrimStart('\\'));
                    if (System.IO.File.Exists(imagepath))
                    {
                        System.IO.File.Delete(imagepath);
                    }
                }
                using (var fileStreams = new FileStream(Path.Combine(uploads, filename + ext), FileMode.Create))
                {
                    files[0].CopyTo(fileStreams);
                }
                product.Image = @"\images\product\" + filename + ext;
            }

            try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
          
        }

        // GET: Admin/Product/Delete/5
        public async Task<IActionResult>Delete (int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                var imagepath = Path.Combine(_he.WebRootPath, product.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
               
            }
            return RedirectToAction(nameof(Index));
        }
        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.ProductID == id)).GetValueOrDefault();
        }
    }
}
