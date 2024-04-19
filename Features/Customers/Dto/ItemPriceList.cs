using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pos.WebApi.Features.Customers.Dto
{
    public class ItemPriceList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal PriceList1 { get; set; }
        public decimal PriceList2 { get; set; }
        public decimal PriceList3 { get; set; }
        public decimal PriceList4 { get; set; }
        public decimal PriceList5 { get; set; }
        public decimal? PriceList6 { get; set; }
        public decimal? PriceList7 { get; set; }
        public decimal? PriceList8 { get; set; }
        public decimal? PriceList9 { get; set; }
        public decimal? PriceList10 { get; set; }

    }
}
