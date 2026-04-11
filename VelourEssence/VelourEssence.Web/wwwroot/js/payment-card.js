// ===== PAYMENT CARD JAVASCRIPT =====

document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('paymentForm');
    const numeroTarjeta = document.getElementById('numeroTarjeta');
    const nombreTitular = document.querySelector('input[name="NombreTitular"]');
    const fechaExpiracion = document.getElementById('fechaExpiracion');
    const cvv = document.getElementById('codigoCVV');
    const payButton = document.getElementById('payButton');
    const cardValidation = document.getElementById('cardValidation');
    const tipoTarjeta = document.getElementById('tipoTarjeta');
    const validacionTarjeta = document.getElementById('validacionTarjeta');

    // Elementos de la tarjeta visual
    const cardNumberDisplay = document.getElementById('cardNumberDisplay');
    const cardHolderDisplay = document.getElementById('cardHolderDisplay');
    const cardExpiryDisplay = document.getElementById('cardExpiryDisplay');
    const cardBrand = document.getElementById('cardBrand');

    // Validación en tiempo real
    function validateForm() {
        console.log('🔍 Validando formulario...');
        
        const numeroTarjetaValue = numeroTarjeta.value.replace(/\s/g, '');
        const nombreTitularValue = nombreTitular.value.trim();
        const fechaExpiracionValue = fechaExpiracion.value;
        const cvvValue = cvv.value;
        
        console.log('📋 Valores del formulario:');
        console.log('  Número tarjeta:', numeroTarjetaValue, '(longitud:', numeroTarjetaValue.length, ')');
        console.log('  Nombre titular:', nombreTitularValue, '(longitud:', nombreTitularValue.length, ')');
        console.log('  Fecha expiración:', fechaExpiracionValue);
        console.log('  CVV:', cvvValue);
        
        const numeroValido = isValidCardNumber(numeroTarjetaValue);
        const nombreValido = nombreTitularValue.length >= 2;
        const fechaValida = isValidExpiryDate(fechaExpiracionValue);
        const cvvValido = isValidCVV(cvvValue);

        console.log('📊 Resultados de validación:');
        console.log('  Número válido:', numeroValido);
        console.log('  Nombre válido:', nombreValido);
        console.log('  Fecha válida:', fechaValida);
        console.log('  CVV válido:', cvvValido);

        const formValid = numeroValido && nombreValido && fechaValida && cvvValido;
        console.log('✅ Formulario válido:', formValid);
        
        payButton.disabled = !formValid;

        // Mostrar validación general
        if (numeroTarjeta.value.length > 10) {
            if (numeroValido) {
                showValidation(true, 'Tarjeta válida - Algoritmo Luhn confirmado');
            } else {
                showValidation(false, 'Número de tarjeta inválido');
            }
        } else {
            hideValidation();
        }

        return formValid;
    }

    // Algoritmo de Luhn para validar tarjetas
    function isValidCardNumber(cardNumber) {
        if (!/^\d+$/.test(cardNumber)) return false;
        if (cardNumber.length < 13 || cardNumber.length > 19) return false;

        let sum = 0;
        let shouldDouble = false;

        for (let i = cardNumber.length - 1; i >= 0; i--) {
            let digit = parseInt(cardNumber.charAt(i));

            if (shouldDouble) {
                digit *= 2;
                if (digit > 9) {
                    digit -= 9;
                }
            }

            sum += digit;
            shouldDouble = !shouldDouble;
        }

        return sum % 10 === 0;
    }

    // Detectar tipo de tarjeta
    function detectCardType(cardNumber) {
        const cleanNumber = cardNumber.replace(/\s/g, '');
        
        if (/^4/.test(cleanNumber)) {
            return { type: 'Visa', icon: 'bi-credit-card', color: '#1A237E' };
        } else if (/^5[1-5]/.test(cleanNumber)) {
            return { type: 'MasterCard', icon: 'bi-credit-card-2-front', color: '#FF5722' };
        } else if (/^3[47]/.test(cleanNumber)) {
            return { type: 'American Express', icon: 'bi-credit-card-2-back', color: '#2E7D32' };
        } else {
            return { type: 'Desconocida', icon: 'bi-credit-card', color: '#666' };
        }
    }

    // Validar fecha de expiración
    function isValidExpiryDate(expiry) {
        if (!/^\d{2}\/\d{2}$/.test(expiry)) return false;

        const [month, year] = expiry.split('/').map(num => parseInt(num, 10));
        const now = new Date();
        const currentYear = now.getFullYear() % 100;
        const currentMonth = now.getMonth() + 1;

        if (month < 1 || month > 12) return false;
        if (year < currentYear || (year === currentYear && month < currentMonth)) return false;

        return true;
    }

    // Validar CVV
    function isValidCVV(cvvValue) {
        return /^\d{3,4}$/.test(cvvValue);
    }

    // Mostrar estado de validación
    function showValidation(isValid, message) {
        validacionTarjeta.className = `validation-indicator ${isValid ? 'valid' : 'invalid'}`;
        validacionTarjeta.querySelector('.validation-icon').className = 
            `validation-icon bi ${isValid ? 'bi-check-circle-fill' : 'bi-x-circle-fill'}`;
        validacionTarjeta.querySelector('.validation-message').textContent = message;
        validacionTarjeta.classList.remove('d-none');
    }

    function hideValidation() {
        validacionTarjeta.classList.add('d-none');
    }

    // Formatear número de tarjeta
    numeroTarjeta.addEventListener('input', function(e) {
        let value = e.target.value.replace(/\s/g, '');
        let formattedValue = value.replace(/(.{4})/g, '$1 ').trim();
        
        if (formattedValue.length > 23) {
            formattedValue = formattedValue.substr(0, 23);
        }
        
        e.target.value = formattedValue;
        
        // Actualizar tarjeta visual
        cardNumberDisplay.textContent = formattedValue || '**** **** **** ****';
        
        // Detectar tipo de tarjeta
        const cardInfo = detectCardType(value);
        tipoTarjeta.textContent = cardInfo.type;
        tipoTarjeta.style.color = cardInfo.color;
        cardBrand.innerHTML = `<i class="${cardInfo.icon}"></i>`;
        cardBrand.style.color = cardInfo.color;
        
        validateForm();
    });

    // Formatear nombre del titular
    nombreTitular.addEventListener('input', function(e) {
        e.target.value = e.target.value.toUpperCase();
        cardHolderDisplay.textContent = e.target.value || 'NOMBRE APELLIDO';
        validateForm();
    });

    // Formatear fecha de expiración
    fechaExpiracion.addEventListener('input', function(e) {
        let value = e.target.value.replace(/\D/g, '');
        
        if (value.length >= 2) {
            value = value.substring(0, 2) + '/' + value.substring(2, 4);
        }
        
        e.target.value = value;
        cardExpiryDisplay.textContent = value || 'MM/YY';
        validateForm();
    });

    // Validar CVV
    cvv.addEventListener('input', function(e) {
        e.target.value = e.target.value.replace(/\D/g, '');
        validateForm();
    });

    // Solo números en campos numéricos
    [numeroTarjeta, cvv].forEach(input => {
        input.addEventListener('keypress', function(e) {
            if (!/\d/.test(e.key) && !['Backspace', 'Delete', 'Tab', 'Enter'].includes(e.key)) {
                e.preventDefault();
            }
        });
    });

    // Envío del formulario - SÚPER SIMPLE CON MÁS LOGS
    form.addEventListener('submit', function(e) {
        console.log('🚀 [JS] SUBMIT EVENT TRIGGERED!');
        e.preventDefault();
        
        // LIMPIAR el número de tarjeta antes de validar y enviar
        const numeroTarjetaValue = numeroTarjeta.value.trim().replace(/\s/g, '');
        const nombreTitularValue = nombreTitular.value.trim();
        const fechaExpiracionValue = fechaExpiracion.value.trim();
        const cvvValue = cvv.value.trim();
        
        console.log('📋 [JS] Valores del formulario (LIMPIO):', {
            numero: numeroTarjetaValue + ' (longitud: ' + numeroTarjetaValue.length + ')',
            nombre: nombreTitularValue,
            fecha: fechaExpiracionValue,
            cvv: cvvValue
        });
        
        // Solo verificar que no estén vacíos y tengan longitud mínima
        if (numeroTarjetaValue.length < 13 || nombreTitularValue.length < 2 || 
            fechaExpiracionValue.length < 4 || cvvValue.length < 3) {
            console.log('❌ [JS] Validación falló - campos insuficientes');
            alert('Por favor, complete todos los campos correctamente.');
            return;
        }

        console.log('✅ [JS] Validación simple exitosa, limpiando datos...');
        
        // ¡IMPORTANTE! Limpiar el número de tarjeta en el input antes del envío
        numeroTarjeta.value = numeroTarjetaValue; // Sin espacios
        
        console.log('🧹 [JS] Número de tarjeta limpiado:', numeroTarjeta.value);
        
        // Mostrar estado de carga
        payButton.classList.add('loading');
        payButton.disabled = true;
        payButton.innerHTML = '<div class="btn-content"><i class="bi bi-hourglass-split"></i><span>Procesando pago...</span></div>';
        
        // Enviar el formulario inmediatamente
        console.log('📤 [JS] ENVIANDO FORMULARIO AHORA...');
        console.log('📤 [JS] Form action:', form.action);
        console.log('📤 [JS] Form method:', form.method);
        
        // ¡ENVIAR AHORA!
        form.submit();
        console.log('📤 [JS] form.submit() EJECUTADO!');
    });

    // Validación inicial
    validateForm();
});

// Función para animar la tarjeta
function animateCard() {
    const card = document.querySelector('.credit-card');
    if (card) {
        card.style.transform = 'rotateY(180deg)';
        setTimeout(() => {
            card.style.transform = 'rotateY(0deg)';
        }, 1000);
    }
}

// Efectos visuales adicionales
document.addEventListener('DOMContentLoaded', function() {
    // Efecto de typing en el número de tarjeta
    const cardNumber = document.getElementById('cardNumberDisplay');
    if (cardNumber) {
        cardNumber.addEventListener('DOMSubtreeModified', function() {
            this.style.animation = 'none';
            setTimeout(() => {
                this.style.animation = 'fadeIn 0.3s ease';
            }, 10);
        });
    }

    // Tooltip para CVV
    const cvvHelp = document.querySelector('.cvv-help');
    if (cvvHelp) {
        cvvHelp.addEventListener('mouseenter', function() {
            // Mostrar tooltip personalizado si se desea
        });
    }
});

// Animaciones CSS adicionales
const style = document.createElement('style');
style.textContent = `
    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(-5px); }
        to { opacity: 1; transform: translateY(0); }
    }
    
    .credit-card {
        transition: transform 0.8s ease;
    }
    
    .form-control:focus {
        animation: focusGlow 0.3s ease;
    }
    
    @keyframes focusGlow {
        0% { box-shadow: 0 0 0 3px rgba(168, 230, 207, 0); }
        100% { box-shadow: 0 0 0 3px rgba(168, 230, 207, 0.1); }
    }
`;
document.head.appendChild(style);
