using System;
using System.Collections.Generic;

#nullable disable

namespace PCliente.DAL.Entities
{
    public partial class Cola
    {
        public int IdCola { get; set; }
        public string NombreCola { get; set; }
        public int TiempoCola { get; set; }
        public string EstadoCola { get; set; }
    }
}
