(function (app) {
    function getAuthHeaders() {
        const token = sessionStorage.getItem('jwtToken');
        if (!token) {
            // Si no hay token, redirigimos a login, la única excepción es la propia página de login
            if (!window.location.pathname.toLowerCase().includes('/login')) {
                window.location.href = '/login';
            }
            return { 'Content-Type': 'application/json' };
        }
        return {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        };
    }

    async function obtenerProductos() {
        const response = await fetch(`${app.config.API_BASE_URL}/api/productos`, { headers: getAuthHeaders() });
        if (response.status === 401) {
            sessionStorage.removeItem('jwtToken');
            window.location.href = '/login';
            throw new Error('Sesión inválida o expirada.');
        }
        if (!response.ok) throw new Error('Error al obtener productos');
        return await response.json();
    }

    // "Adjuntamos" la función al namespace global de servicios
    app.services.productos = {
        obtenerTodos: obtenerProductos
    };

})(window.app);
