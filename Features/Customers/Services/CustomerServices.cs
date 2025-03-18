using Pos.WebApi.Infraestructure;
using System.Collections.Generic;
using System.Linq;
using System;
using Pos.WebApi.Features.Customers.Dto;
using Pos.WebApi.Features.Customers.Entities;
using Pos.WebApi.Features.Items.Dto;
using Pos.WebApi.Features.Items.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Pos.WebApi.Features.Customers.Services
{
    public class CustomerServices
    {
        private readonly PosDbContext _context;
        private readonly IConfiguration _config;

        public CustomerServices(PosDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        //Category
        public List<CustomerCategoryDto> GetCustomerCategory()
        {
            var customerCategory = _context.CustomerCategory.ToList();
            var userId = customerCategory.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from c in customerCategory
                          join u in user on c.CreateBy equals u.UserId
                          select new CustomerCategoryDto
                          {
                              CustomerCategoryId = c.CustomerCategoryId,
                              CustomerCategoryName = c.CustomerCategoryName,
                              CreateBy = c.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = c.CreateDate,
                              Active = c.Active
                          }).ToList();
            return result;

        }

        public List<CustomerCategoryDto> GetCustomerCategoryActive()
        {
            var customerCategory = _context.CustomerCategory.ToList();
            var userId = customerCategory.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from w in customerCategory
                          join u in user on w.CreateBy equals u.UserId
                          where w.Active == true
                          select new CustomerCategoryDto
                          {
                              CustomerCategoryId = w.CustomerCategoryId,
                              CustomerCategoryName = w.CustomerCategoryName,
                              CreateBy = w.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = w.CreateDate,
                              Active = w.Active
                          }).ToList();
            return result;

        }

        public List<CustomerCategoryDto> AddCustomerCategory(CustomerCategory request)
        {
            request.IsValid();
            request.Active = true;
            request.CreateDate = DateTime.Now;
            _context.CustomerCategory.Add(request);
            _context.SaveChanges();
            return GetCustomerCategory();
        }

        public List<CustomerCategoryDto> EditCustomerCategory(CustomerCategory request)
        {
            request.IsValid();
            var currentCustomerCategory = _context.CustomerCategory.Where(x => x.CustomerCategoryId == request.CustomerCategoryId).FirstOrDefault();
            currentCustomerCategory.CustomerCategoryName = request.CustomerCategoryName;
            currentCustomerCategory.UpdateBy = request.CreateBy;
            currentCustomerCategory.UpdateDate = DateTime.Now;
            currentCustomerCategory.Active = request.Active;
            _context.SaveChanges();
            return GetCustomerCategory();
        }
        //Frequency
        public List<CustomerFrequency> GetCustomerFrequency()
        {
         return  _context.CustomerFrequency.ToList();
        }

        public List<CustomerFrequency> AddCustomerFrequency(CustomerFrequency request)
        {   
            request.CreateDate = DateTime.Now;
            _context.CustomerFrequency.Add(request);
            _context.SaveChanges();
            return GetCustomerFrequency();
        }

        public List<CustomerFrequency> EditCustomerFrequency(CustomerFrequency request)
        {     
            var currentCustomerFrequency = _context.CustomerFrequency.Where(x => x.Id == request.Id).FirstOrDefault();
            currentCustomerFrequency.FrequencyName = request.FrequencyName;
            _context.SaveChanges();
            return GetCustomerFrequency();
        }
        //Zone
        public List<CustomerZone> GetCustomerZone()
        {
            return _context.CustomerZone.ToList();
        }

        public List<CustomerZone> AddCustomerZone(CustomerZone request)
        {
            request.CreateDate = DateTime.Now;
            _context.CustomerZone.Add(request);
            _context.SaveChanges();
            return GetCustomerZone();
        }

        public List<CustomerZone> EditCustomerZone(CustomerZone request)
        {
            var currentCustomerZone = _context.CustomerZone.Where(x => x.Id == request.Id).FirstOrDefault();
            currentCustomerZone.ZoneName = request.ZoneName;
            _context.SaveChanges();
            return GetCustomerZone();
        }
        //PriceList
        public List<PriceListDto> GetPriceList()
        {
            var priceList = _context.PriceList.ToList();
            var userId = priceList.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from c in priceList
                          join u in user on c.CreateBy equals u.UserId
                          select new PriceListDto
                          {
                              ListPriceId = c.ListPriceId,
                              ListPriceName = c.ListPriceName,
                              PorcentGain = c.PorcentGain,
                              CreateBy = c.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = c.CreateDate,
                              Active = c.Active
                          }).ToList();
            return result;

        }

        public List<PriceListDto> GetPriceListActive()
        {
            var priceList = _context.PriceList.ToList();
            var userId = priceList.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from c in priceList
                          join u in user on c.CreateBy equals u.UserId
                          where c.Active == true
                          select new PriceListDto
                          {
                              ListPriceId = c.ListPriceId,
                              ListPriceName = c.ListPriceName,
                              PorcentGain = c.PorcentGain,
                              CreateBy = c.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = c.CreateDate,
                              Active = false
                          }).ToList();
            return result;

        }

        public List<PriceListDto> AddPriceList(PriceList request)
        {
            request.IsValid();
            try
            {
                _context.Database.BeginTransaction();
                request.Active = true;
                request.CreateDate = DateTime.Now;
                _context.PriceList.Add(request);
                _context.SaveChanges();
                var result = AddItemPriceList(request.ListPriceId);
                if (result == "OK")
                {
                    _context.Database.CommitTransaction();
                }
                else
                {
                    throw new Exception("Ocurrio un error al agregar la lista de precio.");
                }
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.Message);
            }

            return GetPriceList();
        }
        public string AddItemPriceList(int priceListId)
        {
            var itemsdata = _context.Item.ToList();
            var itemsPriceList = itemsdata.Select(x => new PriceListDetail
            {
                ListPriceId = priceListId,
                ItemId = x.ItemId,
                Price = 0,
                CreateDate = DateTime.Now,
            });
            _context.PriceListDetail.AddRange(itemsPriceList);
            _context.SaveChanges();
            return "OK";
        }
        public List<PriceListDto> EditPriceList(PriceList request)
        {
            request.IsValid();
            var currentPrice = _context.PriceList.Where(x => x.ListPriceId == request.ListPriceId).FirstOrDefault();
            currentPrice.ListPriceName = request.ListPriceName;
            currentPrice.PorcentGain = request.PorcentGain;
            currentPrice.UpdateBy = request.CreateBy;
            currentPrice.UpdateDate = DateTime.Now;
            currentPrice.Active = request.Active;
            var listDetail = GetPriceListDetail(request.ListPriceId);
           // listDetail.ForEach(x => UpdatePriceListDetail(x.ItemId, 0));
            _context.SaveChanges();
            return GetPriceList();
        }

        public List<PriceListDetailDto> GetPriceListDetail(int priceListId)
        {
            var priceList = _context.PriceListDetail.Where(x => x.ListPriceId == priceListId).ToList();
            var itemId = priceList.Select(x => x.ItemId).Distinct().ToList();
            var item = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var result = (from p in priceList
                          join i in item on p.ItemId equals i.ItemId
                          select new PriceListDetailDto
                          {
                              PriceListDetailId = p.PriceListDetailId,
                              ListPriceId = p.ListPriceId,
                              ItemCode = i.ItemCode,
                              ItemName = i.ItemName,
                              ItemId = i.ItemId,
                              Price = p.Price,
                              CreateDate = p.CreateDate,
                              IsModified = false,
                          }).ToList();
            return result.OrderBy(x => x.ItemCode).ToList();
        }

        public List<PriceListDetailDto> GetPriceSpecialListDetail(int idPriceList, int customerId)
        {
            var priceList = _context.PriceListDetail.Where(x => x.ListPriceId == idPriceList).ToList();
            var itemId = priceList.Select(x => x.ItemId).Distinct().ToList();
            var item = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var specialPrices = _context.PriceSpecialCustomerDetail.Where(x => x.CustomerId == customerId).ToList();

            var result = (from p in priceList
                          join i in item on p.ItemId equals i.ItemId
                          join s in specialPrices on i.ItemId equals s.ItemId into specialPrice
                          from s in specialPrice.DefaultIfEmpty()
                          select new PriceListDetailDto
                          {
                              PriceListDetailId = p.PriceListDetailId,
                              ListPriceId = s?.PriceListId ?? p.ListPriceId,
                              ItemCode = i.ItemCode,
                              ItemName = i.ItemName,
                              ItemId = i.ItemId,
                              Price = p.Price,
                              PriceSpecial = s?.PriceSpecial ?? 0,
                              CreateDate = p.CreateDate,
                              IsModified = false,
                          }).ToList();
            return result.OrderBy(x => x.ItemCode).ToList();
        }
        public List<ItemPriceList> GetItemPrices()
        {
            var results = _context.ItemPriceList.FromSqlRaw("EXEC [dbo].[ItemPrice]").ToList();
            return results;
        }

        public List<ItemPriceCustomer> GetPricesCustomer(int idPriceList, int customerId)
        {
            var specialPrices = GetPriceSpecialListDetail(idPriceList, customerId);
            var itemPrice = GetItemPrices();
            var result = (from i in itemPrice
                          join s in specialPrices on i.ItemCode equals s.ItemCode into specialPrice
                          from s in specialPrice.DefaultIfEmpty()
                          select new ItemPriceCustomer
                          {
                              ItemId= i.ItemId,
                              ItemCode = i.ItemCode,
                              ItemName = i.ItemName,
                              PriceList1 = (decimal)i.PriceList1,
                              PriceList1Enabled = s.ListPriceId == 1 ? true : false,
                              PriceList2 = (decimal)i.PriceList2,
                              PriceList2Enabled = s.ListPriceId == 2 ? true : false,
                              PriceList3 = (decimal)i.PriceList3,
                              PriceList3Enabled = s.ListPriceId == 3 ? true : false,
                              PriceList4 = (decimal)i.PriceList4,
                              PriceList4Enabled = s.ListPriceId == 4 ? true : false,
                              PriceList5 = (decimal)i.PriceList5,
                              PriceList5Enabled = s.ListPriceId == 5 ? true : false,
                              PriceList6 = (decimal)i.PriceList6,
                              PriceList6Enabled = s.ListPriceId == 6 ? true : false,
                              PriceList7 = (decimal)i.PriceList7,
                              PriceList7Enabled = s.ListPriceId == 7 ? true : false,
                              PriceList8 = (decimal)i.PriceList8,
                              PriceList8Enabled = s.ListPriceId == 8 ? true : false,
                              PriceList9 = (decimal)i.PriceList9,
                              PriceList9Enabled = s.ListPriceId == 9 ? true : false,
                              PriceList10 = (decimal)i.PriceList10,
                              PriceList10Enabled = s.ListPriceId == 10 ? true : false,
                          }).ToList();
            return result;
        }
        public string AddSpecialPriceCustomer(List<PriceSpecialCustomerDetail> request)
        {
            if ((request.Count == 0)) throw new System.Exception("Debe agregar al menos un precio especial.");
            foreach (var item in request)
            {
                item.IsValid();
                var currentPrice = _context.PriceSpecialCustomerDetail.Where(x => x.ItemId == item.ItemId && x.CustomerId == item.CustomerId).FirstOrDefault();
                if (currentPrice != null)
                {
                    currentPrice.PriceSpecial = item.PriceSpecial;
                    currentPrice.PriceListId = item.PriceListId;
                    currentPrice.UpdateBy = item.CreateBy;
                    currentPrice.UpdateDate = DateTime.Now;
                }
                else
                {
                    item.CreateDate = DateTime.Now;
                    _context.PriceSpecialCustomerDetail.Add(item);
                }

            }
            _context.SaveChanges();

            return "Se agregaron precios especiales a este cliente";
        }
        public List<PriceListDetailDto> EditPriceListDetail(List<PriceListDetail> request)
        {
            foreach (var item in request)
            {
                item.IsValid();
                var currentPrice = _context.PriceListDetail.Where(x => x.PriceListDetailId == item.PriceListDetailId).FirstOrDefault();
                currentPrice.Price = item.Price;
                currentPrice.UpdateBy = item.UpdateBy;
                currentPrice.UpdateDate = DateTime.Now;
            }
            _context.SaveChanges();

            return GetPriceListDetail(request[0].ListPriceId);
        }
        public string UpdatePriceListDetail(int itemId, decimal cost)
        {
            try
            {
                var itemMaster = _context.Item.Where(x => x.ItemId == itemId).FirstOrDefault();
                var pricelist = GetPriceList();
                foreach (var item in pricelist)
                {
                    var currentPrice = _context.PriceListDetail.Where(x => x.ListPriceId == item.ListPriceId && x.ItemId == itemId).FirstOrDefault();
                    currentPrice.Price = itemMaster.AvgPrice + ((itemMaster.AvgPrice * item.PorcentGain) / 100);
                    _context.PriceListDetail.Update(currentPrice);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "OK";
        }
        //Customers
        public List<CustomerDto> GetCustomer()
        {
            var customer = _context.Customer.ToList();
            var userId = customer.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var sellerId = customer.Select(x => x.SellerId).Distinct().ToList();
            var seller = _context.Seller.Where(x => sellerId.Contains(x.SellerId)).ToList();
            var categoryId = customer.Select(x => x.CustomerCategoryId).Distinct().ToList();
            var category = _context.CustomerCategory.Where(x => categoryId.Contains(x.CustomerCategoryId)).ToList();
            var payId = customer.Select(x => x.PayConditionId).Distinct().ToList();
            var pay = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();
            var priceListId = customer.Select(x => x.ListPriceId).Distinct().ToList();
            var priceList = _context.PriceList.Where(x => priceListId.Contains(x.ListPriceId)).ToList();
            var zoneId = customer.Select(x=> x.ZoneId).Distinct().ToList();
            var zone = _context.CustomerZone.Where(x=> zoneId.Contains(x.Id)).ToList();
            var frequencyId = customer.Select(x => x.FrequencyId).Distinct().ToList();
            var frequency = _context.CustomerFrequency.Where(x => frequencyId.Contains(x.Id)).ToList();
            var result = (from c in customer
                          join s in seller on c.SellerId equals s.SellerId
                          join ca in category on c.CustomerCategoryId equals ca.CustomerCategoryId
                          join p in pay on c.PayConditionId equals p.PayConditionId
                          join pl in priceList on c.ListPriceId equals pl.ListPriceId
                          join u in user on c.CreateBy equals u.UserId
                          join z in zone on c.ZoneId equals z.Id
                          join f in frequency on c.FrequencyId equals f.Id
                          select new CustomerDto
                          {
                              CustomerId = c.CustomerId,
                              CustomerCode = c.CustomerCode,
                              CustomerName = c.CustomerName,
                              Rtn = c.Rtn,
                              Phone = c.Phone,
                              Email = c.Email,
                              Address = c.Address,
                              SellerId = c.SellerId,
                              SellerName = s.SellerName,
                              CustomerCategoryId = c.CustomerCategoryId,
                              CategoryName = ca.CustomerCategoryName,
                              PayConditionId = c.PayConditionId,
                              PayConditionName = p.PayConditionName,
                              ListPriceId = c.ListPriceId,
                              PriceListName = pl.ListPriceName,
                              Balance = c.Balance,
                              CreditLine = c.CreditLine,
                              Tax = c.Tax,
                              CreateBy = c.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = c.CreateDate,
                              Active = c.Active,
                              LimitInvoiceCredit = c.LimitInvoiceCredit,
                              TotalInvoiceCredit = c.TotalInvoiceCredit,
                              ContactPerson = c.ContactPerson,
                              Purchase = c.Purchase,
                              FrequencyId = c.FrequencyId,
                              ZoneId= c.ZoneId,
                              RegionId = c.RegionId,
                              FrequencyName = f.FrequencyName,
                              ZoneName = z.ZoneName
                          }).ToList();
            return result.OrderByDescending(x => x.CustomerCode).ToList();

        }
        public List<CustomerDto> GetCustomerActive()
        {
            string conec = _config["connectionStrings:dbpos"];
            using (var connection = new SqlConnection(conec))
            {
                connection.Open();

                var query = @"
                SELECT 
                    c.CustomerId,
                    c.CustomerCode,
                    c.CustomerName,
                    c.Rtn,
                    c.Phone,
                    c.Email,
                    c.Address,
                    c.SellerId,
                    s.SellerName,
                    c.CustomerCategoryId,
                    ca.CustomerCategoryName AS CategoryName,
                    c.PayConditionId,
                    p.PayConditionName,
                    p.PayConditionDays,
                    c.ListPriceId,
                    pl.ListPriceName AS PriceListName,
                    c.Balance,
                    c.CreditLine,
                    c.Tax,
                    c.CreateBy,
                    u.Name AS CreateByName,
                    c.CreateDate,
                    c.Active,
                    c.LimitInvoiceCredit,
                    c.TotalInvoiceCredit,
                    c.Purchase,
                    c.FrequencyId,
                    c.ZoneId,
                    c.RegionId,
                    f.FrequencyName,
                    z.ZoneName
                FROM 
                    Customer c
                INNER JOIN Seller s ON c.SellerId = s.SellerId
                INNER JOIN CustomerCategory ca ON c.CustomerCategoryId = ca.CustomerCategoryId
                INNER JOIN PayCondition p ON c.PayConditionId = p.PayConditionId
                INNER JOIN PriceList pl ON c.ListPriceId = pl.ListPriceId
                INNER JOIN [User] u ON c.CreateBy = u.UserId
                INNER JOIN CustomerZone z ON c.ZoneId = z.Id
                INNER JOIN CustomerFrequency f ON c.FrequencyId = f.Id
                WHERE 
                    c.Active = 1";
                var result = connection.Query<CustomerDto>(query).ToList();
                return result;
            }
        }
        public List<CustomerDto> AddCustomer(Customer request)
        {
            request.IsValid();
            var lastID = _context.Customer.OrderByDescending(x => x.CustomerCode).FirstOrDefault();
            if (lastID == null)
            {
                request.CustomerCode = "C0000001";
            }
            else
            {
                var numberInString = lastID.CustomerCode.ToString().Replace("C", "");
                var number = int.Parse(numberInString) + 1;
                string newCorrelative = "C" + number.ToString().PadLeft(7, ' ').Replace(' ', '0');
                string numberString = number.ToString();
                string ReciboSinZona = "0000000" + number;
                numberString = "C" + ReciboSinZona.Substring(numberString.Length);
                request.CustomerCode = numberString;
            }

            request.Balance = 0;
            request.Active = true;
            request.CreateDate = DateTime.Now;
            _context.Customer.Add(request);
            _context.SaveChanges();
            return GetCustomer();
        }
        public List<CustomerDto> EditCustomer(Customer request)
        {
            request.IsValid();
            var currentCustomer = _context.Customer.Where(x => x.CustomerId == request.CustomerId).FirstOrDefault();
            currentCustomer.CustomerName = request.CustomerName;
            currentCustomer.Rtn = request.Rtn;
            currentCustomer.Phone = request.Phone;
            currentCustomer.Email = request.Email;
            currentCustomer.Address = request.Address;
            currentCustomer.SellerId = request.SellerId;
            currentCustomer.CustomerCategoryId = request.CustomerCategoryId;
            currentCustomer.PayConditionId = request.PayConditionId;
            currentCustomer.ListPriceId = request.ListPriceId;
            currentCustomer.CreditLine = request.CreditLine;
            currentCustomer.Tax = request.Tax;
            currentCustomer.UpdateBy = request.CreateBy;
            currentCustomer.UpdateDate = DateTime.Now;
            currentCustomer.LimitInvoiceCredit = request.LimitInvoiceCredit;
            currentCustomer.Active = request.Active;
            currentCustomer.ContactPerson = request.ContactPerson;
            currentCustomer.Purchase = request.Purchase;
            currentCustomer.FrequencyId = request.FrequencyId;
            currentCustomer.ZoneId = request.ZoneId;
            currentCustomer.RegionId = request.RegionId;
            _context.SaveChanges();
            return GetCustomer();
        }
        public bool UpdateBalanceCustomer(int customerId, decimal total)
        {
            try
            {
                var currentSupplier = _context.Customer.Where(x => x.CustomerId == customerId).FirstOrDefault();
                currentSupplier.Balance += total;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateBalanceCustomerAsync(int customerId, decimal total)
        {
            try
            {
                var currentCustomer = await _context.Customer
                    .FirstOrDefaultAsync(x => x.CustomerId == customerId);

                if (currentCustomer != null)
                {
                    currentCustomer.Balance += total;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


    }
}
