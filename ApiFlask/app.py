from flask import Flask, jsonify, render_template, request ,redirect
import requests , urllib3 , string , random



app = Flask(__name__, template_folder='Frontend', static_folder='Frontend/Static/css')

# Deshabilitar advertencias de SSL para urllib3 (solo para desarrollo)
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

@app.route('/')
def vista():
    productos_url = 'https://localhost:5000/api/producto'
    region_url = 'https://localhost:5000/api/Region'
    provincia_url = 'https://localhost:5000/api/Provincia'
    comuna_url = 'https://localhost:5000/api/Comuna'
    sucursal_url = 'https://localhost:5000/api/Sucursal'
    stock_url = 'https://localhost:5000/api/Stock'  # NUEVA URL PARA STOCK

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
    transbank_url = 'https://localhost:5000/api/Transbank/Crear_transaccion'
    cliente_url = 'https://localhost:5000/api/Cliente'
    region_url = 'https://localhost:5000/api/Region'
    provincia_url = 'https://localhost:5000/api/Provincia'
    comuna_url = 'https://localhost:5000/api/Comuna'

    def generar_codigo(prefijo, longitud=8):
        return f"{prefijo}{''.join(random.choices(string.digits, k=longitud))}"

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
        montoPagar = float(request.form['montoPagar'])
        buy_order = generar_codigo("ORD", 8)
        session_id = generar_codigo("SESSION", 10)
        return_url = "http://127.0.0.1:5001/confirmar_pago"

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
        }

        try:
            response_cliente = requests.post(cliente_url, json=datos_cliente, verify=False)
            if response_cliente.status_code != 201:
                return jsonify({"error": "Error al registrar el cliente"}), 500
        except Exception as e:
            return jsonify({"error": f"Error en el registro del cliente: {e}"}), 500

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
    confirmacion_url = f'https://localhost:5000/api/Transbank/Confirmar_transaccion/{token}'
    tarjeta_url = 'https://localhost:5000/api/Tarjeta'
    venta_url = 'https://localhost:5000/api/Ventas/RealizarVenta'

    try:
        # 1️⃣ Hacer el GET para confirmar la transacción
        response = requests.get(confirmacion_url, verify=False)
        if response.status_code == 200:
            data = response.json()
            if data.get("exito"):
                detalles_transaccion = data.get("data", {})
                cod_transaccion = detalles_transaccion.get("buyOrder")  # Obtener buyOrder de la respuesta
                card_number = detalles_transaccion.get("cardDetail", {}).get("cardNumber")

                if cod_transaccion and card_number:
                    cod_transaccion = str(cod_transaccion)  # Convertimos buyOrder a string
                    cod_tarjeta = int(card_number)  # Convertimos a int

                    # 2️⃣ Verificar si el `codTransaccion` ya está registrado
                    response_verificar = requests.get(f"{tarjeta_url}/{cod_transaccion}", verify=False)

                    if response_verificar.status_code == 200:
                        print("🔍 La transacción ya está registrada, no es necesario volver a insertarla.")
                    else:
                        # 3️⃣ Registrar la transacción si no existe
                        datos_tarjeta = {
                            "codTransaccion": cod_transaccion,
                            "numTarjeta": cod_tarjeta,
                            "nombreTransaccion": "Compra Online"
                        }
                        response_tarjeta = requests.post(tarjeta_url, json=datos_tarjeta, verify=False)

                        if response_tarjeta.status_code == 201:
                            print("✅ Transacción registrada exitosamente")
                        else:
                            print(f"⚠️ Error al registrar la transacción: {response_tarjeta.status_code}")
                            print(f"🔍 Respuesta del servidor: {response_tarjeta.text}")

                    # 4️⃣ Registrar la venta después de confirmar la transacción
                    datos_venta = {
                        "codBoleta": 1,  # Fijo para pruebas
                        "codTransaccion": cod_transaccion,
                        "runCliente": "12345678-9",  # 🔴 DEBES CAMBIAR ESTO POR EL RUN CORRECTO
                        "detalleProductos": [
                            {
                                "codProducto": 1,  # 🔴 DEBES OBTENER ESTO DEL FORMULARIO O CARRITO
                                "codSucursal": 1,
                                "cantidad": 2,
                                "precioUnitario": 10000  # 🔴 DEBES OBTENER EL PRECIO REAL
                            }
                        ]
                    }

                    response_venta = requests.post(venta_url, json=datos_venta, verify=False)

                    if response_venta.status_code == 201:
                        print("✅ Venta registrada exitosamente")
                    else:
                        print(f"⚠️ Error al registrar la venta: {response_venta.status_code}")
                        print(f"🔍 Respuesta del servidor: {response_venta.text}")

                return render_template('transaccion_confirmada.html', detalles=detalles_transaccion)
            else:
                return jsonify({"error": data.get("mensaje", "Error al confirmar la transacción")}), 500
        else:
            return jsonify({"error": f"Error al confirmar la transacción: {response.status_code}"}), 500

    except Exception as e:
        return jsonify({"error": str(e)}), 500






if __name__ == '__main__':
    app.run(port=5001, debug=True)