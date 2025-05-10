﻿namespace Prueba_crud.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public List<PedidoDetalle> Detalles { get; set; } = new();
    }
}
