using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PCliente.BL.Models;
using PCliente.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCliente.BL
{
    public class ClienteServices: IClienteServices
    {
        private readonly DbContextCliente DbContextCliente;
        private string Conexion;
        public ClienteServices(DbContextCliente _DbContextCliente, IConfiguration configuration)
        {
            this.DbContextCliente = _DbContextCliente;
            Conexion = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task<List<Cliente>> GetAllCliente()
        {
            return await DbContextCliente.Clientes.Where(s => s.AsignadoCola == "N").ToListAsync();
        

        }

        public async Task<string> InsertCliente(ClienteDetailModel cliente)
        {
            try
            {

            
                var entidad = new Cliente()
                    {
                        IdCliente = cliente.idCliente,
                        NombreCliente = cliente.nombreCliente

                    };
                //Antes de Insertar se debe verificar que el id, no exista. Caso contrario, debe buscar si lo esta atendiendo
                //sino lo esta atendiendo, se incluye en la tabla Cliente_Cola
                //2 es NuevoCliente,1 yaexiste
                string  resultado = await CheckExistenciaCliente(cliente.idCliente);
                if (resultado == "2")
                {
                    resultado = await checkColaLibre("L");
                    //Libre es resultado 1.
                    if (resultado == "1")

                    {
                        DbContextCliente.Clientes.Add(entidad);
                        await DbContextCliente.SaveChangesAsync();
                        string idCliente = cliente.idCliente;
                        var respuesta =  await EjecutarProcesoAtencionCliente(idCliente);
                       

                    }
                }
               
                return "1";
            
            }

            catch (Exception)
            {

                return "2";
            }
        }

        public async Task<string> UpdateCliente(ClienteDetailModel cliente)
        {
            try
            {

                var entity = await DbContextCliente.ClienteColas.FirstOrDefaultAsync(s => s.IdCliente == cliente.idCliente);
                entity.EstadoCliente = "PE";
                DbContextCliente.Entry(entity).CurrentValues.SetValues(entity);

                //DbContextCliente.ClienteColas.Attach(entity);
                //DbContextCliente.Entry(entity).Property(r => r.EstadoCliente).IsModified = true;

                int respuesta =  await DbContextCliente.SaveChangesAsync();
                return "1";
            }
            catch (Exception ex)
            {

                return "2";
            }
        }

        public async Task DeleteCliente(string ClienteId)
        {
            var entity = new Cliente()
            {
                IdCliente = ClienteId
            };
            DbContextCliente.Clientes.Attach(entity);
            DbContextCliente.Clientes.Remove(entity);
            await DbContextCliente.SaveChangesAsync();
        }

       

        //Verificar cada 1 minuto
        public async Task<string> checkTime()
        {
            try
            {
                //Verifico una cola Libre
                //
                using (var ctx = new DbContextCliente())
                {
                    var studentList = ctx.ClienteColas.Where(s => s.EstadoCliente == "CV").ToList();
                    var d="";
                }

                var data =  await (from Colascliente in DbContextCliente.ClienteColas where Colascliente.EstadoCliente =="CV" select Colascliente).ToListAsync();
                await verificarTiempo(data);
                var resultado =  data;

                return "1";

            }

            catch (Exception ex)
            {

                return "2";
            }
        }
        public async Task<string> verificarTiempo(List<ClienteCola> data)
        {
            try
            {
                using (var ctx = new DbContextCliente())
                {
                    foreach (dynamic item in data)
                    {
                        var tiempoFin = item.TiempoFin;
                        string idCliente = item.IdCliente;
                        int idCola = item.IdCola;

                        DateTime now = DateTime.Now;

                        if (tiempoFin <= now)
                        {

                            #region Liberar-Tablas
                            await EjecutarProcesoLiberarTablas(idCliente, idCola);

                            #endregion Liberar-Tablas

                        }

                    }

                }
                return "1";
            }
            catch (Exception ex)
            {
                return "2";
                throw;
            }


        }
        
        #region ProcesoNuevoCliente
        public async Task<string> CheckExistenciaCliente(string idCliente)
        {
            try
            {
                List<Cliente> cliente= new List<Cliente>();
                using (var ctx = new DbContextCliente())
                {
                    cliente = await ctx.Clientes.Where(s => s.IdCliente == idCliente).ToListAsync();
                   
                }
                if (cliente.Count > 0)
                {
                    return "1";
                }
                else
                {
                    return "2";
                }
                
            }
            catch (Exception ex)
            {

                return "2";
            }


        }
        public async Task<string> checkColaLibre(string estadoCola)
        {
            try
            {
                //Verifico una cola Libre
                var data = from clienteEstado in DbContextCliente.Colas where clienteEstado.EstadoCola == estadoCola select clienteEstado;
                await data.ToListAsync();
                return "1";

            }

            catch (Exception)
            {

                return "2";
            }
        }

        public async Task<string> EjecutarProcesoAtencionCliente(string idCliente)
        {

            try
            {
                string retunvalue = "";

                using (SqlConnection conn = new SqlConnection(Conexion))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "ProcesoAtencionCliente";
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter param = new SqlParameter("@idCliente", SqlDbType.VarChar, 50);
                        param.Direction = ParameterDirection.Input;
                        param.Value = idCliente;
                        cmd.Parameters.Add(param);


                        SqlParameter  paramOut = new SqlParameter("@mensaje_control", SqlDbType.VarChar, 50);
                        paramOut.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(paramOut);

                        

                        await cmd.ExecuteNonQueryAsync();
                        retunvalue = paramOut.Value.ToString();
                       
                    }
                }
                return retunvalue;
            }
            catch (Exception ex)
            {
                return "2";
                throw;
            }

        }

        public async Task<string> ActualizaEstadoCliente(string idCliente)
        {

            try
            {
                string retunvalue = "";

                using (SqlConnection conn = new SqlConnection(Conexion))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "ActualizaEstadoCliente";
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter param = new SqlParameter("@idCliente", SqlDbType.VarChar,20);
                        param.Direction = ParameterDirection.Input;
                        param.Value = idCliente;
                        cmd.Parameters.Add(param);


                        SqlParameter paramOut = new SqlParameter("@mensaje_control", SqlDbType.VarChar, 50);
                        paramOut.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(paramOut);



                        await cmd.ExecuteNonQueryAsync();
                        retunvalue = paramOut.Value.ToString();

                    }
                }
                return retunvalue;
            }
            catch (Exception ex)
            {
                return "2";
                throw;
            }

        }
        public async Task<string> EjecutarProcesoLiberarTablas(string idCliente, int idCola)
        {

            try
            {


                using (SqlConnection conn = new SqlConnection(Conexion))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "VerificarColaCliente";
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter param = new SqlParameter("@idCliente", SqlDbType.VarChar, 50);
                        param.Direction = ParameterDirection.Input;
                        param.Value = idCliente;
                        cmd.Parameters.Add(param);

                        SqlParameter paramCola = new SqlParameter("@idCola", SqlDbType.Int);
                        paramCola.Direction = ParameterDirection.Input;
                        paramCola.Value = idCola;
                        cmd.Parameters.Add(paramCola);

                        SqlParameter paramOut = new SqlParameter("@mensaje_control", SqlDbType.VarChar, 50);
                        paramOut.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(paramOut);



                        await cmd.ExecuteNonQueryAsync();
                        string retunvalue = paramOut.Value.ToString();

                    }
                }
                return "1";
            }
            catch (Exception ex)
            {
                return "2";
                throw;
            }

        }

        #endregion ProcesoNuevoCliente

    }
}
