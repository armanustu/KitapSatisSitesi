using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrnekEticaretsitesi.Areas.Admin.Models;
using OrnekEticaretsitesi.Data;
using OrnekEticaretsitesi.Migrations;
using System.Security.Claims;

namespace OrnekEticaretsitesi.Areas.Admin.Controllers
{

	//OrnekEticaretsitesi üzerine gelip->add new scalfold item->Identity ordan Applicaationdbcontext seçelim  user class seçmiyoruz layout var olan layoutu seçiyoruz  otomatikmen dosyaları Identity altına atar login logout Register Registerconfirmation sayfaları otomatikmen gelir
	//Mehmet@hotmail.com
	//sifre Arman123*
	//arman@hotmail.com
	//sifre Arman1234*
	//Kullanıcı adı arman@hotmail..com şifresi Arman1234*
	//admin adı @Mehmet@hotmail.com şifresi Arman123*
	//Kullanıcı adı hilal@hotmail.com şifresi Hilal123*

	[Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()//+++++++
        {
            //Anasayfada olan ürünleri Index sayfasına getiriyoruz
            var product= _db.Products.Where(x => x.IsHome).ToList();

            //giriş yapan kullanıcının bilgisini alıyoruz her kontrollda kullanıcı bilgisine ulaşabiliriz.Yani AspUserId bilgisine ulaşıyoruz claim.Value Asp.netUser Id'sini verir id'de ApplicationUserId olarak tanımlanmış Shopping chartta
           
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                //giriş yapan kullanıcının kaç çeşit ürün aldığını öğreniyoruz
                var count = _db.ShopingCharts.Where(i => i.ApplicationUserID == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(Diger.ssShoppingCart, count);//ürün çeşit miktarı Sessionda tutuyoruz

            }
            return View(product);
        }

       
        //Detay işleminde sepete eklemi işlemi gerçekleşeceği ilk adet miktarını almak için ShoppingChart klası kullanılmalıdır
        public IActionResult Details(int id)//+++++++++++
        {
            var item = _db.Products.Where(x => x.ProductID == id).FirstOrDefault();
            var product = new ShopingChart()
            {
                 Product=item,
                 ProductID=item.ProductID

            };
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]//Sepete eklemek için logine yönlendiriyoruz yönlendirmek yapmak içinse program.cs içinde kodları yazılmalı kodlar içindedir
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
