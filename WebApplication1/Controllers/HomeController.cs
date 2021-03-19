using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PCliente.BL;
using PCliente.BL.Models;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {

        private readonly IClienteServices _ClienteServices;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IClienteServices _clienteServices)
        {
            _logger = logger;
            _ClienteServices = _clienteServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
           
           
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost()]
        [Route("api/Home/InsertClienteFrontEnd")]
        public async Task<string> InsertClienteFrontEnd(ClienteDetailModel cliente)
        {
            try
            {
                var resultado = await _ClienteServices.InsertCliente(cliente);
                return resultado;
            }
            catch (Exception)
            {

                throw;
            }


        }


        [HttpGet()]
        [Route("Home/Privacy/api/Home/ObtenerTodosClienteAtendidos")]
        public async Task<JsonResult> ObtenerTodosClienteAtendidos()
        {
            try
            {
                var resultado = await _ClienteServices.GetAllCliente();


                return Json(new
                {
                    datos = resultado
                });


            }
            catch (Exception)
            {

                throw;
            }


        }

        [HttpPost()]
        [Route("api/Home/UpdateCliente")]
        public async Task<string> UpdateCliente(ClienteDetailModel cliente)
        {
            try
            {
                ClienteDetailModel Cliente = new ClienteDetailModel();
                Cliente.idCliente = "4";
                var resultado = await _ClienteServices.UpdateCliente(Cliente);
                return resultado;
            }
            catch (Exception)
            {

                throw;
            }


        }

        #region VerificarCadaMinuto
        [HttpPost()]
        [Route("api/Home/checkTime")]
        public async Task<string> checkTime(ClienteDetailModel cliente)
        {
            try
            {
               
                var resultado = await _ClienteServices.checkTime();
                return resultado;
            }
            catch (Exception)
            {

                throw;
            }


        }

        #endregion VerificarCadaMinuto

    }
}
