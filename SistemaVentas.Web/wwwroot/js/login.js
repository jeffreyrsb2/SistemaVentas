(function (app) {
    const form = document.getElementById('form-login');
    if (!form) return;

    form.addEventListener('submit', async function (e) {
        e.preventDefault();
        const nombreUsuario = document.getElementById('usuario').value;
        const password = document.getElementById('password').value;

        try {
            const response = await fetch(`${app.config.API_BASE_URL}/api/auth/login`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ nombreUsuario, password })
            });

            if (!response.ok) throw new Error('Credenciales incorrectas');

            const data = await response.json();
            sessionStorage.setItem('jwtToken', data.token);
            window.location.href = '/';
        } catch (error) {
            alert(error.message);
        }
    });
})(window.app);
