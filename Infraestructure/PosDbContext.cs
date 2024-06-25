using Pos.WebApi.Features.Users.Entities;
using Pos.WebApi.Features.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Pos.WebApi.Features.Common.Dto;
using Pos.WebApi.Features.Items.Entities;
using Pos.WebApi.Features.Sellers.Entities;
using Pos.WebApi.Features.Customers.Entities;
using Pos.WebApi.Features.Suppliers.Entities;
using Pos.WebApi.Features.InventoryTransactions.Entities;
using Pos.WebApi.Features.Purchase.Entities;
using Pos.WebApi.Features.PurchasePayment.Entitie;
using Pos.WebApi.Features.Sales.Entities;
using Pos.WebApi.Features.SalesPayment.Entities;
using Pos.WebApi.Features.Customers.Dto;
using Pos.WebApi.Features.Expenses.Entities;
using Pos.WebApi.Features.Liquidations.Entities;
using Pos.WebApi.Features.Items.Dto;

namespace Pos.WebApi.Infraestructure
{
    public class PosDbContext : DbContext
    {
        public PosDbContext(DbContextOptions<PosDbContext> options) : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Theme> Theme { get; set; }
        public DbSet<TypePermission> TypePermission { get; set; }
        public DbSet<CompanyInfo> CompanyInfo { get; set; }
        public DbSet<FileUpload> FileUpload { get; set; }
        public DbSet<MimeType> MimeType { get; set; }
        public DbSet<PayCondition> PayCondition { get; set; }

        public DbSet<WareHouse> WareHouse { get; set; }

        public DbSet<SellerRegion> SellerRegion { get; set; }
        public DbSet<Seller> Seller { get; set; }
        //Customers
        public DbSet<CustomerCategory> CustomerCategory { get; set; }
        public DbSet<CustomerFrequency> CustomerFrequency { get; set; }
        public DbSet<CustomerZone> CustomerZone { get; set; }
        public DbSet<PriceList> PriceList { get; set; }
        public DbSet<PriceSpecialCustomerDetail> PriceSpecialCustomerDetail { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<ItemPriceList> ItemPriceList { get; set; }
        //Suppliers
        public DbSet<SupplierCategory> SupplierCategory { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<BPJornal> BPJornal { get; set; }
        //Items
        public DbSet<ItemCategory> ItemCategory { get; set; }
        public DbSet<ItemFamily> ItemFamily { get; set; }
        public DbSet<UnitOfMeasure> UnitOfMeasure { get; set; }
        public DbSet<ItemWareHouse> ItemWareHouse { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<PriceListDetail> PriceListDetail { get; set; }
        //Inventory Transaction
        public DbSet<InventoryTransactionType> InventoryTransactionType { get; set; }
        public DbSet<ItemJournal> ItemJournal { get; set; }
        public DbSet<InventoryEntry> InventoryEntry { get; set; }
        public DbSet<InventoryEntryDetail> InventoryEntryDetail { get; set; }
        public DbSet<InventoryOutPut> InventoryOutPut { get; set; }
        public DbSet<InventoryOutPutDetail> InventoryOutPutDetail { get; set; }
        public DbSet<InventoryTransfer> InventoryTransfer { get; set; }
        public DbSet<InventoryTransferDetail> InventoryTransferDetail { get; set; }
        public DbSet<CostRevaluation> CostRevaluation { get; set; }
        public DbSet<InventoryRequestTransfer> InventoryRequestTransfer { get; set; }
        public DbSet<InventoryRequestTransferDetail> InventoryRequestTransferDetail { get; set; }
        public DbSet<InventoryReturn> InventoryReturn { get; set; }
        public DbSet<InventoryReturnDetail> InventoryReturnDetail { get; set; }

        //Purchase
        public DbSet<OrderPurchase> OrderPurchase { get; set; }
        public DbSet<OrderPurchaseDetail> OrderPurchaseDetail { get; set; }
        public DbSet<InvoicePurchase> InvoicePurchase { get; set; }
        public DbSet<InvoicePurchaseDetail> InvoicePurchaseDetail { get; set; }
        //PaymentPurchase
        public DbSet<PaymentPurchase> PaymentPurchase { get; set; }
        public DbSet<PaymentPurchaseDetail> PaymentPurchaseDetail { get; set; }
        #region SAR
        public DbSet<SarBranch> SarBranch { get; set; }
        public DbSet<SarCorrelative> SarCorrelative { get; set; }
        public DbSet<SarPointSale> SarPointSale { get; set; }
        public DbSet<SarTypeDocument> SarTypeDocument { get; set; }
        #endregion

        //Sale
        public DbSet<OrderSale> OrderSale { get; set; }
        public DbSet<OrderSaleDetail> OrderSaleDetail { get; set; }
        public DbSet<InvoiceSale> InvoiceSale { get; set; }
        public DbSet<InvoiceSaleDetail> InvoiceSaleDetail { get; set; }
        //PaymentPurchase
        public DbSet<PaymentSale> PaymentSale { get; set; }
        public DbSet<PaymentSaleDetail> PaymentSaleDetail { get; set; }
        //Expense
        public DbSet<ExpenseType> ExpenseType { get; set; }
        public DbSet<Expense> Expense { get; set; }
        public DbSet<ExpenseDetail> ExpenseDetail { get; set; }
        //Liquidation
        public DbSet<Liquidation> Liquidation { get; set; }
        public DbSet<LiquidationDetail> LiquidationDetail { get; set; }
        public DbSet<LiquidationView> LiquidationView { get; set; }
        public DbSet<ItemWareHouseViewModel> ItemWareHouseViewModel { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new User.Map(modelBuilder.Entity<User>());
            new Permission.Map(modelBuilder.Entity<Permission>());
            new RolePermission.Map(modelBuilder.Entity<RolePermission>());
            new Role.Map(modelBuilder.Entity<Role>());
            new Theme.Map(modelBuilder.Entity<Theme>());
            new TypePermission.Map(modelBuilder.Entity<TypePermission>());       
            new FileUpload.Map(modelBuilder.Entity<FileUpload>()); 
            new MimeType.Map(modelBuilder.Entity<MimeType>());
            new PayCondition.Map(modelBuilder.Entity<PayCondition>());

            new WareHouse.Map(modelBuilder.Entity<WareHouse>());

            new SellerRegion.Map(modelBuilder.Entity<SellerRegion>());
            new Seller.Map(modelBuilder.Entity<Seller>());
            //Customers
            new CustomerCategory.Map(modelBuilder.Entity<CustomerCategory>());
            new CustomerFrequency.Map(modelBuilder.Entity<CustomerFrequency>());
            new CustomerZone.Map(modelBuilder.Entity<CustomerZone>());
            new PriceList.Map(modelBuilder.Entity<PriceList>());
            new PriceSpecialCustomerDetail.Map(modelBuilder.Entity<PriceSpecialCustomerDetail>());
            new Customer.Map(modelBuilder.Entity<Customer>());
            modelBuilder.Entity<ItemPriceList>().ToTable("ItemPrice");
            //Supplier
            new SupplierCategory.Map(modelBuilder.Entity<SupplierCategory>());
            new Supplier.Map(modelBuilder.Entity<Supplier>());
            new BPJornal.Map(modelBuilder.Entity<BPJornal>());
            //Items
            new ItemCategory.Map(modelBuilder.Entity<ItemCategory>());
            new ItemFamily.Map(modelBuilder.Entity<ItemFamily>());
            new UnitOfMeasure.Map(modelBuilder.Entity<UnitOfMeasure>());
            new ItemWareHouse.Map(modelBuilder.Entity<ItemWareHouse>());
            new Item.Map(modelBuilder.Entity<Item>());
            new PriceListDetail.Map(modelBuilder.Entity<PriceListDetail>());
            //Inventory Transaction
            new InventoryTransactionType.Map(modelBuilder.Entity<InventoryTransactionType>());
            new ItemJournal.Map(modelBuilder.Entity<ItemJournal>());
            new InventoryEntry.Map(modelBuilder.Entity<InventoryEntry>());
            new InventoryEntryDetail.Map(modelBuilder.Entity<InventoryEntryDetail>());
            new InventoryOutPut.Map(modelBuilder.Entity<InventoryOutPut>());
            new InventoryOutPutDetail.Map(modelBuilder.Entity<InventoryOutPutDetail>());
            new InventoryTransfer.Map(modelBuilder.Entity<InventoryTransfer>());
            new InventoryTransferDetail.Map(modelBuilder.Entity<InventoryTransferDetail>());
            new CostRevaluation.Map(modelBuilder.Entity<CostRevaluation>());
            new InventoryRequestTransfer.Map(modelBuilder.Entity<InventoryRequestTransfer>());
            new InventoryRequestTransferDetail.Map(modelBuilder.Entity<InventoryRequestTransferDetail>());
            new InventoryReturn.Map(modelBuilder.Entity<InventoryReturn>());
            new InventoryReturnDetail.Map(modelBuilder.Entity<InventoryReturnDetail>());
            //Purchase
            new OrderPurchase.Map(modelBuilder.Entity<OrderPurchase>());
            new OrderPurchaseDetail.Map(modelBuilder.Entity<OrderPurchaseDetail>());
            new InvoicePurchase.Map(modelBuilder.Entity<InvoicePurchase>());
            new InvoicePurchaseDetail.Map(modelBuilder.Entity<InvoicePurchaseDetail>());
            //PaymentPurchase
            new PaymentPurchase.Map(modelBuilder.Entity<PaymentPurchase>());
            new PaymentPurchaseDetail.Map(modelBuilder.Entity<PaymentPurchaseDetail>());
            #region SAR
            new SarBranch.Map(modelBuilder.Entity<SarBranch>());
            new SarCorrelative.Map(modelBuilder.Entity<SarCorrelative>());
            new SarPointSale.Map(modelBuilder.Entity<SarPointSale>());
            new SarTypeDocument.Map(modelBuilder.Entity<SarTypeDocument>());
            #endregion
            //Sale
            new OrderSale.Map(modelBuilder.Entity<OrderSale>());
            new OrderSaleDetail.Map(modelBuilder.Entity<OrderSaleDetail>());
            new InvoiceSale.Map(modelBuilder.Entity<InvoiceSale>());
            new InvoiceSaleDetail.Map(modelBuilder.Entity<InvoiceSaleDetail>());
            //PaymentSale
            new PaymentSale.Map(modelBuilder.Entity<PaymentSale>());
            new PaymentSaleDetail.Map(modelBuilder.Entity<PaymentSaleDetail>());
            //Expense
            new ExpenseType.Map(modelBuilder.Entity<ExpenseType>());
            new Expense.Map(modelBuilder.Entity<Expense>());
            new ExpenseDetail.Map(modelBuilder.Entity<ExpenseDetail>());
            //Liquidation         
            new Liquidation.Map(modelBuilder.Entity<Liquidation>());
            new LiquidationDetail.Map(modelBuilder.Entity<LiquidationDetail>());
            modelBuilder.Entity<LiquidationView>().ToTable("LiquidationView", "dbo")
              .HasKey(c => c.Id);
            modelBuilder.Entity<ItemWareHouseViewModel>().ToTable("ItemPriceView", "dbo").HasNoKey();
            base.OnModelCreating(modelBuilder);
        }
    }
}

