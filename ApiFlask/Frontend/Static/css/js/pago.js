document.addEventListener('DOMContentLoaded', function() {
    // Función para formatear números en miles de pesos chilenos (CLP)
    function formatearCLP(numero) {
        return new Intl.NumberFormat('es-CL', { style: 'currency', currency: 'CLP', minimumFractionDigits: 0 }).format(numero);
    }

    // Obtener datos del carrito desde localStorage
    const productosEnCarrito = JSON.parse(localStorage.getItem('productosEnCarrito')) || {};
    const detalleCompra = document.getElementById('detalleCompra');
    const productoIdInput = document.getElementById('productoId');
    const montoPagarInput = document.getElementById('montoPagar');

    // Mostrar productos en el detalle de compra y calcular el total
    let totalGeneral = 0;
    for (const productoId in productosEnCarrito) {
        const producto = productosEnCarrito[productoId];
        const totalProducto = producto.cantidad * producto.precio;
        totalGeneral += totalProducto;

        const productoElemento = document.createElement('p');
        productoElemento.textContent = `Producto: ${producto.nombre}, Cantidad: ${producto.cantidad}, Precio: ${formatearCLP(producto.precio)}`;
        detalleCompra.appendChild(productoElemento);
    }

    // Mostrar el total general
    const totalElemento = document.createElement('p');
    totalElemento.className = 'fw-bold mt-3';
    totalElemento.textContent = `Total General: ${formatearCLP(totalGeneral)}`;
    detalleCompra.appendChild(totalElemento);

    // Establecer el ID del producto y el monto a pagar en los campos ocultos
    if (Object.keys(productosEnCarrito).length > 0) {
        const primerProductoId = Object.keys(productosEnCarrito)[0];
        productoIdInput.value = primerProductoId;
        montoPagarInput.value = totalGeneral;
    }
});
