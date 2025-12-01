(function (app) {
    // Variable global dentro de este script para acceder a la instancia de DataTable
    let tablaProductos;

    // Función para recargar la tabla de forma asíncrona
    function recargarTabla() {
        if (tablaProductos) {
            tablaProductos.ajax.reload(null, false); // 'false' para que no resetee la paginación
        }
    }

    // Función para limpiar el formulario de la modal y resetear su estado
    function limpiarModal() {
        $('#form-producto').trigger("reset"); // Limpia todos los inputs del formulario
        $('#productoId').val(''); // Se asegura de que el ID oculto esté vacío
        $('#productoModalLabel').text('Nuevo Producto'); // Resetea el título
    }

    // El código se ejecuta cuando el DOM está completamente cargado
    $(document).ready(function () {
        // Verificación de seguridad: si los servicios no se cargaron, no hace nada.
        if (!app.services || !app.services.productos) {
            console.error("Error crítico: el servicio de productos no está cargado. Asegúrate de que los scripts estén en el orden correcto en _Layout.cshtml.");
            return;
        }

        // Inicialización de la DataTable
        tablaProductos = $('#tabla-productos').DataTable({
            "ajax": function (data, callback, settings) {
                app.services.productos.obtenerTodos()
                    .then(productos => {
                        callback({ data: productos });
                    })
                    .catch(error => {
                        console.error("Error al cargar datos en DataTable:", error);
                        alert("No se pudieron cargar los productos. La sesión puede haber expirado.");
                        callback({ data: [] }); // Devuelve data vacía para que la tabla no se rompa
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

        // Evento para el botón principal "Nuevo Producto"
        $('#btn-nuevo').on('click', function () {
            limpiarModal();
            $('#productoModal').modal('show');
        });

        // Evento para los botones "Editar" (usando delegación de eventos en el tbody)
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

        // Evento para el botón "Guardar" dentro de la modal
        $('#btn-guardar').on('click', async function () {
            const id = $('#productoId').val();
            const esEdicion = id !== '';

            const productoData = {
                nombre: $('#nombre').val(),
                descripcion: $('#descripcion').val(),
                precio: parseFloat($('#precio').val()),
                stock: parseInt($('#stock').val())
            };

            // Aquí iría una validación más robusta del formulario si tuviéramos tiempo
            if (!productoData.nombre || productoData.precio <= 0 || productoData.stock < 0) {
                alert("Por favor, complete los campos correctamente.");
                return;
            }

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

        // Evento para los botones "Eliminar" (usando delegación de eventos)
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
