using Pos.WebApi.Infraestructure;
using System.Collections.Generic;
using System.Linq;
using System;
using Pos.WebApi.Features.Items.Dto;
using Pos.WebApi.Features.Items.Entities;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.Ocsp;
using Pos.WebApi.Features.Purchase.Dto;
using Pos.WebApi.Features.Purchase.Entities;
using Pos.WebApi.Features.Customers.Entities;

namespace Pos.WebApi.Features.Items.Services
{
    public class ItemServices
    {
        private readonly PosDbContext _context;
        private readonly WareHouseServices _wareHouseServices;

        public ItemServices(PosDbContext posDbContext, WareHouseServices wareHouseServices)
        {
            _context = posDbContext;
            _wareHouseServices = wareHouseServices;
        }
        //Category
        public List<ItemCategoryDto> GetItemCategory()
        {
            var itemCategory = _context.ItemCategory.ToList();
            var userId = itemCategory.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from c in itemCategory
                          join u in user on c.CreateBy equals u.UserId
                          select new ItemCategoryDto
                          {
                              ItemCategoryId = c.ItemCategoryId,
                              ItemCategoryName = c.ItemCategoryName,
                              CreateBy = c.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = c.CreateDate,
                              Active = c.Active
                          }).ToList();
            return result;

        }

        public List<ItemCategoryDto> GetItemCategoryActive()
        {
            var itemCategory = _context.ItemCategory.Where(x => x.Active == true).ToList();
            var userId = itemCategory.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from w in itemCategory
                          join u in user on w.CreateBy equals u.UserId
                          where w.Active == true
                          select new ItemCategoryDto
                          {
                              ItemCategoryId = w.ItemCategoryId,
                              ItemCategoryName = w.ItemCategoryName,
                              CreateBy = w.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = w.CreateDate,
                              Active = w.Active
                          }).ToList();
            return result;

        }

        public List<ItemCategoryDto> AddItemCategory(ItemCategory request)
        {
            request.IsValid();
            request.Active = true;
            request.CreateDate = DateTime.Now;
            _context.ItemCategory.Add(request);
            _context.SaveChanges();
            return GetItemCategory();
        }

        public List<ItemCategoryDto> EditItemCategory(ItemCategory request)
        {
            request.IsValid();
            var currentItemCategory = _context.ItemCategory.Where(x => x.ItemCategoryId == request.ItemCategoryId).FirstOrDefault();
            currentItemCategory.ItemCategoryName = request.ItemCategoryName;
            currentItemCategory.UpdateBy = request.CreateBy;
            currentItemCategory.UpdateDate = DateTime.Now;
            currentItemCategory.Active = request.Active;
            _context.SaveChanges();
            return GetItemCategory();
        }
        //Family
        public List<ItemFamilyDto> GetItemFamily()
        {
            var itemFamily = _context.ItemFamily.ToList();
            var userId = itemFamily.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from c in itemFamily
                          join u in user on c.CreateBy equals u.UserId
                          select new ItemFamilyDto
                          {
                              ItemFamilyId = c.ItemFamilyId,
                              ItemFamilyName = c.ItemFamilyName,
                              CreateBy = c.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = c.CreateDate,
                              Active = c.Active
                          }).ToList();
            return result;

        }

        public List<ItemFamilyDto> GetItemFamilyActive()
        {
            var itemFamily = _context.ItemFamily.Where(x => x.Active == true).ToList();
            var userId = itemFamily.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from w in itemFamily
                          join u in user on w.CreateBy equals u.UserId
                          where w.Active == true
                          select new ItemFamilyDto
                          {
                              ItemFamilyId = w.ItemFamilyId,
                              ItemFamilyName = w.ItemFamilyName,
                              CreateBy = w.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = w.CreateDate,
                              Active = w.Active
                          }).ToList();
            return result;

        }

        public List<ItemFamilyDto> AddItemFamily(ItemFamily request)
        {
            request.IsValid();
            request.Active = true;
            request.CreateDate = DateTime.Now;
            _context.ItemFamily.Add(request);
            _context.SaveChanges();
            return GetItemFamily();
        }

        public List<ItemFamilyDto> EditItemFamily(ItemFamily request)
        {
            request.IsValid();
            var currentItemFamily = _context.ItemFamily.Where(x => x.ItemFamilyId == request.ItemFamilyId).FirstOrDefault();
            currentItemFamily.ItemFamilyName = request.ItemFamilyName;
            currentItemFamily.UpdateBy = request.CreateBy;
            currentItemFamily.UpdateDate = DateTime.Now;
            currentItemFamily.Active = request.Active;
            if (!request.Active)
            {
                var items = _context.Item.Where(x => x.ItemFamilyId == request.ItemFamilyId).Count();
                if (items > 0)
                {
                    throw new System.Exception(@$"Tiene {items} articulos, asignados a esta familia, porfavor actualice los articulos, antes de inactivar.");
                }
            }
            _context.SaveChanges();
            return GetItemFamily();
        }
        public List<UnitOfMeasureDto> GetUnitOfMeasure()
        {
            var itemCategory = _context.UnitOfMeasure.ToList();
            var userId = itemCategory.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from c in itemCategory
                          join u in user on c.CreateBy equals u.UserId
                          select new UnitOfMeasureDto
                          {
                              UnitOfMeasureId = c.UnitOfMeasureId,
                              UnitOfMeasureName = c.UnitOfMeasureName,
                              CreateBy = c.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = c.CreateDate,
                              Active = c.Active
                          }).ToList();
            return result;

        }

        public List<UnitOfMeasureDto> GetUnitOfMeasureActive()
        {
            var itemCategory = _context.UnitOfMeasure.Where(x => x.Active == true).ToList();
            var userId = itemCategory.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from w in itemCategory
                          join u in user on w.CreateBy equals u.UserId
                          where w.Active == true
                          select new UnitOfMeasureDto
                          {
                              UnitOfMeasureId = w.UnitOfMeasureId,
                              UnitOfMeasureName = w.UnitOfMeasureName,
                              CreateBy = w.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = w.CreateDate,
                              Active = w.Active
                          }).ToList();
            return result;

        }

        public List<UnitOfMeasureDto> AddUnitOfMeasure(UnitOfMeasure request)
        {
            request.IsValid();
            request.Active = true;
            request.CreateDate = DateTime.Now;
            _context.UnitOfMeasure.Add(request);
            _context.SaveChanges();
            return GetUnitOfMeasure();
        }

        public List<UnitOfMeasureDto> EditUnitOfMeasure(UnitOfMeasure request)
        {
            request.IsValid();
            var currentUnitOfMeasure = _context.UnitOfMeasure.Where(x => x.UnitOfMeasureId == request.UnitOfMeasureId).FirstOrDefault();
            currentUnitOfMeasure.UnitOfMeasureName = request.UnitOfMeasureName;
            currentUnitOfMeasure.UpdateBy = request.CreateBy;
            currentUnitOfMeasure.UpdateDate = DateTime.Now;
            currentUnitOfMeasure.Active = request.Active;
            _context.SaveChanges();
            return GetUnitOfMeasure();
        }

        //Items
        private List<ItemDto> GetBase(Func<Item, bool> condition)
        {
            var item = _context.Item.Where(condition).ToList();
            var userId = item.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var categoryId = item.Select(x => x.ItemCategoryId).Distinct().ToList();
            var category = _context.ItemCategory.Where(x => categoryId.Contains(x.ItemCategoryId)).ToList();
            var unitId = item.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var unit = _context.UnitOfMeasure.Where(x => unitId.Contains(x.UnitOfMeasureId)).ToList();
            var familyIds = item.Select(x => x.ItemFamilyId).ToList();
            var family = _context.ItemFamily.Where(x => familyIds.Contains(x.ItemFamilyId)).ToList();

            var result = (from i in item
                          join c in category on i.ItemCategoryId equals c.ItemCategoryId
                          join um in unit on i.UnitOfMeasureId equals um.UnitOfMeasureId
                          join f in family on i.ItemFamilyId equals f.ItemFamilyId
                          join u in user on i.CreateBy equals u.UserId
                          select new ItemDto
                          {
                              ItemId = i.ItemId,
                              ItemCode = i.ItemCode,
                              ItemName = i.ItemName,
                              UnitOfMeasureId = i.UnitOfMeasureId,
                              UnitOfMeasureName = um.UnitOfMeasureName,
                              ItemCategoryId = i.ItemCategoryId,
                              ItemCategoryName = c.ItemCategoryName,
                              ItemFamilyId = i.ItemFamilyId,
                              ItemFamilyName = f.ItemFamilyName,
                              Stock = i.Stock,
                              AvgPrice = i.AvgPrice,
                              PricePurchase = i.PricePurchase,
                              Tax = i.Tax,
                              SalesItem = i.SalesItem,
                              PurchaseItem = i.PurchaseItem,
                              InventoryItem = i.InventoryItem,
                              Weight = i.Weight,
                              BarCode = i.BarCode,
                              ItemWareHouse = _wareHouseServices.GetItemWareHouse(i.ItemId),
                              CreateBy = c.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = c.CreateDate,
                              Active = i.Active
                          }).ToList();
            return result;
        }
        public List<ItemDto> GetItemPurchase()
        {
            var result = GetBase(x => x.PurchaseItem == true).ToList();
            return result;
        }
        public List<ItemDto> GetItemSale()
        {
            var result = GetBase(x => x.SalesItem == true).ToList();
            return result;
        }
        public List<ItemDto> GetItem()
        {
            var itemQuery = (from i in _context.Item
                             join c in _context.ItemCategory on i.ItemCategoryId equals c.ItemCategoryId
                             join um in _context.UnitOfMeasure on i.UnitOfMeasureId equals um.UnitOfMeasureId
                             join f in _context.ItemFamily on i.ItemFamilyId equals f.ItemFamilyId
                             join u in _context.User on i.CreateBy equals u.UserId
                             select new ItemDto
                             {
                                 ItemId = i.ItemId,
                                 ItemCode = i.ItemCode,
                                 ItemName = i.ItemName,
                                 UnitOfMeasureId = i.UnitOfMeasureId,
                                 UnitOfMeasureName = um.UnitOfMeasureName,
                                 ItemCategoryId = i.ItemCategoryId,
                                 ItemCategoryName = c.ItemCategoryName,
                                 ItemFamilyId = i.ItemFamilyId,
                                 ItemFamilyName = f.ItemFamilyName,
                                 Stock = i.Stock,
                                 AvgPrice = i.AvgPrice,
                                 PricePurchase = i.PricePurchase,
                                 Tax = i.Tax,
                                 SalesItem = i.SalesItem,
                                 PurchaseItem = i.PurchaseItem,
                                 InventoryItem = i.InventoryItem,
                                 Weight = i.Weight,
                                 BarCode = i.BarCode,
                                 CreateBy = c.CreateBy,
                                 CreateByName = u.Name,
                                 CreateDate = c.CreateDate,
                                 Active = i.Active
                             }).ToList();

            var itemIds = itemQuery.Select(i => i.ItemId).ToList();

            var itemWareHouseQuery = (from iw in _context.ItemWareHouse
                                      join wh in _context.WareHouse on iw.WhsCode equals wh.WhsCode
                                      where itemIds.Contains(iw.ItemId)
                                      select new ItemWareHouseDto
                                      {
                                          ItemId = iw.ItemId,
                                          WhsCode = iw.WhsCode,
                                          WhsName = wh.WhsName,
                                          Stock = iw.Stock,
                                          AvgPrice = iw.AvgPrice,
                                          DueDate = iw.DueDate,
                                          CreateDate = wh.CreateDate,
                                          Active = wh.Active
                                      }).ToList();

            foreach (var item in itemQuery)
            {
                item.ItemWareHouse = itemWareHouseQuery.Where(iw => iw.ItemId == item.ItemId).ToList();
            }

            return itemQuery;
        }
        public List<ItemDto> GetItemByBarcode(string barcode)
        {
            var itemQuery = (from i in _context.Item
                             join c in _context.ItemCategory on i.ItemCategoryId equals c.ItemCategoryId
                             join um in _context.UnitOfMeasure on i.UnitOfMeasureId equals um.UnitOfMeasureId
                             join f in _context.ItemFamily on i.ItemFamilyId equals f.ItemFamilyId
                             join u in _context.User on i.CreateBy equals u.UserId
                             where i.BarCode == barcode || i.ItemCode == barcode
                             select new ItemDto
                             {
                                 ItemId = i.ItemId,
                                 ItemCode = i.ItemCode,
                                 ItemName = i.ItemName,
                                 UnitOfMeasureId = i.UnitOfMeasureId,
                                 UnitOfMeasureName = um.UnitOfMeasureName,
                                 ItemCategoryId = i.ItemCategoryId,
                                 ItemCategoryName = c.ItemCategoryName,
                                 ItemFamilyId = i.ItemFamilyId,
                                 ItemFamilyName = f.ItemFamilyName,
                                 Stock = i.Stock,
                                 AvgPrice = i.AvgPrice,
                                 PricePurchase = i.PricePurchase,
                                 Tax = i.Tax,
                                 SalesItem = i.SalesItem,
                                 PurchaseItem = i.PurchaseItem,
                                 InventoryItem = i.InventoryItem,
                                 Weight = i.Weight,
                                 BarCode = i.BarCode,
                                 CreateBy = c.CreateBy,
                                 CreateByName = u.Name,
                                 CreateDate = c.CreateDate,
                                 Active = i.Active
                             }).ToList();

            var itemIds = itemQuery.Select(i => i.ItemId).ToList();

            var itemWareHouseQuery = (from iw in _context.ItemWareHouse
                                      join wh in _context.WareHouse on iw.WhsCode equals wh.WhsCode
                                      where itemIds.Contains(iw.ItemId)
                                      select new ItemWareHouseDto
                                      {
                                          ItemId = iw.ItemId,
                                          WhsCode = iw.WhsCode,
                                          WhsName = wh.WhsName,
                                          Stock = iw.Stock,
                                          AvgPrice = iw.AvgPrice,
                                          DueDate = iw.DueDate,
                                          CreateDate = wh.CreateDate,
                                          Active = wh.Active
                                      }).ToList();

            foreach (var item in itemQuery)
            {
                item.ItemWareHouse = itemWareHouseQuery.Where(iw => iw.ItemId == item.ItemId).ToList();
            }

            return itemQuery;
        }

        public List<ItemDto> GetItemActive()
        {
            var itemQuery = (from i in _context.Item
                              join c in _context.ItemCategory on i.ItemCategoryId equals c.ItemCategoryId
                              join um in _context.UnitOfMeasure on i.UnitOfMeasureId equals um.UnitOfMeasureId
                              join f in _context.ItemFamily on i.ItemFamilyId equals f.ItemFamilyId
                              join u in _context.User on i.CreateBy equals u.UserId
                             where i.Active && i.InventoryItem
                             select new ItemDto
                              {
                                  ItemId = i.ItemId,
                                  ItemCode = i.ItemCode,
                                  ItemName = i.ItemName,
                                  UnitOfMeasureId = i.UnitOfMeasureId,
                                  UnitOfMeasureName = um.UnitOfMeasureName,
                                  ItemCategoryId = i.ItemCategoryId,
                                  ItemCategoryName = c.ItemCategoryName,
                                  ItemFamilyId = i.ItemFamilyId,
                                  ItemFamilyName = f.ItemFamilyName,
                                  Stock = i.Stock,
                                  AvgPrice = i.AvgPrice,
                                  PricePurchase = i.PricePurchase,
                                  Tax = i.Tax,
                                  SalesItem = i.SalesItem,
                                  PurchaseItem = i.PurchaseItem,
                                  InventoryItem = i.InventoryItem,
                                  Weight = i.Weight,
                                  BarCode = i.BarCode,
                                  CreateBy = c.CreateBy,
                                  CreateByName = u.Name,
                                  CreateDate = c.CreateDate,
                                  Active = i.Active
                              }).ToList();

            var itemIds = itemQuery.Select(i => i.ItemId).ToList();

            var itemWareHouseQuery = (from iw in _context.ItemWareHouse
                                      join wh in _context.WareHouse on iw.WhsCode equals wh.WhsCode
                                      where itemIds.Contains(iw.ItemId)
                                      select new ItemWareHouseDto
                                      {
                                          ItemId = iw.ItemId,
                                          WhsCode = iw.WhsCode,
                                          WhsName = wh.WhsName,
                                          Stock = iw.Stock,
                                          AvgPrice = iw.AvgPrice,
                                          DueDate = iw.DueDate,
                                          CreateDate = wh.CreateDate,
                                          Active = wh.Active
                                      }).ToList();

            foreach (var item in itemQuery)
            {
                item.ItemWareHouse = itemWareHouseQuery.Where(iw => iw.ItemId == item.ItemId).ToList();
            }

            return itemQuery;

        }

        public List<ItemWareHouseDto> GetItemWareHouseStock(int whsCode)
        {
            var stock = _context.ItemWareHouse.Where(x => x.WhsCode == whsCode).ToList();
            var item = _context.Item.Where(x => x.Active == true).ToList();
            var categoryId = item.Select(x => x.ItemCategoryId).Distinct().ToList();
            var category = _context.ItemCategory.Where(x => categoryId.Contains(x.ItemCategoryId)).ToList();
            var unitId = item.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var unit = _context.UnitOfMeasure.Where(x => unitId.Contains(x.UnitOfMeasureId)).ToList();
            var whs = _context.WareHouse.Where(x => x.WhsCode == whsCode).ToList();

            var result = (from iw in stock
                          join i in item on iw.ItemId equals i.ItemId
                          join c in category on i.ItemCategoryId equals c.ItemCategoryId
                          join um in unit on i.UnitOfMeasureId equals um.UnitOfMeasureId
                          join w in whs on iw.WhsCode equals w.WhsCode
                          where i.InventoryItem == true
                          select new ItemWareHouseDto
                          {
                              ItemId = i.ItemId,
                              ItemCode = i.ItemCode,
                              WhsCode = iw.WhsCode,
                              WhsName = w.WhsName,
                              DueDate = iw.DueDate,
                              ItemName = i.ItemName,
                              UnitOfMeasureId = i.UnitOfMeasureId,
                              UnitOfMeasureName = um.UnitOfMeasureName,
                              ItemCategoryName = c.ItemCategoryName,
                              Stock = iw.Stock,
                              AvgPrice = iw.AvgPrice,
                              Tax = i.Tax,
                              BarCode = i.BarCode,
                              Active = c.Active
                          }).ToList();
            return result;

        }

        public List<ItemWareHouseDto> GetItemWareHouseStockBarCode(int whsCode, string barcode)
        {
            var stock = _context.ItemWareHouse.Where(x => x.WhsCode == whsCode).ToList();
            var item = _context.Item.Where(x => x.Active == true && (x.BarCode == barcode || x.ItemCode.Contains(barcode))).ToList();
            var categoryId = item.Select(x => x.ItemCategoryId).Distinct().ToList();
            var category = _context.ItemCategory.Where(x => categoryId.Contains(x.ItemCategoryId)).ToList();
            var unitId = item.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var unit = _context.UnitOfMeasure.Where(x => unitId.Contains(x.UnitOfMeasureId)).ToList();
            var whs = _context.WareHouse.Where(x => x.WhsCode == whsCode).ToList();

            var result = (from iw in stock
                          join i in item on iw.ItemId equals i.ItemId
                          join c in category on i.ItemCategoryId equals c.ItemCategoryId
                          join um in unit on i.UnitOfMeasureId equals um.UnitOfMeasureId
                          join w in whs on iw.WhsCode equals w.WhsCode
                          where i.InventoryItem == true
                          select new ItemWareHouseDto
                          {
                              ItemId = i.ItemId,
                              ItemCode = i.ItemCode,
                              WhsCode = iw.WhsCode,
                              WhsName = w.WhsName,
                              DueDate = iw.DueDate,
                              ItemName = i.ItemName,
                              UnitOfMeasureId = i.UnitOfMeasureId,
                              UnitOfMeasureName = um.UnitOfMeasureName,
                              ItemCategoryName = c.ItemCategoryName,
                              Stock = iw.Stock,
                              AvgPrice = iw.AvgPrice,
                              Tax = i.Tax,
                              BarCode = i.BarCode,
                              Active = c.Active
                          }).ToList();
            return result;

        }

        public List<ItemWareHouseDto> GetItemWareHouseStockPrice(int whsCode, int customerid, int pricelistid)
        {
            var stock = _context.ItemWareHouse.Where(x => x.WhsCode == whsCode).ToList();
            var item = _context.Item.Where(x => x.Active == true).ToList();
            var categoryId = item.Select(x => x.ItemFamilyId).Distinct().ToList();
            var category = _context.ItemFamily.Where(x => categoryId.Contains(x.ItemFamilyId)).ToList();
            var unitId = item.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var unit = _context.UnitOfMeasure.Where(x => unitId.Contains(x.UnitOfMeasureId)).ToList();
            var whs = _context.WareHouse.Where(x => x.WhsCode == whsCode).ToList();
            var prices = _context.PriceListDetail.Where(x => x.ListPriceId == pricelistid).ToList();
            var specialPrices = _context.PriceSpecialCustomerDetail.Where(x => x.CustomerId == customerid && x.PriceSpecial > 0).ToList();
            var itemService = _context.Item.Where(x => x.Active && !x.InventoryItem && x.SalesItem).ToList();
            var result = (from iw in stock
                          join i in item on iw.ItemId equals i.ItemId
                          join c in category on i.ItemFamilyId equals c.ItemFamilyId
                          join um in unit on i.UnitOfMeasureId equals um.UnitOfMeasureId
                          join w in whs on iw.WhsCode equals w.WhsCode
                          join p in prices on iw.ItemId equals p.ItemId
                          join s in specialPrices on iw.ItemId equals s.ItemId into specialPrice
                          from s in specialPrice.DefaultIfEmpty()
                          where i.InventoryItem == true
                          orderby i.ItemId
                          select new ItemWareHouseDto
                          {
                              ItemId = i.ItemId,
                              ItemCode = i.ItemCode,
                              WhsCode = iw.WhsCode,
                              WhsName = w.WhsName,
                              DueDate = iw.DueDate,
                              ItemName = i.ItemName,
                              UnitOfMeasureId = i.UnitOfMeasureId,
                              UnitOfMeasureName = um.UnitOfMeasureName,
                              ItemCategoryName = c.ItemFamilyName,
                              Stock = iw.Stock,
                              AvgPrice = iw.AvgPrice,
                              PriceSales = s?.PriceSpecial ?? p.Price,
                              Tax = i.Tax,
                              BarCode = i.BarCode,
                              Active = c.Active
                          }).Union
              (from iw in itemService
               join i in item on iw.ItemId equals i.ItemId
               join c in category on i.ItemFamilyId equals c.ItemFamilyId
               join um in unit on i.UnitOfMeasureId equals um.UnitOfMeasureId
               join p in prices on iw.ItemId equals p.ItemId
               join s in specialPrices on iw.ItemId equals s.ItemId into specialPrice
               from s in specialPrice.DefaultIfEmpty()
               where !i.InventoryItem && i.Active
               orderby i.ItemId
               select new ItemWareHouseDto
               {
                   ItemId = i.ItemId,
                   ItemCode = i.ItemCode,
                   WhsCode = 0,
                   WhsName = "Servicio",
                   DueDate = DateTime.Now.AddDays(365),
                   ItemName = i.ItemName,
                   UnitOfMeasureId = i.UnitOfMeasureId,
                   UnitOfMeasureName = um.UnitOfMeasureName,
                   ItemCategoryName = c.ItemFamilyName,
                   Stock = 1,
                   AvgPrice = 1,
                   PriceSales = s?.PriceSpecial ?? p.Price,
                   Tax = i.Tax,
                   BarCode = i.BarCode,
                   Active = c.Active
               })
              .OrderBy(dto => dto.ItemId)
              .ToList();
            return result;

        }

        public List<ItemWareHouseViewModel> GetAllItemWareHouseStockPrice(int whsCode)
        {
            var result = _context.ItemWareHouseViewModel
            .Where(x => x.WhsCode == whsCode)
            .Distinct()
            .OrderBy(x => x.ListPriceId)
            .ThenBy(x => x.ItemId)
           
            .ToList();
            return result;
        }

        public List<ItemWareHouseDto> GetItemWareHouseStockPriceBarCode(int whsCode, int customerid, int pricelistid, string barcode)
        {
            var stock = _context.ItemWareHouse.Where(x => x.WhsCode == whsCode).ToList();
            var item = _context.Item.Where(x => x.Active == true && x.BarCode == barcode || x.ItemCode.Contains(barcode)).ToList();
            var categoryId = item.Select(x => x.ItemCategoryId).Distinct().ToList();
            var category = _context.ItemCategory.Where(x => categoryId.Contains(x.ItemCategoryId)).ToList();
            var unitId = item.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var unit = _context.UnitOfMeasure.Where(x => unitId.Contains(x.UnitOfMeasureId)).ToList();
            var whs = _context.WareHouse.Where(x => x.WhsCode == whsCode).ToList();
            var prices = _context.PriceListDetail.Where(x => x.ListPriceId == pricelistid).ToList();
            var specialPrices = _context.PriceSpecialCustomerDetail.Where(x => x.CustomerId == customerid && x.PriceSpecial > 0).ToList();

            var result = (from iw in stock
                          join i in item on iw.ItemId equals i.ItemId
                          join c in category on i.ItemCategoryId equals c.ItemCategoryId
                          join um in unit on i.UnitOfMeasureId equals um.UnitOfMeasureId
                          join w in whs on iw.WhsCode equals w.WhsCode
                          join p in prices on iw.ItemId equals p.ItemId
                          join s in specialPrices on iw.ItemId equals s.ItemId into specialPrice
                          from s in specialPrice.DefaultIfEmpty()
                          where i.SalesItem == true
                          orderby i.ItemId
                          select new ItemWareHouseDto
                          {
                              ItemId = i.ItemId,
                              ItemCode = i.ItemCode,
                              WhsCode = iw.WhsCode,
                              WhsName = w.WhsName,
                              DueDate = iw.DueDate,
                              ItemName = i.ItemName,
                              UnitOfMeasureId = i.UnitOfMeasureId,
                              UnitOfMeasureName = um.UnitOfMeasureName,
                              ItemCategoryName = c.ItemCategoryName,
                              Stock = iw.Stock,
                              AvgPrice = iw.AvgPrice,
                              PriceSales = s?.PriceSpecial ?? p.Price,
                              Tax = i.Tax,
                              BarCode = i.BarCode,
                              Active = c.Active,
                              Weight = i.Weight,
                          }).ToList();
            return result;
        }
        public List<ItemDto> AddItem(Item request)
        {
            request.IsValid();
            _context.Database.BeginTransaction();
            try
            {
                var lastID = _context.Item.OrderByDescending(x => x.CreateDate).FirstOrDefault();
                if (lastID == null && request.ItemCode.Length==0)
                {
                    request.ItemCode = "0101-00001";
                }
                else if(request.ItemCode.Length == 0)
                {
                    var numberInString = lastID.ItemCode.ToString().Replace("0101-", "");
                    var number = int.Parse(numberInString) + 1;
                    string newCorrelative = "0101-" + number.ToString().PadLeft(5, ' ').Replace(' ', '0');
                    string numberString = number.ToString();
                    string ReciboSinZona = "00000" + number;
                    numberString = "0101-" + ReciboSinZona.Substring(numberString.Length);
                    request.ItemCode = numberString;
                }


                request.Active = true;
                request.CreateDate = DateTime.Now;
                _context.Item.Add(request);
                _context.SaveChanges();
                if (request.InventoryItem)
                {
                    _wareHouseServices.AddItemWareHouse(request.ItemId);
                }
                AddPriceListDetail(request.ItemId);
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.Message);
            }
            return GetItem();
        }
        public List<ItemDto> EditItem(Item request)
        {
            request.IsValid();
            var currentItem = _context.Item.Where(x => x.ItemId == request.ItemId).FirstOrDefault();
            if (currentItem == null) throw new Exception("No existe este articulo, comuniquese con el administrador del sistema.");
            _context.Database.BeginTransaction();
            try
            {
                currentItem.ItemCode = request.ItemCode;
                currentItem.ItemName = request.ItemName;
                currentItem.ItemCategoryId = request.ItemCategoryId;
                currentItem.UnitOfMeasureId = request.UnitOfMeasureId;
                currentItem.Tax = request.Tax;
                currentItem.SalesItem = request.SalesItem;
                currentItem.PurchaseItem = request.PurchaseItem;
                currentItem.ItemFamilyId = request.ItemFamilyId;
                //currentItem.InventoryItem = request.InventoryItem;
                currentItem.Weight = request.Weight;
                currentItem.BarCode = request.BarCode;
                currentItem.PricePurchase = request.PricePurchase;
                currentItem.Active = request.Active;
                currentItem.UpdateBy = request.CreateBy;
                currentItem.UpdateDate = DateTime.Now;
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.Message);
            }
            return GetItem();
        }
        public void UpdateStockItem(int itemid)
        {
            var item = _context.Item.Where(x => x.ItemId == itemid).FirstOrDefault();
            if(item.InventoryItem)
            {
                var itemWareHouse = _context.ItemWareHouse.Where(x => x.ItemId == itemid).ToList();
                var stock = itemWareHouse.Sum(x => x.Stock);
                var currentItem = _context.Item.Where(x => x.ItemId == itemid).FirstOrDefault();
                if (currentItem == null) throw new Exception("No existe este articulo, comuniquese con el administrador del sistema.");
                currentItem.Stock = stock;
                if (currentItem.Stock >= 0 && itemWareHouse.Sum(x => x.Stock) != 0)
                {
                    currentItem.AvgPrice = (itemWareHouse.Where(x => x.Stock > 0).Sum(x => x.Stock * x.AvgPrice)) / itemWareHouse.Where(x => x.Stock > 0).Sum(x => x.Stock);//Promedio ponderado
                }
                else
                {

                    currentItem.AvgPrice = item.AvgPrice;
                }             
                _context.SaveChanges();
            }       
        }
        public void AddPriceListDetail(int itemId)
        {
            var pricelist = _context.PriceList.ToList();
            var priceDetail = new List<PriceListDetail>();
            foreach (var item in pricelist)
            {
                var items = new PriceListDetail();
                items.ItemId = itemId;
                items.ListPriceId = item.ListPriceId;
                items.Price = 0;
                items.CreateDate = DateTime.Now;
                priceDetail.Add(items);
            }
            _context.PriceListDetail.AddRange(priceDetail);
            _context.SaveChanges();
        }

        public List<PriceSpecialCustomerDetail> GetAllPriceSpecialCustomerBySeller()
        {
            return _context.PriceSpecialCustomerDetail.Where(x=> x.PriceSpecial>0).ToList();
        }
    }
}
