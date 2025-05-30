﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using WebAppPharmacy.Models.Dictionaries;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebAppPharmacy.Models
{
    [Table("batches")]
    [DisplayName("Партия товара")]
    public class Batch
    {
        internal DateTime? ExpiryDate;

        public long Id { get; set; }

        [DisplayName("Дата поставки")]
        public DateTime SupplyDate { get; set; }

        [DisplayName("Срок годности")]
        public DateTime ExpirationDate { get; set; }

        [DisplayName("Номер партии")]
        public string BatchNumber { get; set; } = null!;

        [DisplayName("Кол-во")]
        public int Quantity { get; set; }

        [DisplayName("Остаток")]
        public int RemainingQuantity { get; set; }

        [DisplayName("Закупочная цена")]
        public decimal PurchasePrice { get; set; }

        [DisplayName("Условия хранения")]
        public string? StorageConditions { get; set; }

        public long ProductId { get; set; }
        [ValidateNever]
        // Навигация
        public Product Product { get; set; } = null!;
        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
        public ICollection<UnitItem> UnitItems { get; set; } = new List<UnitItem>();
    }
}
