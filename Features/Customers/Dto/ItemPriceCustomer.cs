namespace Pos.WebApi.Features.Customers.Dto
{
    public class ItemPriceCustomer
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal PriceList1 { get; set; }
        public decimal PriceList2 { get; set; }
        public decimal PriceList3 { get; set; }
        public decimal PriceList4 { get; set; }
        public decimal PriceList5 { get; set; }
        public decimal PriceList6 { get; set; }
        public decimal PriceList7 { get; set; }
        public decimal PriceList8 { get; set; }
        public decimal PriceList9 { get; set; }
        public decimal PriceList10 { get; set; }

        public bool PriceList1Enabled { get; set; }
        public bool PriceList2Enabled { get; set; }
        public bool PriceList3Enabled { get; set; }
        public bool PriceList4Enabled { get; set; }
        public bool PriceList5Enabled { get; set; }
        public bool PriceList6Enabled { get; set; }
        public bool PriceList7Enabled { get; set; }
        public bool PriceList8Enabled { get; set; }
        public bool PriceList9Enabled { get; set; }
        public bool PriceList10Enabled { get; set; }
        public bool IsModified { get; set; }
    }
}
