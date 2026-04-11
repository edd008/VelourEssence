// ===== PAYMENT CONFIRMATION JAVASCRIPT =====

document.addEventListener('DOMContentLoaded', function() {
    // Inicializar efectos y animaciones
    initConfirmationEffects();
    setupCountdownTimer();
    setupPrintFunctionality();
    setupEmailFunctionality();
    setupProgressTracking();
});

// Efectos visuales de confirmación
function initConfirmationEffects() {
    // Efecto de confeti
    createConfetti();
    
    // Sonido de éxito (opcional)
    playSuccessSound();
    
    // Animar elementos de la página
    animateConfirmationElements();
}

// Crear efecto de confeti
function createConfetti() {
    const colors = ['#A8E6CF', '#FF8C94', '#FFD93D', '#1F3A5F', '#4ECDC4'];
    const confettiContainer = document.createElement('div');
    confettiContainer.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        pointer-events: none;
        z-index: 1000;
    `;
    document.body.appendChild(confettiContainer);

    // Crear partículas de confeti
    for (let i = 0; i < 50; i++) {
        createConfettiPiece(confettiContainer, colors);
    }

    // Remover confeti después de 5 segundos
    setTimeout(() => {
        confettiContainer.remove();
    }, 5000);
}

function createConfettiPiece(container, colors) {
    const piece = document.createElement('div');
    const color = colors[Math.floor(Math.random() * colors.length)];
    const size = Math.random() * 10 + 5;
    const startX = Math.random() * window.innerWidth;
    const duration = Math.random() * 3 + 2;
    const delay = Math.random() * 2;

    piece.style.cssText = `
        position: absolute;
        width: ${size}px;
        height: ${size}px;
        background: ${color};
        top: -10px;
        left: ${startX}px;
        animation: confettiFall ${duration}s ease-in ${delay}s forwards;
        border-radius: ${Math.random() > 0.5 ? '50%' : '2px'};
    `;

    container.appendChild(piece);
}

// Sonido de éxito (opcional)
function playSuccessSound() {
    // Solo si el usuario ha interactuado previamente con la página
    try {
        const audioContext = new (window.AudioContext || window.webkitAudioContext)();
        const oscillator = audioContext.createOscillator();
        const gainNode = audioContext.createGain();

        oscillator.connect(gainNode);
        gainNode.connect(audioContext.destination);

        oscillator.frequency.setValueAtTime(523.25, audioContext.currentTime); // C5
        oscillator.frequency.setValueAtTime(659.25, audioContext.currentTime + 0.1); // E5
        oscillator.frequency.setValueAtTime(783.99, audioContext.currentTime + 0.2); // G5

        gainNode.gain.setValueAtTime(0.1, audioContext.currentTime);
        gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.5);

        oscillator.start(audioContext.currentTime);
        oscillator.stop(audioContext.currentTime + 0.5);
    } catch (e) {
        // Silenciosamente manejar errores de audio
        console.log('Audio not available');
    }
}

// Animar elementos de confirmación
function animateConfirmationElements() {
    // Contador animado para el número de pedido
    animateOrderNumber();
    
    // Progreso animado del timeline
    animateTimeline();
    
    // Efecto de typing en el monto
    animateAmount();
}

function animateOrderNumber() {
    const orderNumberElement = document.querySelector('.order-number');
    if (orderNumberElement) {
        const finalNumber = orderNumberElement.textContent;
        const numericPart = finalNumber.replace(/\D/g, '');
        let currentNumber = 0;
        const target = parseInt(numericPart);
        const prefix = finalNumber.replace(numericPart, '');

        const increment = Math.max(1, Math.floor(target / 50));
        const interval = setInterval(() => {
            currentNumber += increment;
            if (currentNumber >= target) {
                currentNumber = target;
                clearInterval(interval);
            }
            orderNumberElement.textContent = prefix + currentNumber.toString().padStart(numericPart.length, '0');
        }, 50);
    }
}

function animateTimeline() {
    const timelineSteps = document.querySelectorAll('.status-step');
    timelineSteps.forEach((step, index) => {
        if (step.classList.contains('active')) {
            setTimeout(() => {
                step.style.animation = 'slideInFromLeft 0.6s ease forwards';
            }, index * 200);
        }
    });
}

function animateAmount() {
    const amountElements = document.querySelectorAll('.total-amount, .change-amount');
    amountElements.forEach(element => {
        const text = element.textContent;
        element.textContent = '';
        
        let i = 0;
        const typeInterval = setInterval(() => {
            element.textContent += text.charAt(i);
            i++;
            if (i >= text.length) {
                clearInterval(typeInterval);
            }
        }, 50);
    });
}

// Configurar timer de redirección
function setupCountdownTimer() {
    let countdown = 300; // 5 minutos
    const timerElement = document.createElement('div');
    timerElement.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: rgba(31, 58, 95, 0.9);
        color: white;
        padding: 10px 15px;
        border-radius: 10px;
        font-size: 12px;
        z-index: 1001;
        display: none;
    `;
    document.body.appendChild(timerElement);

    const updateTimer = () => {
        const minutes = Math.floor(countdown / 60);
        const seconds = countdown % 60;
        timerElement.textContent = `Sesión expira en ${minutes}:${seconds.toString().padStart(2, '0')}`;
        
        if (countdown <= 60) {
            timerElement.style.display = 'block';
            timerElement.style.background = 'rgba(255, 140, 148, 0.9)';
        }
        
        if (countdown <= 0) {
            window.location.href = '/';
        }
        
        countdown--;
    };

    updateTimer();
    setInterval(updateTimer, 1000);
}

// Funcionalidad de impresión
function setupPrintFunctionality() {
    // Agregar botón de imprimir si no existe
    const actionButtons = document.querySelector('.action-buttons');
    if (actionButtons) {
        const printButton = document.createElement('button');
        printButton.className = 'btn-secondary-custom';
        printButton.innerHTML = '<i class="bi bi-printer"></i> Imprimir Recibo';
        printButton.onclick = printReceipt;
        actionButtons.appendChild(printButton);
    }
}

function printReceipt() {
    const printWindow = window.open('', '_blank');
    const receiptContent = generateReceiptHTML();
    
    printWindow.document.write(receiptContent);
    printWindow.document.close();
    
    setTimeout(() => {
        printWindow.print();
        printWindow.close();
    }, 500);
}

function generateReceiptHTML() {
    const orderNumber = document.querySelector('.order-number')?.textContent || '';
    const totalAmount = document.querySelector('.total-amount')?.textContent || '';
    const paymentMethod = document.querySelector('.detail-value')?.textContent || '';
    const date = new Date().toLocaleString('es-CO');

    return `
        <!DOCTYPE html>
        <html>
        <head>
            <title>Recibo de Pago - VelourEssence</title>
            <style>
                body { 
                    font-family: Arial, sans-serif; 
                    max-width: 400px; 
                    margin: 0 auto; 
                    padding: 20px;
                    background: white;
                }
                .header { 
                    text-align: center; 
                    border-bottom: 2px solid #1F3A5F; 
                    padding-bottom: 15px; 
                    margin-bottom: 20px;
                }
                .logo { 
                    font-size: 24px; 
                    font-weight: bold; 
                    color: #1F3A5F; 
                    margin-bottom: 5px;
                }
                .detail-row { 
                    display: flex; 
                    justify-content: space-between; 
                    margin: 10px 0; 
                    padding: 5px 0;
                    border-bottom: 1px dotted #ccc;
                }
                .total { 
                    font-weight: bold; 
                    font-size: 18px; 
                    border-top: 2px solid #1F3A5F; 
                    padding-top: 10px; 
                    margin-top: 15px;
                }
                .footer { 
                    text-align: center; 
                    margin-top: 30px; 
                    font-size: 12px; 
                    color: #666;
                }
                .success { 
                    background: #A8E6CF; 
                    color: #1F3A5F; 
                    padding: 10px; 
                    border-radius: 5px; 
                    text-align: center; 
                    margin: 15px 0; 
                    font-weight: bold;
                }
            </style>
        </head>
        <body>
            <div class="header">
                <div class="logo">VelourEssence</div>
                <div>Recibo de Pago</div>
            </div>
            
            <div class="success">✓ PAGO EXITOSO</div>
            
            <div class="detail-row">
                <span>Número de Pedido:</span>
                <span>${orderNumber}</span>
            </div>
            <div class="detail-row">
                <span>Método de Pago:</span>
                <span>${paymentMethod}</span>
            </div>
            <div class="detail-row">
                <span>Fecha y Hora:</span>
                <span>${date}</span>
            </div>
            <div class="detail-row total">
                <span>Total Pagado:</span>
                <span>${totalAmount}</span>
            </div>
            
            <div class="footer">
                <p>¡Gracias por tu compra!</p>
                <p>www.velour-essence.com</p>
                <p>Soporte: soporte@velour-essence.com</p>
            </div>
        </body>
        </html>
    `;
}

// Funcionalidad de envío por email
function setupEmailFunctionality() {
    const actionButtons = document.querySelector('.action-buttons');
    if (actionButtons) {
        const emailButton = document.createElement('button');
        emailButton.className = 'btn-secondary-custom';
        emailButton.innerHTML = '<i class="bi bi-envelope"></i> Enviar por Email';
        emailButton.onclick = sendEmailReceipt;
        actionButtons.appendChild(emailButton);
    }
}

function sendEmailReceipt() {
    const orderNumber = document.querySelector('.order-number')?.textContent || '';
    const totalAmount = document.querySelector('.total-amount')?.textContent || '';
    
    const subject = `Recibo de Pago ${orderNumber} - VelourEssence`;
    const body = `
¡Hola!

Tu pago ha sido procesado exitosamente.

Detalles de la transacción:
- Número de Pedido: ${orderNumber}
- Total Pagado: ${totalAmount}
- Fecha: ${new Date().toLocaleString('es-CO')}

¡Gracias por tu compra en VelourEssence!

Saludos,
Equipo VelourEssence
    `.trim();
    
    const mailtoLink = `mailto:?subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(body)}`;
    window.location.href = mailtoLink;
}

// Seguimiento de progreso
function setupProgressTracking() {
    // Simular actualizaciones de estado del pedido
    setTimeout(() => {
        updateOrderStatus('preparing');
    }, 5000);
    
    setTimeout(() => {
        updateOrderStatus('shipping');
    }, 15000);
}

function updateOrderStatus(status) {
    const statusSteps = document.querySelectorAll('.status-step');
    
    switch(status) {
        case 'preparing':
            if (statusSteps[1]) {
                statusSteps[1].classList.add('completed');
                showStatusNotification('Tu pedido está siendo preparado');
            }
            break;
        case 'shipping':
            if (statusSteps[2]) {
                statusSteps[2].classList.add('active');
                showStatusNotification('Tu pedido está en camino');
            }
            break;
    }
}

function showStatusNotification(message) {
    const notification = document.createElement('div');
    notification.style.cssText = `
        position: fixed;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        background: rgba(168, 230, 207, 0.95);
        color: #1F3A5F;
        padding: 15px 25px;
        border-radius: 10px;
        font-weight: 600;
        z-index: 1002;
        box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
        animation: notificationSlide 0.5s ease;
    `;
    notification.textContent = message;
    document.body.appendChild(notification);
    
    setTimeout(() => {
        notification.style.animation = 'notificationSlide 0.5s ease reverse';
        setTimeout(() => {
            notification.remove();
        }, 500);
    }, 3000);
}

// Estilos CSS adicionales para animaciones
const style = document.createElement('style');
style.textContent = `
    @keyframes confettiFall {
        0% {
            transform: translateY(-10px) rotate(0deg);
            opacity: 1;
        }
        100% {
            transform: translateY(100vh) rotate(360deg);
            opacity: 0;
        }
    }
    
    @keyframes slideInFromLeft {
        0% {
            transform: translateX(-50px);
            opacity: 0;
        }
        100% {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes notificationSlide {
        0% {
            transform: translate(-50%, -50%) scale(0.8);
            opacity: 0;
        }
        100% {
            transform: translate(-50%, -50%) scale(1);
            opacity: 1;
        }
    }
    
    .status-step.completed::after {
        background: linear-gradient(to bottom, #A8E6CF, #FF8C94) !important;
    }
    
    .action-buttons {
        gap: 0.5rem;
        flex-wrap: wrap;
    }
`;
document.head.appendChild(style);
