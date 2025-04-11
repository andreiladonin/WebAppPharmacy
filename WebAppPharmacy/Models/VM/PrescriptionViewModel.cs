namespace WebAppPharmacy.Models.VM
{
    public class PrescriptionViewModel
    {
        public long Id { get; set; }
        public string DoctorName { get; set; } = null!;
        public string PrescriptionCode { get; set; } = null!;
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ClientName { get; set; } = null!;
        public string StatusName { get; set; } = null!;
    }
}
