using Pos.WebApi.Features.Common.Dto;
using Pos.WebApi.Features.Common.Entities;
using Pos.WebApi.Features.Reports.Dto;
using System.Collections.Generic;
using System;
using QuestPDF.Infrastructure;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Linq;
using QuestPDF.Previewer;

namespace Pos.WebApi.Features.Reports.Services
{
    public class CxcReportServices
    {
        private readonly List<CxcreportDto> _result;
        private CompanyInfo _companyInfo;
        private FileUpload _fileUpload;
        private string _sellerName;

        public CxcReportServices(List<CxcreportDto> result, CompanyInfo companyInfo, FileUpload fileUpload, string sellerName)
        {
            _result = result;
            _companyInfo = companyInfo;
            _fileUpload = fileUpload;
            _sellerName = sellerName;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public byte[] ComposeReportSale()
        {
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
                        text.Span("Reporte de cuentas por cobrar").SemiBold().FontSize(14);
                    });

                    column.Item().Element(CellStyleTitle).AlignCenter().Text(text =>
                    {
                        text.Span($"Vendedor: {_sellerName} ").SemiBold().FontSize(14);
                    });
                    column.Item().Text(text =>
                    {
                        text.Span("Fecha impresion: ").SemiBold().FontSize(10);
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy hh:mm")).FontSize(10);
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
            container.Table(table =>
            {
                // step 1
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(55);
                    columns.ConstantColumn(70);
                    columns.ConstantColumn(70);
                    columns.ConstantColumn(70);
                    columns.ConstantColumn(65);
                    columns.ConstantColumn(65);
                    columns.ConstantColumn(95);
                    columns.ConstantColumn(58);             
                });

                // step 2
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text(" Ref.").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("# Fac").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("F. Emision").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("F. Vencimiento ").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text(" S. Original").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("S. Actual").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Observaciones").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Responsable").FontSize(10).FontColor(Colors.White);
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
                    table.Cell().Text(item.CustomerCode).Bold().FontSize(10);
                    table.Cell().ColumnSpan(7).Text(item.CustomerName).Bold().FontSize(10);
                    int d = 0;

                    foreach (var itemdetail in item.Detail)
                    {
                        IContainer CellStyleComment(IContainer container)
                        {
                            if (itemdetail.Days == 0)
                            {
                                return container.DefaultTextStyle(x => x.FontColor(Colors.Red.Accent2)).PaddingVertical(0).BorderBottom(0.1f).BorderColor(Colors.White);
                            }
                            else if (itemdetail.Days > 0)
                            {
                                return container.DefaultTextStyle(x => x.FontColor(Colors.Blue.Accent1)).PaddingVertical(0).BorderBottom(0.1f).BorderColor(Colors.White);
                            }
                            else
                            {
                                return container.DefaultTextStyle(x => x.FontColor(Colors.Red.Darken1)).PaddingVertical(0).BorderBottom(0.1f).BorderColor(Colors.White);
                            }
                            
                        }
               
                        table.Cell().Element(CellStyle).Text(itemdetail.Reference).FontSize(10);
                        table.Cell().Element(CellStyle).Text(itemdetail.DocNum.ToString()).FontSize(10);
                        table.Cell().Element(CellStyle).Text(itemdetail.DocDate.ToString("dd-MM-yyyy")).FontSize(10);
                        table.Cell().Element(CellStyle).Text(itemdetail.DocDueDate.ToString("dd-MM-yyyy")).FontSize(10);
                        table.Cell().Element(CellStyle).Text(itemdetail.DocTotal.ToString("N")).FontSize(10);
                        table.Cell().Element(CellStyle).Text(itemdetail.Balance.ToString("N")).FontSize(10);
                        table.Cell().Element(CellStyleComment).Text(itemdetail.Comment).FontSize(10);
                        table.Cell().Element(CellStyle).Text(itemdetail.SellerName).FontSize(10);
                        d++;
                    }
                    if (d == item.Detail.Count())
                    {
                        // Añadir celdas vacías
                        table.Cell().BorderTop(1).BorderColor(Colors.Black);
                        table.Cell().BorderTop(1).BorderColor(Colors.Black);
                        table.Cell().BorderTop(1).BorderColor(Colors.Black);

                        // Celda con "Total:"
                        table.Cell().BorderTop(1).BorderColor(Colors.Black)
                            .AlignLeft().Text("Total:").Bold().FontSize(10);
                        // Celda con el total de Balance
                        table.Cell().BorderTop(1).BorderColor(Colors.Black)
                            .AlignLeft().Text($"{item.Detail.Sum(x => x.DocTotal).ToString("N")}").Bold().FontSize(10);


                        // Celda con el total de Balance
                        table.Cell().BorderTop(1).BorderColor(Colors.Black)
                            .AlignLeft().Text($"{item.Detail.Sum(x => x.Balance).ToString("N")}").Bold().FontSize(10);

                        // Añadir celdas vacías adicionales
                        table.Cell().BorderTop(1).BorderColor(Colors.Black);
                        table.Cell().BorderTop(1).BorderColor(Colors.Black);
                    }


                    i++;
                }
                if (i == _result.Count())
                {
                    table.Cell();
                    table.Cell();
                    table.Cell();
                    table.Cell().AlignLeft().Text("Total General:").Bold().FontSize(10);
                    table.Cell().AlignLeft().Text($"{_result.Sum(x => x.Detail.Sum(x=> x.DocTotal)).ToString("N")}").Bold().FontSize(10);
                    table.Cell().AlignLeft().Text($"{_result.Sum(x => x.Detail.Sum(x=> x.Balance)).ToString("N")}").Bold().FontSize(10);         
                    table.Cell().ColumnSpan(7).Text("");
                    table.Cell().ColumnSpan(7).Text("");

                    // Resumen"
                    table.Cell().ColumnSpan(2).BorderLeft(1).BorderTop(1).BorderColor(Colors.Black).AlignLeft().Text("Total Vencido---->:").Bold().FontSize(10);
                    table.Cell().ColumnSpan(6).BorderRight(1).BorderTop(1).BorderColor(Colors.Black).AlignLeft().Text($"{_result.Sum(x => x.Detail.Where(d => d.Days<0).Sum(d => d.Balance)).ToString("N")}").Bold().FontColor("#FF0000").FontSize(10);
                    table.Cell().ColumnSpan(2).BorderLeft(1).BorderBottom(1).BorderColor(Colors.Black).AlignLeft().Text("Total General---->:").Bold().FontSize(10);
                    table.Cell().ColumnSpan(6).BorderRight(1).BorderBottom(1).BorderColor(Colors.Black).AlignLeft().Text($"{_result.Sum(x => x.Detail.Sum(d => d.Balance)).ToString("N")}").Bold().FontSize(10);
                    table.Cell();
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
