using Pos.WebApi.Features.Items.Entities;
using System.Collections.Generic;

namespace Pos.WebApi.Features.Items.Dto
{
    public class ItemDto: Item
    {
        public string ItemCategoryName { get; set; }
        public string ItemFamilyName { get; set; }
        public string UnitOfMeasureName { get; set; }
        public string CreateByName { get; set; }
        public List<ItemWareHouseDto> ItemWareHouse { get; set; }
    }
}
