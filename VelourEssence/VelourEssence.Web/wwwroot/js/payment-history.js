// ===== PAYMENT HISTORY JAVASCRIPT =====

document.addEventListener('DOMContentLoaded', function() {
    // Inicializar funcionalidades
    initFilters();
    initSearch();
    // initSorting(); // Función no implementada - comentada temporalmente
    initPagination();
    initExportFunctionality();
    setupPaymentItemInteractions();
    loadPaymentHistory();
});

// Variables globales
let allPayments = [];
let filteredPayments = [];
let currentPage = 1;
const itemsPerPage = 10;

// Inicializar filtros
function initFilters() {
    const dateFilter = document.getElementById('dateFilter');
    const methodFilter = document.getElementById('methodFilter');
    
    if (dateFilter) {
        dateFilter.addEventListener('change', applyFilters);
    }
    
    if (methodFilter) {
        methodFilter.addEventListener('change', applyFilters);
    }
}

// Inicializar búsqueda
function initSearch() {
    const searchFilter = document.getElementById('searchFilter');
    
    if (searchFilter) {
        let searchTimeout;
        searchFilter.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(applyFilters, 300);
        });
    }
}

// Aplicar filtros
function applyFilters() {
    const dateFilter = document.getElementById('dateFilter')?.value || 'all';
    const methodFilter = document.getElementById('methodFilter')?.value || 'all';
    const searchQuery = document.getElementById('searchFilter')?.value.toLowerCase() || '';
    
    filteredPayments = allPayments.filter(payment => {
        // Filtro por fecha
        if (dateFilter !== 'all') {
            const paymentDate = new Date(payment.fecha);
            const now = new Date();
            
            switch(dateFilter) {
                case 'today':
                    if (paymentDate.toDateString() !== now.toDateString()) return false;
                    break;
                case 'week':
                    const weekAgo = new Date(now - 7 * 24 * 60 * 60 * 1000);
                    if (paymentDate < weekAgo) return false;
                    break;
                case 'month':
                    if (paymentDate.getMonth() !== now.getMonth() || 
                        paymentDate.getFullYear() !== now.getFullYear()) return false;
                    break;
                case 'year':
                    if (paymentDate.getFullYear() !== now.getFullYear()) return false;
                    break;
            }
        }
        
        // Filtro por método de pago
        if (methodFilter !== 'all') {
            const method = payment.metodo.toLowerCase();
            if (methodFilter === 'tarjeta' && !method.includes('tarjeta')) return false;
            if (methodFilter === 'efectivo' && method !== 'efectivo') return false;
        }
        
        // Filtro por búsqueda
        if (searchQuery) {
            const searchFields = [
                payment.numeroPedido,
                payment.metodo,
                payment.monto.toString()
            ].join(' ').toLowerCase();
            
            if (!searchFields.includes(searchQuery)) return false;
        }
        
        return true;
    });
    
    currentPage = 1;
    renderPayments();
    updateStats();
}

// Cargar historial de pagos (simular carga desde servidor)
function loadPaymentHistory() {
    // Obtener pagos existentes del DOM
    const paymentItems = document.querySelectorAll('.payment-item');
    allPayments = [];
    
    paymentItems.forEach((item, index) => {
        const orderNumber = item.querySelector('.payment-order-number')?.textContent || `#${index + 1}`;
        const method = item.querySelector('.payment-method')?.textContent || 'Desconocido';
        const amount = item.querySelector('.payment-amount')?.textContent.replace(/[$,]/g, '') || '0';
        const dateStr = item.querySelector('.payment-date')?.textContent || new Date().toLocaleDateString();
        
        allPayments.push({
            id: index + 1,
            numeroPedido: orderNumber,
            metodo: method,
            monto: parseFloat(amount),
            fecha: dateStr,
            elemento: item
        });
    });
    
    filteredPayments = [...allPayments];
    updateStats();
}

// Renderizar pagos filtrados
function renderPayments() {
    const paymentList = document.querySelector('.payment-list');
    if (!paymentList) return;
    
    // Limpiar lista actual
    paymentList.innerHTML = '';
    
    // Calcular items para la página actual
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    const paymentsToShow = filteredPayments.slice(startIndex, endIndex);
    
    // Mostrar mensaje si no hay resultados
    if (paymentsToShow.length === 0) {
        paymentList.innerHTML = `
            <div class="no-results">
                <div class="no-results-icon">
                    <i class="bi bi-search"></i>
                </div>
                <h3>No se encontraron pagos</h3>
                <p>Intenta ajustar los filtros de búsqueda</p>
                <button class="btn-secondary-custom" onclick="clearFilters()">
                    <i class="bi bi-arrow-clockwise"></i>
                    Limpiar filtros
                </button>
            </div>
        `;
        return;
    }
    
    // Renderizar pagos
    paymentsToShow.forEach((payment, index) => {
        const paymentElement = payment.elemento.cloneNode(true);
        paymentElement.style.animation = `fadeInUp 0.5s ease ${index * 0.1}s both`;
        paymentList.appendChild(paymentElement);
    });
    
    // Actualizar paginación
    updatePagination();
}

// Actualizar estadísticas
function updateStats() {
    const statCards = document.querySelectorAll('.stat-card');
    if (statCards.length === 0) return;
    
    const totalPayments = filteredPayments.length;
    const totalAmount = filteredPayments.reduce((sum, payment) => sum + payment.monto, 0);
    const cardPayments = filteredPayments.filter(p => p.metodo.toLowerCase().includes('tarjeta')).length;
    const cashPayments = filteredPayments.filter(p => p.metodo.toLowerCase() === 'efectivo').length;
    
    // Animar actualización de stats
    animateStatUpdate(statCards[0]?.querySelector('h3'), totalPayments);
    animateStatUpdate(statCards[1]?.querySelector('h3'), totalAmount, true);
    animateStatUpdate(statCards[2]?.querySelector('h3'), cardPayments);
    animateStatUpdate(statCards[3]?.querySelector('h3'), cashPayments);
}

// Animar actualización de estadísticas
function animateStatUpdate(element, newValue, isCurrency = false) {
    if (!element) return;
    
    const currentValue = parseFloat(element.textContent.replace(/[$,]/g, '')) || 0;
    const increment = (newValue - currentValue) / 20;
    let current = currentValue;
    
    const animation = setInterval(() => {
        current += increment;
        if ((increment > 0 && current >= newValue) || (increment < 0 && current <= newValue)) {
            current = newValue;
            clearInterval(animation);
        }
        
        if (isCurrency) {
            element.textContent = `$${current.toFixed(2)}`;
        } else {
            element.textContent = Math.round(current).toString();
        }
    }, 50);
}

// Configurar paginación
function updatePagination() {
    const totalPages = Math.ceil(filteredPayments.length / itemsPerPage);
    const paginationSection = document.querySelector('.pagination-section');
    
    if (!paginationSection || totalPages <= 1) {
        if (paginationSection) paginationSection.style.display = 'none';
        return;
    }
    
    paginationSection.style.display = 'flex';
    
    const paginationInfo = paginationSection.querySelector('.pagination-info');
    const startItem = (currentPage - 1) * itemsPerPage + 1;
    const endItem = Math.min(currentPage * itemsPerPage, filteredPayments.length);
    
    if (paginationInfo) {
        paginationInfo.textContent = `Mostrando ${startItem}-${endItem} de ${filteredPayments.length} pagos`;
    }
    
    // Actualizar botones de navegación
    const prevBtn = paginationSection.querySelector('.pagination-btn:first-of-type');
    const nextBtn = paginationSection.querySelector('.pagination-btn:last-of-type');
    const currentPageSpan = paginationSection.querySelector('.pagination-current');
    
    if (prevBtn) {
        prevBtn.disabled = currentPage === 1;
        prevBtn.onclick = () => changePage(currentPage - 1);
    }
    
    if (nextBtn) {
        nextBtn.disabled = currentPage === totalPages;
        nextBtn.onclick = () => changePage(currentPage + 1);
    }
    
    if (currentPageSpan) {
        currentPageSpan.textContent = currentPage;
    }
}

// Cambiar página
function changePage(page) {
    const totalPages = Math.ceil(filteredPayments.length / itemsPerPage);
    if (page < 1 || page > totalPages) return;
    
    currentPage = page;
    renderPayments();
    
    // Scroll suave hacia arriba
    document.querySelector('.payment-list-section')?.scrollIntoView({ 
        behavior: 'smooth' 
    });
}

// Limpiar filtros
function clearFilters() {
    document.getElementById('dateFilter').value = 'all';
    document.getElementById('methodFilter').value = 'all';
    document.getElementById('searchFilter').value = '';
    applyFilters();
}

// Configurar interacciones de items de pago
function setupPaymentItemInteractions() {
    document.addEventListener('click', function(e) {
        // Expandir/contraer detalles del pago
        if (e.target.closest('.payment-item-header')) {
            const paymentItem = e.target.closest('.payment-item');
            const details = paymentItem.querySelector('.payment-item-details');
            
            if (details.style.display === 'none' || !details.style.display) {
                details.style.display = 'block';
                details.style.animation = 'slideDown 0.3s ease';
                paymentItem.classList.add('expanded');
            } else {
                details.style.animation = 'slideUp 0.3s ease';
                setTimeout(() => {
                    details.style.display = 'none';
                    paymentItem.classList.remove('expanded');
                }, 300);
            }
        }
    });
}

// Funcionalidad de exportación
function initExportFunctionality() {
    // El botón ya está configurado en el HTML
}

function exportPayments() {
    const csvContent = generateCSV();
    downloadCSV(csvContent, `historial-pagos-${new Date().toISOString().split('T')[0]}.csv`);
}

function generateCSV() {
    const headers = ['Número de Pedido', 'Método de Pago', 'Monto', 'Fecha', 'Estado'];
    const rows = filteredPayments.map(payment => [
        payment.numeroPedido,
        payment.metodo,
        payment.monto.toFixed(2),
        payment.fecha,
        'Completado'
    ]);
    
    const csvContent = [headers, ...rows]
        .map(row => row.map(field => `"${field}"`).join(','))
        .join('\n');
        
    return csvContent;
}

function downloadCSV(content, filename) {
    const blob = new Blob([content], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    
    if (link.download !== undefined) {
        const url = URL.createObjectURL(blob);
        link.setAttribute('href', url);
        link.setAttribute('download', filename);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
}

// Funciones específicas del historial
function downloadReceipt(pagoId) {
    // Simular descarga de recibo
    const payment = allPayments.find(p => p.id === pagoId);
    if (payment) {
        showNotification(`Descargando recibo para ${payment.numeroPedido}...`, 'info');
        
        // Simular descarga
        setTimeout(() => {
            showNotification('Recibo descargado exitosamente', 'success');
        }, 2000);
    }
}

function viewDetails(pagoId) {
    // Mostrar modal con detalles del pago
    const payment = allPayments.find(p => p.id === pagoId);
    if (payment) {
        showPaymentModal(payment);
    }
}

function showPaymentModal(payment) {
    const modal = document.createElement('div');
    modal.className = 'payment-modal-overlay';
    modal.innerHTML = `
        <div class="payment-modal">
            <div class="payment-modal-header">
                <h3>Detalles del Pago ${payment.numeroPedido}</h3>
                <button class="close-modal" onclick="this.closest('.payment-modal-overlay').remove()">
                    <i class="bi bi-x"></i>
                </button>
            </div>
            <div class="payment-modal-body">
                <div class="modal-detail-row">
                    <span>Número de Pedido:</span>
                    <strong>${payment.numeroPedido}</strong>
                </div>
                <div class="modal-detail-row">
                    <span>Método de Pago:</span>
                    <strong>${payment.metodo}</strong>
                </div>
                <div class="modal-detail-row">
                    <span>Monto:</span>
                    <strong>$${payment.monto.toFixed(2)}</strong>
                </div>
                <div class="modal-detail-row">
                    <span>Fecha:</span>
                    <strong>${payment.fecha}</strong>
                </div>
                <div class="modal-detail-row">
                    <span>Estado:</span>
                    <strong class="status-completed">Completado</strong>
                </div>
            </div>
            <div class="payment-modal-footer">
                <button class="btn-primary-custom" onclick="downloadReceipt(${payment.id})">
                    <i class="bi bi-download"></i>
                    Descargar Recibo
                </button>
                <button class="btn-secondary-custom" onclick="this.closest('.payment-modal-overlay').remove()">
                    Cerrar
                </button>
            </div>
        </div>
    `;
    
    document.body.appendChild(modal);
    
    // Cerrar modal al hacer click fuera
    modal.addEventListener('click', function(e) {
        if (e.target === modal) {
            modal.remove();
        }
    });
}

// Mostrar notificaciones
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.innerHTML = `
        <i class="bi bi-${type === 'success' ? 'check-circle' : 'info-circle'}"></i>
        <span>${message}</span>
    `;
    
    document.body.appendChild(notification);
    
    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

// Estilos CSS adicionales
const style = document.createElement('style');
style.textContent = `
    @keyframes fadeInUp {
        0% {
            opacity: 0;
            transform: translateY(20px);
        }
        100% {
            opacity: 1;
            transform: translateY(0);
        }
    }
    
    @keyframes slideDown {
        0% {
            max-height: 0;
            opacity: 0;
        }
        100% {
            max-height: 500px;
            opacity: 1;
        }
    }
    
    @keyframes slideUp {
        0% {
            max-height: 500px;
            opacity: 1;
        }
        100% {
            max-height: 0;
            opacity: 0;
        }
    }
    
    .no-results {
        text-align: center;
        padding: 4rem 2rem;
        background: white;
        border-radius: 15px;
        box-shadow: 0 5px 20px rgba(31, 58, 95, 0.08);
    }
    
    .no-results-icon {
        width: 80px;
        height: 80px;
        margin: 0 auto 1rem;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 2rem;
        background: var(--primary-light);
        color: var(--primary);
    }
    
    .payment-modal-overlay {
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: rgba(0, 0, 0, 0.5);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 1000;
        animation: fadeIn 0.3s ease;
    }
    
    .payment-modal {
        background: white;
        border-radius: 15px;
        max-width: 500px;
        width: 90%;
        max-height: 80%;
        overflow-y: auto;
        animation: slideInModal 0.3s ease;
    }
    
    @keyframes slideInModal {
        0% {
            transform: scale(0.8) translateY(-50px);
            opacity: 0;
        }
        100% {
            transform: scale(1) translateY(0);
            opacity: 1;
        }
    }
    
    .payment-modal-header {
        padding: 1.5rem;
        border-bottom: 1px solid var(--border);
        display: flex;
        justify-content: space-between;
        align-items: center;
    }
    
    .payment-modal-header h3 {
        margin: 0;
        color: var(--primary);
    }
    
    .close-modal {
        background: none;
        border: none;
        font-size: 1.5rem;
        cursor: pointer;
        color: var(--text-muted);
        padding: 0.5rem;
        border-radius: 50%;
        transition: all 0.3s ease;
    }
    
    .close-modal:hover {
        background: var(--border);
        color: var(--primary);
    }
    
    .payment-modal-body {
        padding: 1.5rem;
    }
    
    .modal-detail-row {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 0.75rem 0;
        border-bottom: 1px solid rgba(224, 224, 224, 0.5);
    }
    
    .payment-modal-footer {
        padding: 1.5rem;
        border-top: 1px solid var(--border);
        display: flex;
        gap: 1rem;
        justify-content: flex-end;
    }
    
    .notification {
        position: fixed;
        top: 20px;
        right: 20px;
        background: white;
        border-left: 4px solid var(--accent);
        padding: 1rem 1.5rem;
        border-radius: 8px;
        box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
        display: flex;
        align-items: center;
        gap: 0.5rem;
        z-index: 1001;
        animation: slideIn 0.3s ease;
    }
    
    .notification-success {
        border-left-color: var(--accent-coral);
    }
    
    @keyframes slideIn {
        0% {
            transform: translateX(100%);
            opacity: 0;
        }
        100% {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOut {
        0% {
            transform: translateX(0);
            opacity: 1;
        }
        100% {
            transform: translateX(100%);
            opacity: 0;
        }
    }
    
    .payment-item.expanded {
        box-shadow: 0 15px 35px rgba(31, 58, 95, 0.15);
    }
    
    .payment-item-header {
        cursor: pointer;
    }
    
    .payment-item-header:hover {
        background: rgba(168, 230, 207, 0.08);
    }
`;
document.head.appendChild(style);
