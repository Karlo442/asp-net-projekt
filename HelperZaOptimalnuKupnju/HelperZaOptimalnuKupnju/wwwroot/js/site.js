// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function() {
    const discountModalOverlay = document.getElementById('discountModalOverlay');
    const discountModal = document.getElementById('discountModal');
    const closeBtn = document.getElementById('closeDiscountModal');
    const copyBtn = document.getElementById('copyPromoBtn');
    const claimBtn = document.getElementById('claimOfferBtn');

    if (!discountModalOverlay || !discountModal) return;

    // Show modal after 2 seconds
    setTimeout(() => {
        discountModalOverlay.classList.add('active');
    }, 2000);

    // Close modal function
    function closeModal() {
        discountModal.classList.add('closing');
        setTimeout(() => {
            discountModalOverlay.classList.remove('active');
            discountModal.classList.remove('closing');
        }, 400);
    }

    // Close button
    if (closeBtn) {
        closeBtn.addEventListener('click', closeModal);
    }

    // Close on overlay click
    discountModalOverlay.addEventListener('click', function(e) {
        if (e.target === discountModalOverlay) {
            closeModal();
        }
    });

    // Copy promo code
    if (copyBtn) {
        copyBtn.addEventListener('click', function() {
            const promoInput = document.querySelector('.promo-input');
            if (promoInput) {
                promoInput.select();
                document.execCommand('copy');
                
                // Visual feedback
                const originalText = copyBtn.textContent;
                copyBtn.textContent = 'Copied!';
                copyBtn.classList.add('copied');
                
                setTimeout(() => {
                    copyBtn.textContent = originalText;
                    copyBtn.classList.remove('copied');
                }, 2000);
            }
        });
    }

    // Claim offer
    if (claimBtn) {
        claimBtn.addEventListener('click', function() {
            closeModal();
            // You can add additional logic here for claiming the offer
        });
    }

    // Close modal with Escape key
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape' && discountModalOverlay.classList.contains('active')) {
            closeModal();
        }
    });
});
