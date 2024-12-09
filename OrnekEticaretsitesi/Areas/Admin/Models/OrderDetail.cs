using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrnekEticaretsitesi.Areas.Admin.Models
{

    //Hangi üründen kaçtane, birim fiyatı, birim toplam fiyatı ve hangi ürüne ait olduğu("Producttablosu") ve bu sipariş detayı hangi siparişe ait olduğu gösterilir 
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }
        [Required]
        public int OrderID { get; set; } //Verilen Spiraşi getirelim
        
        [ForeignKey("OrderID")]
        public OrderHeader OrderHeader { get; set; }//Hangi siparişin sipariş detay bilgisini getireceğiz

        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public Product Product { get; set; }

        public int Count { get; set; }

        public double Price { get; set; }

    }
}
