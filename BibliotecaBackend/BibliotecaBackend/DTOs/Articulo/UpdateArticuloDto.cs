namespace BibliotecaBackend.DTOs.Articulo
{
    public class UpdateArticuloDto
    {
        public string NombreArticulo { get; set; } = null!;
        public int SaldoInventario { get; set; }
        public decimal Precio { get; set; }
    }

}
