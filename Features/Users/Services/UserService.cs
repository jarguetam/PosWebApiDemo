using System;
using System.Collections.Generic;
using System.Linq;
using DPos.Features.Auth.Dto;
using Pos.WebApi.Features.Users.Entities;
using Pos.WebApi.Features.Common.Entities;
using Pos.WebApi.Infraestructure;
using Microsoft.Extensions.Configuration;
using Pos.WebApi.Helpers;

namespace Pos.WebApi.Features.Users
{
    public class UserService
    {
        private readonly PosDbContext _context;
        private readonly IConfiguration _configuration;
        public UserService(PosDbContext logisticaBtdDbContext, IConfiguration configuration)
        {
            _context = logisticaBtdDbContext;
            _configuration = configuration;
        }

        public List<UserDto> Get()
        {
            var users = _context.User.ToList();
            var themes = _context.Theme.ToList();
            var roles = _context.Role.ToList();
            var sarCorrelativo = _context.SarCorrelative.ToList();

            var result = (from u in users
                          join r in roles on u.RoleId equals r.RoleId into userRole
                          from r in userRole.DefaultIfEmpty()
                          join t in themes on u.ThemeId equals t.ThemeId into themeUser
                          from t in themeUser.DefaultIfEmpty()
                          join s in sarCorrelativo on u.SarCorrelativeId equals s.CorrelativeId
                          select new UserDto
                          {
                              Active = u.Active,
                              Email = u.Email,
                              Name = u.Name,
                              Password = null,
                              RoleId = u.RoleId,
                              ThemeId = u.ThemeId,
                              UserId = u.UserId,
                              UserName = u.UserName,
                              Role = r?.Description ??"ROL NO ASIGNADO",
                              Theme = t?.Description ?? "TEMA NO ASIGNADO",
                              SalesPerson = u.SalesPerson,
                              WhsCode = u.WhsCode,
                              SellerId = u.SellerId,
                              EditPrice = u.EditPrice,
                              SarCorrelativeId = u.SarCorrelativeId,
                              CorrelativeName = s.Description
                          }
                          ).ToList();
            return result;
        }

        public List<UserDto> Add(User user)
        {
            user.IsValid();
            if (string.IsNullOrEmpty(user.Password)) throw new Exception("Debe ingresar una contraseña");
            if (user.Password.Length <8) throw new Exception("Debe ingresar una contraseña que contenga al menos 8 caracteres");
            user.Active = true;
            user.Password = Helper.EncryptPassword(user.Password.Trim(), _configuration);
            user.UserName = user.UserName.Trim().ToLower();
            _context.User.Add(user);
            _context.SaveChanges();
            return Get();
        }


        public List<UserDto> Edit(User user)
        {
            user.IsValid();
            if (!string.IsNullOrEmpty(user.Password))
            {
                if (user.Password.Length < 8) throw new Exception("Debe ingresar una contraseña que contenga al menos 8 caracteres");
                user.Password = Helper.EncryptPassword(user.Password.Trim(), _configuration);
            }
            var currentUser = _context.User.Where(x => x.UserId == user.UserId).FirstOrDefault();
            currentUser.Name = user.Name;
            currentUser.Password = user.Password;
            currentUser.Email = user.Email;
            currentUser.RoleId = user.RoleId;
            currentUser.ThemeId = user.ThemeId;
            currentUser.EditPrice = user.EditPrice;
            currentUser.Active = user.Active;
            currentUser.SalesPerson = user.SalesPerson;
            currentUser.WhsCode = user.WhsCode;
            currentUser.SellerId = user.SellerId;
            currentUser.SarCorrelativeId = user.SarCorrelativeId;
            _context.SaveChanges();
            return Get();
        }

        public List<Theme> GetThemes()
        {
            var themes = _context.Theme.ToList();
            return themes;
        }
    }
}
