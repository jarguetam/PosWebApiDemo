using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pos.WebApi.Features.Common.Dto;
using Pos.WebApi.Features.Common.Entities;
using Pos.WebApi.Helpers;
using Pos.WebApi.Infraestructure;
using Microsoft.AspNetCore.Http;

namespace Pos.WebApi.Features.Common
{
    public class CommonService
    {
        private readonly PosDbContext _context;
        public CommonService(PosDbContext logisticaBtdDbContext)
        {
            _context = logisticaBtdDbContext;
        }
 

        public FileUploadDto UploadFile(IFormFile File)
        {
            try
            {
                int lastIndex = File.FileName.ToString().LastIndexOf(".");
                if (lastIndex == -1) throw new Exception("El archivo es incorrecto o tiene un mal nombre");

                string Extension = File.FileName.ToString().Substring(lastIndex);
                Extension = Extension.Replace(".", "");
                Extension = Extension.ToLower();

                var checkIfSupport  = _context.MimeType.Where(x => x.Extension == Extension).FirstOrDefault();
                if (checkIfSupport == null) throw new Exception("Formato no soportado, si desea agregarlo solicitar a soporte");

                string basePath = $"{Directory.GetCurrentDirectory()}/Files/{Extension}";
                if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

                byte[] CoverImageBytes = null;
                BinaryReader reader = new BinaryReader(File.OpenReadStream());
                CoverImageBytes = reader.ReadBytes((int)File.Length);
                string NombreImagen = DateTime.Now.AddMilliseconds(1).ToString("yyyyMMddHHmmssfff") + $".{Extension}";


                if (File != null)
                {
                    string filePath = Path.Combine(basePath, NombreImagen);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        File.CopyTo(fileStream);
                    }
                }
                FileUpload fileUpload = new FileUpload
                {
                    Description = File.FileName.ToString(),
                    Extension = Extension.ToLower(),
                    Name = NombreImagen,
                    FileId = 0
                };
                _context.FileUpload.Add(fileUpload);
                _context.SaveChanges();
                FileUploadDto result = Helper.ToObject<FileUploadDto>(fileUpload);
                result.Path = Helper.GenerateIdEnconde(fileUpload.FileId);
                return result;

            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new System.Exception(mensaje);
            }
        }

        public FileDownloadDto FileInfo(int id)
        {
            var file = _context.FileUpload.Where(x=> x.FileId==id).FirstOrDefault();
            
            if (file == null) throw new Exception("No Found");

            string basePath = $"{Directory.GetCurrentDirectory()}/Files/{file.Extension}/{file.Name}";
            FileDownloadDto fileToDonwload = new FileDownloadDto();

            var mimeType = _context.MimeType.Where(x => x.Extension == file.Extension).FirstOrDefault();
            if (mimeType == null) throw new Exception("No compatible");

            if (System.IO.File.Exists(basePath))
            {
                fileToDonwload.Path = basePath;
                fileToDonwload.MimeType = mimeType.Type;
            }
            else
            {
                throw new Exception("No Found");
            }
            return fileToDonwload;
          
        }
        public FileDownloadDto DownloadFile(string Path)
        {
           long id = long.Parse(Helper.DecodeIdEnconde(Path));
            var file = _context.FileUpload.Where(x => x.FileId == id).FirstOrDefault();
            if (file == null) throw new Exception("No Found");

            string basePath = $"{Directory.GetCurrentDirectory()}/Files/{file.Extension}/{file.Name}";
            FileDownloadDto fileToDonwload = new FileDownloadDto();

            var mimeType = _context.MimeType.Where(x => x.Extension == file.Extension).FirstOrDefault();
            if (mimeType == null) throw new Exception("No compatible");

            if (System.IO.File.Exists(basePath))
            {
                fileToDonwload.Path = basePath;
                fileToDonwload.MimeType = mimeType.Type;
            }
            else
            {
                throw new Exception("No Found");
            }
            return fileToDonwload;
        }

        public List<CompanyInfoDto> GetCompanyInfo()
        {       
            var company = _context.CompanyInfo.ToList();
            var userIds = company.Select(x => x.UserId).Distinct().ToList();
            var user = _context.User.Where(x => userIds.Contains(x.UserId)).ToList();
            var result = (from c in company
                          join u in user on c.UserId equals u.UserId
                          select new CompanyInfoDto
                          {
                              Id = c.Id,
                              CompanyName = c.CompanyName,
                              Rtn = c.Rtn,
                              AddressLine1 = c.AddressLine1,
                              AddressLine2 = c.AddressLine2,
                              Phone1 =  c.Phone1,
                              Phone2 = c.Phone2,
                              Email1 = c.Email1,
                              Email2 = c.Email2,
                              UserId = c.UserId,
                              UserName = u.Name,
                              FileId = c.FileId,
                              Path = Helper.GenerateIdEnconde(c.FileId),
                              Extension = _context.FileUpload.Where(x=> x.FileId == c.FileId).FirstOrDefault().Extension,
                              CreateDate = c.CreateDate,
                              TaxValue = c.TaxValue/100,
                              NegativeInventory = c.NegativeInventory,
                              PrintLetter =c.PrintLetter,
                          }).ToList();
            return result;
        }

        public List<CompanyInfoDto> AddCompany(CompanyInfo companyInfo)
        {
            companyInfo.CreateDate = DateTime.Now;
            _context.CompanyInfo.Add(companyInfo);
            _context.SaveChanges();
            return GetCompanyInfo();
        }
        public List<CompanyInfoDto> EditCompanyInfo(CompanyInfo companyInfo)
        {
            var company = _context.CompanyInfo.Where(x => x.Id == companyInfo.Id).FirstOrDefault();
            company.CompanyName = companyInfo.CompanyName;
            company.Rtn = companyInfo.Rtn;
            company.AddressLine1 = companyInfo.AddressLine1;
            company.AddressLine2 = companyInfo.AddressLine2;
            company.Phone1 = companyInfo.Phone1;
            company.Phone2= companyInfo.Phone2;
            company.Email1 = companyInfo.Email1;
            company.Email2 = companyInfo.Email2;
            company.UserId = companyInfo.UserId;
            company.CreateDate = DateTime.Now;
            company.FileId = companyInfo.FileId==0? company.FileId : companyInfo.FileId;
            company.TaxValue = companyInfo.TaxValue;
            company.NegativeInventory = companyInfo.NegativeInventory;
            company.PrintLetter = companyInfo.PrintLetter;
            _context.SaveChanges();
            return GetCompanyInfo();
        }

        public List<PayConditionDto> GetCPayCondition()
        {
            var payCondition = _context.PayCondition.ToList();
            var userId = payCondition.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from c in payCondition
                          join u in user on c.CreateBy equals u.UserId
                          select new PayConditionDto
                          {
                              PayConditionId = c.PayConditionId,
                              PayConditionName = c.PayConditionName,
                              PayConditionDays = c.PayConditionDays,
                              CreateBy = c.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = c.CreateDate,
                              Active = c.Active
                          }).ToList();
            return result;

        }

        public List<PayConditionDto> GetCPayConditionActive()
        {
            var payCondition = _context.PayCondition.ToList();
            var userId = payCondition.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from c in payCondition
                          join u in user on c.CreateBy equals u.UserId
                          where c.Active==true
                          select new PayConditionDto
                          {
                              PayConditionId = c.PayConditionId,
                              PayConditionName = c.PayConditionName,
                              PayConditionDays = c.PayConditionDays,
                              CreateBy = c.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = c.CreateDate,
                              Active = c.Active
                          }).ToList();
            return result;

        }

        public List<PayConditionDto> AddPayCondition(PayCondition request)
        {
            request.IsValid();
            request.Active = true;
            request.CreateDate = DateTime.Now;
            _context.PayCondition.Add(request);
            _context.SaveChanges();
            return GetCPayCondition();
        }

        public List<PayConditionDto> EditPayCondition(PayCondition request)
        {
            request.IsValid();
            var currentPay = _context.PayCondition.Where(x => x.PayConditionId == request.PayConditionId).FirstOrDefault();
            currentPay.PayConditionName = request.PayConditionName;
            currentPay.PayConditionDays = request.PayConditionDays;
            currentPay.UpdateBy = request.CreateBy;
            currentPay.UpdateDate = DateTime.Now;
            currentPay.Active = request.Active;
            _context.SaveChanges();
            return GetCPayCondition();
        }

        #region SAR
        public List<SarBranch> GetBranch()
        {
            var branch = _context.SarBranch.ToList();

            return branch;
        }

        public List<SarTypeDocument> GetTypeDocument()
        {
            var type = _context.SarTypeDocument.ToList();

            return type;
        }
        public List<SarPointSaleDto> GetPointSale()
        {
            var point = _context.SarPointSale.ToList();
            var userids = point.Select(x => x.UserId).ToList();
            var users = _context.User.Where(x => userids.Contains(x.UserId)).ToList();
            var branch = _context.SarBranch.ToList();
            var result = (from s in point
                          join u in users on s.UserId equals u.UserId
                          join b in branch on s.BranchId equals b.BranchId
                          select new SarPointSaleDto
                          {
                              PointSaleId = s.PointSaleId,
                              Name = s.Name,
                              BranchId = s.BranchId,
                              BranchName = b.Name,
                              CreateDate = s.CreateDate,
                              Active = s.Active,
                              UserId = s.UserId,
                              UserName = u.Name
                          }).ToList();
            return result;
        }

        public List<SarPointSaleDto> AddPointSale(SarPointSale point)
        {
            point.Name = point.Name;
            point.BranchId = point.BranchId;
            point.CreateDate = DateTime.Now;
            point.Active = point.Active;
            point.UserId = point.UserId;
            _context.SarPointSale.Add(point);
            _context.SaveChanges();
            return GetPointSale();
        }
        public List<SarPointSaleDto> EditPointSale(SarPointSale pointEdit)
        {
            var point = _context.SarPointSale.Where(x => x.PointSaleId == pointEdit.PointSaleId).FirstOrDefault();
            point.Name = pointEdit.Name;
            point.BranchId = pointEdit.BranchId;
            point.CreateDate = DateTime.Now;
            point.Active = pointEdit.Active;
            point.UserId = pointEdit.UserId;
            _context.SaveChanges();
            return GetPointSale();
        }

        public List<SarCorrelativeDto> GetCorrelative()
        {
            var correlative = _context.SarCorrelative.ToList();
            var userids = correlative.Select(x => x.UserId).ToList();
            var users = _context.User.Where(x => userids.Contains(x.UserId)).Distinct().ToList();
            var branch = _context.SarBranch.ToList();
            var point = _context.SarPointSale.ToList();
            var document = _context.SarTypeDocument.ToList();
            var result = (from s in correlative
                          join u in users on s.UserId equals u.UserId
                          join b in branch on s.BranchId equals b.BranchId
                          join c in point on s.PointSaleId equals c.PointSaleId
                          join t in document on s.TypeDocument equals t.TypeDocument
                          select new SarCorrelativeDto
                          {
                              CorrelativeId = s.CorrelativeId,
                              AuthorizeRangeFrom = s.AuthorizeRangeFrom,
                              AuthorizeRangeTo = s.AuthorizeRangeTo,
                              CurrentCorrelative = s.CurrentCorrelative,
                              Cai = s.Cai,
                              TypeDocument = s.TypeDocument,
                              DateLimit = s.DateLimit,
                              PointSaleId = s.PointSaleId,
                              BranchId = s.BranchId,
                              PointSale = c.Name,
                              Branch = b.Name,
                              TypeDocumentName = t.Name,
                              CreateDate = s.CreateDate,
                              UserId = s.UserId,
                              UserName = u.Name,
                              Description = s.Description
                          }).ToList();
            return result;
        }
        public List<SarCorrelativeDto> GetCorrelativeInvoice()
        {
            var correlative = _context.SarCorrelative.Where(x=> x.TypeDocument=="01").ToList();
            var userids = correlative.Select(x => x.UserId).ToList();
            var users = _context.User.Where(x => userids.Contains(x.UserId)).Distinct().ToList();
            var branch = _context.SarBranch.ToList();
            var point = _context.SarPointSale.ToList();
            var document = _context.SarTypeDocument.ToList();
            var result = (from s in correlative
                          join u in users on s.UserId equals u.UserId
                          join b in branch on s.BranchId equals b.BranchId
                          join c in point on s.PointSaleId equals c.PointSaleId
                          join t in document on s.TypeDocument equals t.TypeDocument
                          select new SarCorrelativeDto
                          {
                              CorrelativeId = s.CorrelativeId,
                              AuthorizeRangeFrom = s.AuthorizeRangeFrom,
                              AuthorizeRangeTo = s.AuthorizeRangeTo,
                              CurrentCorrelative = s.CurrentCorrelative,
                              Cai = s.Cai,
                              TypeDocument = s.TypeDocument,
                              DateLimit = s.DateLimit,
                              PointSaleId = s.PointSaleId,
                              BranchId = s.BranchId,
                              PointSale = c.Name,
                              Branch = b.Name,
                              TypeDocumentName = t.Name,
                              CreateDate = s.CreateDate,
                              UserId = s.UserId,
                              UserName = u.Name,
                              Description = s.Description
                          }).ToList();
            return result;
        }

        public List<SarCorrelativeDto> GetCorrelativeInvoiceById(int id)
        {
            var correlative = _context.SarCorrelative.Where(x => x.TypeDocument == "01").ToList();
            var userids = correlative.Select(x => x.UserId).ToList();
            var users = _context.User.Where(x => userids.Contains(x.UserId)).Distinct().ToList();
            var branch = _context.SarBranch.ToList();
            var point = _context.SarPointSale.ToList();
            var document = _context.SarTypeDocument.ToList();
            var result = (from s in correlative
                          join u in users on s.UserId equals u.UserId
                          join b in branch on s.BranchId equals b.BranchId
                          join c in point on s.PointSaleId equals c.PointSaleId
                          join t in document on s.TypeDocument equals t.TypeDocument
                          where s.CorrelativeId == id
                          select new SarCorrelativeDto
                          {
                              CorrelativeId = s.CorrelativeId,
                              AuthorizeRangeFrom = s.AuthorizeRangeFrom,
                              AuthorizeRangeTo = s.AuthorizeRangeTo,
                              CurrentCorrelative = s.CurrentCorrelative,
                              Cai = s.Cai,
                              TypeDocument = s.TypeDocument,
                              DateLimit = s.DateLimit,
                              PointSaleId = s.PointSaleId,
                              BranchId = s.BranchId,
                              PointSale = c.Name,
                              Branch = b.Name,
                              TypeDocumentName = t.Name,
                              CreateDate = s.CreateDate,
                              UserId = s.UserId,
                              UserName = u.Name,
                              Description = s.Description
                          }).ToList();
            return result;
        }
        public List<SarCorrelativeDto> AddCorrelative(SarCorrelative correlative)
        {
            correlative.IsValid();
            correlative.AuthorizeRangeFrom = correlative.AuthorizeRangeFrom;
            correlative.AuthorizeRangeTo = correlative.AuthorizeRangeTo;
            correlative.CurrentCorrelative = correlative.CurrentCorrelative;
            correlative.Cai = correlative.Cai;
            correlative.BranchId = correlative.BranchId;
            correlative.PointSaleId = correlative.PointSaleId;
            correlative.TypeDocument = correlative.TypeDocument;
            correlative.DateLimit = correlative.DateLimit;
            correlative.CreateDate = DateTime.Now;
            correlative.UserId = correlative.UserId;
            correlative.Description = correlative.Description;
            _context.SarCorrelative.Add(correlative);
            _context.SaveChanges();
            return GetCorrelative();
        }
        public List<SarCorrelativeDto> EditCorrelative(SarCorrelative correlativeEdit)
        {
            var correlative = _context.SarCorrelative.Where(x => x.CorrelativeId == correlativeEdit.CorrelativeId).FirstOrDefault();
            correlative.AuthorizeRangeFrom = correlativeEdit.AuthorizeRangeFrom;
            correlative.AuthorizeRangeTo = correlativeEdit.AuthorizeRangeTo;
            correlative.CurrentCorrelative = correlativeEdit.CurrentCorrelative;
            correlative.Cai = correlativeEdit.Cai;
            correlative.BranchId = correlativeEdit.BranchId;
            correlative.PointSaleId = correlativeEdit.PointSaleId;
            correlative.TypeDocument = correlativeEdit.TypeDocument;
            correlative.DateLimit = correlativeEdit.DateLimit;
            correlative.CreateDate = DateTime.Now;
            correlative.UserId = correlativeEdit.UserId;
            correlative.Description = correlativeEdit.Description;
            _context.SaveChanges();
            return GetCorrelative();
        }
        #endregion








    }
}
