using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrnekEticaretsitesi.Areas.Admin.Models;
using OrnekEticaretsitesi.Data;
using System.Security.Claims;

namespace OrnekEticaretsitesi.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult CategoryDetails(int? id)
        {

            var product = _db.Products.Where(i => i.CategoryID == id).ToList();
            ViewBag.categoryId = id;
            return View(product);


        }

        public IActionResult Search(string q)
        {
            if (!string.IsNullOrEmpty(q))
            {
                var ara = _db.Products.Where(i => i.Title.Contains(q) || i.Description.Contains(q));
                return View(ara);

            }
            else if (string.IsNullOrEmpty(q))
            {
                var product = _db.Products.ToList();
                return View(product);
            }
            return View();
        }





        public IActionResult Index()
        {
            var product = _db.Products.Where(x => x.IsHome).ToList();

            //giriş yapan kullanıcının bilgisini alıyoruz her kontrollda kullanıcı bilgisine ulaşabiliriz
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                //giriş yapan kullanıcının kaç çeşit ürün aldığını öğreniyoruz
                var count = _db.ShopingCharts.Where(i => i.ApplicationUserID == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(Diger.ssShoppingCart, count);//ürün çeşit miktarını Sessionda tutuyoruz

            }
            return View(product);
        }
        public IActionResult Details(int id)
        {
            var item = _db.Products.Where(x => x.ProductID == id).FirstOrDefault();
            var product = new ShopingChart()
            {
                Product = item,
                ProductID = item.ProductID

            };
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShopingChart Scart)
        {

            Scart.ShoppingChartID = 0;
            //if (ModelState.IsValid)
            //{
            var claimsIdentity = (ClaimsIdentity)User.Identity;//Giriş yapan kullanıcıyı buluyoruz
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Scart.ApplicationUserID = claim.Value;
            ShopingChart cart = _db.ShopingCharts.FirstOrDefault(u => u.ApplicationUserID == Scart.ApplicationUserID && u.ProductID == Scart.ProductID);
            if (cart == null)
            {
                _db.ShopingCharts.Add(Scart);
            }
            else
            {

                cart.Count += Scart.Count;
            }
            _db.SaveChanges();
            //Siparisveren tüm kullanıcıların sayısı
            var count = _db.ShopingCharts.Where(i => i.ApplicationUserID == Scart.ApplicationUserID).ToList().Count;
            HttpContext.Session.SetInt32(Diger.ssShoppingCart, count);

            return RedirectToAction(nameof(Index));

            //}
            //else
            //{
            //    var product = _db.Products.FirstOrDefault(x => x.ProductID == Scart.ShoppingChartID);
            //    ShopingChart cart = new ShopingChart()
            //    {
            //        Product = product,
            //        ProductID = product.ProductID

            //    };
            //}


            return View(Scart);
        }
    }
}
