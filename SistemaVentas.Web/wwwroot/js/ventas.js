(function (app) {
    let tablaVentas;

    function recargarTabla() {
        if (tablaVentas) {
            tablaVentas.ajax.reload(null, false);
        }
    }

    $(document).ready(function () {
        if (!app.services || !app.services.ventas) {
            console.error("Error: el servicio de ventas no está cargado.");
            return;
        }

        tablaVentas = $('#tabla-ventas').DataTable({
            "ajax": function (data, callback, settings) {
                app.services.ventas.obtenerTodas()
                    .then(ventas => callback({ data: ventas }))
                    .catch(error => {
                        console.error("Error al cargar ventas:", error);
                        callback({ data: [] });
                    });
            },
            "columns": [
                { "data": "id" },
                {
                    "data": "fechaVenta",
                    "render": function (data) {
                        return new Date(data).toLocaleString('es-PE'); // Formato localizado
                    }
                },
                { "data": "clienteNombre" },
                { "data": "usuarioNombre" },
                { "data": "total", "render": $.fn.dataTable.render.number(',', '.', 2, 'S/ ') },
                {
                    "data": "id",
                    "render": function (data) {
                        // Añadimos la clase .btn-anular
                        return `<button class="btn btn-sm btn-info btn-detalle" data-id="${data}">Ver Detalle</button>
                                <button class="btn btn-sm btn-danger btn-anular" data-id="${data}">Anular</button>`;
                    },
                    "orderable": false
                }
            ],
            "language": { url: '//cdn.datatables.net/plug-ins/2.0.3/i18n/es-ES.json' }
        });

        // --- MANEJO DE EVENTOS ---

        // Evento para "Ver Detalle"
        $('#tabla-ventas tbody').on('click', '.btn-detalle', async function () {
            const id = $(this).data('id');
            $('#detalleVentaModalLabel').text(`Detalle de Venta #${id}`);
            $('#detalle-venta-info').html('<p>Cargando...</p>');
            $('#tabla-productos-vendidos').empty();

            try {
                const venta = await app.services.ventas.obtenerPorId(id);
                const infoHtml = `
                    <p><strong>Cliente:</strong> ${venta.clienteNombre}</p>
                    <p><strong>Vendedor:</strong> ${venta.usuarioNombre}</p>
                    <p><strong>Fecha:</strong> ${new Date(venta.fechaVenta).toLocaleString('es-PE')}</p>
                `;
                $('#detalle-venta-info').html(infoHtml);

                let detallesHtml = '';
                venta.detalles.forEach(d => {
                    detallesHtml += `
                        <tr>
                            <td>${d.productoNombre}</td>
                            <td>${d.cantidad}</td>
                            <td>S/ ${d.precioUnitario.toFixed(2)}</td>
                            <td>S/ ${(d.cantidad * d.precioUnitario).toFixed(2)}</td>
                        </tr>
                    `;
                });
                $('#tabla-productos-vendidos').html(detallesHtml);

                $('#detalleVentaModal').modal('show');
            } catch (error) {
                alert('No se pudo cargar el detalle de la venta.');
            }
        });

        // Evento para "Anular"
        $('#tabla-ventas tbody').on('click', '.btn-anular', async function () {
            const id = $(this).data('id');
            if (confirm(`¿Estás seguro de que deseas anular la venta #${id}? Esta acción devolverá el stock de los productos.`)) {
                try {
                    await app.services.ventas.anular(id);
                    recargarTabla();
                    alert(`Venta #${id} anulada exitosamente.`);
                } catch (error) {
                    alert(`Error al anular la venta: ${error.message}`);
                }
            }
        });
    });
})(window.app);
