using DPos.Features.Auth.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Pos.WebApi.Infraestructure;
using Pos.WebApi.Features.Users.Entities;
using Pos.WebApi.Features.Common.Dto;
using System.Collections.Generic;
using Pos.WebApi.Helpers;

namespace Pos.WebApi.Features.Auth
{
    public class AuthService
    {
        private readonly PosDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthService(PosDbContext logisticaBtdDbContext, IConfiguration configuration)
        {
            _context = logisticaBtdDbContext;
            _configuration = configuration;
        }

        public UserDto Auth(User User)
        {
            User.Password = Helper.EncryptPassword(User.Password, _configuration);
            var employee = _context.User
                .Where(x => x.UserName.ToUpper() == User.UserName && x.Password == User.Password).FirstOrDefault();
            if (employee == null) throw new Exception("Usuario o contraseña incorrecta.");
            if (!employee.Active) throw new Exception("Usuario Inactivo.");

            var userPermission = _context.RolePermission.Where(x => x.RoleId == employee.RoleId).ToList();
            var permissionIds = userPermission.Select(x => x.PermissionId).ToList();
            if (userPermission.Count() == 0) throw new Exception("Su usuario no tiene ningún permiso asignado");
            var permissions = _context.Permission.Where(x => permissionIds.Contains(x.PermissionId) && x.Active).ToList();

            var role = _context.Role.Where(x => x.RoleId == employee.RoleId).FirstOrDefault();
            var theme = _context.Theme.Where(x => x.ThemeId == employee.ThemeId).FirstOrDefault();
            if (theme == null) throw new Exception("No se le ha asignado un tema.");
            if (role == null) throw new Exception("No se le ha asignado un rol");

            var dato = new UserDto
            {
                Name = employee.Name,
                UserId = employee.UserId,
                Email = employee.Email,
                UserName = employee.UserName,
                Permissions = permissions.OrderBy(x => x.Position).ToList(),
                Menu = GetMenu(permissions),              
                RoleId = employee.RoleId,
                ThemeId = employee.ThemeId,
                Role = role.Description,
                Theme = theme.Code,
                Token = GenerateJwtToken(employee),
                WhsCode = employee.WhsCode,
                SellerId = employee.SellerId,
                EditPrice = employee.EditPrice,
                SarCorrelativeId = employee.SarCorrelativeId,
                SellerName = _context.Seller.Where(x=> x.SellerId == employee.SellerId).Select(x=> x.SellerName).FirstOrDefault()
            };
            return dato;
        }
        public List<MenuDto> GetMenu(List<Permission> permissions)
        {
            var permissionsNoBtn = permissions.Where(x => x.TypeId == (int)TypePermissionEnum.Pantalla).OrderBy(x => x.Position).ToList();
            var data = permissions.Where(x => x.FatherId == 0 && x.TypeId == (int)TypePermissionEnum.Pantalla).Select(x => new MenuDto
            {
                Icon = x.Icon,
                Label = x.Description,
                MenuId = x.PermissionId,
                PositionId = x.Position,
                //RouterLink = x.Path,
                Items = GetMenuItems(permissionsNoBtn.Where(x => x.FatherId != 0 && x.TypeId == (int)TypePermissionEnum.Pantalla).Where(c => c.FatherId == x.PermissionId).ToList(), permissionsNoBtn)
            }).ToList();
            return data;
        }

        public List<MenuDto> GetMenuItems(List<Permission> permissions, List<Permission> originalPermissions)
        {
            var data = permissions.Select(x => new MenuDto
            {
                Icon = x.Icon,
                Label = x.Description,
                MenuId = x.PermissionId,
                RouterLink = x.Path,
                PositionId = x.Position,
                Items = GetMenuItems(originalPermissions.Where(c => c.FatherId == x.PermissionId).ToList(), originalPermissions).Count() == 0 ? null :
                GetMenuItems(originalPermissions.Where(c => c.FatherId == x.PermissionId).ToList(), originalPermissions)
            }).ToList();
            return data;
        }


        private string GenerateJwtToken(User User)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["secret"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, User.UserName),
                    new Claim(ClaimTypes.NameIdentifier, User.UserId.ToString())
                    //Aqui poner el correo
                }),
                Expires = DateTime.UtcNow.AddDays(1), //TIempo de duracion del token
                Audience = "localhost",
                Issuer = "localhost",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
