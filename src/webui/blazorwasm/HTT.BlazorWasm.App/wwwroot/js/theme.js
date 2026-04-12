window.themeHelper = {
    isDarkMode: function () {
        return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    },
    watchSystemTheme: function (dotNetHelper) {
        const query = window.matchMedia('(prefers-color-scheme: dark)');
        const handler = (e) => {
            dotNetHelper.invokeMethodAsync('OnSystemThemeChanged', e.matches);
        };
        query.addEventListener('change', handler);
    },
    setAttributes: function (theme, brand) {
        document.documentElement.setAttribute('data-theme', theme);
        if (brand) {
            document.documentElement.setAttribute('data-brand', brand);
            // Also apply a composite theme class for brand colors
            document.documentElement.className = `theme-${theme} brand-${brand}`;
        } else {
            document.documentElement.removeAttribute('data-brand');
            document.documentElement.className = `theme-${theme}`;
        }
    }
};
