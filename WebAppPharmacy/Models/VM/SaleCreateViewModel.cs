using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAppPharmacy.Models.VM
{
    public class SaleCreateViewModel
    {
            public long ClientId { get; set; }
            public long EmployeeId { get; set; }
            public long StatusId { get; set; }
            public List<SaleItemViewModel> Items { get; set; } = new List<SaleItemViewModel>();
            public List<SelectListItem> Clients { get; set; } = new List<SelectListItem>();
            public List<SelectListItem> Employees { get; set; } = new List<SelectListItem>();
            public List<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
            public List<ProductBatchViewModel> AvailableProducts { get; set; } = new List<ProductBatchViewModel>();
    }
    
}
