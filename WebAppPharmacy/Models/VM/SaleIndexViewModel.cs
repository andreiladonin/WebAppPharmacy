using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAppPharmacy.Models.VM
{
    public class SaleIndexViewModel
    {
        public List<SaleListItemViewModel> Sales { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string Search { get; set; }
        public string SortOrder { get; set; }
    }
    public class SaleDto
    {
        public long Id { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal Total { get; set; }
        public string ClientName { get; set; } = null!;
        public string EmployeeName { get; set; } = null!;
        public string StatusName { get; set; } = null!;
    }
}
