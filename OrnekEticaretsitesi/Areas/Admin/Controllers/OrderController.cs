using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrnekEticaretsitesi.Areas.Admin.Models;
using OrnekEticaretsitesi.Data;
using System.Security.Claims;

namespace OrnekEticaretsitesi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        //https://gelecegiyazanlar.turkcell.com.tr/konu/egitim/sql-ile-veritabani-sorgulama-giris-201/ornek-e-ticaret-sistemi-tablo-ve-alanlarin
       
       public IActionResult Details(int id)
        {
            OrderVM = new OrderDetailsVM
            {
                OrderHeader = _db.OrderHeaders.FirstOrDefault(i => i.OrderHearderID == id),
                OrderDetails=_db.OrderDetails.Where(x=>x.OrderID==id).Include(x=>x.Product)
             };
            return View(OrderVM);
        }


        //Verilen spiarişlerin listesini görmek
        //Kullanıcı adminse email gelmeyecek kullancı admin değilse ApplicationUser ile email gelecek
        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeaderlist;
            if (User.IsInRole(Diger.Role_Admin)){
                orderHeaderlist = _db.OrderHeaders.ToList();
            }
            else
            {
                orderHeaderlist = _db.OrderHeaders.Where(i => i.ApplicationUserID == claim.Value).Include(i => i.ApplicationUser);
            }


            return View(orderHeaderlist);
        }

        //Role admin ise ve veritabanında sipariş durumu beklemede olanların hepsini getir. Değilse girilen kullanıcının claimsIdentity ile durumu beklemede olanları getir 
        public IActionResult Beklenen()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeaderlist;
            if (User.IsInRole(Diger.Role_Admin))
            {
                orderHeaderlist = _db.OrderHeaders.Where(i => i.OrderStatus == Diger.Durum_Beklemede);
            }
            else
            {
                orderHeaderlist = _db.OrderHeaders.Where(i => i.ApplicationUserID == claim.Value && i.OrderStatus == Diger.Durum_Beklemede).Include(i => i.ApplicationUser);
            }


            return View(orderHeaderlist);
        }

        //Role admin ise ve veritabanında sipariş durumu onaylanların hepsini getir. Değilse girilen kullanıcının claimsIdentity ile durumu onaylananları getir 

        public IActionResult Onaylanan()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeaderlist;
            if (User.IsInRole(Diger.Role_Admin))
            {
                orderHeaderlist = _db.OrderHeaders.Where(i => i.OrderStatus == Diger.Durum_Onaylandi);
            }
            else
            {
                orderHeaderlist = _db.OrderHeaders.Where(i => i.ApplicationUserID == claim.Value && i.OrderStatus == Diger.Durum_Onaylandi).Include(i => i.ApplicationUser);
            }


            return View(orderHeaderlist);
        }

        //Role admin ise ve veritabanında sipariş durumu kargolananların hepsini getir. Değilse girilen kullanıcının claimsIdentity ile durumu kargolananları getir 

        public IActionResult Kargolanan()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeaderlist;
            if (User.IsInRole(Diger.Role_Admin))
            {
                orderHeaderlist = _db.OrderHeaders.Where(i => i.OrderStatus == Diger.Durum_Kargo);
            }
            else
            {
                orderHeaderlist = _db.OrderHeaders.Where(i => i.ApplicationUserID == claim.Value && i.OrderStatus == Diger.Durum_Kargo).Include(i => i.ApplicationUser);
            }


            return View(orderHeaderlist);
        }
        [BindProperty]
        public OrderDetailsVM OrderVM { get; set; }

        [HttpPost]
        [Authorize(Roles =Diger.Role_Admin)]
        public IActionResult Onaylandi()
        {
            OrderHeader orderHeader = _db.OrderHeaders.FirstOrDefault(i => i.OrderHearderID==OrderVM.OrderHeader.OrderHearderID);//OrderHeaderID değerinin ne olduğunu nasıl anlıyor????
            orderHeader.OrderStatus = Diger.Durum_Onaylandi;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = Diger.Role_Admin)]
        public IActionResult KargoyaVer()
        {
            OrderHeader orderHeader = _db.OrderHeaders.FirstOrDefault(i => i.OrderHearderID == OrderVM.OrderHeader.OrderHearderID);
            orderHeader.OrderStatus = Diger.Durum_Kargo;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }















    }
}
