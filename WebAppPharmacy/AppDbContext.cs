using Microsoft.EntityFrameworkCore;
using WebAppPharmacy.Models;
using WebAppPharmacy.Models.Dictionaries;
using WebAppPharmacy.Models.VM;

namespace WebAppPharmacy
{
    public class AppDbContext:DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Основные таблицы
        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleDetail> SaleDetails { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<SupplierOrder> SupplierOrders { get; set; }

        // Дополнительные таблицы
        public DbSet<UnitItem> UnitItems { get; set; } 
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; }

        // Справочники
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<MeasurementUnit> MeasurementUnits { get; set; }
        public DbSet<SaleStatus> SaleStatuses { get; set; }
        public DbSet<SupplierOrderStatus> SupplierOrderStatuses { get; set; }
        public DbSet<PrescriptionStatus> PrescriptionStatuses { get; set; }
        public DbSet<SaleListItemViewModel> SaleListItemViewModel { get; set; }
        public DbSet<SaleDetailViewModel> SaleDetailViewModel { get; set; }
        public DbSet<SaleDetailItemViewModel> SaleDetailItemViewModel { get; set; }
        public DbSet<SupplierOrderListItemViewModel> SupplierOrderListItemViewModel { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // ViewModel без ключей и без привязки к миграциям
            modelBuilder.Entity<OrderDetailViewModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<SaleViewModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<ProductViewModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<BatchViewModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<PrescriptionViewModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<SaleItemViewModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<SaleDetailViewModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<SaleDetailItemViewModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<SupplierOrderListItemViewModel>().HasNoKey().ToView(null);

        }

    }
}
