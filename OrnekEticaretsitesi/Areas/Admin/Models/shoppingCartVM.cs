namespace OrnekEticaretsitesi.Areas.Admin.Models
{
    public class shoppingCartVM
    {


        public IEnumerable<ShopingChart> ListCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
