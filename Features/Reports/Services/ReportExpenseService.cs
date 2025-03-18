using Pos.WebApi.Features.Common.Dto;
using Pos.WebApi.Features.Common.Entities;
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
    public class ReportExpenseService
    {
        private readonly List<ReportExpenseDto> _result;
        private CompanyInfo _companyInfo;
        private FileUpload _fileUpload;
        private int _totalLines;
        private string _sellerName;
        private DateTime _from;
        private DateTime _to;

        public ReportExpenseService(List<ReportExpenseDto> result, CompanyInfo companyInfo, FileUpload fileUpload, int totalLines, string sellerName, DateTime from, DateTime to)
        {
            _result = result;
            _companyInfo = companyInfo;
            _fileUpload = fileUpload;
            _totalLines = totalLines;
            _sellerName = sellerName;
            _from = from;
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
                        text.Span("Reporte de gastos resumen").SemiBold().FontSize(14);

                    });
                    column.Item().Element(CellStyleTitle).AlignCenter().Text(text =>
                    {
                        text.Span($"Desde: {_from.ToString("dd/MM/yyyy")} Hasta: {_to.ToString("dd/MM/yyyy")} ").SemiBold().FontSize(14);
                    });

                    column.Item().Element(CellStyleTitle).AlignCenter().Text(text =>
                    {
                        text.Span("Empleado: " + _sellerName).SemiBold().FontSize(14);
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
                    columns.RelativeColumn();
                    columns.ConstantColumn(85);
                    columns.ConstantColumn(85);
                });

                // step 2
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Codigo").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Descripcion").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Tipo Gasto").FontSize(10).FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Total").FontSize(10).FontColor(Colors.White);

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
                    table.Cell().Element(CellStyle).Text(item.ExpenseId.ToString()).FontSize(10);
                    table.Cell().Element(CellStyle).Text(item.ExpenseName).FontSize(10);
                    table.Cell().Element(CellStyle).Text(item.ExpenseType).FontSize(10);
                    table.Cell().Element(CellStyle).Text(item.ExpenseAmount.ToString("N")).FontSize(10);
                    i++;
                }
                if (i == _result.Count())
                {
                    table.Cell().ColumnSpan(3).BorderTop(1).BorderColor(Colors.Black)
                     .AlignLeft().Text(@$"Registros: {_totalLines}").Bold().FontSize(10);
                    table.Cell().BorderTop(1).BorderColor(Colors.Black).AlignLeft().Text($"{_result.Sum((x => x.ExpenseAmount)).ToString("N")}").Bold().FontSize(10);
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
