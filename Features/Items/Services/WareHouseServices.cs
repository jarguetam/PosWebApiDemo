using DPos.Features.Auth.Dto;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using Pos.WebApi.Features.Items.Dto;
using Pos.WebApi.Features.Items.Entities;
using Pos.WebApi.Features.Users.Entities;
using Pos.WebApi.Helpers;
using Pos.WebApi.Infraestructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pos.WebApi.Features.Items.Services
{
    public class WareHouseServices
    {
        private readonly PosDbContext _context;

        public WareHouseServices(PosDbContext posDbContext)
        {
            _context = posDbContext;
        }

        public List<WareHouseDto> GetWareHouse()
        {
            var warehouse = _context.WareHouse.ToList();
            var userId = warehouse.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from w in warehouse
                          join u in user on w.CreateBy equals u.UserId
                          select new WareHouseDto
                          {
                              WhsCode = w.WhsCode,
                              WhsName = w.WhsName,
                              CreateBy = w.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = w.CreateDate,
                              Active = w.Active
                          }).ToList();
            return result;

        }
        public List<WareHouseDto> GetWareHouseActive()
        {
            var warehouse = _context.WareHouse.ToList();
            var userId = warehouse.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from w in warehouse
                          join u in user on w.CreateBy equals u.UserId
                          where w.Active == true
                          select new WareHouseDto
                          {
                              WhsCode = w.WhsCode,
                              WhsName = w.WhsName,
                              CreateBy = w.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = w.CreateDate,
                              Active = w.Active
                          }).ToList();
            return result;

        }
        public List<WareHouseDto> AddWareHouse(WareHouse request)
        {
            request.IsValid();
            try
            {
                _context.Database.BeginTransaction();         
                request.Active = true;
                request.CreateDate = DateTime.Now;
                _context.WareHouse.Add(request);
                _context.SaveChanges();
                var result =AddWareHouseItems(request.WhsCode);
                if (result=="OK")
                {
                    _context.Database.CommitTransaction();
                }
                else
                {
                    throw new Exception("Ocurrio un error al agregar el almacen");
                }
                
            }
            catch(Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.Message);
            }
            
            return GetWareHouse();
        }
        public string AddWareHouseItems(int whsCode)
        {
            var itemsdata = _context.Item.ToList();
            var itemWarehouse = itemsdata.Select(x => new ItemWareHouse
            {
                ItemId = x.ItemId,
                WhsCode = whsCode,
                Stock = 0,
                AvgPrice = 0,
                CreateDate = DateTime.Now,
            });
            _context.ItemWareHouse.AddRange(itemWarehouse);
            _context.SaveChanges();
            return "OK";
        }
        public List<WareHouseDto> EditWareHouse(WareHouse request)
        {
            request.IsValid();
            var currentWarehouse = _context.WareHouse.Where(x => x.WhsCode == request.WhsCode).FirstOrDefault();
            currentWarehouse.WhsName = request.WhsName;
            currentWarehouse.UpdateBy = request.CreateBy;
            currentWarehouse.UpdateDate = DateTime.Now;
            currentWarehouse.Active = request.Active;
            _context.SaveChanges();
            return GetWareHouse();
        }
        //Inventario en almacenes
        public List<ItemWareHouseDto> GetItemWareHouse(int itemId)
        {
            var itemwarehouse = _context.ItemWareHouse.Where(x => x.ItemId == itemId).ToList();
            var whsCode = itemwarehouse.Select(x => x.WhsCode).Distinct().ToList();
            var whs = _context.WareHouse.Where(x => whsCode.Contains(x.WhsCode)).ToList();
            var result = (from i in itemwarehouse
                          join w in whs on i.WhsCode equals w.WhsCode
                          select new ItemWareHouseDto
                          {
                              ItemId = i.ItemId,
                              WhsCode = w.WhsCode,
                              WhsName = w.WhsName,
                              Stock = i.Stock,
                              AvgPrice = i.AvgPrice,
                              DueDate = i.DueDate,
                              CreateDate = w.CreateDate,
                              Active = w.Active
                          }).ToList();
            return result;
        }

        public string AddItemWareHouse(int itemId)
        {
            var whs = _context.WareHouse.ToList();
            var itemWarehouse = whs.Select(x => new ItemWareHouse
            {
                ItemId = itemId,
                WhsCode = x.WhsCode,
                Stock = 0,
                AvgPrice = 0,
                CreateDate = DateTime.Now,
            });          
            _context.ItemWareHouse.AddRange(itemWarehouse);
            _context.SaveChanges();
            return "ok";
        }

        public string UpdateItemWareHouse(ItemWareHouse request)
        {
            var item = _context.Item.Where(x => x.ItemId == request.ItemId).FirstOrDefault();
            if (item.InventoryItem)
            {
                var companyInfo = _context.CompanyInfo.FirstOrDefault();
                var currentLine = _context.ItemWareHouse.Where(x => x.ItemId == request.ItemId && x.WhsCode == request.WhsCode).FirstOrDefault();
                if (currentLine == null) throw new Exception("No existe este articulo en el almacen. Favor comuniquese con el administrador del sistema.");         
                if(request.Stock>0)

                    currentLine.AvgPrice = ((currentLine.Stock* currentLine.AvgPrice) + (request.Stock* request.AvgPrice))/(request.Stock+currentLine.Stock);//Promedio ponderado
                currentLine.Stock = currentLine.Stock + request.Stock;
                currentLine.DueDate = request.DueDate;
                if (currentLine.Stock < 0 && !companyInfo.NegativeInventory) throw new Exception($"Inventario del producto {request.ItemId} recae en negativo. Revise su inventario.");
                if (currentLine.AvgPrice < 0) throw new Exception($"Costo de producto en negativo. Revise su inventario.");
                _context.SaveChanges();
            }
            return "OK";
        }
    }
}
