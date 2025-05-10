namespace Prueba_crud.Models
{
    public class PedidoCrearDTO
    {
        public int IdCliente { get; set; }
        public List<PedidoDetalle> Detalles { get; set; } = new();
    }
}
