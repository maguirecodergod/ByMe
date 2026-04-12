/* ============================================
   LHAFurniture — Premium Landing Page
   JavaScript: Interactions & Animations
   ============================================ */

(function () {
  'use strict';

  // ---- DOM Ready ----
  document.addEventListener('DOMContentLoaded', init);

  function init() {
    initNavbar();
    initSmoothScroll();
    initScrollReveal();
    initCounterAnimation();
    initMobileMenu();
    initParallaxHero();
  }

  /* ============================================
     1. STICKY NAVBAR WITH BLUR
     ============================================ */
  function initNavbar() {
    const navbar = document.getElementById('navbar');
    if (!navbar) return;

    let lastScrollY = 0;
    let ticking = false;

    function onScroll() {
      lastScrollY = window.scrollY;
      if (!ticking) {
        window.requestAnimationFrame(() => {
          if (lastScrollY > 40) {
            navbar.classList.add('scrolled');
          } else {
            navbar.classList.remove('scrolled');
          }
          ticking = false;
        });
        ticking = true;
      }
    }

    window.addEventListener('scroll', onScroll, { passive: true });
    // Trigger once on load
    onScroll();
  }

  /* ============================================
     2. SMOOTH SCROLL FOR ANCHOR LINKS
     ============================================ */
  function initSmoothScroll() {
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
      anchor.addEventListener('click', function (e) {
        const targetId = this.getAttribute('href');
        if (targetId === '#') return;

        const target = document.querySelector(targetId);
        if (!target) return;

        e.preventDefault();

        // Close mobile menu if open
        const mobileMenu = document.getElementById('nav-links');
        if (mobileMenu && mobileMenu.classList.contains('mobile-open')) {
          mobileMenu.classList.remove('mobile-open');
          document.getElementById('mobile-toggle')?.classList.remove('active');
        }

        const navbarHeight = document.getElementById('navbar')?.offsetHeight || 72;
        const targetPosition = target.getBoundingClientRect().top + window.scrollY - navbarHeight - 16;

        window.scrollTo({
          top: targetPosition,
          behavior: 'smooth'
        });
      });
    });
  }

  /* ============================================
     3. SCROLL REVEAL ANIMATIONS
     ============================================ */
  function initScrollReveal() {
    const reveals = document.querySelectorAll('.reveal');
    if (!reveals.length) return;

    const observerOptions = {
      root: null,
      rootMargin: '0px 0px -60px 0px',
      threshold: 0.12
    };

    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('visible');
          observer.unobserve(entry.target);
        }
      });
    }, observerOptions);

    reveals.forEach(el => observer.observe(el));
  }

  /* ============================================
     4. COUNTER ANIMATION FOR TRUST NUMBERS
     ============================================ */
  function initCounterAnimation() {
    const counters = document.querySelectorAll('[data-count]');
    if (!counters.length) return;

    let animated = false;

    const observerOptions = {
      root: null,
      rootMargin: '0px',
      threshold: 0.5
    };

    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting && !animated) {
          animated = true;
          animateCounters(counters);
          observer.disconnect();
        }
      });
    }, observerOptions);

    // Observe the CTA section
    const ctaSection = document.getElementById('cta');
    if (ctaSection) {
      observer.observe(ctaSection);
    }
  }

  function animateCounters(counters) {
    counters.forEach(counter => {
      const target = parseInt(counter.getAttribute('data-count'), 10);
      const suffix = counter.getAttribute('data-suffix') || '';
      const duration = 2000; // ms
      const startTime = performance.now();

      function easeOutCubic(t) {
        return 1 - Math.pow(1 - t, 3);
      }

      function update(currentTime) {
        const elapsed = currentTime - startTime;
        const progress = Math.min(elapsed / duration, 1);
        const easedProgress = easeOutCubic(progress);
        const current = Math.round(easedProgress * target);

        counter.textContent = formatNumber(current) + suffix;

        if (progress < 1) {
          requestAnimationFrame(update);
        }
      }

      requestAnimationFrame(update);
    });
  }

  function formatNumber(num) {
    if (num >= 1000) {
      return num.toLocaleString('vi-VN');
    }
    return num.toString();
  }

  /* ============================================
     5. MOBILE MENU
     ============================================ */
  function initMobileMenu() {
    const toggle = document.getElementById('mobile-toggle');
    const navLinks = document.getElementById('nav-links');
    if (!toggle || !navLinks) return;

    // Add mobile styles dynamically
    const style = document.createElement('style');
    style.textContent = `
      @media (max-width: 768px) {
        .navbar__links.mobile-open {
          display: flex !important;
          flex-direction: column;
          position: fixed;
          top: 0;
          left: 0;
          right: 0;
          bottom: 0;
          background: rgba(244, 247, 242, 0.97);
          backdrop-filter: blur(24px);
          -webkit-backdrop-filter: blur(24px);
          z-index: 999;
          align-items: center;
          justify-content: center;
          gap: 32px;
          padding: 80px 40px;
          animation: fadeInMenu 0.35s ease;
        }
        .navbar__links.mobile-open a {
          font-size: 1.25rem;
          font-weight: 500;
          color: #1F2937;
        }
        .navbar__mobile-toggle {
          z-index: 1001;
          position: relative;
        }
        .navbar__mobile-toggle.active span:nth-child(1) {
          transform: translateY(7px) rotate(45deg);
        }
        .navbar__mobile-toggle.active span:nth-child(2) {
          opacity: 0;
        }
        .navbar__mobile-toggle.active span:nth-child(3) {
          transform: translateY(-7px) rotate(-45deg);
        }
        @keyframes fadeInMenu {
          from { opacity: 0; }
          to { opacity: 1; }
        }
      }
    `;
    document.head.appendChild(style);

    toggle.addEventListener('click', () => {
      navLinks.classList.toggle('mobile-open');
      toggle.classList.toggle('active');
    });
  }

  /* ============================================
     6. SUBTLE PARALLAX ON HERO
     ============================================ */
  function initParallaxHero() {
    const heroBg = document.querySelector('.hero__bg img');
    const heroCard = document.querySelector('.hero__featured-card');
    if (!heroBg) return;

    // Only on desktop
    if (window.innerWidth < 1024) return;

    let ticking = false;

    window.addEventListener('scroll', () => {
      if (!ticking) {
        window.requestAnimationFrame(() => {
          const scrollY = window.scrollY;
          const heroHeight = document.querySelector('.hero')?.offsetHeight || 800;

          if (scrollY < heroHeight) {
            const parallaxOffset = scrollY * 0.15;
            heroBg.style.transform = `translateY(${parallaxOffset}px) scale(1.05)`;

            if (heroCard) {
              const cardOffset = scrollY * -0.08;
              heroCard.style.transform = `translateY(${cardOffset}px)`;
            }
          }
          ticking = false;
        });
        ticking = true;
      }
    }, { passive: true });
  }

})();
