using System;
using Pos.WebApi.Features.Users.Entities;

namespace Pos.WebApi.Features.Users.Dto
{
    public class PermissionDto: Permission
    {
        public int RoleId { get; set; }
    }
}
