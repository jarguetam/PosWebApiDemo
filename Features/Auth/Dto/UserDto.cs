using System.Collections.Generic;
using Pos.WebApi.Features.Users.Entities;
using Pos.WebApi.Features.Common.Dto;

namespace DPos.Features.Auth.Dto
{
    public class UserDto : User
    {
        public string Theme { get; set; }
        public string Role { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<MenuDto> Menu { get; set; }
        public string Token { get; set; }
        public string CorrelativeName { get; set; }
        public string SellerName { get; set; }
    }
}
