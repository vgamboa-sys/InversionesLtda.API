from flask import Flask, jsonify, render_template, request, redirect, session
import requests
import urllib3
import string
import random
import json  # 👈 importante

# Flask config
app = Flask(__name__, template_folder='Frontend', static_folder='Frontend/Static/css')

# Clave para usar sesiones (SOLO DESARROLLO, cámbiala en producción)
app.secret_key = "dev-unishop-secret-key"

# Deshabilitar advertencias de SSL para urllib3 (solo para desarrollo)
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)


@app.route('/')
def vista():
    base_url = 'https://localhost:5000/api'
    productos_url = f'{base_url}/Producto'
    region_url = f'{base_url}/Region'
    provincia_url = f'{base_url}/Provincia'
    comuna_url = f'{base_url}/Comuna'
    sucursal_url = f'{base_url}/Sucursal'
    stock_url = f'{base_url}/Stock'  # URL PARA STOCK

    try:
        # Obtener regiones
        response_regiones = requests.get(region_url, verify=False)
        regiones = response_regiones.json() if response_regiones.status_code == 200 else []

        # Obtener provincias
        response_provincias = requests.get(provincia_url, verify=False)
        provincias = response_provincias.json() if response_provincias.status_code == 200 else []

        # Obtener comunas
        response_comunas = requests.get(comuna_url, verify=False)
        comunas = response_comunas.json() if response_comunas.status_code == 200 else []

        # Obtener sucursales
        response_sucursales = requests.get(sucursal_url, verify=False)
        sucursales = response_sucursales.json() if response_sucursales.status_code == 200 else []

        # Obtener productos
        response_productos = requests.get(productos_url, verify=False)
        productos = response_productos.json() if response_productos.status_code == 200 else []

        # Obtener stock
        response_stock = requests.get(stock_url, verify=False)
        stock_data = response_stock.json() if response_stock.status_code == 200 else []

        # Crear un diccionario para asociar el stock con los productos
        stock_dict = {}
        for stock in stock_data:
            cod_producto = stock["codProducto"]
            if cod_producto not in stock_dict:
                stock_dict[cod_producto] = 0
            stock_dict[cod_producto] += stock["cantidad"]  # Sumamos todas las cantidades por producto

        # Añadir la cantidad de stock a cada producto
        for producto in productos:
            producto["stock_disponible"] = stock_dict.get(producto["codProducto"], 0)

        return render_template(
            'index.html',
            productos=productos,
            regiones=regiones,
            provincias=provincias,
            comunas=comunas,
            sucursales=sucursales
        )

    except requests.exceptions.RequestException as e:
        return "Error de conexión: " + str(e)


@app.route('/pago', methods=['GET', 'POST'])
def pago():
    base_url = 'https://localhost:5000/api'
    transbank_url = f'{base_url}/Transbank/Crear_transaccion'
    cliente_url = f'{base_url}/Cliente'
    region_url = f'{base_url}/Region'
    provincia_url = f'{base_url}/Provincia'
    comuna_url = f'{base_url}/Comuna'

    def generar_codigo(prefijo, longitud=8):
        return f"{prefijo}{''.join(random.choices(string.digits, k=longitud))}"

    # Cargar datos de región/provincia/comuna para el formulario
    try:
        response_regiones = requests.get(region_url, verify=False)
        regiones = response_regiones.json() if response_regiones.status_code == 200 else []

        response_provincias = requests.get(provincia_url, verify=False)
        provincias = response_provincias.json() if response_provincias.status_code == 200 else []

        response_comunas = requests.get(comuna_url, verify=False)
        comunas = response_comunas.json() if response_comunas.status_code == 200 else []
    except requests.exceptions.RequestException as e:
        return f"Error de conexión: {e}"

    if request.method == 'POST':
        # 🔹 Carrito real enviado desde pago.html en carrito_json (hidden)
        carrito_json = request.form.get('carrito_json', '[]')
        try:
            carrito = json.loads(carrito_json)
        except json.JSONDecodeError:
            carrito = []

        if not carrito:
            return jsonify({"error": "El carrito está vacío o no se pudo leer."}), 400

        # Guardamos el carrito en sesión para usarlo al confirmar la transacción
        session['carrito'] = carrito

        # 🔹 Monto total a pagar (proviene del formulario, calculado en frontend)
        montoPagar = float(request.form['montoPagar'])

        buy_order = generar_codigo("ORD", 8)
        session_id = generar_codigo("SESSION", 10)
        return_url = "http://127.0.0.1:5001/confirmar_pago"

        # Datos del cliente desde el formulario
        datos_cliente = {
            "numRun": int(request.form['numRun']),
            "dvRun": request.form['dvRun'],
            "p_Nombre": request.form['p_Nombre'],
            "s_Nombre": request.form.get('s_Nombre', ''),
            "a_Paterno": request.form['a_Paterno'],
            "a_Materno": request.form['a_Materno'],
            "correo": request.form['correo'],
            "direccion": request.form['direccion'],
            "codRegion": int(request.form['codRegion']),
            "codProvincia": int(request.form['codProvincia']),
            "codComuna": int(request.form['codComuna'])
            # usuarioId no se envía (cliente invitado)
        }

        # 🔹 Guardamos datos del cliente en sesión para usarlos luego al crear la Boleta
        session['cliente'] = datos_cliente

        try:
            # Registrar/actualizar cliente en la API
            response_cliente = requests.post(cliente_url, json=datos_cliente, verify=False)
            # Si ya existe (409), lo consideramos ok para efectos de flujo
            if response_cliente.status_code not in (200, 201, 409):
                return jsonify({"error": "Error al registrar el cliente"}), 500
        except Exception as e:
            return jsonify({"error": f"Error en el registro del cliente: {e}"}), 500

        # Datos para iniciar transacción con Transbank
        datos_transbank = {
            "buy_order": buy_order,
            "session_id": session_id,
            "amount": montoPagar,
            "return_url": return_url
        }

        try:
            response = requests.post(transbank_url, json=datos_transbank, verify=False)
            if response.status_code == 200:
                data = response.json()
                if data.get("exito"):
                    urlCompleta = data["data"].get("urlCompleta")
                    if urlCompleta:
                        return redirect(urlCompleta)
                    else:
                        return jsonify({"error": "No se encontró la URL de pago"}), 500
                else:
                    return jsonify({"error": data.get("mensaje", "Error en la transacción")}), 500
            else:
                return jsonify({"error": f"Error en la solicitud a Transbank: {response.status_code}"}), 500
        except Exception as e:
            return jsonify({"error": str(e)}), 500

    return render_template('pago.html', regiones=regiones, provincias=provincias, comunas=comunas)


@app.route('/confirmar_pago', methods=['GET'])
def recibir_token():
    """Recibe el token de Transbank después del pago y redirige a la confirmación."""
    token = request.args.get('token_ws')

    if not token:
        return jsonify({"error": "No se recibió token de transacción"}), 400

    return redirect(f"/confirmar_transaccion/{token}")


@app.route('/confirmar_transaccion/<token>', methods=['GET'])
def confirmar_transaccion(token):
    base_url = 'https://localhost:5000/api'
    confirmacion_url = f'{base_url}/Transbank/Confirmar_transaccion/{token}'
    tarjeta_url = f'{base_url}/Tarjeta'
    boleta_url = f'{base_url}/Boleta'

    try:
        # 1️⃣ Confirmar la transacción con TransbankController
        response = requests.get(confirmacion_url, verify=False)
        if response.status_code != 200:
            return jsonify({"error": f"Error al confirmar la transacción: {response.status_code}"}), 500

        data = response.json()
        if not data.get("exito"):
            return jsonify({"error": data.get("mensaje", "Error al confirmar la transacción")}), 500

        detalles_transaccion = data.get("data", {})
        cod_transaccion = detalles_transaccion.get("buyOrder")  # buyOrder
        card_number = detalles_transaccion.get("cardDetail", {}).get("cardNumber")

        if not cod_transaccion or not card_number:
            return jsonify({"error": "No se pudo obtener la información de la transacción"}), 500

        cod_transaccion = str(cod_transaccion)
        cod_tarjeta = int(card_number)

        # 2️⃣ Verificar si la transacción ya está registrada en Tarjeta
        response_verificar = requests.get(f"{tarjeta_url}/{cod_transaccion}", verify=False)

        if response_verificar.status_code != 200:
            # 3️⃣ Registrar la transacción si no existe
            datos_tarjeta = {
                "codTransaccion": cod_transaccion,
                "numTarjeta": cod_tarjeta,
                "nombreTransaccion": "Compra Online"
            }
            response_tarjeta = requests.post(tarjeta_url, json=datos_tarjeta, verify=False)

            if response_tarjeta.status_code not in (200, 201):
                print(f"⚠️ Error al registrar la transacción: {response_tarjeta.status_code}")
                print(f"🔍 Respuesta del servidor: {response_tarjeta.text}")

        # 4️⃣ Crear la Boleta en la API usando BoletaController
        cliente_session = session.get('cliente')
        carrito = session.get('carrito', [])

        if not cliente_session:
            return jsonify({
                "error": "No se encontraron datos de cliente en sesión. No se puede emitir la boleta."
            }), 400

        if not carrito:
            return jsonify({
                "error": "No se encontró el carrito en sesión. No se puede emitir la boleta."
            }), 400

        num_run = cliente_session.get("numRun")
        dv_run = cliente_session.get("dvRun")
        correo = cliente_session.get("correo")

        if not num_run or not dv_run:
            return jsonify({"error": "Datos de RUN del cliente incompletos."}), 400

        # Asumimos que todos los ítems salen de la misma sucursal (la del primer item)
        cod_sucursal = carrito[0].get('codSucursal', 1)

        # Construir los detalles para Boleta (solo codProducto y cantidad, sucursal va en la boleta)
        detalles = []
        for item in carrito:
            detalles.append({
                "codProducto": item["codProducto"],
                "cantidad": item["cantidad"]
            })

        datos_boleta = {
            "numRun": num_run,
            "dvRun": dv_run,
            "correoContacto": correo,
            "esInvitada": True,          # cuando tengas login, podrás cambiar esto
            "codSucursal": cod_sucursal,
            "codTransaccion": cod_transaccion,
            "detalles": detalles
        }

        response_boleta = requests.post(boleta_url, json=datos_boleta, verify=False)

        if response_boleta.status_code not in (200, 201):
            print(f"⚠️ Error al crear la boleta: {response_boleta.status_code}")
            print(f"🔍 Respuesta del servidor: {response_boleta.text}")
        else:
            print("✅ Boleta creada correctamente en la API.")

        # 5️⃣ Renderizar la vista de confirmación
        return render_template('transaccion_confirmada.html', detalles=detalles_transaccion)

    except Exception as e:
        return jsonify({"error": str(e)}), 500


if __name__ == '__main__':
    app.run(port=5001, debug=True)
