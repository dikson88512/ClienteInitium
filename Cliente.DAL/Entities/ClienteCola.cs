using System;
using System.Collections.Generic;

#nullable disable

namespace PCliente.DAL.Entities
{
    public partial class ClienteCola
    {
        public string IdCliente { get; set; }
        public int IdCola { get; set; }
        public DateTime? TiempoInicio { get; set; }
        public DateTime? TiempoFin { get; set; }
        public int Ticket { get; set; }
        public string EstadoCliente { get; set; }
    }
}
