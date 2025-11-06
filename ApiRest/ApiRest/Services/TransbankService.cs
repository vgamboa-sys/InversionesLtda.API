

using ApiRest.Dto.Request;
using ApiRest.Dto.Response;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.WebpayPlus;
namespace ApiRest.Services
{
    public class TransbankService
    {
        private Transaction tx;
        private ResponseGlobal response;

        public TransbankService()
        {
            tx = new Transaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            response = new ResponseGlobal();
        }

        public ResponseGlobal Crear_Transaccion_Service(CreateTransaction request)
        {

            if (request == null ||
                string.IsNullOrWhiteSpace(request.buy_order) ||
                string.IsNullOrWhiteSpace(request.session_id) ||
                string.IsNullOrWhiteSpace(request.return_url) ||
                request.amount <= 0
                )
            {
                response.exito = false;
                response.mensaje = "No se ingresaron datos validos";
                response.data = null;

                return response;
            }

            try
            {
                var CrearTransaccion = tx.Create(request.buy_order, request.session_id, request.amount, request.return_url);
                var RespuestaTransbank = new
                {
                    url = CrearTransaccion.Url,
                    token = CrearTransaccion.Token,
                    urlCompleta = CrearTransaccion.Url + "?token_ws=" + CrearTransaccion.Token,
                };

                response.exito = true;
                response.mensaje = "Transaccion generada";
                response.data = RespuestaTransbank;

                return response;
            }
            catch (Exception ex)
            {
                response.exito = false;
                response.mensaje = "Error , codigo : " + ex.Message;
                response.data = null;

                return response;
            }

        }

        public ResponseGlobal Confirmar_Transaccion_Service(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                response.exito = false;
                response.mensaje = "No se ingresaron datos validos";
                response.data = null;

                return response;
            }
            try
            {
                var ConfirmarTransaccion = tx.Commit(token);

                response.exito = true;
                response.mensaje = "Confirmacion de la transaccion";
                response.data = ConfirmarTransaccion;

                return response;
            }
            catch (Exception ex)
            {
                response.exito = false;
                response.mensaje = "Error , codigo : " + ex.Message;
                response.data = null;

                return response;
            }

        }

        public ResponseGlobal ObtenerEstado_Transaccion_Service(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                response.exito = false;
                response.mensaje = "No se ingresaron datos validos";
                response.data = null;

                return response;
            }
            try
            {
                var ObtenerEstadoTransaccion = tx.Status(token);

                response.exito = true;
                response.mensaje = "Estado de la transaccion";
                response.data = ObtenerEstadoTransaccion;

                return response;
            }
            catch (Exception ex)
            {
                response.exito = false;
                response.mensaje = "Error , codigo : " + ex.Message;
                response.data = null;

                return response;
            }

        }

        public ResponseGlobal AnularoDevolver_Transaccion_Service(AnularoDevolver request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.token) ||
                request.amount <= 0)
            {
                response.exito = false;
                response.mensaje = "No se ingresaron datos validos";
                response.data = null;

                return response;
            }
            try
            {
                var DevolveroAnularTransaccion = tx.Refund(request.token, request.amount);

                response.exito = true;
                response.mensaje = "Transaccion anulada con exito";
                response.data = DevolveroAnularTransaccion;

                return response;
            }
            catch (Exception ex)
            {
                response.exito = false;
                response.mensaje = "Error , codigo : " + ex.Message;
                response.data = null;

                return response;
            }
        }

    }
}
