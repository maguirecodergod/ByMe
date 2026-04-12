window.sidebarHelper = {
    dotNetRef: null,
    isResizing: false,

    init: function (dotNetRef) {
        this.dotNetRef = dotNetRef;
    },

    startResizing: function () {
        this.isResizing = true;
        document.body.classList.add('is-resizing-sidebar');
        
        const onMouseMove = (e) => {
            if (!this.isResizing) return;
            
            // Calculate new width based on mouse position
            // This is a simplified version
            const newWidth = e.clientX; 
            if (newWidth > 180 && newWidth < 600) {
                this.dotNetRef.invokeMethodAsync('SetSidebarWidth', newWidth);
            }
        };

        const onMouseUp = () => {
            this.isResizing = false;
            document.body.classList.remove('is-resizing-sidebar');
            window.removeEventListener('mousemove', onMouseMove);
            window.removeEventListener('mouseup', onMouseUp);
        };

        window.addEventListener('mousemove', onMouseMove);
        window.addEventListener('mouseup', onMouseUp);
    }
};
