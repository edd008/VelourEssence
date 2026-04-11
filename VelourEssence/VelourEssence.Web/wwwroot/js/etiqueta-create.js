document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('.etiqueta-create-container form');
    const btn = form.querySelector('button[type="submit"]');
    const mensaje = document.createElement('div');
    mensaje.className = 'etiqueta-ajax-message';
    form.appendChild(mensaje);

    form.addEventListener('submit', function (e) {
        e.preventDefault();
        btn.disabled = true;
        mensaje.textContent = '';
        mensaje.style.display = 'none';

        const formData = new FormData(form);
        fetch(form.action, {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
        .then(response => response.json())
        .then(data => {
            btn.disabled = false;
            if (data.success) {
                mensaje.textContent = 'Etiqueta creada exitosamente.';
                mensaje.style.display = 'block';
                mensaje.classList.add('etiqueta-success');
                form.reset();
            } else {
                mensaje.textContent = data.error || 'Error al crear la etiqueta.';
                mensaje.style.display = 'block';
                mensaje.classList.add('etiqueta-error');
            }
        })
        .catch(() => {
            btn.disabled = false;
            mensaje.textContent = 'Error de conexión.';
            mensaje.style.display = 'block';
            mensaje.classList.add('etiqueta-error');
        });
    });
});
