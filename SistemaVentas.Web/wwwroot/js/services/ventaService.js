(function (app) {
    // Verificamos que el servicio genérico de API exista
    if (!app.services || !app.services.api) {
        console.error("Error: apiService.js debe cargarse antes que ventaService.js");
        return;
    }

    const api = app.services.api;

    // "Adjuntamos" TODAS las funciones específicas de ventas al namespace global
    app.services.ventas = {
        obtenerTodas: () => api.get('/api/ventas'),
        obtenerPorId: (id) => api.get(`/api/ventas/${id}`),
        crear: (ventaData) => api.post('/api/ventas', ventaData),
        actualizar: (id, ventaData) => api.put(`/api/ventas/${id}`, ventaData),
        anular: (id) => api.delete(`/api/ventas/${id}`)
    };

})(window.app);
