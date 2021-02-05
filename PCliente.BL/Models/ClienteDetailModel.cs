using System;
using System.Collections.Generic;
using System.Text;

namespace PCliente.BL.Models
{
    public class ClienteDetailModel
    {
        public string idCliente { get; set; }

        public int idCola { get; set; }

        public string nombreCliente { get; set; }

        public DateTime TiempoInicio { get; set; }

        public DateTime TiempoFin { get; set; }

        public int Ticket { get; set; }

        public string EstadoCliente { get; set; }

    }
}
