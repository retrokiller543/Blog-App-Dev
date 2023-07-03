function toggleDarkMode() {
    // Check if 'dark-mode' is currently enabled
    const darkModeEnabled = localStorage.getItem('dark-mode') === 'true';

    // Switch the main CSS file
    document.getElementById('main-css').href = darkModeEnabled ? '/css/Light-Mode/light-mode.css' : '/css/Dark-Mode/dark-mode.css';

    // Save the new preference in localStorage
    localStorage.setItem('dark-mode', !darkModeEnabled);

    // Change the icon depending on the mode
    const iconElement = document.getElementById('dark-mode-icon');
    iconElement.className = darkModeEnabled ? 'fa fa-moon-o' : 'fa fa-sun-o';
}

(function () {
    // Retrieve dark mode preference from localStorage
    const darkModeEnabled = localStorage.getItem('dark-mode') === 'true';

    // Set the main CSS file and icon depending on the preference
    document.getElementById('main-css').href = darkModeEnabled ? '/css/Dark-Mode/dark-mode.css' : '/css/Light-Mode/light-mode.css';
    const iconElement = document.getElementById('dark-mode-icon');
    iconElement.className = darkModeEnabled ? 'fa fa-sun-o' : 'fa fa-moon-o';
})();
