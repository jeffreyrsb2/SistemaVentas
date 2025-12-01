(function (app) {
    // Verificamos que el servicio genérico de API exista
    if (!app.services || !app.services.api) {
        console.error("Error: apiService.js debe cargarse antes que productoService.js");
        return;
    }

    const api = app.services.api;

    // "Adjuntamos" las funciones específicas de productos
    app.services.productos = {
        obtenerTodos: () => api.get('/api/productos'),
        obtenerPorId: (id) => api.get(`/api/productos/${id}`),
        crear: (productoData) => api.post('/api/productos', productoData),
        actualizar: (id, productoData) => api.put(`/api/productos/${id}`, productoData),
        eliminar: (id) => api.delete(`/api/productos/${id}`)
    };

})(window.app);
