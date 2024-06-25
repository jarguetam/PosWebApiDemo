using System;
using System.Collections.Generic;

namespace Pos.WebApi.Features.Reports.Dto
{
    public class CxcreportDto
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public List<CxcreportDetailDto> Detail {  get; set; }    
    }

    public class CxcreportDetailDto
    {
        public int DocNum { get; set; }
        public string Reference { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public decimal DocTotal { get; set; }
        public decimal Balance { get; set; }
        public string Comment { get; set; }
        public string SellerName { get; set; }
        public int Days {  get; set; }
    }
}
