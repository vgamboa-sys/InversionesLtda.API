using ApiRest.Dto.Request;
using ApiRest.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiRest.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransbankController : ControllerBase
    {

        private readonly TransbankService _transbankService;
        public TransbankController(TransbankService transbankService)
        {
            _transbankService = transbankService;
        }


        [HttpPost]
        public IActionResult Crear_transaccion(CreateTransaction request)
        {

            try
            {
                var response = _transbankService.Crear_Transaccion_Service(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error, codigo : " + ex.Message);

            }

        }

        [HttpGet("{token}")]
        public IActionResult Confirmar_Transaccion(string token)
        {
            try
            {
                var response = _transbankService.Confirmar_Transaccion_Service(token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(" Hubo un error, codigo del error " + ex.Message);
            }

        }


        [HttpGet("{token}")]
        public IActionResult ObtenerEstado_Transaccion(string token)
        {
            try
            {
                var response = _transbankService.ObtenerEstado_Transaccion_Service(token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(" Hubo un error, codigo del error " + ex.Message);
            }

        }


        [HttpPost]
        public IActionResult AnularoDevolver_Transaccion(AnularoDevolver request)
        {
            try
            {
                var response = _transbankService.AnularoDevolver_Transaccion_Service(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(" Hubo un error, codigo del error " + ex.Message);
            }
        }
    }
}
