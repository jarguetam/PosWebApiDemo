using Pos.WebApi.Features.Common.Dto;
using Pos.WebApi.Features.Common.Entities;
using Pos.WebApi.Features.Items.Entities;
using Pos.WebApi.Features.Reports.Dto;
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
    public class ReportSalesMargenServices
    {
        private readonly ReportSalesMargenDto _result;
        private CompanyInfo _companyInfo;
        private FileUpload _fileUpload;
        private string _sellerName;
        private DateTime _from;
        private DateTime _to;

        public ReportSalesMargenServices(ReportSalesMargenDto result, CompanyInfo companyInfo, FileUpload fileUpload, string sellerName, DateTime fro, DateTime to)
        {
            _result = result;
            _companyInfo = companyInfo;
            _fileUpload = fileUpload;
            _sellerName = sellerName;
            _from = fro;
            _to = to;
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
                    page.Size(PageSizes.Letter.Landscape());
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
                        text.Span("Reporte de rentabilidad").SemiBold().FontSize(14);

                    });
                    column.Item().Element(CellStyleTitle).AlignCenter().Text(text =>
                    {
                        text.Span($"Desde: {_from.ToString("dd/MM/yyyy")} Hasta: {_to.ToString("dd/MM/yyyy")} ").SemiBold().FontSize(16);
                    });

                    column.Item().Element(CellStyleTitle).AlignCenter().Text(text =>
                    {
                        text.Span("Vendedor: " + _sellerName).SemiBold().FontSize(14);
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
            var totalCantidad = _result.Familias.Sum(f => f.SubFamilias.Sum(s => s.Detalles.Sum(d => d.Cantidad)));
            var totalLibras = _result.Familias.Sum(f => f.SubFamilias.Sum(s => s.Detalles.Sum(d => d.Libras)));
            var totalCostoTotal = _result.Familias.Sum(f => f.SubFamilias.Sum(s => s.Detalles.Sum(d => d.CostoTotal)));
            var totalVentaTotal = _result.Familias.Sum(f => f.SubFamilias.Sum(s => s.Detalles.Sum(d => d.VentaTotal)));
            var totalRentabilidadBruta = _result.Familias.Sum(f => f.SubFamilias.Sum(s => s.Detalles.Sum(d => d.RentabilidadBruta)));
            var margenTotal = totalVentaTotal != 0 ? ((totalVentaTotal - totalCostoTotal) / totalVentaTotal) * 100 : 0;
            container.Table(table =>
            {
                // step 1
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(45);
                    columns.ConstantColumn(180);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();

                });

                // step 2
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Codigo").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Descripcion").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Costo Promedio").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Venta Promedio").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Cantidad").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Libras").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Costo Total").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Venta Total").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Rentabilidad").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Rent. %").FontSize(10).FontColor(Colors.White);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.White).Background(Colors.BlueGrey.Medium);
                    }
                });
                // step 3
               
                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.Medium()).PaddingLeft(10).PaddingVertical(0).BorderBottom(0.1f).BorderColor(Colors.White);
                }
                int i = 0;
                foreach (var itemdetail in _result.Familias)
                {
                    int f = 0;
                    table.Cell().ColumnSpan(10).BorderBottom(1).BorderColor(Colors.Black).Text(itemdetail.Familia).Bold().FontSize(10);
                    foreach (var subFamilia in itemdetail.SubFamilias)
                    {
                        int d = 0;
                        table.Cell().ColumnSpan(10).Text("    " +subFamilia.SubFamilia).Bold().FontSize(10);
                        foreach (var detail in subFamilia.Detalles)
                        {
                            table.Cell().Element(CellStyle).Text(detail.Codigo).FontSize(8);
                            table.Cell().Element(CellStyle).Text(detail.Descripcion).FontSize(8);
                            table.Cell().Element(CellStyle).Text(detail.Costo.ToString("N")).FontSize(8);
                            table.Cell().Element(CellStyle).Text(detail.Venta.ToString("N")).FontSize(8);
                            table.Cell().Element(CellStyle).Text(detail.Cantidad.ToString("N")).FontSize(8);
                            table.Cell().Element(CellStyle).Text(detail.Libras.ToString("N")).FontSize(8);
                            table.Cell().Element(CellStyle).Text(detail.CostoTotal.ToString("N")).FontSize(8);
                            table.Cell().Element(CellStyle).Text(detail.VentaTotal.ToString("N")).FontSize(8);
                            table.Cell().Element(CellStyle).Text(detail.RentabilidadBruta.ToString("N")).FontSize(8);
                            table.Cell().Element(CellStyle).Text((detail.PorcentajeRentabilidad*100).ToString("N")+" %").FontSize(8);
                            d++;
                        }
                        if (d == subFamilia.Detalles.Count)
                        {
                            table.Cell().ColumnSpan(4).BorderColor(Colors.Black)
                                .AlignRight().Element(CellStyle).Text("").Bold().FontSize(8);
                            table.Cell().BorderTop(1).BorderColor(Colors.Black)
                                .AlignLeft().Element(CellStyle).Text($"{subFamilia.Detalles.Sum(x => x.Cantidad).ToString("N")}").Bold().FontSize(8);
                            table.Cell().BorderTop(1).BorderColor(Colors.Black)
                                .AlignLeft().Element(CellStyle).Text($"{subFamilia.Detalles.Sum(x => x.Libras).ToString("N")}").Bold().FontSize(8);
                            table.Cell().BorderTop(1).BorderColor(Colors.Black)
                                .AlignLeft().Element(CellStyle).Text($"{subFamilia.Detalles.Sum(x => x.CostoTotal).ToString("N")}").Bold().FontSize(8);
                            table.Cell().BorderTop(1).BorderColor(Colors.Black)
                                .AlignLeft().Element(CellStyle).Text($"{subFamilia.Detalles.Sum(x => x.VentaTotal).ToString("N")}").Bold().FontSize(8);
                            table.Cell().BorderTop(1).BorderColor(Colors.Black)
                               .AlignLeft().Element(CellStyle).Text($"{subFamilia.Detalles.Sum(x => x.RentabilidadBruta).ToString("N")}").Bold().FontSize(8);
                            var margen = ((subFamilia.Detalles.Sum(x => x.VentaTotal) - subFamilia.Detalles.Sum(x => x.CostoTotal)) / subFamilia.Detalles.Sum(x => x.VentaTotal))*100;
                            table.Cell().BorderTop(1).BorderColor(Colors.Black)
                               .AlignLeft().Element(CellStyle).Text($"{margen.ToString("N")} %").Bold().FontSize(8);
                        }
                        f++;  
                    }
                    i++;
                    if (f == itemdetail.SubFamilias.Count)
                    {
                        var familiaCantidad = itemdetail.SubFamilias.Sum(s => s.Detalles.Sum(d => d.Cantidad));
                        var familiaLibras = itemdetail.SubFamilias.Sum(s => s.Detalles.Sum(d => d.Libras));
                        var familiaCostoTotal = itemdetail.SubFamilias.Sum(s => s.Detalles.Sum(d => d.CostoTotal));
                        var familiaVentaTotal = itemdetail.SubFamilias.Sum(s => s.Detalles.Sum(d => d.VentaTotal));
                        var familiaRentabilidadBruta = itemdetail.SubFamilias.Sum(s => s.Detalles.Sum(d => d.RentabilidadBruta));
                        var familiaMargen = familiaVentaTotal != 0 ? ((familiaVentaTotal - familiaCostoTotal) / familiaVentaTotal) * 100 : 0;

                        table.Cell().ColumnSpan(4).BorderTop(1).BorderColor(Colors.Black)
                            .AlignLeft().Text($"TOTAL {itemdetail.Familia} --->").Bold().FontSize(8);
                        table.Cell().BorderTop(1).BorderColor(Colors.Black)
                            .AlignLeft().Element(CellStyle).Text(familiaCantidad.ToString("N")).Bold().FontSize(8);
                        table.Cell().BorderTop(1).BorderColor(Colors.Black)
                            .AlignLeft().Element(CellStyle).Text(familiaLibras.ToString("N")).Bold().FontSize(8);
                        table.Cell().BorderTop(1).BorderColor(Colors.Black)
                            .AlignLeft().Element(CellStyle).Text(familiaCostoTotal.ToString("N")).Bold().FontSize(8);
                        table.Cell().BorderTop(1).BorderColor(Colors.Black)
                            .AlignLeft().Element(CellStyle).Text(familiaVentaTotal.ToString("N")).Bold().FontSize(8);
                        table.Cell().BorderTop(1).BorderColor(Colors.Black)
                           .AlignLeft().Element(CellStyle).Text(familiaRentabilidadBruta.ToString("N")).Bold().FontSize(8);
                        table.Cell().BorderTop(1).BorderColor(Colors.Black)
                           .AlignLeft().Element(CellStyle).Text($"{familiaMargen:N2} %").Bold().FontSize(8);
                    }
                }
                if (i == _result.Familias.Count)
                {
                    var totalArticulos = _result.Familias.Sum(f => f.SubFamilias.Sum(s => s.Detalles.Count));
                    table.Cell().ColumnSpan(4).BorderTop(1).BorderColor(Colors.Black)
                    .AlignLeft().Text($"Total Lineas ---> {totalArticulos}").Bold().FontSize(8);
                    table.Cell().BorderTop(1).BorderColor(Colors.Black)
                        .AlignLeft().Element(CellStyle).Text(totalCantidad.ToString("N")).Bold().FontSize(8);
                    table.Cell().BorderTop(1).BorderColor(Colors.Black)
                        .AlignLeft().Element(CellStyle).Text(totalLibras.ToString("N")).Bold().FontSize(8);
                    table.Cell().BorderTop(1).BorderColor(Colors.Black)
                        .AlignLeft().Element(CellStyle).Text(totalCostoTotal.ToString("N")).Bold().FontSize(8);
                    table.Cell().BorderTop(1).BorderColor(Colors.Black)
                        .AlignLeft().Element(CellStyle).Text(totalVentaTotal.ToString("N")).Bold().FontSize(8);
                    table.Cell().BorderTop(1).BorderColor(Colors.Black)
                        .AlignLeft().Element(CellStyle).Text(totalRentabilidadBruta.ToString("N")).Bold().FontSize(8);
                    table.Cell().BorderTop(1).BorderColor(Colors.Black)
                        .AlignLeft().Element(CellStyle).Text($"{margenTotal.ToString("N")} %").Bold().FontSize(8);

                }            
            }      
            );
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
