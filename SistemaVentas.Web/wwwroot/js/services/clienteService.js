(function (app) {
    if (!app.services.api) return;
    app.services.clientes = {
        obtenerTodos: () => app.services.api.get('/api/clientes')
    };
})(window.app);
