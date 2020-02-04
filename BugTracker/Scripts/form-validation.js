// Wait for the DOM to be ready

$(function () {  
    // Initialize form validation on the registration form.
    // It has the name attribute "create"
    $("form[name='create']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            title: "required",
            description: "required",
            version: "required"
        },
        // Specify validation error messages
        messages: {
            title: "Please enter a title",
            description: "Please enter a description",
            version: "Please enter a version"
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }
    });
    $("form[name='edit']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            title: "required",
            description: "required",
            version: "required"
        },
        // Specify validation error messages
        messages: {
            title: "Please enter a title",
            description: "Please enter a description",
            version: "Please enter a version"
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }
    });
    $("form[name='asedit']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            password: "required"
        },
        // Specify validation error messages
        messages: {
            password: "Please enter a password"
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }
    });
    $("form[name='ascreate']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            email: {
                required: true,
                email: true
            },
            password: "required"
        },
        // Specify validation error messages
        messages: {
            email: "Please enter a valid email address",
            password: "Please enter a password"
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }
    });
});