// site.js - Custom Front-End Helpers and Notifications
window.showToast = function (message, type = 'success') {
    const container = document.getElementById('toast-container');
    if (!container) {
        // Fallback to console or alert if container is missing
        console.log(`[${type}] ${message}`);
        return;
    }
    
    const toast = document.createElement('div');
    toast.className = `custom-toast ${type}`;
    
    let icon = 'fa-check-circle';
    if (type === 'error') icon = 'fa-exclamation-circle';
    if (type === 'info') icon = 'fa-info-circle';
    if (type === 'warning') icon = 'fa-exclamation-triangle';
    
    toast.innerHTML = `
        <i class="fas ${icon} toast-icon"></i>
        <div class="toast-content">${message}</div>
        <button class="toast-close-btn" onclick="this.parentElement.remove()">&times;</button>
    `;
    
    container.appendChild(toast);
    
    // Auto-remove after 4 seconds
    setTimeout(() => {
        toast.classList.add('fade-out-toast');
        setTimeout(() => toast.remove(), 400);
    }, 4000);
};
