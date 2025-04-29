document.addEventListener('DOMContentLoaded', function() {
    // Handle toggling read status in notifications overview
    document.addEventListener('click', async function(e) {
        const toggleButton = e.target.closest('.toggle-read');
        if (toggleButton) {
            e.preventDefault();
            e.stopPropagation();
            
            // Disable button while processing
            toggleButton.disabled = true;
            
            const notificationId = toggleButton.dataset.notificationId;
            const notificationCard = toggleButton.closest('.notification-item');
            const messageElement = notificationCard.querySelector('p');
            const currentIsRead = toggleButton.textContent.trim() === 'Mark as Unread';
            
            try {
                const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
                const response = await fetch(`/Notifications/ToggleRead/${notificationId}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': tokenElement.value
                    }
                });
                
                if (response.ok) {
                    const result = await response.json();
                    if (result.success) {
                        // Toggle state only after successful server response
                        const newIsRead = !currentIsRead;
                        
                        // Update notification card
                        notificationCard.classList.toggle('border-primary', !newIsRead);
                        messageElement.classList.toggle('fw-bold', !newIsRead);
                        
                        // Update button state
                        toggleButton.textContent = newIsRead ? 'Mark as Unread' : 'Mark as Read';
                        toggleButton.classList.toggle('btn-outline-secondary', newIsRead);
                        toggleButton.classList.toggle('btn-outline-primary', !newIsRead);
                        
                        // Update data attribute for filtering
                        notificationCard.dataset.isRead = newIsRead.toString();
                        
                        // Get fresh count from server
                        await updateBadgeCount();
                        
                        // Update filtered view if needed
                        const activeFilter = document.querySelector('.filter-buttons .btn.active');
                        if (activeFilter) {
                            updateFilteredView(activeFilter.dataset.filter);
                        }
                    } else {
                        // Revert any UI changes if server update failed
                        console.error('Server returned success: false');
                    }
                } else {
                    throw new Error('Server returned status: ' + response.status);
                }
            } catch (error) {
                console.error('Error toggling notification status:', error);
                // Could add visual feedback here for the error
            } finally {
                toggleButton.disabled = false;
            }
        }
    });

    // Filter buttons functionality
    const filterButtons = document.querySelectorAll('.filter-buttons .btn');
    filterButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            filterButtons.forEach(btn => btn.classList.remove('active'));
            this.classList.add('active');
            updateFilteredView(this.dataset.filter);
        });
    });
});

async function updateBadgeCount() {
    try {
        const response = await fetch('/Notifications/GetUnreadCount', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const unreadCount = await response.json();
            const badge = document.querySelector('.notification-badge');
            
            if (unreadCount > 0) {
                if (badge) {
                    badge.textContent = unreadCount;
                } else {
                    // Recreate badge if it was removed but now needed
                    const bellIcon = document.querySelector('.bi-bell');
                    const newBadge = document.createElement('span');
                    newBadge.className = 'position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger notification-badge';
                    newBadge.textContent = unreadCount;
                    bellIcon.parentElement.appendChild(newBadge);
                }
            } else if (badge) {
                badge.remove();
            }
        }
    } catch (error) {
        console.error('Error updating notification count:', error);
    }
}

function updateFilteredView(filterType) {
    const notificationItems = document.querySelectorAll('.notification-item');
    notificationItems.forEach(item => {
        const isRead = item.dataset.isRead === 'true';
        const shouldShow = 
            filterType === 'all' || 
            (filterType === 'read' && isRead) || 
            (filterType === 'unread' && !isRead);
            
        item.style.display = shouldShow ? 'block' : 'none';
    });
}

function updateNotificationUI(notificationId, isRead) {
    // Update dropdown item based on read status
    const dropdownItem = document.querySelector(`.dropdown-item[data-notification-id="${notificationId}"]`);
    if (dropdownItem) {
        dropdownItem.classList.toggle('fw-bold', !isRead);
    }

    // Update badge count from server to ensure accuracy
    updateBadgeCount();
}

// Replace the dropdown click handler
document.querySelector('.notification-dropdown').addEventListener('click', async function(e) {
    const notificationItem = e.target.closest('.dropdown-item[data-notification-id]');
    if (!notificationItem) return;
    
    e.preventDefault();
    const notificationId = notificationItem.dataset.notificationId;
    const href = notificationItem.href;
    const isUnread = notificationItem.classList.contains('fw-bold');

    // If notification is already read, just navigate
    if (!isUnread) {
        if (href) {
            window.location.href = href;
        }
        return;
    }

    // Only proceed with marking as read if notification was unread
    try {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        if (!tokenElement) {
            console.error('Antiforgery token not found');
            return;
        }

        const response = await fetch(`/Notifications/ToggleRead/${notificationId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': tokenElement.value
            }
        });

        if (response.ok) {
            const result = await response.json();
            if (result.success) {
                updateNotificationUI(notificationId, true); // Mark as read
                
                // Navigate after successful update
                if (href) {
                    window.location.href = href;
                }
            }
        }
    } catch (error) {
        console.error('Error marking notification as read:', error);
    }
});