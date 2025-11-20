document.addEventListener('DOMContentLoaded', function () {
    const CARRITO_KEY = 'carrito_unishop';

    // Función para formatear números en CLP
    function formatearCLP(numero) {
        return new Intl.NumberFormat('es-CL', {
            style: 'currency',
            currency: 'CLP',
            minimumFractionDigits: 0
        }).format(numero);
    }

    const detalleCompra = document.getElementById('detalleCompra');
    const montoPagarInput = document.getElementById('montoPagar');
    const carritoInput = document.getElementById('carrito_json');

    const carrito = JSON.parse(localStorage.getItem(CARRITO_KEY) || '[]');

    detalleCompra.innerHTML = '';

    if (!carrito || carrito.length === 0) {
        detalleCompra.textContent = 'No hay productos en el carrito.';
        if (montoPagarInput) montoPagarInput.value = '0';
        if (carritoInput) carritoInput.value = '[]';
        return;
    }

    let totalGeneral = 0;

    carrito.forEach(item => {
        const totalProducto = item.cantidad * item.precio;
        totalGeneral += totalProducto;

        const productoElemento = document.createElement('p');
        productoElemento.textContent =
            `Producto: ${item.nombre}, Cantidad: ${item.cantidad}, ` +
            `Precio: ${formatearCLP(item.precio)}, Total: ${formatearCLP(totalProducto)} ` +
            `(Sucursal: ${item.codSucursal})`;
        detalleCompra.appendChild(productoElemento);
    });

    const totalElemento = document.createElement('p');
    totalElemento.className = 'fw-bold mt-3';
    totalElemento.textContent = `Total General: ${formatearCLP(totalGeneral)}`;
    detalleCompra.appendChild(totalElemento);

    // Inputs ocultos para enviar al backend
    if (montoPagarInput) {
        montoPagarInput.value = totalGeneral;
    }
    if (carritoInput) {
        carritoInput.value = JSON.stringify(carrito);
    }
});
