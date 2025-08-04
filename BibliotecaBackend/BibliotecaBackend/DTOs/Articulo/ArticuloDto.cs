namespace BibliotecaBackend.DTOs.Articulo
{
    public class ArticuloDto
    {
        public int CodigoArticulo { get; set; }
        public string NombreArticulo { get; set; } = null!;
        public int SaldoInventario { get; set; }
        public decimal Precio { get; set; }
    }
}
