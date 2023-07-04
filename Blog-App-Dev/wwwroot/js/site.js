var darkSwitch = document.getElementById('darkSwitch');

// Check for saved 'darkMode' in localStorage, change the switch and the theme on page load
document.addEventListener('DOMContentLoaded', (event) => {
    if (localStorage.getItem('darkMode') === 'dark') {
        darkSwitch.checked = true;
        document.documentElement.setAttribute('data-bs-theme', 'dark');
        toggleDarkMode(true);
    }
});

darkSwitch.addEventListener('change', function () {
    if (this.checked) {
        document.documentElement.setAttribute('data-bs-theme', 'dark');
        localStorage.setItem('darkMode', 'dark');
        toggleDarkMode(true);
    } else {
        document.documentElement.setAttribute('data-bs-theme', 'light');
        localStorage.setItem('darkMode', 'light');
        toggleDarkMode(false);
    }
});

function toggleDarkMode(isDarkMode) {
    const header = document.querySelector('header');
    const footer = document.querySelector('footer');
    const navLinks = document.querySelectorAll('.nav-link');
    const logout_btn = document.getElementById('logout-btn')
    const body = document.querySelector('body');
    const darkText = document.querySelectorAll('.text-dark')
    if (isDarkMode) {
        body.classList.remove('bg-light')
        body.classList.add('bg-dark')
        header.classList.remove('bg-light');
        footer.classList.remove('bg-light');
        header.classList.add('bg-dark');
        footer.classList.add('bg-dark');
        navLinks.forEach(link => link.classList.add('text-white'));
        logout_btn.classList.add('btn-light');
        logout_btn.classList.remove('btn-dark');
        darkText.forEach((text) => {
            text.classList.remove('text-dark')
            text.classList.add('text-muted')
        })
        //logout_btn.classList.remove('btn-outline-dark');
        //logout_btn.classList.add('btn-outline-light');
    } else {
        body.classList.remove('bg-dark')
        body.classList.add('bg-light')
        header.classList.remove('bg-dark');
        footer.classList.remove('bg-dark');
        header.classList.add('bg-light');
        footer.classList.add('bg-light');
        navLinks.forEach(link => link.classList.remove('text-white'));
        logout_btn.classList.remove('btn-light');
        logout_btn.classList.add('btn-dark');
        darkText.forEach((text) => {
            text.classList.remove('text-muted')
            text.classList.add('text-dark')
        })
        //logout_btn.classList.add('btn-outline-dark');
        //logout_btn.classList.remove('btn-outline-light');
    }
}
