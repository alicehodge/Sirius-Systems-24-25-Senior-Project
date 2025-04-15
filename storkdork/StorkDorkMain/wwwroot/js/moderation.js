function showApprovalModal(contentId) {
    $('#approvalModal').modal('show');
    $('#submit-approval').data('content-id', contentId);
}

function showRejectionModal(contentId) {
    $('#rejectionModal').modal('show');
    $('#submit-rejection').data('content-id', contentId);
}

$(document).ready(function() {
    // Get the antiforgery token
    const token = $('input[name="__RequestVerificationToken"]').val();

    // Filter functionality
    function filterContent(status) {
        $.ajax({
            url: '/Moderation/FilterByStatus',
            type: 'GET',
            data: { status: status },
            success: function(response) {
                $('#contentList').html(response);
                updateActiveFilter(status);
            },
            error: function(xhr, status, error) {
                console.error('Error filtering content:', error);
            }
        });
    }

    function updateActiveFilter(status) {
        $('.filter-button').removeClass('active');
        $(`.filter-button[data-filter="${status}"]`).addClass('active');
        $('.btn-group .btn').removeClass('active');
        $(`.btn-group .btn:contains("${status}")`).addClass('active');
    }

    // Event handlers for filter buttons
    $('.filter-button, .btn-group .btn').click(function() {
        const status = $(this).data('filter') || $(this).text();
        filterContent(status);
    });

    // Approval/Rejection functionality
    $('#submit-approval').click(function() {
        handleModeration('approve');
    });

    $('#submit-rejection').click(function() {
        handleModeration('reject');
    });

    function handleModeration(action) {
        const contentId = $('#current-content-id').val();
        const notes = action === 'approve' ? 
            $('#moderator-notes').val() :
            $('#rejection-reason').val();
        
        if (!notes) {
            alert(`Please enter ${action === 'approve' ? 'moderator notes' : 'rejection reason'}`);
            return;
        }

        $.ajax({
            url: `/Moderation/${action === 'approve' ? 'ApproveContent' : 'RejectContent'}`,
            type: 'POST',
            data: {
                id: contentId,
                notes: notes
            },
            headers: {
                'RequestVerificationToken': token
            },
            success: function(response) {
                $(`#${action}Modal`).modal('hide');
                filterContent('Pending'); // Refresh pending content
                resetForm(action);
            },
            error: function(xhr, status, error) {
                alert(`Error ${action}ing content: ${error}`);
            }
        });
    }

    function resetForm(action) {
        if (action === 'approve') {
            $('#moderator-notes').val('');
        } else {
            $('#rejection-reason').val('');
        }
    }
});