const API_BASE_URL = 'https://localhost:7276';

export async function obtenerProductos() {
    try {
        const response = await fetch(`${API_BASE_URL}/api/productos`);
        if (!response.ok) {
            throw new Error('Error de red al obtener los productos.');
        }
        return await response.json();
    } catch (error) {
        console.error('Error en obtenerProductos:', error);
        throw error;
    }
}
