using System.Collections.Generic;

namespace Pos.WebApi.Features.Dashboard.Dto
{
    public class DasboardDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<DashboardDto> Data { get; set; }
    }
}
