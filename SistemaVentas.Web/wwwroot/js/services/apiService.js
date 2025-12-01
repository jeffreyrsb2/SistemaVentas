(function (app) {
    function getAuthHeaders() {
        const token = sessionStorage.getItem('jwtToken');
        const headers = {
            'Content-Type': 'application/json'
        };
        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }
        return headers;
    }

    async function request(endpoint, method = 'GET', body = null) {
        const url = `${app.config.API_BASE_URL}${endpoint}`;
        const options = {
            method: method,
            headers: getAuthHeaders()
        };

        if (body) {
            options.body = JSON.stringify(body);
        }

        const response = await fetch(url, options);

        if (response.status === 401) {
            sessionStorage.removeItem('jwtToken');
            window.location.href = '/login';
            throw new Error('Sesión inválida o expirada.');
        }

        if (!response.ok) {
            // Intenta obtener un mensaje de error más detallado del cuerpo de la respuesta
            const errorBody = await response.json().catch(() => ({ message: 'Error desconocido en el servidor.' }));
            throw new Error(errorBody.message || `Error HTTP ${response.status}`);
        }

        // Si el método no devuelve contenido (ej. 204 No Content), no se intenta parsear JSON
        if (response.status === 204) {
            return null;
        }

        return await response.json();
    }

    // Exponer los métodos del servicio API en el espacio de nombres app.services.api
    app.services.api = {
        get: (endpoint) => request(endpoint, 'GET'),
        post: (endpoint, body) => request(endpoint, 'POST', body),
        put: (endpoint, body) => request(endpoint, 'PUT', body),
        delete: (endpoint) => request(endpoint, 'DELETE')
    };

})(window.app);
