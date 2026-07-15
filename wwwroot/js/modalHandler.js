// ACTIVITY 6 (Partial Views): handles the "Hello, {FirstName}" click in the
// nav bar. Instead of navigating to a full Profile page, this fetches the
// edit-profile partial view via AJAX and shows it inside the Bootstrap
// modal shell defined in _Layout.cshtml (#editProfileModal).
$(document).ready(function () {
    $(document).on('click', '#editProfileLink', function (e) {
        e.preventDefault();

        $.ajax({
            url: '/Account/ShowEditProfileModal',
            type: 'GET',
            success: function (result) {
                $('#editProfileModal .modal-content').html(result);
                var modal = new bootstrap.Modal(document.getElementById('editProfileModal'));
                modal.show();
            },
            error: function (xhr, status, error) {
                console.error('Error loading edit profile form:', error);
                alert('An error occurred while loading the profile form.');
            }
        });
    });
});
