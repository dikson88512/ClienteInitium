﻿using System;
using System.Collections.Generic;

#nullable disable

namespace PCliente.DAL.Entities
{
    public partial class Cliente
    {
        public string IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string AsignadoCola { get; set; }
    }
}
