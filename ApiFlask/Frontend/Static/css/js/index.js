document.addEventListener('DOMContentLoaded', function () {
    const CARRITO_KEY = 'carrito_unishop';

    // Limpieza de la versión antigua (por si quedó algo)
    localStorage.removeItem('productosEnCarrito');

    let carrito = JSON.parse(localStorage.getItem(CARRITO_KEY) || '[]');

    // Función para formatear números en CLP
    function formatearCLP(numero) {
        return new Intl.NumberFormat('es-CL', {
            style: 'currency',
            currency: 'CLP',
            minimumFractionDigits: 0
        }).format(numero);
    }

    function guardarCarrito() {
        localStorage.setItem(CARRITO_KEY, JSON.stringify(carrito));
    }

    function actualizarContadorCarrito() {
        const contadorCarrito = document.getElementById('contador-carrito');
        const cantidadTotal = carrito.reduce((acc, item) => acc + item.cantidad, 0);
        if (contadorCarrito) {
            contadorCarrito.textContent = cantidadTotal;
        }
    }

    function mostrarProductosEnCarrito() {
        const modalBody = document.querySelector('#carritoModal .modal-body');
        if (!modalBody) return;

        modalBody.innerHTML = '';

        if (carrito.length === 0) {
            modalBody.textContent = 'No hay productos en el carrito.';
            return;
        }

        let totalGeneral = 0;

        carrito.forEach(item => {
            const totalProducto = item.cantidad * item.precio;
            totalGeneral += totalProducto;

            const productoElemento = document.createElement('p');
            productoElemento.textContent =
                `Producto: ${item.nombre} | Cantidad: ${item.cantidad} | ` +
                `Precio: ${formatearCLP(item.precio)} | Total: ${formatearCLP(totalProducto)} ` +
                `(Sucursal: ${item.codSucursal})`;
            modalBody.appendChild(productoElemento);
        });

        const totalElemento = document.createElement('p');
        totalElemento.className = 'fw-bold mt-3';
        totalElemento.textContent = `Total General: ${formatearCLP(totalGeneral)}`;
        modalBody.appendChild(totalElemento);
    }

    // Inicializar listeners para botones "Añadir al carrito"
    const botonesAgregar = document.querySelectorAll('.btn-agregar');

    botonesAgregar.forEach(boton => {
        boton.addEventListener('click', function (e) {
            e.preventDefault();

            const cardBody = this.closest('.card-body');
            if (!cardBody) return;

            const productoId = parseInt(this.dataset.productoId);
            const precio = parseInt(this.dataset.precio);
            const nombre = cardBody.querySelector('.card-title').textContent;

            const inputCantidad = cardBody.querySelector('.cantidad');
            const selectSucursal = cardBody.querySelector('.codSucursal');

            const cantidad = parseInt(inputCantidad ? inputCantidad.value : '1') || 1;
            const codSucursal = parseInt(selectSucursal ? selectSucursal.value : '1') || 1;

            // Buscar si ya existe mismo producto + sucursal en el carrito
            const existente = carrito.find(item =>
                item.codProducto === productoId && item.codSucursal === codSucursal
            );

            if (existente) {
                existente.cantidad += cantidad;
            } else {
                carrito.push({
                    codProducto: productoId,
                    nombre: nombre,
                    precio: precio,
                    cantidad: cantidad,
                    codSucursal: codSucursal
                });
            }

            guardarCarrito();
            actualizarContadorCarrito();
            mostrarProductosEnCarrito();

            alert('Producto agregado al carrito');
        });
    });

    // Botón "Vaciar carrito"
    const btnVaciarCarrito = document.getElementById('btnVaciarCarrito');
    if (btnVaciarCarrito) {
        btnVaciarCarrito.addEventListener('click', function () {
            carrito = [];
            guardarCarrito();
            actualizarContadorCarrito();
            mostrarProductosEnCarrito();
        });
    }

    // Mostrar al cargar
    mostrarProductosEnCarrito();
    actualizarContadorCarrito();
});
