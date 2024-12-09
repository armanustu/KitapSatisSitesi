using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrnekEticaretsitesi.Areas.Admin.Models
{
    public class ApplicationUser : IdentityUser//migration işlemi gerçekleştikten Asp.NetUsers tablosuna kayıtları ekler.Sonra bu propertyleri Register.cshtml klasına ekleyelim 
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
    
        public string Adres { get; set; }
        
        public string Sehir { get; set; }
      
        public string Semt { get; set; }
     
        public string PostalKodu { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}