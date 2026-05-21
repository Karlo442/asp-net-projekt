// Celebration Animation Module
// Prikazuje fancy animaciju kada se uspješno kreira korisnik ili naruđba

const CelebrationAnimation = (function() {
    
    // Konfiguracija boja za confetti
    const confettiColors = [
        '#FF6B6B', // Crvena
        '#4ECDC4', // Tirkizna
        '#45B7D1', // Plava
        '#FFA07A', // Koraljnacrvena
        '#98D8C8', // Mint
        '#F7DC6F', // Žuta
        '#BB8FCE', // Ljubičasta
        '#85C1E2'  // Svetloplava
    ];

    // Kreiramo confetti čestice
    function createConfetti() {
        const confetti = [];
        const confettiCount = 50;

        for (let i = 0; i < confettiCount; i++) {
            const isRibbon = Math.random() > 0.7;
            confetti.push({
                x: Math.random() * window.innerWidth,
                y: -10,
                size: Math.random() * 10 + 5,
                speedX: (Math.random() - 0.5) * 8,
                speedY: Math.random() * 5 + 5,
                rotation: Math.random() * 360,
                rotationSpeed: (Math.random() - 0.5) * 20,
                color: confettiColors[Math.floor(Math.random() * confettiColors.length)],
                opacity: 1,
                isRibbon: isRibbon
            });
        }

        return confetti;
    }

    // Animiraj confetti
    function animateConfetti(canvas, ctx, confetti) {
        const animationDuration = 3000; // 3 sekunde
        const startTime = Date.now();

        function frame() {
            const elapsed = Date.now() - startTime;
            const progress = elapsed / animationDuration;

            ctx.clearRect(0, 0, canvas.width, canvas.height);

            confetti.forEach((particle, index) => {
                particle.y += particle.speedY;
                particle.x += particle.speedX;
                particle.rotation += particle.rotationSpeed;
                particle.opacity = Math.max(0, 1 - progress);

                ctx.save();
                ctx.globalAlpha = particle.opacity;
                ctx.fillStyle = particle.color;
                ctx.translate(particle.x, particle.y);
                ctx.rotate((particle.rotation * Math.PI) / 180);

                if (particle.isRibbon) {
                    ctx.fillRect(-particle.size / 2, -particle.size / 4, particle.size, particle.size / 2);
                } else {
                    ctx.beginPath();
                    ctx.arc(0, 0, particle.size / 2, 0, Math.PI * 2);
                    ctx.fill();
                }

                ctx.restore();
            });

            if (progress < 1) {
                requestAnimationFrame(frame);
            } else {
                canvas.remove();
            }
        }

        requestAnimationFrame(frame);
    }

    // Kreiraj vatromet efekt
    function createFireworksParticles(x, y) {
        const particles = [];
        const particleCount = 30;

        for (let i = 0; i < particleCount; i++) {
            const angle = (i / particleCount) * Math.PI * 2;
            const velocity = Math.random() * 6 + 3;

            particles.push({
                x: x,
                y: y,
                vx: Math.cos(angle) * velocity,
                vy: Math.sin(angle) * velocity,
                size: Math.random() * 3 + 2,
                color: confettiColors[Math.floor(Math.random() * confettiColors.length)],
                life: 1,
                decay: Math.random() * 0.015 + 0.01
            });
        }

        return particles;
    }

    // Animiraj vatromet
    function animateFireworks(canvas, ctx, fireworks) {
        const startTime = Date.now();

        function frame() {
            const elapsed = Date.now() - startTime;

            ctx.clearRect(0, 0, canvas.width, canvas.height);

            let hasAlive = false;

            fireworks.forEach((particle) => {
                if (particle.life > 0) {
                    hasAlive = true;
                    particle.x += particle.vx;
                    particle.y += particle.vy;
                    particle.vy += 0.2; // Gravitacija
                    particle.life -= particle.decay;

                    ctx.save();
                    ctx.globalAlpha = particle.life;
                    ctx.fillStyle = particle.color;
                    ctx.beginPath();
                    ctx.arc(particle.x, particle.y, particle.size, 0, Math.PI * 2);
                    ctx.fill();
                    ctx.restore();
                }
            });

            if (hasAlive) {
                requestAnimationFrame(frame);
            } else {
                canvas.remove();
            }
        }

        requestAnimationFrame(frame);
    }

    // Prikaži poruku sa animacijom
    function showCelebrationMessage(message, type = 'success') {
        const messageDiv = document.createElement('div');
        messageDiv.className = `celebration-message celebration-${type}`;
        messageDiv.innerHTML = `
            <div class="celebration-content">
                <div class="celebration-icon">🎉</div>
                <div class="celebration-text">
                    <h2>${message}</h2>
                    <p>Operacija je uspješno izvršena!</p>
                </div>
            </div>
        `;

        document.body.appendChild(messageDiv);

        // Animiraj ulazak
        setTimeout(() => {
            messageDiv.classList.add('show');
        }, 10);

        // Ukloni nakon što nestane
        setTimeout(() => {
            messageDiv.classList.remove('show');
            setTimeout(() => {
                messageDiv.remove();
            }, 300);
        }, 3000);
    }

    // Glavna funkcija za pokretanje animacije
    function startCelebration(message = 'Čestitam!') {
        // Kreiraj canvas za confetti
        const canvas = document.createElement('canvas');
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
        canvas.style.position = 'fixed';
        canvas.style.top = '0';
        canvas.style.left = '0';
        canvas.style.pointerEvents = 'none';
        canvas.style.zIndex = '9998';

        document.body.appendChild(canvas);

        const ctx = canvas.getContext('2d');

        // Kreiraj više valova confettija
        const confetti1 = createConfetti();
        const confetti2 = createConfetti();
        const confetti3 = createConfetti();

        // Animiraj confetti sa kašnjenjem
        animateConfetti(canvas.cloneNode(), ctx, confetti1);
        setTimeout(() => {
            const canvas2 = canvas.cloneNode();
            const ctx2 = canvas2.getContext('2d');
            document.body.appendChild(canvas2);
            animateConfetti(canvas2, ctx2, confetti2);
        }, 200);

        setTimeout(() => {
            const canvas3 = canvas.cloneNode();
            const ctx3 = canvas3.getContext('2d');
            document.body.appendChild(canvas3);
            animateConfetti(canvas3, ctx3, confetti3);
        }, 400);

        // Kreiraj vatromet efekte na više lokacija
        const fireworksCanvas = document.createElement('canvas');
        fireworksCanvas.width = window.innerWidth;
        fireworksCanvas.height = window.innerHeight;
        fireworksCanvas.style.position = 'fixed';
        fireworksCanvas.style.top = '0';
        fireworksCanvas.style.left = '0';
        fireworksCanvas.style.pointerEvents = 'none';
        fireworksCanvas.style.zIndex = '9999';

        document.body.appendChild(fireworksCanvas);
        const fireworksCtx = fireworksCanvas.getContext('2d');

        // Kreiraj vatromet efekte na različitim lokacijama
        const fireworks = [];
        for (let i = 0; i < 3; i++) {
            const x = (i + 1) * (window.innerWidth / 4);
            const y = window.innerHeight * 0.3;
            fireworks.push(...createFireworksParticles(x, y));
        }

        animateFireworks(fireworksCanvas, fireworksCtx, fireworks);

        // Prikaži poruku
        showCelebrationMessage(message);

        // Reproduciraj zvuk ako postoji
        playSuccessSound();

        // Čisti canvas nakon animacije
        setTimeout(() => {
            canvas.remove();
            fireworksCanvas.remove();
        }, 3500);
    }

    // Reproduciraj zvuk (koristi Web Audio API)
    function playSuccessSound() {
        try {
            const audioContext = new (window.AudioContext || window.webkitAudioContext)();
            
            // Kreiraj jednostavan zvuk prosljeđivanja (uspješan tonos)
            const now = audioContext.currentTime;
            const successTones = [
                { frequency: 523.25, duration: 0.1 }, // C5
                { frequency: 659.25, duration: 0.1 }, // E5
                { frequency: 783.99, duration: 0.2 }  // G5
            ];

            let currentTime = now;

            successTones.forEach(tone => {
                const osc = audioContext.createOscillator();
                const gain = audioContext.createGain();

                osc.frequency.value = tone.frequency;
                osc.type = 'sine';

                gain.gain.setValueAtTime(0.3, currentTime);
                gain.gain.exponentialRampToValueAtTime(0.01, currentTime + tone.duration);

                osc.connect(gain);
                gain.connect(audioContext.destination);

                osc.start(currentTime);
                osc.stop(currentTime + tone.duration);

                currentTime += tone.duration;
            });
        } catch (e) {
            console.log('Audio API nije dostupan, preskačem zvuk');
        }
    }

    // Javni API
    return {
        celebrate: startCelebration
    };
})();

// Detektuj je li trebala animacija pri učitavanju stranice
document.addEventListener('DOMContentLoaded', function() {
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.has('showCelebration')) {
        const type = urlParams.get('type') || 'korisnik';
        const message = type === 'korisnik' 
            ? '🎉 Korisnik je uspješno kreiran!' 
            : '🎉 Naruđba je uspješno kreirana!';
        
        CelebrationAnimation.celebrate(message);
        
        // Ukloni parametar iz URL-a nakon što je animacija prikazana
        window.history.replaceState({}, document.title, window.location.pathname);
    }
});
