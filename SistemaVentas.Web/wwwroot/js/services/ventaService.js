(function (app) {
    if (!app.services || !app.services.api) {
        console.error("apiService.js debe cargarse primero.");
        return;
    }
    const api = app.services.api;

    app.services.ventas = {
        obtenerTodas: () => api.get('/api/ventas'),
        obtenerPorId: (id) => api.get(`/api/ventas/${id}`),
        crear: (ventaData) => api.post('/api/ventas', ventaData),
        anular: (id) => api.delete(`/api/ventas/${id}`)
    };
})(window.app);
