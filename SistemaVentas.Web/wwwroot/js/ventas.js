(function (app) {
    $(document).ready(function () {
        $('#tabla-ventas').DataTable({
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
                        return new Date(data).toLocaleString();
                    }
                },
                { "data": "clienteNombre" },
                { "data": "usuarioNombre" },
                { "data": "total", "render": $.fn.dataTable.render.number(',', '.', 2, 'S/ ') },
                {
                    "data": "id",
                    "render": function (data) {
                        return `<button class="btn btn-sm btn-secondary btn-detalle" data-id="${data}">Ver Detalle</button>`;
                    },
                    "orderable": false
                }
            ],
            "language": { url: '//cdn.datatables.net/plug-ins/2.0.3/i18n/es-ES.json' }
        });
    });
})(window.app);
