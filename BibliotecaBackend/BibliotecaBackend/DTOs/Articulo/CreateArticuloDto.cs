namespace BibliotecaBackend.DTOs.Articulo
{
    public class CreateArticuloDto
    {
        public string NombreArticulo { get; set; } = null!;
        public int SaldoInventario { get; set; }
        public decimal Precio { get; set; }
    }
}
