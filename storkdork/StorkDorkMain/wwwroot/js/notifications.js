document.addEventListener('DOMContentLoaded', function() {
    const notificationItems = document.querySelectorAll('[data-notification-id]');
    
    notificationItems.forEach(item => {
        item.addEventListener('click', async function(e) {
            const notificationId = this.dataset.notificationId;
            try {
                const response = await fetch(`/api/notifications/${notificationId}/markAsRead`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    }
                });
                
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
            } catch (error) {
                console.error('Error marking notification as read:', error);
            }
        });
    });
});