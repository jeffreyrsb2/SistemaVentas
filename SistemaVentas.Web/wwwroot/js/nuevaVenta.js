(function (app) {
    let productosDisponibles = [];
    let detalleVenta = []; // Array para guardar los items del carrito

    async function inicializar() {
        // Cargar clientes y productos en los dropdowns
        const clientes = await app.services.clientes.obtenerTodos();
        const selectCliente = $('#select-cliente');
        clientes.forEach(c => selectCliente.append(new Option(c.nombreCompleto, c.id)));

        productosDisponibles = await app.services.productos.obtenerTodos();
        const selectProducto = $('#select-producto');
        selectProducto.append(new Option("Seleccione un producto...", ""));
        productosDisponibles.forEach(p => selectProducto.append(new Option(`${p.nombre} (Stock: ${p.stock})`, p.id)));
    }

    function renderizarDetalle() {
        const tbody = $('#tabla-detalle-venta');
        tbody.empty();
        let total = 0;
        detalleVenta.forEach((item, index) => {
            const subtotal = item.cantidad * item.precioUnitario;
            total += subtotal;
            tbody.append(`
                <tr>
                    <td>${item.nombreProducto}</td>
                    <td>${item.cantidad}</td>
                    <td>S/ ${item.precioUnitario.toFixed(2)}</td>
                    <td>S/ ${subtotal.toFixed(2)}</td>
                    <td><button class="btn btn-sm btn-danger btn-quitar" data-index="${index}">Quitar</button></td>
                </tr>
            `);
        });
        $('#total-venta').text(`S/ ${total.toFixed(2)}`);
    }

    $('#select-producto').on('change', function () {
        const productoId = parseInt($(this).val());
        if (!productoId) return;

        const producto = productosDisponibles.find(p => p.id === productoId);
        if (producto) {
            const itemExistente = detalleVenta.find(item => item.productoId === productoId);
            if (itemExistente) {
                itemExistente.cantidad++;
            } else {
                detalleVenta.push({
                    productoId: producto.id,
                    nombreProducto: producto.nombre,
                    cantidad: 1,
                    precioUnitario: producto.precio
                });
            }
            renderizarDetalle();
        }
        $(this).val(""); // Reset dropdown
    });

    $('#tabla-detalle-venta').on('click', '.btn-quitar', function () {
        const index = $(this).data('index');
        detalleVenta.splice(index, 1);
        renderizarDetalle();
    });

    $('#btn-registrar-venta').on('click', async function () {
        if (detalleVenta.length === 0) {
            alert("Debe añadir al menos un producto a la venta.");
            return;
        }
        const ventaData = {
            clienteId: parseInt($('#select-cliente').val()),
            detalles: detalleVenta.map(item => ({
                productoId: item.productoId,
                cantidad: item.cantidad
            }))
        };

        try {
            await app.services.ventas.crear(ventaData);
            alert("Venta registrada exitosamente");
            window.location.href = '/Ventas';
        } catch (error) {
            alert(`Error al registrar la venta: ${error.message}`);
        }
    });

    $(document).ready(inicializar);

})(window.app);
