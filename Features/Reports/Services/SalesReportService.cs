using Pos.WebApi.Features.Common.Dto;
using Pos.WebApi.Features.Common.Entities;
using Pos.WebApi.Features.Items.Entities;
using Pos.WebApi.Features.Reports.Dto;
using Pos.WebApi.Features.Sales.Dto;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pos.WebApi.Features.Reports.Services
{
    public class SalesReportService
    {
        private readonly List<SalesReportDto> _result;
        private DateTime _from;
        private DateTime _to;
        private CompanyInfo _companyInfo;
        private FileUpload _fileUpload;

        public SalesReportService(List<SalesReportDto> result, DateTime from, DateTime to, CompanyInfo companyInfo, FileUpload fileUpload)
        {
            _result = result;
            _from = from;
            _to = to;
            _companyInfo = companyInfo;
            _fileUpload = fileUpload;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public byte[] ComposeReportSale()
        {
            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.MarginVertical(20);
                    page.MarginHorizontal(20);
                    page.Size(PageSizes.ARCH_B.Landscape());
                    page.Header().ShowOnce().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
            });
            byte[] pdfBytes = document.GeneratePdf();
          //  document.ShowInPreviewer();
            return pdfBytes;
        }

        void ComposeHeader(IContainer container)
        {

            string basePath = $"{Directory.GetCurrentDirectory()}/Files/{_fileUpload.Extension}/{_fileUpload.Name}";
            var assetsImage = $"{Directory.GetCurrentDirectory()}/Files/{_fileUpload.Extension}/{_fileUpload.Name}";//System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\LogoAgromoney.png");
            var titleStyle = TextStyle.Default.FontSize(20).SemiBold();

            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().AlignCenter().Text(_companyInfo.CompanyName).Style(titleStyle);

                    column.Item().Element(CellStyleTitle).AlignCenter().Text(text =>
                    {
                        text.Span("Reporte de ventas").SemiBold().FontSize(20);
                    });

                    column.Item().Element(CellStyleTitle).AlignCenter().Text(text =>
                    {
                        text.Span($"Desde: {_from.ToString("dd/MM/yyyy")} Hasta: {_to.ToString("dd/MM/yyyy")} ").SemiBold().FontSize(16);
                    });
                    column.Item().Text(text =>
                    {
                        text.Span("Fecha impresion: ").SemiBold();
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                    });
                });
               // row.ConstantItem(100).AlignRight().Width(100).Image(basePath);

            });
            static IContainer CellStyleTitle(IContainer container)
            {
                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5);
            }
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(5).Column(column =>
            {
                column.Spacing(5);
                column.Item().Element(ComposeTableImage);
            });
        }

        void ComposeTableImage(IContainer container)
        {
            // Variables para los totales
            decimal totalQuantity = 0;
            decimal totalPrice = 0;
            decimal totalLineTotal = 0;
            container.Table(table =>
            {
                // step 1
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(75);
                    columns.ConstantColumn(75);
                    columns.ConstantColumn(75);
                    columns.ConstantColumn(180);
                    columns.ConstantColumn(65);
                    columns.ConstantColumn(75);
                    columns.ConstantColumn(160);
                    columns.ConstantColumn(75);
                    columns.ConstantColumn(75);
                    columns.ConstantColumn(100);
                    columns.ConstantColumn(75);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                // step 2
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#").FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Fecha").FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Cod. Cliente").FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Cliente").FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Condicion").FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Cod. articulo").FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Descripcion").FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Cant.").FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Precio").FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Total").FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Almacen").FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Vendedor").FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Zona").FontColor(Colors.White);
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.White).Background(Colors.BlueGrey.Medium);
                    }
                });
                // step 3
                int i = 0;
                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.Medium()).PaddingVertical(0).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
                }
                foreach (var item in _result)
                {
                    table.Cell().Element(CellStyle).Text(item.InvoiceFiscalNo);
                    table.Cell().Element(CellStyle).Text(item.DocDate.ToString("dd-MM-yyyy"));
                    table.Cell().Element(CellStyle).Text(item.CustomerCode);
                    table.Cell().Element(CellStyle).Text(item.CustomerName);
                    table.Cell().Element(CellStyle).Text(item.PayConditionName);
                    table.Cell().Element(CellStyle).Text(item.ItemCode);
                    table.Cell().Element(CellStyle).Text(item.ItemName);
                    table.Cell().Element(CellStyle).Text(item.Quantity.ToString("N"));
                    table.Cell().Element(CellStyle).Text(item.Price.ToString("N"));
                    table.Cell().Element(CellStyle).Text(item.LineTotal.ToString("N"));
                    table.Cell().Element(CellStyle).Text(item.WhsName);
                    table.Cell().Element(CellStyle).Text(item.SellerName);
                    table.Cell().Element(CellStyle).Text(item.SellerZone);
                    totalQuantity += item.Quantity;
                    totalPrice += item.Price;
                    totalLineTotal += item.LineTotal;
                    i++;
                }
                if (i == _result.Count())
                {
                    table.Cell();
                    table.Cell();
                    table.Cell();
                    table.Cell();
                    table.Cell();
                    table.Cell();
                    table.Cell().AlignLeft().Text("Total:").Bold().FontSize(12);
                    table.Cell().AlignLeft().Text($"{_result.Sum(x=> x.Quantity).ToString("N")}").Bold().FontSize(12);
                    table.Cell().AlignLeft().Text($"");
                    table.Cell().AlignLeft().Text($"L.{_result.Sum(x => x.LineTotal).ToString("N")}").Bold().FontSize(12); ;
                    table.Cell().AlignRight().Text($"");
                    table.Cell().AlignRight().Text($"");
                    table.Cell().AlignRight().Text($"");
                }
                
            });
        }

        public void ComposeFooter(IContainer container)
        {
            container.AlignRight().Text(x =>
            {
                x.Span("Pagina ");
                x.CurrentPageNumber();
            });
        }
    }
}
