using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrnekEticaretsitesi.Areas.Admin.Models
{
    //Product shoppingchart ve application user arasındaki ilişki arasındaki ilişkiyi nasıl yorumlarsın
    /// <summary>
    /// Diagrama bakınca yanlış yorum çıkıyor??
    /// </summary>
    public class ShopingChart
    {
        public ShopingChart()
        {
            Count = 1;

        }
        [Key]
        public int ShoppingChartID { get; set; }
        public string ApplicationUserID { get; set; }
        [ForeignKey("ApplicationUserID")]
        public ApplicationUser ApplicationUser { get; set; }

        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public Product Product { get; set; }
        public int Count { get; set; }
        [NotMapped]
        public double Price { get; set; }
    }
}
