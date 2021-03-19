using PCliente.BL.Models;
using PCliente.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PCliente.BL
{
    public interface IClienteServices
    {
        Task<List<Cliente>> GetAllCliente();
        Task<string> InsertCliente(ClienteDetailModel cliente);
        Task<string> UpdateCliente(ClienteDetailModel cliente);
        Task DeleteCliente(string ClienteId);

        Task<string> checkTime();

        Task<string> EjecutarProcesoAtencionCliente(string idCliente);
        
    }
}
