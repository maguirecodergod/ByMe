window.iconPickerHelper = {
    addOutsideClickListener: function (elementId, dotNetHelper) {
        const handler = (event) => {
            const el = document.getElementById(elementId);
            if (el && !el.contains(event.target)) {
                dotNetHelper.invokeMethodAsync('ClosePicker');
            }
        };
        window.addEventListener('mousedown', handler);
        return {
            dispose: () => window.removeEventListener('mousedown', handler)
        };
    },
    // Trả về true nếu nên hiện lên trên
    shouldFlipUp: function (triggerId) {
        const trigger = document.getElementById(triggerId);
        if (!trigger) return false;
        
        const rect = trigger.getBoundingClientRect();
        const viewportHeight = window.innerHeight;
        const pickerHeight = 480; // Chiều cao cố định của picker
        
        const spaceBelow = viewportHeight - rect.bottom;
        const spaceAbove = rect.top;
        
        // Nếu không gian bên dưới < chiều cao picker VÀ không gian phía trên rộng hơn
        return spaceBelow < pickerHeight && spaceAbove > spaceBelow;
    },
    focusInput: function (element) {
        if (element) {
            setTimeout(() => element.focus(), 50);
        }
    }
};
