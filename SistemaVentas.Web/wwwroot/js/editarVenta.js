(function (app) {
    let productosDisponibles = [];
    let detalleVenta = [];
    let ventaId = null;

    async function inicializar() {
        // Obtenemos el ID de la venta desde la URL
        const params = new URLSearchParams(window.location.search);
        ventaId = parseInt(params.get('id'));
        if (!ventaId) {
            alert('ID de venta no válido.');
            window.location.href = '/Ventas';
            return;
        }

        // Cargamos todos los datos necesarios en paralelo
        const [venta, clientes, productos] = await Promise.all([
            app.services.ventas.obtenerPorId(ventaId),
            app.services.clientes.obtenerTodos(),
            app.services.productos.obtenerTodos()
        ]);

        productosDisponibles = productos;

        // Llenar dropdown de clientes y seleccionar el correcto
        const selectCliente = $('#select-cliente');
        clientes.forEach(c => selectCliente.append(new Option(c.nombreCompleto, c.id)));
        selectCliente.val(venta.clienteId);

        // Llenar dropdown de productos
        const selectProducto = $('#select-producto');
        selectProducto.append(new Option("Seleccione un producto...", ""));
        productosDisponibles.forEach(p => selectProducto.append(new Option(`${p.nombre} (Stock: ${p.stock})`, p.id)));

        // Llenar el detalle de la venta con los datos existentes
        detalleVenta = venta.detalles.map(d => {
            const producto = productosDisponibles.find(p => p.id === d.productoId);
            return {
                productoId: d.productoId,
                nombreProducto: d.productoNombre,
                cantidad: d.cantidad,
                precioUnitario: d.precioUnitario,
                stock: producto ? producto.stock + d.cantidad : d.cantidad // Stock actual + lo que se vendió
            };
        });

        renderizarDetalle();
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
                    <td>${item.nombreProducto} (Stock Disp: ${item.stock})</td>
                    <td><input type="number" class="form-control form-control-sm item-cantidad" value="${item.cantidad}" min="1" max="${item.stock}" data-index="${index}"></td>
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
                if (itemExistente.cantidad < producto.stock) {
                    itemExistente.cantidad++;
                } else {
                    alert('No hay más stock disponible para este producto.');
                }
            } else {
                detalleVenta.push({
                    productoId: producto.id,
                    nombreProducto: producto.nombre,
                    cantidad: 1,
                    precioUnitario: producto.precio,
                    stock: producto.stock
                });
            }
            renderizarDetalle();
        }
        $(this).val("");
    });

    $('#tabla-detalle-venta').on('change', '.item-cantidad', function () {
        const index = $(this).data('index');
        let nuevaCantidad = parseInt($(this).val());
        const item = detalleVenta[index];

        if (nuevaCantidad > item.stock) {
            alert(`Stock máximo para ${item.nombreProducto} es ${item.stock}.`);
            nuevaCantidad = item.stock;
            $(this).val(nuevaCantidad);
        }
        if (nuevaCantidad < 1) {
            nuevaCantidad = 1;
            $(this).val(nuevaCantidad);
        }

        item.cantidad = nuevaCantidad;
        renderizarDetalle();
    });

    $('#tabla-detalle-venta').on('click', '.btn-quitar', function () {
        const index = $(this).data('index');
        detalleVenta.splice(index, 1);
        renderizarDetalle();
    });

    $('#btn-registrar-venta').on('click', async function () {
        if (detalleVenta.length === 0) {
            alert("Debe haber al menos un producto.");
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
            await app.services.ventas.actualizar(ventaId, ventaData);
            alert("Venta actualizada exitosamente");
            window.location.href = '/Ventas';
        } catch (error) {
            alert(`Error al actualizar la venta: ${error.message}`);
        }
    });

    $(document).ready(inicializar);

})(window.app);
