using Pos.WebApi.Features.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pos.WebApi.Features.Common.Dto
{
    public class SarCorrelativeDto: SarCorrelative
    {
        public string UserName { get; set; }
        public string PointSale { get; set; }
        public string Branch { get; set; }
        public string TypeDocumentName { get; set; }
    }
}
