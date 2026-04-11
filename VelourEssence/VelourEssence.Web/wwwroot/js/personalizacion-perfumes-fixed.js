/* ========================================
   JAVASCRIPT PARA PERSONALIZACIÓN DE PERFUMES
   ======================================== */

class PersonalizacionPerfumes {
    constructor() {
        this.baseProductoId = window.baseProductoId;
        this.basePrice = window.basePrice;
        this.currentSelections = new Map();
        this.criterios = [];
        this.precioPersonalizaciones = 0;
        this.texts = window.personalizacionTexts;
        
        this.init();
    }

    async init() {
        try {
            this.cargarCriteriosEstaticos();
            this.renderizarCriterios();
            this.configurarEventListeners();
            this.actualizarVistaPrevia();
        } catch (error) {
            console.error('Error inicializando personalización:', error);
            this.mostrarError('Error al cargar las opciones de personalización');
        }
    }

    cargarCriteriosEstaticos() {
        // Datos estáticos para concentración y tamaño
        this.criterios = [
            {
                idCriterio: 1,
                nombre: 'Concentración del Perfume',
                esObligatorio: true,
                opciones: [
                    {
                        idOpcion: 1,
                        nombre: 'Eau de Cologne',
                        precioAdicional: 0,
                        descripcion: '5-8% de concentración - Ligera y refrescante'
                    },
                    {
                        idOpcion: 2,
                        nombre: 'Eau de Toilette',
                        precioAdicional: 2500,
                        descripcion: '8-12% de concentración - Fresca y versátil'
                    },
                    {
                        idOpcion: 3,
                        nombre: 'Eau de Parfum',
                        precioAdicional: 5000,
                        descripcion: '12-20% de concentración - Intensa y duradera'
                    },
                    {
                        idOpcion: 4,
                        nombre: 'Parfum',
                        precioAdicional: 10000,
                        descripcion: '20-30% de concentración - Máxima intensidad'
                    }
                ]
            },
            {
                idCriterio: 2,
                nombre: 'Tamaño del Frasco',
                esObligatorio: true,
                opciones: [
                    {
                        idOpcion: 5,
                        nombre: '30ml',
                        precioAdicional: 0,
                        descripcion: 'Tamaño compacto - Ideal para viajes'
                    },
                    {
                        idOpcion: 6,
                        nombre: '50ml',
                        precioAdicional: 3000,
                        descripcion: 'Tamaño estándar - Perfecto para uso diario'
                    },
                    {
                        idOpcion: 7,
                        nombre: '100ml',
                        precioAdicional: 7500,
                        descripcion: 'Tamaño grande - Mayor duración'
                    },
                    {
                        idOpcion: 8,
                        nombre: '150ml',
                        precioAdicional: 12000,
                        descripcion: 'Tamaño extra grande - Máximo valor'
                    }
                ]
            }
        ];
    }

    renderizarCriterios() {
        const container = document.getElementById('criteriosContainer');
        if (!container) return;

        container.innerHTML = '';

        this.criterios.forEach(criterio => {
            const criterioElement = this.crearCriterioElement(criterio);
            container.appendChild(criterioElement);
        });
    }

    crearCriterioElement(criterio) {
        const div = document.createElement('div');
        div.className = 'criterio-group';
        div.setAttribute('data-criterio-id', criterio.idCriterio);

        const iconos = {
            'Concentración del Perfume': 'fas fa-tint',
            'Tamaño del Frasco': 'fas fa-wine-bottle'
        };

        div.innerHTML = `
            <h4 class="criterio-title">
                <i class="${iconos[criterio.nombre] || 'fas fa-cog'}"></i>
                ${criterio.nombre}
                ${criterio.esObligatorio ? '<span class="text-danger">*</span>' : ''}
            </h4>
            <div class="opciones-grid" data-criterio-id="${criterio.idCriterio}">
                ${criterio.opciones.map(opcion => this.crearOpcionElement(criterio.idCriterio, opcion)).join('')}
            </div>
        `;

        return div;
    }

    crearOpcionElement(criterioId, opcion) {
        const iconosOpciones = {
            // Concentraciones
            'Eau de Cologne': 'fas fa-tint opacity-25',
            'Eau de Toilette': 'fas fa-tint opacity-50', 
            'Eau de Parfum': 'fas fa-tint opacity-75',
            'Parfum': 'fas fa-tint',
            // Tamaños
            '30ml': 'fas fa-wine-bottle text-small',
            '50ml': 'fas fa-wine-bottle',
            '100ml': 'fas fa-wine-bottle text-large',
            '150ml': 'fas fa-wine-bottle text-xl'
        };

        const precioTexto = opcion.precioAdicional > 0 
            ? `+₡${opcion.precioAdicional.toLocaleString()}` 
            : opcion.precioAdicional < 0 
                ? `-₡${Math.abs(opcion.precioAdicional).toLocaleString()}`
                : 'Incluido';

        return `
            <div class="opcion-card" 
                 data-criterio-id="${criterioId}" 
                 data-opcion-id="${opcion.idOpcion}"
                 onclick="personalizacion.seleccionarOpcion(${criterioId}, ${opcion.idOpcion})">
                <div class="opcion-icon">
                    <i class="${iconosOpciones[opcion.nombre] || 'fas fa-star'}"></i>
                </div>
                <div class="opcion-nombre">${opcion.nombre}</div>
                <div class="opcion-precio">${precioTexto}</div>
            </div>
        `;
    }

    seleccionarOpcion(criterioId, opcionId) {
        // Quitar selección anterior del mismo criterio
        const criterioContainer = document.querySelector(`[data-criterio-id="${criterioId}"] .opciones-grid`);
        criterioContainer.querySelectorAll('.opcion-card').forEach(card => {
            card.classList.remove('selected');
        });

        // Agregar selección nueva
        const opcionCard = document.querySelector(`[data-criterio-id="${criterioId}"][data-opcion-id="${opcionId}"]`);
        opcionCard.classList.add('selected');

        // Actualizar selecciones
        this.currentSelections.set(criterioId, opcionId);

        // Actualizar vista previa y precio
        this.actualizarPrecio();
        this.actualizarVistaPrevia();
        this.validarSelecciones();
    }

    actualizarPrecio() {
        const precioElement = document.getElementById('precioPersonalizaciones');
        const totalElement = document.getElementById('precioTotal');
        
        // Calcular precio local
        let precioPersonalizaciones = 0;
        
        this.currentSelections.forEach((opcionId, criterioId) => {
            const criterio = this.criterios.find(c => c.idCriterio === criterioId);
            if (criterio) {
                const opcion = criterio.opciones.find(o => o.idOpcion === opcionId);
                if (opcion) {
                    precioPersonalizaciones += opcion.precioAdicional;
                }
            }
        });

        this.precioPersonalizaciones = precioPersonalizaciones;
        const precioTotal = this.basePrice + precioPersonalizaciones;

        // Actualizar UI
        precioElement.textContent = `₡${precioPersonalizaciones.toLocaleString()}`;
        totalElement.textContent = `₡${precioTotal.toLocaleString()}`;
        totalElement.classList.add('precio-updating');
        
        setTimeout(() => {
            totalElement.classList.remove('precio-updating');
        }, 300);
    }

    actualizarVistaPrevia() {
        this.actualizarBotella();
        this.actualizarConcentracion();
    }

    actualizarConcentracion() {
        const concentrationText = document.getElementById('concentrationText');
        const concentrationDesc = document.getElementById('concentrationDesc');
        const concentrationIcon = document.getElementById('concentrationIcon');
        
        // Buscar selección de concentración
        const criterioConcentracion = this.criterios.find(c => c.nombre === 'Concentración del Perfume');
        if (!criterioConcentracion) return;

        const opcionSeleccionada = criterioConcentracion.idCriterio && this.currentSelections.has(criterioConcentracion.idCriterio)
            ? criterioConcentracion.opciones.find(o => o.idOpcion === this.currentSelections.get(criterioConcentracion.idCriterio))
            : null;

        if (opcionSeleccionada) {
            // Actualizar texto principal
            concentrationText.textContent = opcionSeleccionada.nombre;
            
            // Actualizar descripción
            concentrationDesc.textContent = opcionSeleccionada.descripcion;
            
            // Configuraciones para cada concentración
            const configuraciones = {
                'Eau de Cologne': {
                    icono: 'fas fa-tint',
                    clase: 'concentration-light'
                },
                'Eau de Toilette': {
                    icono: 'fas fa-tint',
                    clase: 'concentration-medium'
                },
                'Eau de Parfum': {
                    icono: 'fas fa-tint',
                    clase: 'concentration-strong'
                },
                'Parfum': {
                    icono: 'fas fa-gem',
                    clase: 'concentration-premium'
                }
            };
            
            const config = configuraciones[opcionSeleccionada.nombre] || configuraciones['Eau de Cologne'];
            
            // Actualizar icono
            concentrationIcon.className = config.icono;
            
            // Añadir animación de actualización
            concentrationText.classList.add('text-update-animation');
            concentrationDesc.classList.add('text-update-animation');
            
            // Remover animación después de un tiempo
            setTimeout(() => {
                concentrationText.classList.remove('text-update-animation');
                concentrationDesc.classList.remove('text-update-animation');
            }, 500);
        } else {
            // Valores por defecto cuando no hay selección
            concentrationText.textContent = 'Selecciona concentración';
            concentrationDesc.textContent = 'Elige el tipo de concentración para tu perfume';
            concentrationIcon.className = 'fas fa-tint';
        }
    }

    actualizarBotella() {
        const bottleLabel = document.getElementById('bottleLabel');
        const perfumeBottle = document.getElementById('perfumeBottle');
        
        // Buscar selección de tamaño
        const criterioTamano = this.criterios.find(c => c.nombre === 'Tamaño del Frasco');
        if (!criterioTamano) return;

        const opcionSeleccionada = criterioTamano.idCriterio && this.currentSelections.has(criterioTamano.idCriterio)
            ? criterioTamano.opciones.find(o => o.idOpcion === this.currentSelections.get(criterioTamano.idCriterio))
            : null;

        if (opcionSeleccionada) {
            // Actualizar etiqueta de tamaño en la botella
            bottleLabel.textContent = opcionSeleccionada.nombre;
            
            // Configuraciones para diferentes tamaños
            const configuracionesTamano = {
                '30ml': { 
                    escala: 0.8,
                    clase: 'size-small'
                },
                '50ml': { 
                    escala: 1,
                    clase: 'size-medium'
                },
                '100ml': { 
                    escala: 1.2,
                    clase: 'size-large'
                },
                '150ml': { 
                    escala: 1.4,
                    clase: 'size-xlarge'
                }
            };

            const config = configuracionesTamano[opcionSeleccionada.nombre] || configuracionesTamano['50ml'];
            
            // Aplicar escala
            perfumeBottle.style.transform = `scale(${config.escala})`;
            
            // Remover clases anteriores de tamaño
            perfumeBottle.classList.remove('size-small', 'size-medium', 'size-large', 'size-xlarge');
            perfumeBottle.classList.add(config.clase);
            
            // Añadir animación de actualización
            bottleLabel.classList.add('text-update-animation');
            perfumeBottle.classList.add('bottle-scale-animation');
            
            // Remover animación después de un tiempo
            setTimeout(() => {
                bottleLabel.classList.remove('text-update-animation');
                perfumeBottle.classList.remove('bottle-scale-animation');
            }, 500);
        } else {
            // Valores por defecto cuando no hay selección
            bottleLabel.textContent = '50ml';
            perfumeBottle.style.transform = 'scale(1)';
        }
    }

    validarSelecciones() {
        const validationContainer = document.getElementById('validationMessages');
        const agregarBtn = document.getElementById('agregarCarritoBtn');
        
        // Verificar criterios obligatorios
        const criteriosObligatorios = this.criterios.filter(c => c.esObligatorio);
        const seleccionesCompletas = criteriosObligatorios.every(criterio => 
            this.currentSelections.has(criterio.idCriterio)
        );

        if (seleccionesCompletas) {
            validationContainer.style.display = 'none';
            agregarBtn.disabled = false;
            agregarBtn.innerHTML = '<i class="fas fa-shopping-cart"></i> Agregar al Carrito';
        } else {
            const criteriosFaltantes = criteriosObligatorios.filter(criterio => 
                !this.currentSelections.has(criterio.idCriterio)
            );

            validationContainer.innerHTML = `
                <div class="validation-message">
                    <i class="fas fa-exclamation-triangle"></i>
                    Por favor selecciona: ${criteriosFaltantes.map(c => c.nombre).join(', ')}
                </div>
            `;
            validationContainer.style.display = 'block';
            agregarBtn.disabled = true;
        }
    }

    configurarEventListeners() {
        const agregarBtn = document.getElementById('agregarCarritoBtn');
        if (agregarBtn) {
            agregarBtn.addEventListener('click', () => this.agregarAlCarrito());
        }
    }

    async agregarAlCarrito() {
        const btn = document.getElementById('agregarCarritoBtn');
        const originalText = btn.innerHTML;
        
        try {
            btn.disabled = true;
            btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Agregando...';
            
            // Simular llamada al servidor
            await new Promise(resolve => setTimeout(resolve, 1000));
            
            // Mostrar éxito
            btn.innerHTML = '<i class="fas fa-check"></i> ¡Agregado!';
            btn.classList.add('btn-success');
            
            setTimeout(() => {
                btn.innerHTML = originalText;
                btn.disabled = false;
                btn.classList.remove('btn-success');
            }, 2000);
            
        } catch (error) {
            console.error('Error agregando al carrito:', error);
            btn.innerHTML = originalText;
            btn.disabled = false;
            this.mostrarError('Error al agregar al carrito');
        }
    }

    mostrarError(mensaje) {
        const container = document.getElementById('validationMessages');
        container.innerHTML = `
            <div class="validation-message">
                <i class="fas fa-exclamation-circle"></i>
                ${mensaje}
            </div>
        `;
        container.style.display = 'block';
    }
}

// CSS adicional para animaciones
const style = document.createElement('style');
style.textContent = `
    .text-update-animation {
        animation: textUpdate 0.5s ease-in-out;
    }
    
    .bottle-scale-animation {
        animation: bottleScale 0.5s ease-in-out;
    }
    
    @keyframes textUpdate {
        0%, 100% { transform: scale(1); }
        50% { transform: scale(1.05); color: var(--detail-accent); }
    }
    
    @keyframes bottleScale {
        0%, 100% { transform: scale(var(--current-scale, 1)); }
        50% { transform: scale(calc(var(--current-scale, 1) * 1.1)); }
    }
    
    .btn-success {
        background: var(--detail-success) !important;
    }
`;
document.head.appendChild(style);

// Inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
    window.personalizacion = new PersonalizacionPerfumes();
});
