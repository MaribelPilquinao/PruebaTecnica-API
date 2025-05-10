namespace Prueba_crud.Models
{
    public class PedidoDetalle
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string? NombreProducto { get; set; } 

    }
}
