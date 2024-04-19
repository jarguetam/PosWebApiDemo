using Pos.WebApi.Features.Items.Entities;

namespace Pos.WebApi.Features.Items.Dto
{
    public class ItemCategoryDto: ItemCategory
    {
        public string CreateByName { get; set; }
    }
}
