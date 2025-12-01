(function (app) {
    if (!app.services || !app.services.api) {
        console.error("apiService.js debe cargarse primero.");
        return;
    }
    const api = app.services.api;

    app.services.ventas = {
        obtenerTodas: () => api.get('/api/ventas'),
        crear: (ventaData) => api.post('/api/ventas', ventaData)
        // TODO: Aquí irían los demás métodos del CRUD de ventas
    };
})(window.app);
