// ACTIVITY 6 (Partial Views): handles the Save button inside the edit-profile
// modal. The form is loaded dynamically (see modalHandler.js), so the click
// handler is delegated from document. Submitting via AJAX means the profile
// gets updated - and the nav bar greeting refreshed - without a full page
// reload, matching the ProductApp formHandler.js pattern from Activity 6.
$(document).ready(function () {
    $(document).on('click', '#saveProfileBtn', function (e) {
        e.preventDefault();

        var formData = $('#editProfileForm').serialize();

        $.ajax({
            url: '/Account/UpdateProfileAjax',
            type: 'POST',
            data: formData,
            success: function (result) {
                // The server returns JSON on success ({ success: true, firstName })
                // or re-renders the form partial (HTML) if validation failed.
                if (result && typeof result === 'object' && result.success) {
                    // Update the nav greeting in place - no page reload
                    $('#editProfileLink').text('Hello, ' + result.firstName);

                    var modalEl = document.getElementById('editProfileModal');
                    var modal = bootstrap.Modal.getInstance(modalEl);
                    if (modal) {
                        modal.hide();
                    }
                } else {
                    // Validation errors: redisplay the form with the messages
                    $('#editProfileModal .modal-content').html(result);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error updating profile:', error);
                alert('An error occurred while updating your profile.');
            }
        });
    });
});
