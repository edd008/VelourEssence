// ===== PAYMENT CASH JAVASCRIPT =====

document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('cashPaymentForm');
    const montoRecibidoInput = document.querySelector('input[name="MontoPagado"]');
    const payButton = document.getElementById('payButton');
    const receivedAmountDisplay = document.getElementById('receivedAmount');
    const changeAmountDisplay = document.getElementById('changeAmount');
    const displayAmount = document.getElementById('displayAmount');
    const quickAmountButtons = document.querySelectorAll('.quick-amount-btn');

    // Verificar que todos los elementos existen
    if (!montoRecibidoInput) {
        console.error('No se encontró el input MontoPagado');
        return;
    }
    if (!payButton) {
        console.error('No se encontró el botón de pago');
        return;
    }

    // Obtener el total del pedido desde el input hidden o el atributo min
    const totalPedidoInput = document.querySelector('input[name="TotalPedido"]');
    const totalPedido = totalPedidoInput ? parseFloat(totalPedidoInput.value) : parseFloat(montoRecibidoInput.getAttribute('min')) || 0;
    
    console.log('Total del pedido cargado:', totalPedido);

    // Función para formatear moneda
    function formatCurrency(amount) {
        return '₡' + new Intl.NumberFormat('es-CR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        }).format(amount);
    }

    // Función para calcular el vuelto
    function calculateChange() {
        const montoRecibido = parseFloat(montoRecibidoInput.value) || 0;
        const change = Math.max(0, montoRecibido - totalPedido);
        
        // ===== DEBUG =====
        console.log('=== CÁLCULO DE VUELTO ===');
        console.log('Monto recibido:', montoRecibido);
        console.log('Total pedido:', totalPedido);
        console.log('Diferencia:', montoRecibido - totalPedido);
        console.log('Cambio calculado:', change);
        
        receivedAmountDisplay.textContent = formatCurrency(montoRecibido);
        changeAmountDisplay.textContent = formatCurrency(change);
        
        // Actualizar el estado del botón - mejorar la validación de decimales
        const difference = montoRecibido - totalPedido;
        const isValid = difference >= -0.01 && montoRecibido > 0; // Permitir pequeñas diferencias por precisión de decimales
        
        console.log('¿Es válido?', isValid);
        console.log('Botón disabled antes:', payButton.disabled);
        
        payButton.disabled = !isValid; // Validación correcta restaurada
        
        console.log('Botón disabled después:', payButton.disabled);
        console.log('========================');
        
        // Efectos visuales para el cambio
        if (change > 0) {
            changeAmountDisplay.parentElement.classList.add('has-change');
            animateChangeAmount();
        } else {
            changeAmountDisplay.parentElement.classList.remove('has-change');
        }
        
        return change;
    }

    // Función para animar el monto del vuelto
    function animateChangeAmount() {
        const changeRow = changeAmountDisplay.parentElement;
        changeRow.style.animation = 'none';
        setTimeout(() => {
            changeRow.style.animation = 'changeHighlight 0.5s ease';
        }, 10);
    }

    // Función para animar los billetes
    function animateCashBills() {
        const bills = document.querySelectorAll('.cash-bill');
        bills.forEach((bill, index) => {
            bill.style.animation = 'none';
            setTimeout(() => {
                bill.style.animation = `billFloat 2s ease infinite ${index * 0.2}s`;
            }, 100 * index);
        });
    }

    // Event listener para el campo de monto recibido
    montoRecibidoInput.addEventListener('input', function(e) {
        // Permitir solo números y punto decimal
        let value = e.target.value.replace(/[^0-9.]/g, '');
        
        // Evitar múltiples puntos decimales
        const parts = value.split('.');
        if (parts.length > 2) {
            value = parts[0] + '.' + parts.slice(1).join('');
        }
        
        // Limitar a 2 decimales
        if (parts[1] && parts[1].length > 2) {
            value = parts[0] + '.' + parts[1].substring(0, 2);
        }
        
        e.target.value = value;
        calculateChange();
        
        // Actualizar display visual
        const amount = parseFloat(value) || 0;
        displayAmount.textContent = amount.toFixed(2);
        
        // Animar billetes si hay cambio significativo
        if (amount > totalPedido + 10) {
            animateCashBills();
        }
    });

    // Event listeners para botones de montos rápidos
    quickAmountButtons.forEach(button => {
        button.addEventListener('click', function() {
            const amount = parseFloat(this.getAttribute('data-amount'));
            montoRecibidoInput.value = amount.toFixed(2);
            
            // Trigger input event para recalcular
            montoRecibidoInput.dispatchEvent(new Event('input'));
            
            // Efecto visual en el botón seleccionado
            quickAmountButtons.forEach(btn => btn.classList.remove('selected'));
            this.classList.add('selected');
            
            // Remover selección después de un tiempo
            setTimeout(() => {
                this.classList.remove('selected');
            }, 1000);
        });
    });

    // Validación del formulario
    function validateForm() {
        const montoRecibido = parseFloat(montoRecibidoInput.value) || 0;
        return montoRecibido >= totalPedido && montoRecibido > 0;
    }

    // Event listener para el envío del formulario
    form.addEventListener('submit', function(e) {
        e.preventDefault();
        
        if (!validateForm()) {
            alert('El monto recibido debe ser mayor o igual al total del pedido.');
            montoRecibidoInput.focus();
            return;
        }

        const change = calculateChange();
        
        // Confirmación si hay vuelto grande
        if (change > totalPedido * 0.5) {
            const confirmMessage = `El vuelto es de ${formatCurrency(change)}. ¿Está seguro de continuar?`;
            if (!confirm(confirmMessage)) {
                return;
            }
        }

        // Mostrar estado de carga
        payButton.classList.add('loading');
        payButton.disabled = true;
        payButton.innerHTML = '<i class="bi bi-hourglass-split"></i> Procesando pago...';

        // Animar los billetes una vez más
        animateCashBills();

        // Enviar el formulario inmediatamente - sin retraso innecesario
        form.submit();
    });

    // Función para sugerir denominaciones
    function suggestDenominations(change) {
        const denominations = [50000, 20000, 10000, 5000, 2000, 1000, 500, 200, 100, 50];
        const result = {};
        let remaining = change;

        denominations.forEach(denom => {
            if (remaining >= denom) {
                const count = Math.floor(remaining / denom);
                result[denom] = count;
                remaining -= count * denom;
            }
        });

        return result;
    }

    // Mostrar sugerencia de denominaciones
    function showDenominationSuggestion() {
        const change = calculateChange();
        if (change > 0) {
            const suggestions = suggestDenominations(change);
            console.log('Sugerencia de billetes para el vuelto:', suggestions);
            // Aquí se podría mostrar una interfaz visual con las denominaciones sugeridas
        }
    }

    // Efectos visuales adicionales
    function addVisualEffects() {
        // Efecto de brillo en los billetes
        const cashContainer = document.querySelector('.cash-container');
        if (cashContainer) {
            cashContainer.addEventListener('mouseenter', function() {
                this.style.transform = 'scale(1.05)';
            });
            
            cashContainer.addEventListener('mouseleave', function() {
                this.style.transform = 'scale(1)';
            });
        }

        // Efecto de hover en la calculadora
        const calculator = document.querySelector('.change-calculator');
        if (calculator) {
            calculator.addEventListener('mouseenter', function() {
                this.style.boxShadow = '0 8px 25px rgba(255, 183, 77, 0.2)';
            });
            
            calculator.addEventListener('mouseleave', function() {
                this.style.boxShadow = '';
            });
        }
    }

    // Inicialización
    function init() {
        calculateChange();
        animateCashBills();
        addVisualEffects();
        
        // Focus automático en el campo de monto
        setTimeout(() => {
            montoRecibidoInput.focus();
        }, 500);
    }

    // Atajos de teclado
    document.addEventListener('keydown', function(e) {
        // Teclas numéricas rápidas para montos comunes
        if (e.altKey) {
            switch(e.key) {
                case '1':
                    e.preventDefault();
                    montoRecibidoInput.value = Math.ceil(totalPedido).toFixed(2);
                    montoRecibidoInput.dispatchEvent(new Event('input'));
                    break;
                case '2':
                    e.preventDefault();
                    montoRecibidoInput.value = (Math.ceil(totalPedido / 5) * 5).toFixed(2);
                    montoRecibidoInput.dispatchEvent(new Event('input'));
                    break;
                case '3':
                    e.preventDefault();
                    montoRecibidoInput.value = (Math.ceil(totalPedido / 10) * 10).toFixed(2);
                    montoRecibidoInput.dispatchEvent(new Event('input'));
                    break;
            }
        }
    });

    // Inicializar la página
    init();
});

// Estilos CSS adicionales para animaciones
const style = document.createElement('style');
style.textContent = `
    @keyframes changeHighlight {
        0% { 
            background: rgba(255, 183, 77, 0.1);
            transform: scale(1);
        }
        50% { 
            background: rgba(255, 183, 77, 0.2);
            transform: scale(1.02);
        }
        100% { 
            background: rgba(255, 183, 77, 0.1);
            transform: scale(1);
        }
    }
    
    @keyframes billFloat {
        0%, 100% { transform: translateY(0) rotate(var(--rotation, 0deg)); }
        50% { transform: translateY(-5px) rotate(var(--rotation, 0deg)); }
    }
    
    .bill-1 { --rotation: -5deg; }
    .bill-2 { --rotation: 5deg; }
    .bill-3 { --rotation: 0deg; }
    
    .quick-amount-btn.selected {
        background: var(--warning) !important;
        color: white !important;
        transform: scale(1.05);
        box-shadow: 0 5px 15px rgba(255, 183, 77, 0.4);
    }
    
    .cash-container {
        transition: transform 0.3s ease;
    }
    
    .change-calculator {
        transition: box-shadow 0.3s ease;
    }
    
    .has-change .change-amount {
        animation: pulse 1s ease infinite;
    }
    
    @keyframes pulse {
        0%, 100% { opacity: 1; }
        50% { opacity: 0.7; }
    }
`;
document.head.appendChild(style);
