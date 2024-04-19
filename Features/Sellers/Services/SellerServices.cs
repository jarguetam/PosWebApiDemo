using Pos.WebApi.Infraestructure;
using System.Collections.Generic;
using System.Linq;
using System;
using Pos.WebApi.Features.Sellers.Dto;
using Pos.WebApi.Features.Sellers.Entities;

namespace Pos.WebApi.Features.Sellers.Services
{
    public class SellerServices
    {
        private readonly PosDbContext _context;

        public SellerServices(PosDbContext posDbContext)
        {
            _context = posDbContext;
        }

        public List<SellerRegionDto> GetSellerRegion()
        {
            var sellerregion = _context.SellerRegion.ToList();
            var userId = sellerregion.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from w in sellerregion
                          join u in user on w.CreateBy equals u.UserId
                          select new SellerRegionDto
                          {
                              RegionId = w.RegionId,
                              NameRegion = w.NameRegion,
                              CreateBy = w.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = w.CreateDate,
                              Active = w.Active
                          }).ToList();
            return result;

        }

        public List<SellerRegionDto> GetSellerRegionActive()
        {
            var sellerregion = _context.SellerRegion.ToList();
            var userId = sellerregion.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from w in sellerregion
                          join u in user on w.CreateBy equals u.UserId
                          where w.Active == true
                          select new SellerRegionDto
                          {
                              RegionId = w.RegionId,
                              NameRegion = w.NameRegion,
                              CreateBy = w.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = w.CreateDate,
                              Active = w.Active
                          }).ToList();
            return result;

        }

        public List<SellerRegionDto> AddSellerRegion(SellerRegion request)
        {
            request.IsValid();
            request.Active = true;
            request.CreateDate = DateTime.Now;
            _context.SellerRegion.Add(request);
            _context.SaveChanges();
            return GetSellerRegion();
        }

        public List<SellerRegionDto> EditSellerRegion(SellerRegion request)
        {
            request.IsValid();
            var currentWarehouse = _context.SellerRegion.Where(x => x.RegionId == request.RegionId).FirstOrDefault();
            currentWarehouse.NameRegion = request.NameRegion;
            currentWarehouse.UpdateBy = request.CreateBy;
            currentWarehouse.UpdateDate = DateTime.Now;
            currentWarehouse.Active = request.Active;
            _context.SaveChanges();
            return GetSellerRegion();
        }

        public List<SellerDto> GetSeller()
        {
            var seller = _context.Seller.ToList();            
            var sellerRegionId = seller.Select(x => x.RegionId).Distinct().ToList();
            var sellerregion = _context.SellerRegion.Where(x=> sellerRegionId.Contains(x.RegionId)).ToList();       
            var wareHouseId = seller.Select(x => x.WhsCode).Distinct().ToList();
            var wareHouse = _context.WareHouse.Where(x=> wareHouseId.Contains(x.WhsCode)).ToList();
            var userId = seller.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from s in seller
                          join r in sellerregion on s.RegionId equals r.RegionId
                          join w in wareHouse on s.WhsCode equals w.WhsCode
                          join u in user on s.CreateBy equals u.UserId
                          select new SellerDto
                          {
                              SellerId = s.SellerId,
                              SellerName = s.SellerName,
                              RegionId = r.RegionId,
                              RegionName = r.NameRegion,
                              WhsCode = s.WhsCode,
                              WhsName = w.WhsName,
                              CreateBy = w.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = w.CreateDate,
                              Active = w.Active
                          }).ToList();
            return result;

        }

        public List<SellerDto> AddSeller(Seller request)
        {
            request.IsValid();
            request.Active = true;
            request.CreateDate = DateTime.Now;
            _context.Seller.Add(request);
            _context.SaveChanges();
            return GetSeller();
        }

        public List<SellerDto> EditSeller(Seller request)
        {
            request.IsValid();
            var currentSeller = _context.Seller.Where(x => x.SellerId == request.SellerId).FirstOrDefault();
            currentSeller.SellerName = request.SellerName;
            currentSeller.RegionId = request.RegionId;
            currentSeller.WhsCode = request.WhsCode;
            currentSeller.UpdateBy = request.CreateBy;
            currentSeller.UpdateDate = DateTime.Now;
            currentSeller.Active = request.Active;
            _context.SaveChanges();
            return GetSeller();
        }
    }
}
