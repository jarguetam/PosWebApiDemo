using System.Collections.Generic;

namespace Pos.WebApi.Features.Reports.Dto
{
    public class ReportSalesMargenDto
    {
        public List<FamiliaViewModel> Familias { get; set; }
    }

    public class FamiliaViewModel
    {
        public string Familia { get; set; }
        public List<SubFamiliaViewModel> SubFamilias { get; set; }
    }

    public class SubFamiliaViewModel
    {
        public string SubFamilia { get; set; }
        public List<DetalleVentaViewModel> Detalles { get; set; }
    }

    public class DetalleVentaViewModel
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Costo { get; set; }
        public decimal Venta { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Libras { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal VentaTotal { get; set; }
        public decimal RentabilidadBruta { get; set; }
        public decimal PorcentajeRentabilidad { get; set; }
        public string Ruta { get; set; }
    }

    public class ResultadoVenta
    {
        public string Familia { get; set; }
        public string Sub_Familia { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Costo { get; set; }
        public decimal Venta { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Libas { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal VentaTotal { get; set; }
        public decimal RentabilidadBruta { get; set; }
        public decimal Rent { get; set; }
        public string Ruta { get; set; }
    }

}
