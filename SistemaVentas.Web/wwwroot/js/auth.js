(function (app) {
    const token = sessionStorage.getItem('jwtToken');
    const isLoginPage = window.location.pathname.toLowerCase().includes('/login');

    if (!token && !isLoginPage) {
        window.location.href = '/login';
    }

    const btnLogout = document.getElementById('btn-logout');
    if (btnLogout) {
        btnLogout.addEventListener('click', function (e) {
            e.preventDefault();
            sessionStorage.removeItem('jwtToken');
            window.location.href = '/login';
        });
    }

})(window.app);
