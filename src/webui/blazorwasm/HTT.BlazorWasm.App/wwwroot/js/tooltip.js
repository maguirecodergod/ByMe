window.tooltipHelper = {
    globalTooltip: null,

    init: function () {
        if (!this.globalTooltip) {
            this.globalTooltip = document.createElement('div');
            this.globalTooltip.className = 'vip-tooltip-portal';
            this.globalTooltip.innerHTML = '<div class="tooltip-content"></div><div class="tooltip-arrow"></div>';
            document.body.appendChild(this.globalTooltip);
        }
    },

    show: function (target, text) {
        this.init();
        const content = this.globalTooltip.querySelector('.tooltip-content');
        content.textContent = text;
        
        // Temporarily show to calculate measures
        this.globalTooltip.style.opacity = '0';
        this.globalTooltip.classList.add('is-visible');
        
        const rect = target.getBoundingClientRect();
        const tooltipRect = this.globalTooltip.getBoundingClientRect();
        const viewportWidth = window.innerWidth;
        const viewportHeight = window.innerHeight;
        const offset = 8;
        const padding = 10;
        
        // Initial placement logic (Priority: Top > Bottom > Right > Left)
        let pos = "top";
        let top = rect.top - offset;
        let left = rect.left + (rect.width / 2);

        // Vertical adjustment
        if (rect.top < tooltipRect.height + offset + padding) {
            pos = "bottom";
            top = rect.bottom + offset;
        }

        // Horizontal adjustment (Keep within viewport)
        const halfWidth = tooltipRect.width / 2;
        
        if (left - halfWidth < padding) {
            // Too far left
            left = padding + halfWidth;
        } else if (left + halfWidth > viewportWidth - padding) {
            // Too far right
            left = viewportWidth - padding - halfWidth;
        }

        // Apply
        this.globalTooltip.setAttribute('data-position', pos);
        this.globalTooltip.style.top = top + 'px';
        this.globalTooltip.style.left = left + 'px';
        this.globalTooltip.style.transform = (pos === 'top') ? "translate(-50%, -100%)" : "translate(-50%, 0)";
        this.globalTooltip.style.opacity = '1';

        // Fix arrow position if we shifted horizontally significantly
        const arrow = this.globalTooltip.querySelector('.tooltip-arrow');
        const shiftX = (rect.left + rect.width / 2) - left;
        arrow.style.left = `calc(50% + ${shiftX}px)`;
    },

    hide: function () {
        if (this.globalTooltip) {
            this.globalTooltip.classList.remove('is-visible');
            const arrow = this.globalTooltip.querySelector('.tooltip-arrow');
            if (arrow) arrow.style.left = '50%';
        }
    }
};
