using Pos.WebApi.Features.Common.Dto;
using Pos.WebApi.Features.Common.Entities;
using Pos.WebApi.Features.Reports.Dto;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pos.WebApi.Features.Reports.Services
{
    public class ReportInventoryCategoryServices
    {
        private readonly List<ReportInventoryCategoryDto> _result;
        private CompanyInfo _companyInfo;
        private FileUpload _fileUpload;
        private int _totalLines;
        private string _whsName;

        public ReportInventoryCategoryServices(List<ReportInventoryCategoryDto> result, CompanyInfo companyInfo, FileUpload fileUpload, int totalLines, string whsName)
        {
            _result = result;
            _companyInfo = companyInfo;
            _fileUpload = fileUpload;
            _totalLines = totalLines;
            _whsName = whsName;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public byte[] ComposeReportSale()
        {
            QuestPDF.Settings.License = LicenseType.Community; 
            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.MarginVertical(25);
                    page.MarginHorizontal(25);
                    page.Size(PageSizes.Letter.Portrait());
                    page.Header().ShowOnce().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
            });
            byte[] pdfBytes = document.GeneratePdf();
            //document.ShowInPreviewer();
            return pdfBytes;
        }

        void ComposeHeader(IContainer container)
        {

            string basePath = $"{Directory.GetCurrentDirectory()}/Files/{_fileUpload.Extension}/{_fileUpload.Name}";
            var assetsImage = $"{Directory.GetCurrentDirectory()}/Files/{_fileUpload.Extension}/{_fileUpload.Name}";//System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\LogoAgromoney.png");
            var titleStyle = TextStyle.Default.FontSize(12).SemiBold();

            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().AlignCenter().Text(_companyInfo.CompanyName).Style(titleStyle);

                    column.Item().Element(CellStyleTitle).AlignCenter().Text(text =>
                    {
                        text.Span("Reporte de inventario").SemiBold().FontSize(14);

                    });

                    column.Item().Element(CellStyleTitle).AlignCenter().Text(text =>
                    {
                        text.Span("Almacen: " + _whsName).SemiBold().FontSize(14);
                    });


                    column.Item().Text(text =>
                    {
                        text.Span("Fecha impresion: ").SemiBold().FontSize(10);
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy hh:mm")).FontSize(10);
  
                    });
                });
                row.ConstantItem(100).AlignRight().Width(100).Image(basePath);

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
            container.Table(table =>
            {
                // step 1
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(55);
                    columns.ConstantColumn(270);
                    columns.ConstantColumn(70);
                    columns.ConstantColumn(70);
                    columns.ConstantColumn(80);     
                });

                // step 2
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Codigo").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Descripcion").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Existencia").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Observaciones").FontSize(10).FontColor(Colors.White);
                   
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.White).Background(Colors.BlueGrey.Medium);
                    }
                });
                // step 3
                int i = 0;
                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.Medium()).PaddingVertical(0).BorderBottom(0.1f).BorderColor(Colors.White);
                }

                foreach (var item in _result)
                {
                    table.Cell().ColumnSpan(5).BorderBottom(1).BorderColor(Colors.Black).Text(item.ItemCategoryName).Bold().FontSize(10);
                    int d = 0;
                    foreach (var itemdetail in item.Detail)
                    {
                        table.Cell().Element(CellStyle).Text(itemdetail.ItemCode).FontSize(10);
                        table.Cell().Element(CellStyle).Text(itemdetail.ItemName).FontSize(10);
                        table.Cell().Element(CellStyle).Text(itemdetail.Stock.ToString("N")).FontSize(10);
                        table.Cell().Element(CellStyle).Text(itemdetail.AvgPrice.ToString("N")).FontSize(10);
                        table.Cell().Element(CellStyle).Text(itemdetail.Total.ToString("N")).FontSize(10);
                        d++;
                    }
                    if (d == item.Detail.Count())
                    {

                        table.Cell().ColumnSpan(2).BorderTop(1).BorderColor(Colors.Black)
                            .AlignLeft().Text("Total:").Bold().FontSize(10);
                        // Celda con el total de Balance
                        table.Cell().BorderTop(1).BorderColor(Colors.Black)
                            .AlignLeft().Text($"{item.Detail.Sum(x => x.Stock).ToString("N")}").Bold().FontSize(10);
                       table.Cell().BorderTop(1).BorderColor(Colors.Black);

                        // Celda con el total de Balance
                        table.Cell().BorderTop(1).BorderColor(Colors.Black)
                            .AlignLeft().Text($"{item.Detail.Sum(x => x.Total).ToString("N")}").Bold().FontSize(10);
                    }
                    i++;
                    }
                    if (i == _result.Count())
                    {
                        table.Cell().ColumnSpan(2).BorderTop(1).BorderColor(Colors.Black)
                         .AlignLeft().Text(@$"Lineas: {_totalLines}").Bold().FontSize(10);
                         table.Cell().ColumnSpan(2).BorderTop(1).BorderColor(Colors.Black).AlignLeft().Text($"{_result.Sum(x => x.Detail.Sum(x => x.Stock)).ToString("N")}").Bold().FontSize(10);
                         table.Cell().BorderTop(1).BorderColor(Colors.Black).AlignLeft().Text($"{_result.Sum(x => x.Detail.Sum(x => x.Total)).ToString("N")}").Bold().FontSize(10);
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
