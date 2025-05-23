﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using WebAppPharmacy.Models.Dictionaries;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebAppPharmacy.Models
{
    [Table("prescriptions")]
    [DisplayName("Рецепт")]
    public class Prescription
    {
        public long Id { get; set; }

        [DisplayName("Врач")]
        public string DoctorName { get; set; } = null!;

        [DisplayName("Код рецепта")]
        public string PrescriptionCode { get; set; } = null!;

        [DisplayName("Дата выдачи")]
        public DateTime IssueDate { get; set; }

        [DisplayName("Срок действия")]
        public DateTime? ExpiryDate { get; set; }

        public long ClientId { get; set; }
        public long StatusId { get; set; }
        [ValidateNever]
        // Навигация
        public Client Client { get; set; } = null!;
        [ValidateNever]
        public PrescriptionStatus Status { get; set; } = null!;

        public List<PrescriptionItem> Items { get; set; } = new();
    }
    
}
