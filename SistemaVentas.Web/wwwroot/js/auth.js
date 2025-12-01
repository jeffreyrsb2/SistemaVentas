(function (app) {
    const token = sessionStorage.getItem('jwtToken');
    const isLoginPage = window.location.pathname.toLowerCase().includes('/login');

    if (!token && !isLoginPage) {
        window.location.href = '/login';
    }
})(window.app);
