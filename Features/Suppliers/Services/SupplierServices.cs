using Pos.WebApi.Infraestructure;
using System.Collections.Generic;
using System;
using Pos.WebApi.Features.Suppliers.Dto;
using System.Linq;
using Pos.WebApi.Features.Suppliers.Entities;


namespace Pos.WebApi.Features.Suppliers.Services
{
    public class SupplierServices
    {
        private readonly PosDbContext _context;

        public SupplierServices(PosDbContext context)
        {
            _context = context;
        }

        public List<SupplierCategoryDto> GetSupplierCategory()
        {
            var supplierCategory = _context.SupplierCategory.ToList();
            var userId = supplierCategory.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from c in supplierCategory
                          join u in user on c.CreateBy equals u.UserId
                          select new SupplierCategoryDto
                          {
                              SupplierCategoryId = c.SupplierCategoryId,
                              SupplierCategoryName = c.SupplierCategoryName,
                              CreateBy = c.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = c.CreateDate,
                              Active = c.Active
                          }).ToList();
            return result;

        }

        public List<SupplierCategoryDto> GetSupplierCategoryActive()
        {
            var supplierCategory = _context.SupplierCategory.Where(x=> x.Active==true).ToList();
            var userId = supplierCategory.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from w in supplierCategory
                          join u in user on w.CreateBy equals u.UserId
                          where w.Active == true
                          select new SupplierCategoryDto
                          {
                              SupplierCategoryId = w.SupplierCategoryId,
                              SupplierCategoryName = w.SupplierCategoryName,
                              CreateBy = w.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = w.CreateDate,
                              Active = w.Active
                          }).ToList();
            return result;

        }

        public List<SupplierCategoryDto> AddSupplierCategory(SupplierCategory request)
        {
            request.IsValid();
            request.Active = true;
            request.CreateDate = DateTime.Now;
            _context.SupplierCategory.Add(request);
            _context.SaveChanges();
            return GetSupplierCategory();
        }

        public List<SupplierCategoryDto> EditSupplierCategory(SupplierCategory request)
        {
            request.IsValid();
            var currentSupplierCategory = _context.SupplierCategory.Where(x => x.SupplierCategoryId == request.SupplierCategoryId).FirstOrDefault();
            currentSupplierCategory.SupplierCategoryName = request.SupplierCategoryName;
            currentSupplierCategory.UpdateBy = request.CreateBy;
            currentSupplierCategory.UpdateDate = DateTime.Now;
            currentSupplierCategory.Active = request.Active;
            _context.SaveChanges();
            return GetSupplierCategory();
        }
        //Supplier
        public List<SupplierDto> GetSupplier()
        {
            var customer = _context.Supplier.ToList();
            var userId = customer.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();        
            var categoryId = customer.Select(x => x.SupplierCategoryId).Distinct().ToList();
            var category = _context.SupplierCategory.Where(x => categoryId.Contains(x.SupplierCategoryId)).ToList();
            var payId = customer.Select(x => x.PayConditionId).Distinct().ToList();
            var pay = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();
            var result = (from c in customer                    
                          join ca in category on c.SupplierCategoryId equals ca.SupplierCategoryId
                          join p in pay on c.PayConditionId equals p.PayConditionId
                          join u in user on c.CreateBy equals u.UserId
                          select new SupplierDto
                          {
                              SupplierId = c.SupplierId,
                              SupplierCode = c.SupplierCode,
                              SupplierName = c.SupplierName,
                              Rtn = c.Rtn,
                              Phone = c.Phone,
                              Email = c.Email,
                              Address = c.Address,
                              SupplierCategoryId = c.SupplierCategoryId,
                              CategoryName = ca.SupplierCategoryName,
                              PayConditionId = c.PayConditionId,
                              PayConditionName = p.PayConditionName,
                              PayConditionDays = p.PayConditionDays,
                              Balance = c.Balance,
                              CreditLine = c.CreditLine,
                              Tax = c.Tax,
                              CreateBy = c.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = c.CreateDate,
                              Active = c.Active
                          }).ToList();
            return result;

        }

        public List<SupplierDto> GetSupplierActive()
        {
            var customer = _context.Supplier.Where(x => x.Active == true).ToList();
            var userId = customer.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var categoryId = customer.Select(x => x.SupplierCategoryId).Distinct().ToList();
            var category = _context.SupplierCategory.Where(x => categoryId.Contains(x.SupplierCategoryId)).ToList();
            var payId = customer.Select(x => x.PayConditionId).Distinct().ToList();
            var pay = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();
            var result = (from c in customer
                          join ca in category on c.SupplierCategoryId equals ca.SupplierCategoryId
                          join p in pay on c.PayConditionId equals p.PayConditionId
                          join u in user on c.CreateBy equals u.UserId
                          select new SupplierDto
                          {
                              SupplierId = c.SupplierId,
                              SupplierCode = c.SupplierCode,
                              SupplierName = c.SupplierName,
                              Rtn = c.Rtn,
                              Phone = c.Phone,
                              Email = c.Email,
                              Address = c.Address,
                              SupplierCategoryId = c.SupplierCategoryId,
                              CategoryName = ca.SupplierCategoryName,
                              PayConditionId = c.PayConditionId,
                              PayConditionName = p.PayConditionName,
                              PayConditionDays = p.PayConditionDays,
                              Balance = c.Balance,
                              CreditLine = c.CreditLine,
                              Tax = c.Tax,
                              CreateBy = c.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = c.CreateDate,
                              Active = c.Active
                          }).ToList();
            return result;
        }

        public List<SupplierDto> AddSupplier(Supplier request)
        {
            request.IsValid();
            var lastID = _context.Supplier.OrderByDescending(x => x.SupplierCode).FirstOrDefault();
            if (lastID == null)
            {
                request.SupplierCode = "P0000001";
            }
            else
            {
                var numberInString = lastID.SupplierCode.ToString().Replace("P", "");
                var number = int.Parse(numberInString) + 1;
                string newCorrelative = "P" + number.ToString().PadLeft(7, ' ').Replace(' ', '0');
                string numberString = number.ToString();
                string ReciboSinZona = "0000000" + number;
                numberString = "P" + ReciboSinZona.Substring(numberString.Length);
                request.SupplierCode = numberString;
            }
            request.Balance = 0;
            request.Active = true;
            request.CreateDate = DateTime.Now;
            _context.Supplier.Add(request);
            _context.SaveChanges();
            return GetSupplier();
        }

        public List<SupplierDto> EditSupplier(Supplier request)
        {
            request.IsValid();
            var currentSupplier = _context.Supplier.Where(x => x.SupplierId == request.SupplierId).FirstOrDefault();
            currentSupplier.SupplierName = request.SupplierName;
            currentSupplier.Rtn = request.Rtn;
            currentSupplier.Phone = request.Phone;
            currentSupplier.Email = request.Email;
            currentSupplier.Address = request.Address;
            currentSupplier.SupplierCategoryId = request.SupplierCategoryId;
            currentSupplier.PayConditionId = request.PayConditionId;
            currentSupplier.CreditLine = request.CreditLine;
            currentSupplier.Tax = request.Tax;
            currentSupplier.UpdateBy = request.CreateBy;
            currentSupplier.UpdateDate = DateTime.Now;
            currentSupplier.Active = request.Active;
            _context.SaveChanges();
            return GetSupplier();
        }

        public bool UpdateBalanceSupplier(int supplierId, decimal total)
        {
            try
            {
                var currentSupplier = _context.Supplier.Where(x => x.SupplierId == supplierId).FirstOrDefault();
                currentSupplier.Balance += total;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }        
        }

    }
}
