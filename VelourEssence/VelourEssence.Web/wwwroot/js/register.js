// register.js - Funcionalidad de la página de registro
document.addEventListener('DOMContentLoaded', function() {
    setupPasswordToggles();
    setupRealtimeValidation();
    setupPasswordStrength();
    setupFormSubmission();
    animateEntry();
});

function setupPasswordToggles() {
    const toggles = [
        { button: '#togglePassword', input: '#passwordInput' },
        { button: '#toggleConfirmPassword', input: '#confirmPasswordInput' }
    ];
    
    toggles.forEach(function(toggle) {
        const toggleBtn = document.querySelector(toggle.button);
        const passwordInput = document.querySelector(toggle.input);
        if (toggleBtn && passwordInput) {
            toggleBtn.addEventListener('click', function() {
                const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
                passwordInput.setAttribute('type', type);
                const icon = this.querySelector('i');
                icon.classList.toggle('bi-eye');
                icon.classList.toggle('bi-eye-slash');
            });
        }
    });
}

function setupRealtimeValidation() {
    const usernameInput = document.querySelector('[data-validate="username"]');
    const emailInput = document.querySelector('[data-validate="email"]');
    
    if (usernameInput) {
        usernameInput.addEventListener('blur', function() {
            validateUsername(this.value);
        });
    }
    
    if (emailInput) {
        emailInput.addEventListener('blur', function() {
            validateEmail(this.value);
        });
    }
}

function validateUsername(username) {
    if (username.length < 3) {
        return;
    }
    
    fetch('/Auth/ValidarUsuarioDisponible?nombreUsuario=' + encodeURIComponent(username))
        .then(function(response) { 
            return response.json(); 
        })
        .then(function(isAvailable) {
            const messageDiv = document.getElementById('username-validation');
            if (isAvailable) {
                showValidationMessage(messageDiv, window.registerTexts.usernameAvailable, 'success');
            } else {
                showValidationMessage(messageDiv, window.registerTexts.usernameTaken, 'error');
            }
        })
        .catch(function() {
            const messageDiv = document.getElementById('username-validation');
            showValidationMessage(messageDiv, window.registerTexts.validationError, 'error');
        });
}

function validateEmail(email) {
    if (email.indexOf('@') === -1) {
        return;
    }
    
    fetch('/Auth/ValidarCorreoDisponible?correo=' + encodeURIComponent(email))
        .then(function(response) { 
            return response.json(); 
        })
        .then(function(isAvailable) {
            const messageDiv = document.getElementById('email-validation');
            if (isAvailable) {
                showValidationMessage(messageDiv, window.registerTexts.emailAvailable, 'success');
            } else {
                showValidationMessage(messageDiv, window.registerTexts.emailTaken, 'error');
            }
        })
        .catch(function() {
            const messageDiv = document.getElementById('email-validation');
            showValidationMessage(messageDiv, window.registerTexts.validationError, 'error');
        });
}

function showValidationMessage(element, message, type) {
    element.textContent = message;
    element.className = 'register-validation-message ' + type;
}

function setupPasswordStrength() {
    const passwordInput = document.getElementById('passwordInput');
    const strengthDiv = document.getElementById('passwordStrength');
    
    if (!passwordInput || !strengthDiv) return;
    
    const strengthFill = strengthDiv.querySelector('.strength-fill');
    const strengthText = strengthDiv.querySelector('.strength-text');
    
    passwordInput.addEventListener('input', function() {
        const password = this.value;
        const strength = calculatePasswordStrength(password);
        strengthFill.style.width = strength.percentage + '%';
        strengthFill.className = 'strength-fill ' + strength.class;
        strengthText.textContent = strength.text;
    });
}

function calculatePasswordStrength(password) {
    let score = 0;
    
    if (password.length >= 6) score += 20;
    if (password.length >= 10) score += 20;
    if (/[a-z]/.test(password)) score += 20;
    if (/[A-Z]/.test(password)) score += 20;
    if (/[0-9]/.test(password)) score += 10;
    if (/[^A-Za-z0-9]/.test(password)) score += 10;
    
    if (score < 30) {
        return { 
            percentage: score, 
            class: 'weak', 
            text: window.registerTexts.passwordStrengthWeak 
        };
    } else if (score < 60) {
        return { 
            percentage: score, 
            class: 'medium', 
            text: window.registerTexts.passwordStrengthMedium 
        };
    } else if (score < 90) {
        return { 
            percentage: score, 
            class: 'good', 
            text: window.registerTexts.passwordStrengthGood 
        };
    } else {
        return { 
            percentage: score, 
            class: 'strong', 
            text: window.registerTexts.passwordStrengthStrong 
        };
    }
}

function setupFormSubmission() {
    const submitBtn = document.getElementById('submitBtn');
    const form = document.getElementById('registerForm');
    
    if (form && submitBtn) {
        form.addEventListener('submit', function() {
            submitBtn.classList.add('loading');
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<i class="bi bi-hourglass-split"></i><span>' + window.registerTexts.btnLoading + '</span>';
        });
    }
}

function animateEntry() {
    const wrapper = document.querySelector('.register-wrapper');
    if (wrapper) {
        setTimeout(function() {
            wrapper.classList.add('animate-in');
        }, 100);
    }
}
