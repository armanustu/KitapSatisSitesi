using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrnekEticaretsitesi.Areas.Admin.Models
{
    //Sipariş Kimin adına(kişisel bilgiler yada özel bilgiler),kişisel hangi tarihte ,durumu,toplam sipariş tutarı bilgileri tutulur
    //Hangi kullanıcının kişisel bilgilerini görmek istiyorsun o yüzden applictionUser propertyi dahil ediyoruz
    public class OrderHeader
    {
        [Key]
        public int OrderHearderID { get; set; }
        public string ApplicationUserID { get; set; }

        [ForeignKey("ApplicationUserID")]
        public ApplicationUser ApplicationUser { get; set; }

        
        public DateTime OrderDate { get; set; }
        
        public double OrderTotal { get; set; }
        
        public string? OrderStatus { get; set; }

        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Surname { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? Adres { get; set; }
        [Required]
        public string? Semt { get; set; }
        [Required]
        public string?  PostalKod { get; set; }
        [Required]
        public string? Sehir { get; set; }

        [Required]
        public string? CartName { get; set; }

        [Required]
        public string? CartNumber { get; set; }

        [Required]
        public string? Cvc { get; set; }

        [Required]
        public string? ExpirationMonth { get; set; }
        [Required]
        public string? ExpirationYear { get; set; }


    }
}
