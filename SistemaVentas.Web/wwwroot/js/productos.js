import { obtenerProductos } from './services/productoService.js';

$(document).ready(function () {
    $('#tabla-productos').DataTable({
        "ajax": function (data, callback, settings) {
            obtenerProductos()
                .then(productos => {
                    callback({
                        data: productos
                    });
                })
                .catch(error => {
                    alert('No se pudieron cargar los productos.');
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
                "render": function (data, type, row) {
                    // 'data' contiene el valor de "data", es decir, el 'id'.
                    // 'row' contiene el objeto completo del producto para esa fila.
                    return `<button class="btn btn-sm btn-info btn-editar" data-id="${data}">Editar</button>
                            <button class="btn btn-sm btn-danger btn-eliminar" data-id="${data}">Eliminar</button>`;
                },
                "orderable": false
            }
        ],
        "language": { url: '//cdn.datatables.net/plug-ins/2.0.3/i18n/es-ES.json' }
    });
});
