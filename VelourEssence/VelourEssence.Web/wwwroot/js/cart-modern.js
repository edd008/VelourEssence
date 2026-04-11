/* ========================================
   CARRITO MODERNO - JAVASCRIPT INTERACTIVO
   ======================================== */

document.addEventListener('DOMContentLoaded', function() {
    initializeCart();
    initializeAnimations();
    initializeQuantityControls();
    initializeTooltips();
});

/* ========================================
   INICIALIZACIÓN DEL CARRITO
   ======================================== */

function initializeCart() {
    // Actualizar contadores en tiempo real
    updateCartCounters();
    
    // Inicializar tooltips de Bootstrap si está disponible
    if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
    
    // Agregar efecto de hover mejorado a los productos
    addProductHoverEffects();
}

/* ========================================
   ANIMACIONES Y EFECTOS VISUALES
   ======================================== */

function initializeAnimations() {
    // Animación de entrada para los productos
    const cartItems = document.querySelectorAll('.cart-item');
    cartItems.forEach((item, index) => {
        item.style.opacity = '0';
        item.style.transform = 'translateY(20px)';
        
        setTimeout(() => {
            item.style.transition = 'all 0.5s ease';
            item.style.opacity = '1';
            item.style.transform = 'translateY(0)';
        }, index * 100);
    });
    
    // Animación del resumen del carrito
    const cartSummary = document.querySelector('.cart-summary');
    if (cartSummary) {
        cartSummary.style.opacity = '0';
        cartSummary.style.transform = 'translateX(30px)';
        
        setTimeout(() => {
            cartSummary.style.transition = 'all 0.6s ease';
            cartSummary.style.opacity = '1';
            cartSummary.style.transform = 'translateX(0)';
        }, 300);
    }
}

function addProductHoverEffects() {
    const cartItems = document.querySelectorAll('.cart-item');
    
    cartItems.forEach(item => {
        item.addEventListener('mouseenter', function() {
            this.style.transform = 'translateX(5px) scale(1.02)';
            this.style.boxShadow = '0 10px 25px rgba(0, 0, 0, 0.15)';
        });
        
        item.addEventListener('mouseleave', function() {
            this.style.transform = 'translateX(0) scale(1)';
            this.style.boxShadow = 'none';
        });
    });
}

/* ========================================
   CONTROLES DE CANTIDAD MEJORADOS
   ======================================== */

function initializeQuantityControls() {
    const quantityInputs = document.querySelectorAll('.quantity-input');
    
    quantityInputs.forEach(input => {
        // Validación en tiempo real
        input.addEventListener('input', function() {
            validateQuantityInput(this);
        });
        
        // Actualización con Enter
        input.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                this.blur();
                const productId = this.closest('.cart-item').dataset.productoId;
                const isPersonalized = this.closest('.cart-item').dataset.tipo === 'personalizado';
                actualizarCantidad(productId, this.value, isPersonalized);
            }
        });
        
        // Seleccionar todo al hacer foco
        input.addEventListener('focus', function() {
            this.select();
        });
        
        // Validación al perder el foco
        input.addEventListener('blur', function() {
            validateQuantityInput(this);
        });
    });
}

function validateQuantityInput(input) {
    const value = parseInt(input.value);
    const min = parseInt(input.min) || 1;
    const max = parseInt(input.max) || 999;
    
    if (isNaN(value) || value < min) {
        input.value = min;
        showNotification('La cantidad mínima es ' + min, 'warning');
    } else if (value > max) {
        input.value = max;
        showNotification('La cantidad máxima disponible es ' + max, 'warning');
    }
    
    // Efecto visual de validación
    input.style.borderColor = value >= min && value <= max ? '#10B981' : '#EF4444';
    setTimeout(() => {
        input.style.borderColor = '';
    }, 1500);
}

/* ========================================
   FUNCIONES DE CARRITO MEJORADAS
   ======================================== */

// Mostrar/ocultar loading con mejor UX
function showLoading(message = 'Procesando...') {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) {
        // Agregar mensaje de loading si no existe
        let loadingText = overlay.querySelector('.loading-text');
        if (!loadingText) {
            loadingText = document.createElement('div');
            loadingText.className = 'loading-text';
            loadingText.style.cssText = 'color: white; margin-top: 1rem; font-size: 1.1rem; font-weight: 500;';
            overlay.querySelector('.spinner').parentNode.appendChild(loadingText);
        }
        loadingText.textContent = message;
        
        overlay.classList.add('show');
        document.body.style.overflow = 'hidden';
    }
}

function hideLoading() {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) {
        overlay.classList.remove('show');
        document.body.style.overflow = '';
    }
}

// Actualizar cantidad con mejor feedback
async function actualizarCantidad(idProducto, nuevaCantidad, esPersonalizado = false) {
    if (nuevaCantidad < 1) {
        showNotification('La cantidad debe ser mayor a 0', 'error');
        return;
    }
    
    const item = document.querySelector(`[data-producto-id="${idProducto}"][data-tipo="${esPersonalizado ? 'personalizado' : 'normal'}"]`);
    const originalQuantity = item ? item.querySelector('.quantity-input').value : nuevaCantidad;
    
    showLoading('Actualizando cantidad...');
    
    try {
        const response = await fetch(window.cartUrls.updateQuantity, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: `idProducto=${idProducto}&cantidad=${nuevaCantidad}&esPersonalizado=${esPersonalizado}`
        });

        const data = await response.json();
        
        if (data.success) {
            // Animación de éxito
            if (item) {
                item.style.backgroundColor = 'rgba(16, 185, 129, 0.1)';
                setTimeout(() => {
                    item.style.backgroundColor = '';
                }, 1000);
            }
            
            showNotification('Cantidad actualizada correctamente', 'success');
            
            // Actualizar totales en tiempo real si están disponibles en la respuesta
            if (data.newSubtotal) {
                updateProductSubtotal(idProducto, data.newSubtotal, esPersonalizado);
            }
            
            if (data.newTotal) {
                updateCartTotals(data.newTotal, data.newTax, data.newGrandTotal);
            }
            
            // Recargar después de una pequeña pausa para mostrar la animación
            setTimeout(() => {
                location.reload();
            }, 1500);
            
        } else {
            // Revertir cantidad en caso de error
            if (item) {
                item.querySelector('.quantity-input').value = originalQuantity;
                item.style.backgroundColor = 'rgba(239, 68, 68, 0.1)';
                setTimeout(() => {
                    item.style.backgroundColor = '';
                }, 1000);
            }
            
            showNotification(data.message || 'Error al actualizar la cantidad', 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        
        // Revertir cantidad en caso de error
        if (item) {
            item.querySelector('.quantity-input').value = originalQuantity;
        }
        
        showNotification('Error de conexión. Por favor, intenta de nuevo.', 'error');
    } finally {
        hideLoading();
    }
}

// Eliminar producto con confirmación mejorada
async function eliminarProducto(idProducto, esPersonalizado = false) {
    const item = document.querySelector(`[data-producto-id="${idProducto}"][data-tipo="${esPersonalizado ? 'personalizado' : 'normal'}"]`);
    const productName = item ? item.querySelector('h5').textContent.trim() : 'este producto';
    
    // Confirmación personalizada más atractiva
    const confirmed = await showConfirmDialog(
        '¿Eliminar producto?',
        `¿Estás seguro de que deseas eliminar "${productName}" de tu carrito?`,
        'Eliminar',
        'Cancelar'
    );
    
    if (!confirmed) return;
    
    showLoading('Eliminando producto...');
    
    // Animación de salida
    if (item) {
        item.style.transform = 'translateX(-100%)';
        item.style.opacity = '0.5';
    }
    
    try {
        const response = await fetch(window.cartUrls.removeProduct, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: `idProducto=${idProducto}&esPersonalizado=${esPersonalizado}`
        });

        const data = await response.json();
        
        if (data.success) {
            showNotification('Producto eliminado correctamente', 'success');
            
            // Remover el elemento con animación
            if (item) {
                item.style.height = item.offsetHeight + 'px';
                item.style.transition = 'all 0.5s ease';
                setTimeout(() => {
                    item.style.height = '0';
                    item.style.padding = '0';
                    item.style.margin = '0';
                    item.style.opacity = '0';
                }, 100);
                
                setTimeout(() => {
                    location.reload();
                }, 600);
            } else {
                setTimeout(() => location.reload(), 1000);
            }
        } else {
            // Revertir animación en caso de error
            if (item) {
                item.style.transform = '';
                item.style.opacity = '';
            }
            showNotification(data.message || 'Error al eliminar el producto', 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        
        // Revertir animación en caso de error
        if (item) {
            item.style.transform = '';
            item.style.opacity = '';
        }
        
        showNotification('Error de conexión. Por favor, intenta de nuevo.', 'error');
    } finally {
        hideLoading();
    }
}

// Limpiar carrito con confirmación mejorada
async function limpiarCarrito() {
    const confirmed = await showConfirmDialog(
        '¿Vaciar carrito completo?',
        '¿Estás seguro de que deseas eliminar todos los productos de tu carrito? Esta acción no se puede deshacer.',
        'Vaciar carrito',
        'Cancelar'
    );
    
    if (!confirmed) return;
    
    showLoading('Vaciando carrito...');
    
    // Animación de salida para todos los productos
    const items = document.querySelectorAll('.cart-item');
    items.forEach((item, index) => {
        setTimeout(() => {
            item.style.transform = 'translateX(-100%)';
            item.style.opacity = '0.3';
        }, index * 100);
    });
    
    try {
        const response = await fetch(window.cartUrls.clearCart, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        });

        const data = await response.json();
        
        if (data.success) {
            showNotification('Carrito vaciado correctamente', 'success');
            setTimeout(() => location.reload(), 1500);
        } else {
            // Revertir animaciones en caso de error
            items.forEach(item => {
                item.style.transform = '';
                item.style.opacity = '';
            });
            showNotification(data.message || 'Error al vaciar el carrito', 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        
        // Revertir animaciones en caso de error
        items.forEach(item => {
            item.style.transform = '';
            item.style.opacity = '';
        });
        
        showNotification('Error de conexión. Por favor, intenta de nuevo.', 'error');
    } finally {
        hideLoading();
    }
}

// Proceder al checkout con validaciones mejoradas
function procederCheckout() {
    try {
        console.log('Iniciando proceso de checkout...');
        console.log('=== DEBUG CHECKOUT ===');
        console.log('cartInfo:', window.cartInfo);
        console.log('cartUrls:', window.cartUrls);
        console.log('idCarrito:', window.cartInfo?.idCarrito);
        console.log('checkout URL:', window.cartUrls?.checkout);
        
        // Verificar elementos visuales del carrito
        const cartItems = document.querySelectorAll('.cart-item:not(.clear-cart-section)');
        console.log('Elementos del carrito encontrados:', cartItems.length);
        
        if (cartItems.length === 0) {
            showNotification('Tu carrito está vacío. Agrega productos para continuar.', 'warning');
            return;
        }
        
        // Verificar que tenemos información válida del carrito
        if (!window.cartInfo) {
            console.error('window.cartInfo no está definido');
            // En lugar de bloquear, continuar si hay productos visibles
            console.log('Continuando sin cartInfo porque hay productos visibles...');
        } else if (window.cartInfo.idCarrito === 0) {
            console.error('ID del carrito es 0');
            showNotification('Debes agregar productos al carrito antes de proceder al pago.', 'warning');
            return;
        }
        
        // Verificar stock antes de proceder
        const outOfStockItems = document.querySelectorAll('.text-danger');
        if (outOfStockItems.length > 0) {
            showNotification('Algunos productos no tienen stock disponible', 'error');
            return;
        }

        // Verificar que existe URL de checkout
        if (!window.cartUrls || !window.cartUrls.checkout) {
            console.error('URL de checkout no configurada');
            showNotification('Error de configuración. Contacte al administrador.', 'error');
            return;
        }
        
        // Verificar que la URL del checkout no tenga ID = 0
        if (window.cartUrls.checkout.includes('/0')) {
            console.error('URL de checkout contiene /0:', window.cartUrls.checkout);
            showNotification('Debes agregar productos al carrito antes de proceder al pago.', 'warning');
            return;
        }
        
        console.log('Todas las validaciones pasaron. Checkout URL:', window.cartUrls.checkout);
        showLoading('Redirigiendo al checkout...');
        
        // Pequeña pausa para mejor UX
        setTimeout(() => {
            try {
                window.location.href = window.cartUrls.checkout;
            } catch (redirectError) {
                console.error('Error al redirigir:', redirectError);
                hideLoading();
                showNotification('Error al proceder al pago. Inténtelo nuevamente.', 'error');
            }
        }, 1000);

    } catch (error) {
        console.error('Error en procederCheckout:', error);
        hideLoading();
        showNotification('Error inesperado. Inténtelo nuevamente.', 'error');
    }
}

/* ========================================
   UTILIDADES Y HELPERS
   ======================================== */

function updateCartCounters() {
    // Actualizar contadores visuales si existen
    const totalItems = document.querySelectorAll('.cart-item:not(.clear-cart-section)').length;
    const counterElements = document.querySelectorAll('.cart-counter, .badge-cart');
    
    counterElements.forEach(counter => {
        counter.textContent = totalItems;
        
        // Animación de actualización
        counter.style.transform = 'scale(1.3)';
        counter.style.backgroundColor = '#10B981';
        setTimeout(() => {
            counter.style.transform = 'scale(1)';
            counter.style.backgroundColor = '';
        }, 300);
    });
}

function updateProductSubtotal(productId, newSubtotal, isPersonalized) {
    const item = document.querySelector(`[data-producto-id="${productId}"][data-tipo="${isPersonalized ? 'personalizado' : 'normal'}"]`);
    if (item) {
        const subtotalElement = item.querySelector('.subtotal-amount');
        if (subtotalElement) {
            subtotalElement.textContent = `₡${newSubtotal.toLocaleString()}`;
            
            // Animación de actualización
            subtotalElement.style.backgroundColor = '#10B981';
            subtotalElement.style.color = 'white';
            setTimeout(() => {
                subtotalElement.style.backgroundColor = '';
                subtotalElement.style.color = '';
            }, 1000);
        }
    }
}

function updateCartTotals(newTotal, newTax, newGrandTotal) {
    const subtotalElement = document.getElementById('subtotal-amount');
    const taxElement = document.getElementById('tax-amount');
    const totalElement = document.getElementById('total-amount');
    
    if (subtotalElement) subtotalElement.textContent = `₡${newTotal.toLocaleString()}`;
    if (taxElement) taxElement.textContent = `₡${newTax.toLocaleString()}`;
    if (totalElement) totalElement.textContent = `₡${newGrandTotal.toLocaleString()}`;
    
    // Animación de actualización
    [subtotalElement, taxElement, totalElement].forEach(element => {
        if (element) {
            element.style.color = '#10B981';
            element.style.fontWeight = '700';
            setTimeout(() => {
                element.style.color = '';
                element.style.fontWeight = '';
            }, 1500);
        }
    });
}

/* ========================================
   SISTEMA DE NOTIFICACIONES
   ======================================== */

function showNotification(message, type = 'info') {
    // Remover notificación anterior si existe
    const existingNotification = document.querySelector('.cart-notification');
    if (existingNotification) {
        existingNotification.remove();
    }
    
    const notification = document.createElement('div');
    notification.className = `cart-notification notification-${type}`;
    
    const colors = {
        success: { bg: '#10B981', icon: 'fas fa-check-circle' },
        error: { bg: '#EF4444', icon: 'fas fa-exclamation-circle' },
        warning: { bg: '#F59E0B', icon: 'fas fa-exclamation-triangle' },
        info: { bg: '#3B82F6', icon: 'fas fa-info-circle' }
    };
    
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${colors[type].bg};
        color: white;
        padding: 1rem 1.5rem;
        border-radius: 10px;
        box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
        z-index: 10000;
        display: flex;
        align-items: center;
        gap: 0.5rem;
        font-weight: 500;
        transform: translateX(100%);
        transition: transform 0.3s ease;
        max-width: 400px;
    `;
    
    notification.innerHTML = `
        <i class="${colors[type].icon}"></i>
        <span>${message}</span>
        <button onclick="this.parentElement.remove()" style="
            background: none;
            border: none;
            color: white;
            font-size: 1.2rem;
            cursor: pointer;
            margin-left: 1rem;
        ">&times;</button>
    `;
    
    document.body.appendChild(notification);
    
    // Animación de entrada
    setTimeout(() => {
        notification.style.transform = 'translateX(0)';
    }, 100);
    
    // Auto-remover después de 5 segundos
    setTimeout(() => {
        if (notification.parentElement) {
            notification.style.transform = 'translateX(100%)';
            setTimeout(() => notification.remove(), 300);
        }
    }, 5000);
}

function showConfirmDialog(title, message, confirmText, cancelText) {
    return new Promise((resolve) => {
        // Crear modal de confirmación personalizado
        const modal = document.createElement('div');
        modal.className = 'cart-confirm-modal';
        modal.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.7);
            backdrop-filter: blur(5px);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 10000;
            opacity: 0;
            transition: opacity 0.3s ease;
        `;
        
        modal.innerHTML = `
            <div class="confirm-dialog" style="
                background: white;
                border-radius: 20px;
                padding: 2rem;
                max-width: 500px;
                margin: 1rem;
                box-shadow: 0 20px 40px rgba(0, 0, 0, 0.2);
                transform: scale(0.9);
                transition: transform 0.3s ease;
            ">
                <h3 style="
                    color: var(--primary, #1F3A5F);
                    margin-bottom: 1rem;
                    font-size: 1.5rem;
                    font-weight: 600;
                ">${title}</h3>
                <p style="
                    color: var(--text-medium, #4A5568);
                    margin-bottom: 2rem;
                    line-height: 1.6;
                ">${message}</p>
                <div style="
                    display: flex;
                    gap: 1rem;
                    justify-content: flex-end;
                ">
                    <button class="cancel-btn" style="
                        background: #E2E8F0;
                        color: #4A5568;
                        border: none;
                        padding: 0.8rem 1.5rem;
                        border-radius: 10px;
                        cursor: pointer;
                        font-weight: 500;
                        transition: all 0.3s ease;
                    ">${cancelText}</button>
                    <button class="confirm-btn" style="
                        background: linear-gradient(135deg, #EF4444, #DC2626);
                        color: white;
                        border: none;
                        padding: 0.8rem 1.5rem;
                        border-radius: 10px;
                        cursor: pointer;
                        font-weight: 500;
                        transition: all 0.3s ease;
                    ">${confirmText}</button>
                </div>
            </div>
        `;
        
        document.body.appendChild(modal);
        document.body.style.overflow = 'hidden';
        
        // Animación de entrada
        setTimeout(() => {
            modal.style.opacity = '1';
            modal.querySelector('.confirm-dialog').style.transform = 'scale(1)';
        }, 100);
        
        // Event listeners
        modal.querySelector('.cancel-btn').onclick = () => {
            closeModal(false);
        };
        
        modal.querySelector('.confirm-btn').onclick = () => {
            closeModal(true);
        };
        
        modal.onclick = (e) => {
            if (e.target === modal) closeModal(false);
        };
        
        function closeModal(result) {
            modal.style.opacity = '0';
            modal.querySelector('.confirm-dialog').style.transform = 'scale(0.9)';
            setTimeout(() => {
                document.body.removeChild(modal);
                document.body.style.overflow = '';
                resolve(result);
            }, 300);
        }
    });
}

function initializeTooltips() {
    // Agregar tooltips informativos
    const tooltips = [
        { selector: '.quantity-btn', text: 'Cambiar cantidad' },
        { selector: '.remove-btn', text: 'Eliminar producto' },
        { selector: '.checkout-btn', text: 'Proceder al pago' },
        { selector: '.continue-shopping-btn', text: 'Seguir comprando' }
    ];
    
    tooltips.forEach(tooltip => {
        const elements = document.querySelectorAll(tooltip.selector);
        elements.forEach(element => {
            element.setAttribute('title', tooltip.text);
            element.setAttribute('data-bs-toggle', 'tooltip');
        });
    });
}

/* ========================================
   CONFIGURACIÓN DE URLS (se define desde la vista)
   ======================================== */

window.cartUrls = window.cartUrls || {
    updateQuantity: '/Carrito/ActualizarCantidad',
    removeProduct: '/Carrito/EliminarProducto',
    clearCart: '/Carrito/LimpiarCarrito',
    checkout: '/Carrito/Checkout'
};
