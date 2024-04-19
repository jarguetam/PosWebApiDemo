using Pos.WebApi.Features.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pos.WebApi.Features.Common.Dto
{
    public class SarPointSaleDto: SarPointSale
    {
        public string UserName { get; set; }
        public string BranchName { get; set; }

    }
}
