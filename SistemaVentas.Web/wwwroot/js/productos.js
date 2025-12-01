(function (app) {
    $(document).ready(function () {
        // Verificamos que el servicio exista antes de intentar usarlo
        if (app.services && app.services.productos) {
            $('#tabla-productos').DataTable({
                "ajax": function (data, callback, settings) {
                    app.services.productos.obtenerTodos()
                        .then(productos => {
                            callback({ data: productos });
                        })
                        .catch(error => {
                            console.error("Error al cargar datos en DataTable:", error);
                            // Opcional: mostrar un error en la tabla
                            callback({ data: [] });
                            alert("No se pudieron cargar los productos. La sesión puede haber expirado.");
                        });
                },
                "columns": [
                    { "data": "id" },
                    { "data": "nombre" },
                    { "data": "descripcion" },
                    { "data": "precio" },
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
        }
    });
})(window.app);
