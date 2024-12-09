using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using OrnekEticaretsitesi.Areas.Admin.Models;
using OrnekEticaretsitesi.Data;
using OrnekEticaretsitesi.Migrations;
using System;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace OrnekEticaretsitesi.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : BaseController
    {

        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        [BindProperty]
       
        public shoppingCartVM shoppingCartVM { get; set; }
        public CartController(ApplicationDbContext db, IEmailSender emailSender,UserManager<IdentityUser>userManager)
        {
            _db = db;
            _emailSender = emailSender;
            _userManager=userManager;
        }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCartVM = new shoppingCartVM()
            {
                OrderHeader =new OrnekEticaretsitesi.Areas.Admin.Models.OrderHeader(),
                ListCart = _db.ShopingCharts.Where(i => i.ApplicationUserID == claim.Value).Include(i => i.Product)//ürünü alan kişinin ürünlerini getiriyoruz
            };
            shoppingCartVM.OrderHeader.OrderTotal = 0;
            shoppingCartVM.OrderHeader.ApplicationUser = _db.ApplicationUsers.FirstOrDefault(i => i.Id == claim.Value);
            //Kart listesini foreach ile döndürüyoruz
            foreach (var item in shoppingCartVM.ListCart)
            {
                shoppingCartVM.OrderHeader.OrderTotal += (item.Count * item.Product.Price);
            }


            return View(shoppingCartVM);
        }


        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _db.ApplicationUsers.FirstOrDefault(i => i.Id == claim.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Doğrulama email boş");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            ModelState.AddModelError(string.Empty, "Email doğrulama kodu gönder");
            return RedirectToAction("Success");
        }
        public IActionResult Success()
        {
            return View();
        }
        public IActionResult Add(int id)
        {
            var cart = _db.ShopingCharts.FirstOrDefault(i => i.ProductID == id);
            cart.Count += 1;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Decrease(int id)
        {
            var cart = _db.ShopingCharts.FirstOrDefault(i => i.ProductID == id);
            if (cart.Count == 1)
            {

                var count = _db.ShopingCharts.Where(u => u.ApplicationUserID == cart.ApplicationUserID).ToList().Count;
                _db.ShopingCharts.Remove(cart);

                _db.SaveChanges();
                HttpContext.Session.SetInt32(Diger.ssShoppingCart, count - 1);

            }
            else {
                cart.Count -= 1;
                _db.SaveChanges();
                }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int id)
        {
                 var cart = _db.ShopingCharts.FirstOrDefault(i => i.ProductID == id);
                 var count = _db.ShopingCharts.Where(u => u.ApplicationUserID == cart.ApplicationUserID).ToList().Count;
                _db.ShopingCharts.Remove(cart);
                _db.SaveChanges();
                HttpContext.Session.SetInt32(Diger.ssShoppingCart, count - 1);
                 return RedirectToAction(nameof(Index));

            
        }
        //Summary'de bize  adedi kadar alınan her ürün için toplam fiyatı alıp yazdırıyoruz
        //ve Bize lazım olan sepet bilgileri("Product") ve toplam hesaplanacağı için orderheader gerekiyor bu iki veriyi ShoppingCartWM'de tutyoruz
        //O yüzden bize hem sepet bilgileri görüntülenmeli hemde toplam fiyat görüntülenmeli 
        public IActionResult Summary()

        {

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCartVM = new shoppingCartVM()
            {
                OrderHeader = new OrnekEticaretsitesi.Areas.Admin.Models.OrderHeader(),
                ListCart = _db.ShopingCharts.Where(i => i.ApplicationUserID == claim.Value).Include(i => i.Product)//ürünü alan kişinin ürünlerini getiriyoruz
            };
            shoppingCartVM.OrderHeader.OrderTotal = 0;
            shoppingCartVM.OrderHeader.ApplicationUser = _db.ApplicationUsers.FirstOrDefault(i => i.Id == claim.Value);
            //Kart listesini foreach ile döndürüyoruz
            foreach (var item in shoppingCartVM.ListCart)//ShoppingChartaki("yani sepetteki") ürün alıp toplam fiyatı hesaplıyoruz
            {
                shoppingCartVM.OrderHeader.OrderTotal += (item.Count * item.Product.Price);
            }

            return View(shoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(shoppingCartVM model)//
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCartVM.ListCart = _db.ShopingCharts.Where(i => i.ApplicationUserID == claim.Value).Include(i => i.Product);
            shoppingCartVM.OrderHeader.OrderStatus = Diger.Durum_Beklemede;
            shoppingCartVM.OrderHeader.ApplicationUserID = claim.Value;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            _db.OrderHeaders.Add(shoppingCartVM.OrderHeader);
            _db.SaveChanges();
            foreach (var item in shoppingCartVM.ListCart)
            {
                item.Price = item.Product.Price;
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductID = item.ProductID,
                    OrderID = shoppingCartVM.OrderHeader.OrderHearderID,
                    Price = item.Price,
                    Count = item.Count
                };
                shoppingCartVM.OrderHeader.OrderTotal += item.Count + item.Product.Price;
                model.OrderHeader.OrderTotal += item.Count + item.Product.Price;
                _db.OrderDetails.Add(orderDetail);
            }
            var payment = PaymentProcess(model);
            _db.ShopingCharts.RemoveRange(shoppingCartVM.ListCart);
            _db.SaveChanges();
            HttpContext.Session.SetInt32(Diger.ssShoppingCart, 0);
            return RedirectToAction("SiparisTamam");
        }

        private Payment PaymentProcess(shoppingCartVM model)
        {
            Options options = new Options();
            options.ApiKey = "sandbox-83clwcaEEiozaJLKRLePbYxT7tCSsZMJ";
            options.SecretKey = "sandbox-opxyhuQJbFEdtXGQ4y9OdclxUDBuVIYQ";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(1111,9999).ToString();
            request.Price = model.OrderHeader.OrderTotal.ToString();
            request.PaidPrice = model.OrderHeader.OrderTotal.ToString(); 
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.OrderHeader.CartName;
            paymentCard.CardNumber = model.OrderHeader.CartNumber;
            paymentCard.ExpireMonth =model.OrderHeader.ExpirationMonth;
            paymentCard.ExpireYear = model.OrderHeader.ExpirationYear;
            paymentCard.Cvc = model.OrderHeader.Cvc;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;



            //PaymentCard paymentCard = new PaymentCard();
            //paymentCard.CardHolderName = "John Doe";
            //paymentCard.CardNumber = "5528790000000008";
            //paymentCard.ExpireMonth = "12";
            //paymentCard.ExpireYear = "2030";
            //paymentCard.Cvc = "123";
            //paymentCard.RegisterCard = 0;
            //request.PaymentCard = paymentCard;





            Buyer buyer = new Buyer();
            buyer.Id = model.OrderHeader.OrderHearderID.ToString();
            buyer.Name =model.OrderHeader.Name;
            buyer.Surname = model.OrderHeader.Surname;
            buyer.GsmNumber = model.OrderHeader.PhoneNumber;
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress =model.OrderHeader.Adres;
            buyer.Ip = "85.34.78.112";
            buyer.City = model.OrderHeader.Sehir;
            buyer.Country = "Turkey";
            buyer.ZipCode = model.OrderHeader.PostalKod;
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();           
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            foreach (var item in _db.ShopingCharts.Where(i=>i.ApplicationUserID==claim.Value).Include(i=>i.Product))
            {
                basketItems.Add(new BasketItem
                {
                    Id = item.ProductID.ToString(),
                    Name = item.Product.Title,
                    Category1=item.Product.CategoryID.ToString(),
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = (item.Price * item.Count).ToString()
                }); 
            }
            request.BasketItems = basketItems;

         return Payment.Create(request, options);
        }

        public IActionResult SiparisTamam()
        {
            return View();
        }


    }
}
