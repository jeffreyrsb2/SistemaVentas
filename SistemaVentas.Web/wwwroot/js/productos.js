(function (app) {
    let tablaProductos;

    // Función para recargar la tabla
    function recargarTabla() {
        tablaProductos.ajax.reload(null, false); // 'false' para mantener la paginación actual
    }

    // Función para limpiar el formulario de la modal
    function limpiarModal() {
        $('#form-producto').trigger("reset");
        $('#productoId').val(''); // Nos aseguramos de limpiar el ID oculto
        $('#productoModalLabel').text('Nuevo Producto');
    }

    $(document).ready(function () {
        if (!app.services || !app.services.productos) {
            console.error("Error crítico: los servicios no están cargados.");
            return;
        }

        tablaProductos = $('#tabla-productos').DataTable({
            "ajax": function (data, callback, settings) {
                app.services.productos.obtenerTodos()
                    .then(productos => callback({ data: productos }))
                    .catch(error => {
                        console.error("Error al cargar datos en DataTable:", error);
                        callback({ data: [] });
                        alert("No se pudieron cargar los productos.");
                    });
            },
            "columns": [
                { "data": "id" },
                { "data": "nombre" },
                { "data": "descripcion" },
                { "data": "precio", "render": $.fn.dataTable.render.number(',', '.', 2, 'S/ ') },
                { "data": "stock" },
                {
                    "data": "id",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-info btn-editar" data-id="${data}">Editar</button>
                                <button class="btn btn-sm btn-danger btn-eliminar" data-id="${data}">Eliminar</button>`;
                    },
                    "orderable": false
                }
            ],
            "language": { url: '//cdn.datatables.net/plug-ins/2.0.3/i18n/es-ES.json' }
        });

        // --- MANEJO DE EVENTOS ---

        // Botón "Nuevo Producto"
        $('#btn-nuevo').on('click', function () {
            limpiarModal();
            $('#productoModal').modal('show');
        });

        // Botones "Editar" en la tabla (delegación de eventos)
        $('#tabla-productos tbody').on('click', '.btn-editar', async function () {
            const id = $(this).data('id');
            limpiarModal();
            $('#productoModalLabel').text('Editar Producto');

            try {
                const producto = await app.services.productos.obtenerPorId(id);
                $('#productoId').val(producto.id);
                $('#nombre').val(producto.nombre);
                $('#descripcion').val(producto.descripcion);
                $('#precio').val(producto.precio);
                $('#stock').val(producto.stock);
                $('#productoModal').modal('show');
            } catch (error) {
                alert('No se pudo cargar la información del producto.');
            }
        });

        // Botón "Guardar" en la modal
        $('#btn-guardar').on('click', async function () {
            const id = $('#productoId').val();
            const esEdicion = id !== '';

            const productoData = {
                nombre: $('#nombre').val(),
                descripcion: $('#descripcion').val(),
                precio: parseFloat($('#precio').val()),
                stock: parseInt($('#stock').val())
            };

            // TODO: Añadir validación del formulario

            try {
                if (esEdicion) {
                    await app.services.productos.actualizar(id, productoData);
                } else {
                    await app.services.productos.crear(productoData);
                }
                $('#productoModal').modal('hide');
                recargarTabla();
            } catch (error) {
                alert(`Error al guardar el producto: ${error.message}`);
            }
        });

        // Botones "Eliminar" en la tabla (delegación de eventos)
        $('#tabla-productos tbody').on('click', '.btn-eliminar', async function () {
            const id = $(this).data('id');
            if (confirm('¿Estás seguro de que deseas eliminar este producto?')) {
                try {
                    await app.services.productos.eliminar(id);
                    recargarTabla();
                } catch (error) {
                    alert(`Error al eliminar el producto: ${error.message}`);
                }
            }
        });
    });
})(window.app);
