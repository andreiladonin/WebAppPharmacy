namespace WebAppPharmacy.Models.VM
{
    public class SupplierOrderListItemViewModel
    {

        public long Id { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string SupplierTitle { get; set; } = "";
        public string StatusName { get; set; } = "";

    }
}
